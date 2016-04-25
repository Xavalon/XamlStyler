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
        private IStylerOptions Options { get; set; }
        private readonly AttributeInfoFactory _attributeInfoFactory;
        private readonly AttributeInfoFormatter _attributeInfoFormatter;
        private readonly IndentService _indentService;
        private readonly IList<string> _noNewLineElementsList;

        public ElementDocumentProcessor(IStylerOptions options, AttributeInfoFactory attributeInfoFactory, AttributeInfoFormatter attributeInfoFormatter, IndentService indentService)
        {
            Options = options;
            _attributeInfoFactory = attributeInfoFactory;
            _attributeInfoFormatter = attributeInfoFormatter;
            _indentService = indentService;
            _noNewLineElementsList = options.NoNewLineElements.ToList();
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

            elementProcessContext.Push(
                new ElementProcessStatus
                {
                    Parent = elementProcessContext.Current,
                    Name = xmlReader.Name,
                    ContentType = ContentTypeEnum.NONE,
                    IsMultlineStartTag = false,
                    IsSelfClosingElement = false,
                    IsPreservingSpace = elementProcessContext.Current.IsPreservingSpace
                }
                );

            string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);
            string elementName = xmlReader.Name;

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
                        elementProcessContext.Current.IsPreservingSpace = (xmlReader.Value == "preserve");
                    }
                }

                if (Options.EnableAttributeReordering)
                    list.Sort(AttributeInfoComparison);

                currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

                var noLineBreakInAttributes = (list.Count <= Options.AttributesTolerance) || IsNoLineBreakElement(elementName);
                var forceLineBreakInAttributes = false;

                // Root element?
                if (elementProcessContext.Count == 2)
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

                    elementProcessContext.Current.IsMultlineStartTag = false;
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

                    elementProcessContext.Current.IsMultlineStartTag = true;
                }

                // Determine if to put ending bracket on new line
                if (Options.PutEndingBracketOnNewLine
                    && elementProcessContext.Current.IsMultlineStartTag)
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

                elementProcessContext.Current.IsSelfClosingElement = true;
            }
            else
            {
                output.Append(">");
            }

            if (elementProcessContext.Current.IsSelfClosingElement)
            {
                elementProcessContext.Pop();
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

            return Options.OrderAttributesByName
                ? string.Compare(x.Name, y.Name, StringComparison.Ordinal)
                : 0;
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            return _noNewLineElementsList.Contains(elementName);
        }
    }
}