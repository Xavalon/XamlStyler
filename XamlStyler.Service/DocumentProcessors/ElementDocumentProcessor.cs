using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.MarkupExtensions.Formatter;
using XamlStyler.Core.Model;
using XamlStyler.Core.Options;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Services;

namespace XamlStyler.Core.DocumentProcessors
{
    internal class ElementDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions _options;
        private readonly AttributeInfoFactory _attributeInfoFactory;
        private readonly AttributeInfoFormatter _attributeInfoFormatter;
        private readonly IndentService _indentService;
        private readonly IList<string> _noNewLineElementsList;

        public ElementDocumentProcessor(IStylerOptions options, AttributeInfoFactory attributeInfoFactory, AttributeInfoFormatter attributeInfoFormatter, IndentService indentService)
        {
            _options = options;
            _attributeInfoFactory = attributeInfoFactory;
            _attributeInfoFormatter = attributeInfoFormatter;
            _indentService = indentService;
            _noNewLineElementsList = options.NoNewLineElements.ToList();
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

            var elementName = xmlReader.Name;
            elementProcessContext.Push(
                new ElementProcessStatus
                {
                    Parent = elementProcessContext.Current,
                    Name = elementName,
                    ContentType = ContentTypeEnum.NONE,
                    IsMultlineStartTag = false,
                    IsPreservingSpace = elementProcessContext.Current.IsPreservingSpace
                }
                );

            var currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
            var attributeIndetationString = GetAttributeIndetationString(xmlReader);

            // Calculate how element should be indented
            if (!elementProcessContext.Current.IsPreservingSpace)
            {
                // "Run" get special treatment to try to preserve spacing. Use xml:space='preserve' to make sure!
                if (elementName.Equals("Run"))
                {
                    elementProcessContext.Current.Parent.IsSignificantWhiteSpace = true;
                    if (output.Length == 0 || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    elementProcessContext.Current.Parent.IsSignificantWhiteSpace = false;
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
                .Append(elementName);

            bool isEmptyElement = xmlReader.IsEmptyElement;

            if (xmlReader.HasAttributes)
            {
                bool isNoLineBreakElement = IsNoLineBreakElement(elementName);
                ProcessAttributes(xmlReader, output, elementProcessContext, isNoLineBreakElement, attributeIndetationString);
            }

            // Determine if to put ending bracket on new line
            bool putEndingBracketOnNewLine = (_options.PutEndingBracketOnNewLine && elementProcessContext.Current.IsMultlineStartTag);
            if(putEndingBracketOnNewLine)
            {
                // Indent ending bracket just like an attribute
                output
                    .Append(Environment.NewLine)
                    .Append(attributeIndetationString);
            }

            if (isEmptyElement)
            {
                if (putEndingBracketOnNewLine == false && _options.SpaceBeforeClosingSlash)
                {
                    output.Append(' ');
                }
                output.Append("/>");

                // Self closing element. Remember to pop element context.
                elementProcessContext.Pop();
            }
            else
            {
                output.Append(">");
            }
        }

        private void ProcessAttributes(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext, bool isNoLineBreakElement, string attributeIndentationString)
        {
            var list = new List<AttributeInfo>(xmlReader.AttributeCount);
            while (xmlReader.MoveToNextAttribute())
            {
                list.Add(_attributeInfoFactory.Create(xmlReader));

                // Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
                if (xmlReader.IsXmlSpaceAttribute())
                {
                    elementProcessContext.Current.IsPreservingSpace = (xmlReader.Value == "preserve");
                }
            }

            if (_options.EnableAttributeReordering)
                list.Sort(AttributeInfoComparison);

            var noLineBreakInAttributes = (list.Count <= _options.AttributesTolerance) || isNoLineBreakElement;
            var forceLineBreakInAttributes = false;

            // Root element?
            if (elementProcessContext.Count == 2)
            {
                switch (_options.RootElementLineBreakRule)
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

                elementProcessContext.Current.IsMultlineStartTag = false;
            }

            // Need to break attributes
            else
            {
                var attributeLines = new List<string>();
                var currentLineBuffer = new StringBuilder();
                int attributeCountInCurrentLineBuffer = 0;

                AttributeInfo lastAttributeInfo = null;
                foreach (AttributeInfo attrInfo in list)
                {
                    // Attributes with markup extension, always put on new line
                    if (attrInfo.IsMarkupExtension && _options.FormatMarkupExtension)
                    {
                        if (currentLineBuffer.Length > 0)
                        {
                            attributeLines.Add(currentLineBuffer.ToString());
                            currentLineBuffer.Length = 0;
                            attributeCountInCurrentLineBuffer = 0;
                        }

                        attributeLines.Add(_attributeInfoFormatter.ToMultiLineString(attrInfo, attributeIndentationString));
                    }
                    else
                    {
                        string pendingAppend = _attributeInfoFormatter.ToSingleLineString(attrInfo);

                        bool isAttributeCharLengthExceeded =
                            (attributeCountInCurrentLineBuffer > 0 && _options.MaxAttributeCharatersPerLine > 0
                             &&
                             currentLineBuffer.Length + pendingAppend.Length > _options.MaxAttributeCharatersPerLine);

                        bool isAttributeCountExceeded =
                            (_options.MaxAttributesPerLine > 0 &&
                             attributeCountInCurrentLineBuffer + 1 > _options.MaxAttributesPerLine);

                        bool isAttributeRuleGroupChanged = _options.PutAttributeOrderRuleGroupsOnSeparateLines
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
                    // Put first attribute line on same line as element?
                    if (0 == i && _options.KeepFirstAttributeOnSameLine)
                    {
                        output
                            .Append(' ')
                            .Append(attributeLines[i].Trim());
                    }
                    else
                    {
                        output
                            .Append(Environment.NewLine)
                            .Append(_indentService.Normalize(attributeIndentationString + attributeLines[i].Trim()));
                    }
                }

                elementProcessContext.Current.IsMultlineStartTag = true;
            }
        }

        private int AttributeInfoComparison(AttributeInfo x, AttributeInfo y)
        {
            if (x.OrderRule.Group != y.OrderRule.Group)
            {
                return x.OrderRule.Group.CompareTo(y.OrderRule.Group);
            }

            if (x.OrderRule.Priority != y.OrderRule.Priority)
            {
                return x.OrderRule.Priority.CompareTo(y.OrderRule.Priority);
            }

            return _options.OrderAttributesByName
                ? string.Compare(x.Name, y.Name, StringComparison.Ordinal)
                : 0;
        }

        private string GetAttributeIndetationString(XmlReader xmlReader)
        {
            if (_options.AttributeIndentation == 0)
            {
                if (_options.KeepFirstAttributeOnSameLine)
                    return _indentService.GetIndentString(xmlReader.Depth, xmlReader.Name.Length + 2);
                else
                    return _indentService.GetIndentString(xmlReader.Depth + 1);
            }
            else
            {
                return _indentService.GetIndentString(xmlReader.Depth, _options.AttributeIndentation);
            }
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            return _noNewLineElementsList.Contains(elementName);
        }
    }
}