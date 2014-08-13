using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using XamlStyler.Core.Helpers;
using XamlStyler.Core.Model;
using XamlStyler.Core.Options;
using XamlStyler.Core.Parser;

namespace XamlStyler.Core
{
    public class StylerService
    {
        //private readonly Regex _htmlReservedCharRegex = new Regex(@"&#([a-z]|[A-Z]|[0-9])*;");
        private readonly Regex _htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{3,7});");
        private readonly Regex _htmlReservedCharRestoreRegex = new Regex(@"__amp__([\d\D]{3,7})__scln__");
        private readonly Stack<ElementProcessStatus> _elementProcessStatusStack;

        private IStylerOptions Options { get; set; }
        private IList<string> NoNewLineElementsList { get; set; }
        private AttributeOrderRules OrderRules { get; set; }

        private StylerService()
        {
            _elementProcessStatusStack = new Stack<ElementProcessStatus>();
        }

        public static StylerService CreateInstance(IStylerOptions options)
        {
            var stylerServiceInstance = new StylerService {Options = options};

            if (!String.IsNullOrEmpty(stylerServiceInstance.Options.NoNewLineElements))
            {
                stylerServiceInstance.NoNewLineElementsList = stylerServiceInstance.Options.NoNewLineElements.Split(',')
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();
            }
            stylerServiceInstance.OrderRules = new AttributeOrderRules(options);

            stylerServiceInstance._elementProcessStatusStack.Clear();
            stylerServiceInstance._elementProcessStatusStack.Push(new ElementProcessStatus());

            return stylerServiceInstance;
        }

        public string Format(string xamlSource)
        {
            string output = String.Empty;
            StringReader sourceReader = null;

            try
            {
                sourceReader = new StringReader(_htmlReservedCharRegex.Replace(xamlSource, @"__amp__$1__scln__"));

                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
                    sourceReader = null;

                    xmlReader.Read();

                    while (!xmlReader.EOF)
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

                                _elementProcessStatusStack.Push(
                                    new ElementProcessStatus
                                        {
                                            Name = xmlReader.Name,
                                            ContentType = ContentTypeEnum.NONE,
                                            IsMultlineStartTag = false,
                                            IsSelfClosingElement = false
                                        }
                                    );

                                ProcessElement(xmlReader, ref output);

                                if (_elementProcessStatusStack.Peek().IsSelfClosingElement)
                                {
                                    _elementProcessStatusStack.Pop();
                                }

                                break;

                            case XmlNodeType.Text:
                                UpdateParentElementProcessStatus(ContentTypeEnum.SINGLE_LINE_TEXT_ONLY);
                                ProcessTextNode(xmlReader, ref output);
                                break;

                            case XmlNodeType.ProcessingInstruction:
                                UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);
                                ProcessInstruction(xmlReader, ref output);
                                break;

                            case XmlNodeType.Comment:
                                UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);
                                ProcessComment(xmlReader, ref output);
                                break;

                            case XmlNodeType.Whitespace:
                                ProcessWhitespace(xmlReader, ref output);
                                break;

                            case XmlNodeType.EndElement:
                                ProcessEndElement(xmlReader, ref output);
                                _elementProcessStatusStack.Pop();
                                break;

                            default:
                                Trace.WriteLine(String.Format("Unprocessed NodeType: {0} Name: {1} Value: {2}",
                                                              xmlReader.NodeType, xmlReader.Name, xmlReader.Value));
                                break;
                        }

                        xmlReader.Read();
                    }
                }
            }
            finally
            {
                if (null != sourceReader)
                {
                    sourceReader.Close();
                }
            }

            return  _htmlReservedCharRestoreRegex.Replace(output, @"&$1;");
        }

        public string ManipulateTreeAndFormatFile(string filePath)
        {
            // first, manipulate the tree
            var xDoc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
            ManipulateTree(xDoc);

            // then, write it to a string
            return Format(xDoc.ToString());
        }



        private XDocument ManipulateTree(XDocument xDoc)
        {

            var rootElement = xDoc.Root;

            if (rootElement.HasElements)
            {
                // run through the elements and, one by one, handle them

                foreach (var element in rootElement.Elements())
                {
                    HandleNode(element);
                }
            }

            return xDoc;
        }
        private void HandleNode(XNode node)
        {

            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    XElement element = node as XElement;

                    if (element.Nodes().Any())
                    {
                        // handle children first
                        foreach (var childNode in element.Nodes())
                        {
                            HandleNode(childNode);
                        }
                    }

                    // is this a Grid or Canvas with child elements?
                    //
                    // Note: we look at elements, not just nodes - if there's only non-element nodes,
                    // we don't need to reorder.

                    if (element.Name.LocalName == "Grid" && element.HasElements)
                    {
                        // process the grid
                        ProcessGrid(element);
                    }
                    else if (element.Name.LocalName == "Canvas" && element.HasElements)
                    {
                        ProcessCanvas(element);
                    }


                    break;
                default:
                    break;
            }

        }

        private void ProcessCanvas(XElement element)
        {
            List<CanvasNodeContainer> lstNodeContainers = new List<CanvasNodeContainer>();

            var children = element.Nodes();

            // Run through child elements & read the attributes

            foreach (var child in children)
            {
                switch (child.NodeType)
                {

                    case XmlNodeType.Element:

                        // it's an element.  Search for attached Canvas properties
                        var leftAttr = (child as XElement).Attributes("Canvas.Left").FirstOrDefault();
                        var topAttr = (child as XElement).Attributes("Canvas.Top").FirstOrDefault();
                        var rightAttr = (child as XElement).Attributes("Canvas.Right").FirstOrDefault();
                        var bottomAttr = (child as XElement).Attributes("Canvas.Bottom").FirstOrDefault();

                        // no attribute?  0,0
                        lstNodeContainers.Add(new CanvasNodeContainer
                            (child,
                            leftAttr == null ? 0 : int.Parse(leftAttr.Value),
                            topAttr == null ? 0 : int.Parse(topAttr.Value),
                            rightAttr == null ? 0 : int.Parse(rightAttr.Value),
                            bottomAttr == null ? 0 : int.Parse(bottomAttr.Value)
                            ));

                        break;

                    default:
                        // it's not an element - add it, passing in the previous attached property value - this ensures
                        // comments, whitespace, ... are kept in the correct place

                        var prev = lstNodeContainers.FirstOrDefault();
                        if (prev != null)
                        {
                            lstNodeContainers.Add(new CanvasNodeContainer(child, prev.Left, prev.Top, prev.Right, prev.Bottom));
                        }
                        else
                        {
                            lstNodeContainers.Add(new CanvasNodeContainer(child, 0, 0, 0, 0));
                        }

                        break;
                }
            }

            // sort that list.
            lstNodeContainers = lstNodeContainers.OrderBy(nc => nc.Left).ThenBy(nc => nc.Top)
                .ThenBy(nc => nc.Right).ThenBy(nc => nc.Bottom).ToList();

            // replace the element's nodes
            element.ReplaceAll(lstNodeContainers.Select(nc => nc.Node));
        }


 

        private void ProcessGrid(XElement element)
        {
            List<GridNodeContainer> lstNodeContainers = new List<GridNodeContainer>();

            var children = element.Nodes();

            // Run through child elements & read the attributes

            foreach (var child in children)
            {
                switch (child.NodeType)
                {

                    case XmlNodeType.Element:

                        // it's an element.  Search for Grid.Row attribute / Grid.Column attribute
                        var rowAttr = (child as XElement).Attributes("Grid.Row").FirstOrDefault();
                        var columnAttr = (child as XElement).Attributes("Grid.Column").FirstOrDefault();

                        // no attribute?  0,0
                        lstNodeContainers.Add(new GridNodeContainer
                            (child,
                            rowAttr == null ? 0 : int.Parse(rowAttr.Value),
                            columnAttr == null ? 0 : int.Parse(columnAttr.Value)
                            ));

                        break;

                    default:
                        // it's not an element - add it, passing in the previous row/column value - this ensures
                        // comments, whitespace, ... are kept in the correct place

                        var prev = lstNodeContainers.FirstOrDefault();
                        if (prev != null)
                        {
                            lstNodeContainers.Add(new GridNodeContainer(child, prev.Row, prev.Column));
                        }
                        else
                        {
                            lstNodeContainers.Add(new GridNodeContainer(child, 0, 0));
                        }

                        break;
                }
            }

            // sort that list.
            lstNodeContainers = lstNodeContainers.OrderBy(nc => nc.Row).ThenBy(nc => nc.Column).ToList();

            // replace the element's nodes
            element.ReplaceAll(lstNodeContainers.Select(nc => nc.Node));

        }


        private string GetIndentString(int depth)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if (Options.IndentWithTabs)
            {
                return new String('\t', depth);
            }
            
            return new String(' ', depth*Options.IndentSize);
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            return NoNewLineElementsList.Contains<string>(elementName);
        }

        private void ProcessComment(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value.Trim();

            if (!output.EndsWith("\n"))
            {
                output += Environment.NewLine;
            }

            if (content.Contains("\n"))
            {
                output += currentIndentString + "<!--";

                string contentIndentString = GetIndentString(xmlReader.Depth + 1);
                string[] lines = content.Split('\n');

                output = lines.Aggregate(output, (current, line) => current + (Environment.NewLine + contentIndentString + line.Trim()));

                output += Environment.NewLine + currentIndentString + "-->";
            }
            else
            {
                output += currentIndentString + "<!--  " + content + "  -->";
            }
        }

        private void ProcessElement(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            if ("Run".Equals(elementName))
            {
                if (output.EndsWith("\n"))
                {
                    // Shall not add extra whitespaces (including linefeeds) before <Run/>,
                    // because it will affect the rendering of <TextBlock><Run/><Run/></TextBlock>
                    output += currentIndentString + "<" + xmlReader.Name;
                }
                else
                {
                    output += "<" + xmlReader.Name;
                }
            }
            else if (output.Length == 0 || output.EndsWith("\n"))
            {
                output += currentIndentString + "<" + xmlReader.Name;
            }
            else
            {
                output += Environment.NewLine + currentIndentString + "<" + xmlReader.Name;
            }

            bool isEmptyElement = xmlReader.IsEmptyElement;
            bool hasPutEndingBracketOnNewLine = false;
            var list = new List<AttributeInfo>(xmlReader.AttributeCount);

            if (xmlReader.HasAttributes)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string attributeName = xmlReader.Name;
                    string attributeValue = xmlReader.Value;
                    AttributeOrderRule orderRule = OrderRules.GetRuleFor(attributeName);
                    list.Add(new AttributeInfo(attributeName, attributeValue, orderRule));
                }

                list.Sort();

                currentIndentString = GetIndentString(xmlReader.Depth);

                // No need to break attributes
                if ((list.Count <= Options.AttributesTolerance)
                    || IsNoLineBreakElement(elementName))
                {
                    output = list.Select(attrInfo => attrInfo.ToSingleLineString()).Aggregate(output, (current, pendingAppend) => current + String.Format(" {0}", pendingAppend));

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = false;
                }

                    // Need to break attributes
                else
                {
                    IList<String> attributeLines = new List<String>();

                    var currentLineBuffer = new StringBuilder();
                    int attributeCountInCurrentLineBuffer = 0;

                    foreach (AttributeInfo attrInfo in list)
                    {
                        // Attributes with markup extension, always put on new line
                        if (attrInfo.IsMarkupExtension && Options.FormatMarkupExtension)
                        {
                            string baseIndetationString = GetIndentString(xmlReader.Depth - 1) +
                                                          String.Empty.PadLeft(elementName.Length + 2, ' ');
                            string pendingAppend = attrInfo.ToMultiLineString(baseIndetationString);

                            if (currentLineBuffer.Length > 0)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            attributeLines.Add(pendingAppend);
                        }
                        else
                        {
                            string pendingAppend = attrInfo.ToSingleLineString();

                            bool isAttributeCharLengthExceeded =
                                (attributeCountInCurrentLineBuffer > 0 && Options.MaxAttributeCharatersPerLine > 0
                                 &&
                                 currentLineBuffer.Length + pendingAppend.Length > Options.MaxAttributeCharatersPerLine);

                            bool isAttributeCountExceeded =
                                (Options.MaxAttributesPerLine > 0 &&
                                 attributeCountInCurrentLineBuffer + 1 > Options.MaxAttributesPerLine);

                            if (isAttributeCharLengthExceeded || isAttributeCountExceeded)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            currentLineBuffer.AppendFormat("{0} ", pendingAppend);
                            attributeCountInCurrentLineBuffer++;
                        }
                    }

                    if (currentLineBuffer.Length > 0)
                    {
                        attributeLines.Add(currentLineBuffer.ToString());
                    }

                    for (int i = 0; i < attributeLines.Count; i++)
                    {
                        if (0 == i && Options.KeepFirstAttributeOnSameLine)
                        {
                            output += String.Format(" {0}", attributeLines[i].Trim());

                            // Align subsequent attributes with first attribute
                            currentIndentString = GetIndentString(xmlReader.Depth - 1) +
                                                  String.Empty.PadLeft(elementName.Length + 2, ' ');
                            continue;
                        }
                        output += Environment.NewLine + currentIndentString + attributeLines[i].Trim();
                    }

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = true;
                }

                // Determine if to put ending bracket on new line
                if (Options.PutEndingBracketOnNewLine
                    && _elementProcessStatusStack.Peek().IsMultlineStartTag)
                {
                    output += Environment.NewLine + currentIndentString;
                    hasPutEndingBracketOnNewLine = true;
                }
            }

            if (isEmptyElement)
            {
                if (hasPutEndingBracketOnNewLine)
                {
                    output += "/>";
                }
                else
                {
                    output += " />";
                }

                _elementProcessStatusStack.Peek().IsSelfClosingElement = true;
            }
            else
            {
                output += ">";
            }
        }

        private void ProcessEndElement(XmlReader xmlReader, ref string output)
        {
            // Shrink the current element, if it has no content.
            // E.g., <Element>  </Element> => <Element />
            if (ContentTypeEnum.NONE == _elementProcessStatusStack.Peek().ContentType
                && Options.RemoveEndingTagOfEmptyElement)
            {
                #region shrink element with no content

                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');

                if ('\t' != output[bracketIndex - 1] && ' ' != output[bracketIndex - 1])
                {
                    output = output.Insert(bracketIndex, " /");
                }
                else
                {
                    output = output.Insert(bracketIndex, "/");
                }

                #endregion shrink element with no content
            }
            else if (ContentTypeEnum.SINGLE_LINE_TEXT_ONLY == _elementProcessStatusStack.Peek().ContentType
                     && false == _elementProcessStatusStack.Peek().IsMultlineStartTag)
            {
                int bracketIndex = output.LastIndexOf('>');

                string text = output.Substring(bracketIndex + 1, output.Length - bracketIndex - 1).Trim();

                output = output.Remove(bracketIndex + 1);
                output += text + "</" + xmlReader.Name + ">";
            }
            else
            {
                string currentIndentString = GetIndentString(xmlReader.Depth);

                if (!output.EndsWith("\n"))
                {
                    output += Environment.NewLine;
                }

                output += currentIndentString + "</" + xmlReader.Name + ">";
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);

            if (!output.EndsWith("\n"))
            {
                output += Environment.NewLine;
            }

            output += currentIndentString + "<?Mapping " + xmlReader.Value + " ?>";
        }

        private void ProcessTextNode(XmlReader xmlReader, ref string output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            IEnumerable<String> textLines =
                xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true).Trim().Split('\n').Where(
                    x => x.Trim().Length > 0).ToList();

            output = textLines.Where(line => line.Trim().Length > 0).Aggregate(output, (current, line) => current + (Environment.NewLine + currentIndentString + line.Trim()));

            if (textLines.Count() > 1)
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
            }
        }

        private void ProcessWhitespace(XmlReader xmlReader, ref string output)
        {
            if (xmlReader.Value.Contains('\n'))
            {
                // For WhiteSpaces contain linefeed, trim all spaces/tab，
                // since the intent of this whitespace node is to break line,
                // and preserve the line feeds
                output += xmlReader.Value.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n",
                                                                                                       Environment.
                                                                                                           NewLine);
            }
            else
            {
                // Preserve "pure" WhiteSpace between elements
                // e.g.,
                //   <TextBlock>
                //     <Run>A</Run> <Run>
                //      B
                //     </Run>
                //  </TextBlock>
                output += xmlReader.Value;
            }
        }

        private void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = _elementProcessStatusStack.Peek();

            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}