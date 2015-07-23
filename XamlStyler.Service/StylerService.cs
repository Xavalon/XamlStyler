using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XamlStyler.Core.Helpers;
using XamlStyler.Core.Model;
using XamlStyler.Core.Options;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Reorder;

namespace XamlStyler.Core
{
    public class StylerService: XmlEscapingService
    {
        private readonly Stack<ElementProcessStatus> _elementProcessStatusStack;

        private IStylerOptions Options { get; set; }
        private IList<string> NoNewLineElementsList { get; set; }
        private IList<string> NoNewLineMarkupExtensionsList { get; set; }
        private AttributeOrderRules OrderRules { get; set; }
        private List<NodeReorderService> ReorderServices { get; set; }

        private StylerService()
        {
            _elementProcessStatusStack = new Stack<ElementProcessStatus>();
        }

        private void Initialize()
        {
            ReorderServices = new List<NodeReorderService>
            {
                GetReorderGridChildrenService(),
                GetReorderCanvasChildrenService(),
                GetReorderSettersService()
            };
        }

        private NodeReorderService GetReorderGridChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = Options.ReorderGridChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Grid", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Row", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Column", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderCanvasChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = Options.ReorderCanvasChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Canvas", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Left", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Top", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Right", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Bottom", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderSettersService()
        {
            var reorderService = new NodeReorderService();
            reorderService.ParentNodeNames.Add(new NameSelector("DataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiDataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Style", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Trigger", null));
            reorderService.ChildNodeNames.Add(new NameSelector("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));

            switch (Options.ReorderSetters)
            {
                case ReorderSettersBy.None:
                    reorderService.IsEnabled = false;
                    break;
                case ReorderSettersBy.Property:
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;
                case ReorderSettersBy.TargetName:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    break;
                case ReorderSettersBy.TargetNameThenProperty:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return reorderService;
        }

        public static StylerService CreateInstance(IStylerOptions options)
        {
            var stylerServiceInstance = new StylerService { Options = options };

            stylerServiceInstance.NoNewLineElementsList = stylerServiceInstance.Options.NoNewLineElements.ToList();
            stylerServiceInstance.NoNewLineMarkupExtensionsList = stylerServiceInstance.Options.NoNewLineMarkupExtensions.ToList();

            stylerServiceInstance.OrderRules = new AttributeOrderRules(options);

            stylerServiceInstance._elementProcessStatusStack.Clear();
            stylerServiceInstance._elementProcessStatusStack.Push(new ElementProcessStatus());

            return stylerServiceInstance;
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
                    while (xmlReader.Read())
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
                                        IsSelfClosingElement = false,
                                        IsPreservingSpace = _elementProcessStatusStack.Peek().IsPreservingSpace
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

                            case XmlNodeType.SignificantWhitespace:
                                ProcessSignificantWhitespace(xmlReader, output);
                                break;

                            case XmlNodeType.EndElement:
                                ProcessEndElement(xmlReader, output);
                                _elementProcessStatusStack.Pop();
                                break;
                            case XmlNodeType.XmlDeclaration:
                                //ignoring xml declarations for Xamarin support
                                ProcessXMLRoot(xmlReader, output);
                                break;

                            default:
                                Trace.WriteLine(
                                    $"Unprocessed NodeType: {xmlReader.NodeType} Name: {xmlReader.Name} Value: {xmlReader.Value}");
                                break;
                        }
                    }
                }
            }

            return output.ToString();
        }

        private void ProcessCDATA(XmlReader xmlReader, StringBuilder output)
        {
            // If there is linefeed(s) between element and CDATA then treat CDATA as element and indent accordingly, otherwise treat as single line text
            if (output.IsNewLine())
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
                if (!_elementProcessStatusStack.Peek().IsPreservingSpace)
                {
                    string currentIndentString = GetIndentString(xmlReader.Depth);
                    output.Append(currentIndentString);
                }
            }
            else
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.SINGLE_LINE_TEXT_ONLY);
            }

            output
                .Append("<![CDATA[")
                // All newlines are returned by XmlReader as \n due to requirements in the XML Specification (http://www.w3.org/TR/2008/REC-xml-20081126/#sec-line-ends)
                // Change them back into the environment newline characters.
                .Append(xmlReader.Value.Replace("\n", Environment.NewLine))
                .Append("]]>");
        }

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="xamlSource"></param>
        /// <returns></returns>
        public string ManipulateTreeAndFormatInput(string xamlSource)
        {
            Initialize();

            // parse XDocument
            var xDoc = XDocument.Parse(EscapeDocument(xamlSource), LoadOptions.PreserveWhitespace);

            // first, manipulate the tree; then, write it to a string
            return UnescapeDocument(Format(ManipulateTree(xDoc)));
        }

        private string ManipulateTree(XDocument xDoc)
        {
            var xmlDeclaration = xDoc.Declaration?.ToString() ?? string.Empty;
            var rootElement = xDoc.Root;

            if (rootElement != null && rootElement.HasElements)
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

                    if (element != null && element.Nodes().Any())
                    {
                        // handle children first
                        foreach (var childNode in element.Nodes())
                        {
                            HandleNode(childNode);
                        }
                    }

                    if (element != null && element.HasElements)
                    {
                        foreach (var reorderService in ReorderServices)
                        {
                            reorderService.HandleElement(element);
                        }
                    }
                    break;
            }
        }

        private string GetIndentString(int depth)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if (Options.IndentWithTabs)
            {
                return new string('\t', depth);
            }

            return new string(' ', depth * Options.IndentSize);
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

            if (!output.IsNewLine())
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

        public int AttributeInfoComparison(AttributeInfo x, AttributeInfo y)
        {
            if (x.OrderRule.Group != y.OrderRule.Group)
            {
                return x.OrderRule.Group.CompareTo(y.OrderRule.Group);
            }

            if (x.OrderRule.Priority != y.OrderRule.Priority)
            {
                return x.OrderRule.Priority.CompareTo(y.OrderRule.Priority);
            }

            return Options.OrderAttributesByName 
                ? string.Compare(x.Name, y.Name, StringComparison.Ordinal) 
                : 0;
        }

        private void ProcessElement(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            // Calculate how element should be indented
            if (!_elementProcessStatusStack.Peek().IsPreservingSpace)
            {
                if (output.Length == 0 || output.IsNewLine())
                {
                    output.Append(currentIndentString);
                }
                else
                {
                    output
                        .Append(Environment.NewLine)
                        .Append(currentIndentString);
                }
            }

            // Output the element itself
            output
                .Append('<')
                .Append(xmlReader.Name);

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

                    // Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
                    if (xmlReader.IsXmlSpaceAttribute())
                    {
                        _elementProcessStatusStack.Peek().IsPreservingSpace = (xmlReader.Value == "preserve");
                    }
                }

                if (Options.EnableAttributeReordering)
                    list.Sort(AttributeInfoComparison);

                currentIndentString = GetIndentString(xmlReader.Depth);

                var noLineBreakInAttributes = (list.Count <= Options.AttributesTolerance) || IsNoLineBreakElement(elementName);
                var forceLineBreakInAttributes = false;

                // Root element?
                if (_elementProcessStatusStack.Count == 2)
                {
                    switch (Options.RootElementLineBreakRule)
                    {
                        case LineBreakRule.Default:
                            break;
                        case LineBreakRule.Always:
                            noLineBreakInAttributes = false;
                            forceLineBreakInAttributes = true;
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
                            string baseIndetationString;

                            if (!Options.KeepFirstAttributeOnSameLine)
                            {
                                baseIndetationString = GetIndentString(xmlReader.Depth);
                            }
                            else
                            {
                                baseIndetationString = GetIndentString(xmlReader.Depth - 1) +
                                                       string.Empty.PadLeft(elementName.Length + 2, ' ');
                            }

                            string pendingAppend;

                            if(NoNewLineMarkupExtensionsList.Contains(attrInfo.MarkupExtension))
                            {
                                pendingAppend = " " + attrInfo.ToSingleLineString();
                            }
                            else
                            {
                                pendingAppend = attrInfo.ToMultiLineString(baseIndetationString);
                            }

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
                                                               && lastAttributeInfo.OrderRule.Group != attrInfo.OrderRule.Group;

                            if (currentLineBuffer.Length > 0 && (forceLineBreakInAttributes || isAttributeCharLengthExceeded || isAttributeCountExceeded || isAttributeRuleGroupChanged))
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
                if (hasPutEndingBracketOnNewLine == false && Options.SpaceBeforeClosingSlash)
                {
                    output.Append(' ');
                }
                output.Append("/>");

                _elementProcessStatusStack.Peek().IsSelfClosingElement = true;
            }
            else
            {
                output.Append(">");
            }
        }

        private void ProcessEndElement(XmlReader xmlReader, StringBuilder output)
        {
            if (_elementProcessStatusStack.Peek().IsPreservingSpace)
            {
                output.Append("</").Append(xmlReader.Name).Append(">");
            }
            // Shrink the current element, if it has no content.
            // E.g., <Element>  </Element> => <Element />
            else if (ContentTypeEnum.NONE == _elementProcessStatusStack.Peek().ContentType
                && Options.RemoveEndingTagOfEmptyElement)
            {
                #region shrink element with no content

                output = output.TrimEnd(' ', '\t', '\r', '\n');

                int bracketIndex = output.LastIndexOf('>');
                output.Insert(bracketIndex, '/');

                if (output[bracketIndex - 1] != '\t' && output[bracketIndex - 1] != ' ' && Options.SpaceBeforeClosingSlash)
                {
                    output.Insert(bracketIndex, ' ');
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

                if (!output.IsNewLine())
                {
                    output.Append(Environment.NewLine);
                }

                output.Append(currentIndentString).Append("</").Append(xmlReader.Name).Append(">");
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = GetIndentString(xmlReader.Depth);

            if (!output.IsNewLine())
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
            var xmlEncodedContent = xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true);
            if (_elementProcessStatusStack.Peek().IsPreservingSpace)
            {
                output.Append(xmlEncodedContent.Replace("\n", Environment.NewLine));
            }
            else
            {
                string currentIndentString = GetIndentString(xmlReader.Depth);
                IEnumerable<String> textLines =
                    xmlEncodedContent.Trim().Split('\n').Where(
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
            }

            if (xmlEncodedContent.Any(x => x == '\n'))
            {
                UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
            }
        }

        private void ProcessWhitespace(XmlReader xmlReader, StringBuilder output)
        {
            if (xmlReader.Value.Contains('\n') && !_elementProcessStatusStack.Peek().IsPreservingSpace)
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
                output.Append(xmlReader.Value.Replace("\n", Environment.NewLine));
            }
        }
        private void ProcessSignificantWhitespace(XmlReader xmlReader, StringBuilder output)
        {
            output.Append(xmlReader.Value.Replace("\n", Environment.NewLine));
        }

        private void UpdateParentElementProcessStatus(ContentTypeEnum contentType)
        {
            ElementProcessStatus parentElementProcessStatus = _elementProcessStatusStack.Peek();

            parentElementProcessStatus.ContentType |= contentType;
        }
    }
}