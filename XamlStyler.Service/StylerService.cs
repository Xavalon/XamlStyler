using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
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
        private readonly Regex _htmlReservedCharRegex = new Regex(@"&([\d\D][^;]{3,7});");
        private readonly Regex _htmlReservedCharRestoreRegex = new Regex(@"__amp__([^;]{2,7})__scln__");
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

        private string UnescapeDocument(string source)
        {
            return _htmlReservedCharRestoreRegex.Replace(source, @"&$1;");
        }

        private string EscapeDocument(string source)
        {
            return _htmlReservedCharRegex.Replace(source, @"__amp__$1__scln__");
        }

        private string Format(string xamlSource)
        {
            StringBuilder output = new StringBuilder();

            using (var sourceReader = new StringReader(xamlSource))
            {
                // Not used
                // var settings = new XmlReaderSettings {IgnoreComments = false};
                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
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

                                ProcessElement(xmlReader, output);

                                if (_elementProcessStatusStack.Peek().IsSelfClosingElement)
                                {
                                    _elementProcessStatusStack.Pop();
                                }

                                break;

                            case XmlNodeType.Text:
                                UpdateParentElementProcessStatus(ContentTypeEnum.SINGLE_LINE_TEXT_ONLY);
                                ProcessTextNode(xmlReader, output);
                                break;

                            case XmlNodeType.ProcessingInstruction:
                                UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);
                                ProcessInstruction(xmlReader, output);
                                break;

                            case XmlNodeType.Comment:
                                UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);
                                ProcessComment(xmlReader, output);
                                break;
                            case XmlNodeType.CDATA:
                                ProcessCDATA(xmlReader, output);
                                break;
                            case XmlNodeType.Whitespace:
                                ProcessWhitespace(xmlReader, output);
                                break;

                            case XmlNodeType.EndElement:
                                ProcessEndElement(xmlReader, output);
                                _elementProcessStatusStack.Pop();
                                break;
                            case XmlNodeType.XmlDeclaration:
                                //ignoring xml declarations for Xamarin support
                                ProcessXMLRoot(xmlReader, output);
                                break;
                            //case XmlNodeType.CDATA:
                            //    break;
                            default:
                                Trace.WriteLine(String.Format("Unprocessed NodeType: {0} Name: {1} Value: {2}",
                                    xmlReader.NodeType, xmlReader.Name, xmlReader.Value));
                                break;
                        }

                        xmlReader.Read();
                    }
                }
            }

            return output.ToString();
        }

        private void ProcessCDATA(XmlReader xmlReader, StringBuilder output)
        {
            output
                .Append("<![CDATA[")
                .Append(xmlReader.Value)
                .Append("]]>");
        }

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="xamlSource"></param>
        /// <returns></returns>
        public string ManipulateTreeAndFormatInput(string xamlSource)
        {
            // parse XDocument
            var xDoc = XDocument.Parse(EscapeDocument(xamlSource), LoadOptions.PreserveWhitespace);

            // first, manipulate the tree; then, write it to a string
            return UnescapeDocument(Format(ManipulateTree(xDoc)));
        }

        private string ManipulateTree(XDocument xDoc)
        {
            var xmlDeclaration = xDoc.Declaration != null
                ? xDoc.Declaration.ToString()
                : string.Empty;
            var rootElement = xDoc.Root;

            if (rootElement.HasElements)
            {
                // run through the elements and, one by one, handle them

                foreach (var element in rootElement.Elements())
                {
                    HandleNode(element);
                }
            }

            return xmlDeclaration + xDoc;
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
                    // we don't need to reorder.  We should also take into account a user can decide not to allow
                    // reordering

                    if (Options.ReorderGridChildren && element.Name.LocalName == "Grid" && element.HasElements)
                    {
                        // process the grid
                        ProcessGrid(element);
                    }
                    else if (Options.ReorderCanvasChildren && element.Name.LocalName == "Canvas" && element.HasElements)
                    {
                        // process the canvas
                        ProcessCanvas(element);
                    }
                    else if (Options.ReorderSetters != ReorderSettersBy.None && ElementsWithSetters.Contains(element.Name.LocalName) && element.HasElements)
                    {
                        // process the setters
                        ProcessSetters(element);
                    }
                    break;
                default:
                    break;
            }

        }

        private readonly string[] ElementsWithSetters = new[]
        {
            "DataTrigger",
            "MultiDataTrigger",
            "MultiTrigger",
            "Style",
            "Trigger",
        };

        /// <summary>
        /// Order child setters by property/targetname
        /// </summary>
        /// <param name="element"></param>
        private void ProcessSetters(XElement element)
        {
            // A string that hopefully always are sorted at the en
            List<SetterNodeCollection> nodeCollections = new List<SetterNodeCollection>();

            var children = element.Nodes();

            // This is increased each time Define sortable parameters
            int settersBlockIndex = 1;
            bool inSettersBlock = false;
            SetterNodeCollection currentNodeCollection = null;

            // Run through children
            foreach (var child in children)
            {
                if (currentNodeCollection == null)
                {
                    currentNodeCollection = new SetterNodeCollection();
                    nodeCollections.Add(currentNodeCollection);
                }

                if (child.NodeType == XmlNodeType.Element)
                {
                    XElement childElement = (XElement) child;

                    var isSetter = childElement.Name.LocalName == "Setter" && childElement.Name.NamespaceName == "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

                    if (isSetter != inSettersBlock)
                    {
                        settersBlockIndex++;
                        inSettersBlock = isSetter;
                    }

                    if (isSetter)
                    {
                        currentNodeCollection.Property = (string) childElement.Attribute("Property");
                        currentNodeCollection.TargetName = (string) childElement.Attribute("TargetName");
                    }

                    currentNodeCollection.BlockIndex = settersBlockIndex;
                }

                currentNodeCollection.Nodes.Add(child);

                if (child.NodeType == XmlNodeType.Element)
                    currentNodeCollection = null;
            }

            if (currentNodeCollection != null)
                currentNodeCollection.BlockIndex = settersBlockIndex + 1;

            // sort that list.
            switch (Options.ReorderSetters)
            {
                case ReorderSettersBy.None:
                    break;
                case ReorderSettersBy.Property:
                    nodeCollections = nodeCollections.OrderBy(x => x.BlockIndex).ThenBy(x => x.Property).ToList();
                    break;
                case ReorderSettersBy.TargetName:
                    nodeCollections = nodeCollections.OrderBy(x => x.BlockIndex).ThenBy(x => x.TargetName).ToList();
                    break;
                case ReorderSettersBy.TargetNameThenProperty:
                    nodeCollections = nodeCollections.OrderBy(x => x.BlockIndex).ThenBy(x => x.TargetName).ThenBy(x => x.Property).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // replace the element's nodes
            element.ReplaceNodes(nodeCollections.SelectMany(nc => nc.Nodes));
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
                        var leftAttr = ((XElement) child).Attributes("Canvas.Left");
                        var topAttr = ((XElement) child).Attributes("Canvas.Top");
                        var rightAttr = ((XElement) child).Attributes("Canvas.Right");
                        var bottomAttr = ((XElement) child).Attributes("Canvas.Bottom");

                        int left = -1;
                        int right = -1;
                        int top = -1;
                        int bottom = -1;

                        if (leftAttr != null && leftAttr.Any() && !int.TryParse(leftAttr.First().Value, out left))
                        {
                            left = -1;
                        }

                        if (rightAttr != null && rightAttr.Any() && !int.TryParse(rightAttr.First().Value, out right))
                        {
                            right = -1;
                        }

                        if (bottomAttr != null && bottomAttr.Any() && !int.TryParse(bottomAttr.First().Value, out bottom))
                        {
                            bottom = -1;
                        }

                        if (topAttr != null && topAttr.Any() && !int.TryParse(topAttr.First().Value, out top))
                        {
                            top = -1;
                        }

                        // no attribute?  0,0
                        lstNodeContainers.Add(new CanvasNodeContainer(child, left, top, right, bottom));

                        break;
                    default:
                        // it's not an element - add it, passing in the previous attached property value - this ensures
                        // comments, whitespace, ... are kept in the correct place

                        var prev = lstNodeContainers.LastOrDefault();
                        if (prev != null)
                        {
                            lstNodeContainers.Add(new CanvasNodeContainer(child, prev.Left, prev.Top, prev.Right, prev.Bottom));
                        }
                        else
                        {
                            // add with minvalue - this must be the first item at all times.
                            // cfr: https://github.com/NicoVermeir/XamlStyler/issues/9
                            lstNodeContainers.Add(new CanvasNodeContainer(child, double.MinValue, double.MinValue, double.MinValue, double.MinValue));
                        }


                        break;
                }
            }

            // sort that list.
            lstNodeContainers = lstNodeContainers.OrderBy(nc => nc.LeftNumeric).ThenBy(nc => nc.TopNumeric)
                .ThenBy(nc => nc.RightNumeric).ThenBy(nc => nc.BottomNumeric).ToList();

            // replace the element's nodes
            element.ReplaceNodes(lstNodeContainers.Select(nc => nc.Node));
        }




        private void ProcessGrid(XElement element)
        {
            List<GridNodeContainer> lstNodeContainers = new List<GridNodeContainer>();
            int commentIndex = int.MaxValue;

            var children = element.Nodes();

            // Run through child elements & read the attributes

            foreach (var child in children)
            {
                switch (child.NodeType)
                {

                    case XmlNodeType.Element:

                        // it's an element.  Search for Grid.Row attribute / Grid.Column attribute
                        var childElement = (XElement) child;

                        var rowAttr = childElement.Attributes("Grid.Row");
                        var columnAttr = childElement.Attributes("Grid.Column");

                        int row;
                        int column;

                        if (rowAttr == null || !rowAttr.Any() || !int.TryParse(rowAttr.First().Value, out row))
                        {
                            row = childElement.Name.LocalName.Contains(".") ? -2 : -1;
                        }

                        if (columnAttr == null || !columnAttr.Any() || !int.TryParse(columnAttr.First().Value, out column))
                        {
                            column = -1;
                        }

                        while (commentIndex < lstNodeContainers.Count)
                        {
                            lstNodeContainers[commentIndex].Row = row;

                            commentIndex++;
                        }
                        commentIndex = int.MaxValue;

                        // no attribute?  0,0
                        lstNodeContainers.Add(new GridNodeContainer(child, row, column));

                        break;

                    default:
                        // it's not an element - add it, passing in the previous row/column value - this ensures
                        // comments, whitespace, ... are kept in the correct place

                        var prev = lstNodeContainers.LastOrDefault();
                        if (prev != null)
                        {
                            lstNodeContainers.Add(new GridNodeContainer(child, prev.Row, prev.Column));
                        }
                        else
                        {
                            lstNodeContainers.Add(new GridNodeContainer(child, int.MinValue, int.MinValue));
                        }

                        if (child.NodeType == XmlNodeType.Comment && commentIndex == int.MaxValue)
                        {
                            commentIndex = lstNodeContainers.Count - 1;
                        }

                        break;
                }
            }

            // sort that list.
            lstNodeContainers = lstNodeContainers.OrderBy(nc => nc.Row).ThenBy(nc => nc.Column).ToList();

            // replace the element's nodes
            element.ReplaceNodes(lstNodeContainers.Select(nc => nc.Node));

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

        private void ProcessXMLRoot(XmlReader xmlReader, StringBuilder output)
        {
            output.Append("<?xml ");
            output.Append(xmlReader.Value.Trim());
            output.Append(" ?>");
        }

        private void ProcessComment(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value;

            if (!output.IsEndOfLine())
            {
                output.Append(Environment.NewLine);
            }

            if (content.Contains("<") && content.Contains(">"))
            {
                output.Append(currentIndentString);
                output.Append("<!--");
                if (content.Contains("\n"))
                {
                    output.Append(string.Join(Environment.NewLine, content.GetLines().Select(x => x.TrimEnd(' '))));
                    if (content.TrimEnd(' ').EndsWith("\n"))
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                    output.Append(content);

                output.Append("-->");
            }
            else if (content.Contains("\n"))
            {
                output
                    .Append(currentIndentString)
                    .Append("<!--");

                var contentIndentString = GetIndentString(xmlReader.Depth + 1);
                foreach (var line in content.Trim().GetLines())
                {
                    output
                        .Append(Environment.NewLine)
                        .Append(contentIndentString)
                        .Append(line.Trim());
                }

                output
                    .Append(Environment.NewLine)
                    .Append(currentIndentString)
                    .Append("-->");
            }
            else
            {
                output
                    .Append(currentIndentString)
                    .Append("<!--  ")
                    .Append(content.Trim())
                    .Append("  -->");
            }
        }

        private void ProcessElement(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            if ("Run".Equals(elementName))
            {
                if (output.IsEndOfLine())
                {
                    // Shall not add extra whitespaces (including linefeeds) before <Run/>,
                    // because it will affect the rendering of <TextBlock><Run/><Run/></TextBlock>
                    output
                        .Append(currentIndentString)
                        .Append('<')
                        .Append(xmlReader.Name);
                }
                else
                {
                    output.Append('<');
                    output.Append(xmlReader.Name);
                }
            }
            else if (output.Length == 0 || output.IsEndOfLine())
            {
                output
                    .Append(currentIndentString)
                    .Append('<')
                    .Append(xmlReader.Name);
            }
            else
            {
                output
                    .Append(Environment.NewLine)
                    .Append(currentIndentString)
                    .Append('<')
                    .Append(xmlReader.Name);
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

                if (Options.OrderAttributesByName)
                    list.Sort();

                currentIndentString = GetIndentString(xmlReader.Depth);

                var noLineBreakInAttributes = (list.Count <= Options.AttributesTolerance) || IsNoLineBreakElement(elementName);
                // Root element?
                if (_elementProcessStatusStack.Count == 2)
                {
                    switch (Options.RootElementLineBreakRule)
                    {
                        case LineBreakRule.Default:
                            break;
                        case LineBreakRule.Always:
                            noLineBreakInAttributes = false;
                            break;
                        case LineBreakRule.Never:
                            noLineBreakInAttributes = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // No need to break attributes
                if (noLineBreakInAttributes)
                {
                    foreach (var attrInfo in list)
                    {
                        output
                            .Append(' ')
                            .Append(attrInfo.ToSingleLineString());
                    }

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = false;
                }

                // Need to break attributes
                else
                {
                    IList<String> attributeLines = new List<String>();

                    var currentLineBuffer = new StringBuilder();
                    int attributeCountInCurrentLineBuffer = 0;

                    AttributeInfo lastAttributeInfo = null;
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

                            bool isAttributeRuleGroupChanged = Options.PutAttributeOrderRuleGroupsOnSeparateLines
                                                               && lastAttributeInfo != null
                                                               && lastAttributeInfo.OrderRule.AttributeTokenType != attrInfo.OrderRule.AttributeTokenType;

                            if (isAttributeCharLengthExceeded || isAttributeCountExceeded || isAttributeRuleGroupChanged)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            currentLineBuffer.AppendFormat("{0} ", pendingAppend);
                            attributeCountInCurrentLineBuffer++;
                        }

                        lastAttributeInfo = attrInfo;
                    }

                    if (currentLineBuffer.Length > 0)
                    {
                        attributeLines.Add(currentLineBuffer.ToString());
                    }

                    for (int i = 0; i < attributeLines.Count; i++)
                    {
                        if (0 == i && Options.KeepFirstAttributeOnSameLine)
                        {
                            output
                                .Append(' ')
                                .Append(attributeLines[i].Trim());

                            // Align subsequent attributes with first attribute
                            currentIndentString = GetIndentString(xmlReader.Depth - 1) +
                                                  String.Empty.PadLeft(elementName.Length + 2, ' ');
                            continue;
                        }
                        output
                            .Append(Environment.NewLine)
                            .Append(currentIndentString)
                            .Append(attributeLines[i].Trim());
                    }

                    _elementProcessStatusStack.Peek().IsMultlineStartTag = true;
                }

                // Determine if to put ending bracket on new line
                if (Options.PutEndingBracketOnNewLine
                    && _elementProcessStatusStack.Peek().IsMultlineStartTag)
                {
                    output
                        .Append(Environment.NewLine)
                        .Append(currentIndentString);
                    hasPutEndingBracketOnNewLine = true;
                }
            }

            if (isEmptyElement)
            {
                if (hasPutEndingBracketOnNewLine)
                {
                    output.Append("/>");
                }
                else
                {
                    output.Append(" />");
                }

                _elementProcessStatusStack.Peek().IsSelfClosingElement = true;
            }
            else
            {
                output.Append(">");
            }
        }

        private void ProcessEndElement(XmlReader xmlReader, StringBuilder output)
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
                    output.Insert(bracketIndex, " /");
                }
                else
                {
                    output.Insert(bracketIndex, "/");
                }

                #endregion shrink element with no content
            }
            else if (ContentTypeEnum.SINGLE_LINE_TEXT_ONLY == _elementProcessStatusStack.Peek().ContentType
                     && false == _elementProcessStatusStack.Peek().IsMultlineStartTag)
            {
                int bracketIndex = output.LastIndexOf('>');

                string text = output.Substring(bracketIndex + 1, output.Length - bracketIndex - 1).Trim();

                output.Length = bracketIndex + 1;
                output.Append(text).Append("</").Append(xmlReader.Name).Append(">");
            }
            else
            {
                string currentIndentString = GetIndentString(xmlReader.Depth);

                if (!output.IsEndOfLine())
                {
                    output.Append(Environment.NewLine);
                }

                output.Append(currentIndentString).Append("</").Append(xmlReader.Name).Append(">");
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);

            if (!output.IsEndOfLine())
            {
                output.Append(Environment.NewLine);
            }

            output
                .Append(currentIndentString)
                .Append("<?Mapping ")
                .Append(xmlReader.Value)
                .Append(" ?>");
        }

        private void ProcessTextNode(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            IEnumerable<String> textLines =
                xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true).Trim().Split('\n').Where(
                    x => x.Trim().Length > 0).ToList();

            foreach (var line in textLines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Length > 0)
                {
                    output
                        .Append(Environment.NewLine)
                        .Append(currentIndentString)
                        .Append(trimmedLine);
                }
            }

            if (textLines.Count() > 1)
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
            }
        }

        private void ProcessWhitespace(XmlReader xmlReader, StringBuilder output)
        {
            if (xmlReader.Value.Contains('\n'))
            {
                // For WhiteSpaces contain linefeed, trim all spaces/tab，
                // since the intent of this whitespace node is to break line,
                // and preserve the line feeds
                output.Append(xmlReader.Value
                    .Replace(" ", "")
                    .Replace("\t", "")
                    .Replace("\r", "")
                    .Replace("\n", Environment.NewLine));
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
                output.Append(xmlReader.Value);
            }
        }

        private void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = _elementProcessStatusStack.Peek();

            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}