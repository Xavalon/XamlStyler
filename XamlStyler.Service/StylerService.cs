using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XamlStyler.Core.DocumentManipulation;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.MarkupExtensions.Formatter;
using XamlStyler.Core.MarkupExtensions.Parser;
using XamlStyler.Core.Model;
using XamlStyler.Core.Options;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Services;

namespace XamlStyler.Core
{
    public class StylerService : XmlEscapingService
    {
        private readonly Stack<ElementProcessStatus> _elementProcessStatusStack;
        private readonly IndentService _indentService;

        private readonly IStylerOptions Options;
        private DocumentManipulationService _documentManipulationService;
        private readonly IList<string> NoNewLineElementsList;
        private readonly AttributeInfoFormatter _attributeInfoFormatter;
        private readonly AttributeInfoFactory _attributeInfoFactory;

        private StylerService(IStylerOptions options)
        {
            Options = options;

            _documentManipulationService = new DocumentManipulationService(options);

            _elementProcessStatusStack = new Stack<ElementProcessStatus>();
            _elementProcessStatusStack.Push(new ElementProcessStatus());
            _indentService = new IndentService(options.IndentWithTabs, options.IndentSize);

            var markupExtensionFormatter = new MarkupExtensionFormatter(options.NoNewLineMarkupExtensions.ToList());

            _attributeInfoFactory = new AttributeInfoFactory(new MarkupExtensionParser(), new AttributeOrderRules(options));
            _attributeInfoFormatter = new AttributeInfoFormatter(markupExtensionFormatter,_indentService);

            NoNewLineElementsList = options.NoNewLineElements.ToList();

            ;
        }


        public static StylerService CreateInstance(IStylerOptions options)
        {
            var stylerServiceInstance = new StylerService(options);


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
                                        Parent = _elementProcessStatusStack.Peek(),
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
                    string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
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
            // parse XDocument
            var xDoc = XDocument.Parse(EscapeDocument(xamlSource), LoadOptions.PreserveWhitespace);

            // first, manipulate the tree; then, write it to a string
            return UnescapeDocument(Format(_documentManipulationService.ManipulateDocument(xDoc)));
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
            string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value;

            if (output.Length > 0 && !output.IsNewLine())
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
            else if (content.Contains("#region") || content.Contains("#endregion"))
            {
                output
                    .Append(currentIndentString)
                    .Append("<!--")
                    .Append(content.Trim())
                    .Append("-->");
            }
            else if (content.Contains("\n"))
            {
                output
                    .Append(currentIndentString)
                    .Append("<!--");

                var contentIndentString = _indentService.GetIndentString(xmlReader.Depth + 1);
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
                    .Append("<!--")
                    .Append(' ', Options.CommentSpaces)
                    .Append(content.Trim())
                    .Append(' ', Options.CommentSpaces)
                    .Append("-->");
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
            string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

            // Calculate how element should be indented
            if (!_elementProcessStatusStack.Peek().IsPreservingSpace)
            {
                // "Run" get special treatment to try to preserve spacing. Use xml:space='preserve' to make sure!
                if (elementName.Equals("Run"))
                {
                    _elementProcessStatusStack.Peek().Parent.IsSignificantWhiteSpace = true;
                    if (output.Length == 0 || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    _elementProcessStatusStack.Peek().Parent.IsSignificantWhiteSpace = false;
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
                    list.Add(_attributeInfoFactory.Create(xmlReader));

                    // Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
                    if (xmlReader.IsXmlSpaceAttribute())
                    {
                        _elementProcessStatusStack.Peek().IsPreservingSpace = (xmlReader.Value == "preserve");
                    }
                }

                if (Options.EnableAttributeReordering)
                    list.Sort(AttributeInfoComparison);

                currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

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
                            .Append(_attributeInfoFormatter.ToSingleLineString(attrInfo));
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
                            var baseIndetationString = !Options.KeepFirstAttributeOnSameLine
                                ? _indentService.GetIndentString(xmlReader.Depth)
                                : _indentService.GetIndentString(xmlReader.Depth - 1, elementName.Length + 2);

                            if (currentLineBuffer.Length > 0)
                            {
                                attributeLines.Add(currentLineBuffer.ToString());
                                currentLineBuffer.Length = 0;
                                attributeCountInCurrentLineBuffer = 0;
                            }

                            attributeLines.Add(_attributeInfoFormatter.ToMultiLineString(attrInfo, baseIndetationString));
                        }
                        else
                        {
                            string pendingAppend = _attributeInfoFormatter.ToSingleLineString(attrInfo);

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
                            currentIndentString = _indentService.GetIndentString(xmlReader.Depth - 1, elementName.Length + 2);
                            continue;
                        }
                        output
                            .Append(Environment.NewLine)
                            .Append(_indentService.Normalize(currentIndentString + attributeLines[i].Trim()));
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
            else if (_elementProcessStatusStack.Peek().IsSignificantWhiteSpace && !output.IsNewLine())
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
                string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

                if (!output.IsNewLine())
                {
                    output.Append(Environment.NewLine);
                }

                output.Append(currentIndentString).Append("</").Append(xmlReader.Name).Append(">");
            }
        }

        private void ProcessInstruction(XmlReader xmlReader, StringBuilder output)
        {
            string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

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
                string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
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
            var hasNewline = xmlReader.Value.Contains('\n');

            if (_elementProcessStatusStack.Peek().IsSignificantWhiteSpace && hasNewline)
                _elementProcessStatusStack.Peek().IsSignificantWhiteSpace = false;

            if (hasNewline && !_elementProcessStatusStack.Peek().IsPreservingSpace)
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