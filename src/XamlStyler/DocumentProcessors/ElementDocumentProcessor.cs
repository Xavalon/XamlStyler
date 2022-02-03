// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.MarkupExtensions.Formatter;
using Xavalon.XamlStyler.Model;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Parser;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class ElementDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;
        private readonly XamlLanguageOptions xamlLanguageOptions;
        private readonly AttributeInfoFactory attributeInfoFactory;
        private readonly AttributeInfoFormatter attributeInfoFormatter;
        private readonly IndentService indentService;
        private readonly XmlEscapingService xmlEscapingService;
        private readonly IList<string> noNewLineElementsList;
        private readonly IList<string> firstLineAttributes;
        private readonly string[] inlineCollections = { "TextBlock", "RichTextBlock", "Paragraph", "Run", "Span", "InlineUIContainer", "AnchoredBlock" };
        private readonly string[] inlineTypes = { "Paragraph", "Run", "Span", "InlineUIContainer", "AnchoredBlock", "Hyperlink", "Bold", "Italic", "Underline", "LineBreak" };

        public ElementDocumentProcessor(
            IStylerOptions options,
            XamlLanguageOptions xamlLanguageOptions,
            AttributeInfoFactory attributeInfoFactory,
            AttributeInfoFormatter attributeInfoFormatter,
            IndentService indentService,
            XmlEscapingService xmlEscapingService)
        {
            this.options = options;
            this.xamlLanguageOptions = xamlLanguageOptions;
            this.attributeInfoFactory = attributeInfoFactory;
            this.attributeInfoFormatter = attributeInfoFormatter;
            this.indentService = indentService;
            this.xmlEscapingService = xmlEscapingService;
            this.noNewLineElementsList = options.NoNewLineElements.ToList();
            this.firstLineAttributes = options.FirstLineAttributes.ToList();
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.Mixed);

            var elementName = xmlReader.Name;
            elementProcessContext.Push(
                new ElementProcessStatus
                {
                    Parent = elementProcessContext.Current,
                    Name = elementName,
                    ContentType = ContentTypes.None,
                    IsMultlineStartTag = false,
                    IsPreservingSpace = elementProcessContext.Current.IsPreservingSpace
                });

            var currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
            var attributeIndetationString = this.GetAttributeIndetationString(xmlReader);

            // Calculate how element should be indented
            if (!elementProcessContext.Current.IsPreservingSpace)
            {
                // Preserve spacing if element is an inline type has a parent that supports inline types.
                if ((elementProcessContext.Current.Parent.Name != null)
                    && this.inlineCollections.Any(elementProcessContext.Current.Parent.Name.Contains)
                    && this.inlineTypes.Any(elementName.Contains))
                {
                    elementProcessContext.Current.Parent.IsSignificantWhiteSpace = true;
                    if ((output.Length == 0) || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    elementProcessContext.Current.Parent.IsSignificantWhiteSpace = false;
                    if ((output.Length == 0) || output.IsNewLine())
                    {
                        output.Append(currentIndentString);
                    }
                    else
                    {
                        output.Append(Environment.NewLine).Append(currentIndentString);
                    }
                }
            }

            // Output the element itself.
            output.Append('<').Append(elementName);

            bool isEmptyElement = xmlReader.IsEmptyElement;

            if (xmlReader.HasAttributes)
            {
                bool isNoLineBreakElement = this.IsNoLineBreakElement(elementName);
                this.ProcessAttributes(
                    xmlReader,
                    output,
                    elementProcessContext,
                    isNoLineBreakElement,
                    attributeIndetationString);
            }

            // Determine if to put ending bracket on new line.
            bool putEndingBracketOnNewLine = (this.options.PutEndingBracketOnNewLine
                && elementProcessContext.Current.IsMultlineStartTag);

            if (putEndingBracketOnNewLine)
            {
                // Indent ending bracket just like an attribute.
                output.Append(Environment.NewLine).Append(attributeIndetationString);
            }

            if (isEmptyElement)
            {
                if (!putEndingBracketOnNewLine && this.options.SpaceBeforeClosingSlash)
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

        private void ProcessAttributes(
            XmlReader xmlReader,
            StringBuilder output,
            ElementProcessContext elementProcessContext,
            bool isNoLineBreakElement,
            string attributeIndentationString)
        {
            var list = new List<AttributeInfo>(xmlReader.AttributeCount);
            var firstLineList = new List<AttributeInfo>(xmlReader.AttributeCount);

            while (xmlReader.MoveToNextAttribute())
            {
                var attributeInfo = this.attributeInfoFactory.Create(xmlReader);

                list.Add(attributeInfo);

                // Maintain separate list of first line attributes.
                if (this.options.EnableAttributeReordering && this.IsFirstLineAttribute(attributeInfo.Name))
                {
                    firstLineList.Add(attributeInfo);
                }

                // Check for xml:space as defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-white-space
                if (xmlReader.IsXmlSpaceAttribute())
                {
                    elementProcessContext.Current.IsPreservingSpace = (xmlReader.Value == "preserve");
                }
            }

            if (this.options.EnableAttributeReordering)
            {
                // .NET performs insertion sort if collection partition size is fewer than 16 elements, but it uses
                // Heapsort or Quicksort under different conditions. This can lead to an unstable sort and randomized
                // attributbes while formatting. Even though insertion sort is less performant, XAML elements with more
                // than 16 attributes are not common, so the effect of forcing insertion sort is negligable in all but
                // the most extreme of cases. - https://msdn.microsoft.com/en-us/library/b0zbh7b6(v=vs.110).aspx
                list.InsertionSort(this.AttributeInfoComparison);
                firstLineList.InsertionSort(this.AttributeInfoComparison);
            }

            var noLineBreakInAttributes = (list.Count <= this.options.AttributesTolerance) || isNoLineBreakElement;
            var forceLineBreakInAttributes = false;

            // Root element?
            if (elementProcessContext.Count == 2)
            {
                switch (this.options.RootElementLineBreakRule)
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
                        throw new NotImplementedException();
                }
            }

            // No need to break attributes.
            if (noLineBreakInAttributes)
            {
                foreach (var attrInfo in list)
                {
                    output.Append(' ').Append(this.attributeInfoFormatter.ToSingleLineString(attrInfo, xamlLanguageOptions));
                }

                elementProcessContext.Current.IsMultlineStartTag = false;
            }
            else
            {
                // Need to break attributes.
                var attributeLines = new List<string>();
                var currentLineBuffer = new StringBuilder();
                int attributeCountInCurrentLineBuffer = 0;
                int xmlnsAliasesBypassLengthInCurrentLine = 0;

                AttributeInfo lastAttributeInfo = null;

                // Process first line attributes.
                string firstLine = String.Empty;
                foreach (var attrInfo in firstLineList)
                {
                    firstLine = $"{firstLine} {this.attributeInfoFormatter.ToSingleLineString(attrInfo, xamlLanguageOptions)}";
                }

                if (firstLine.Length > 0)
                {
                    attributeLines.Add(firstLine);
                }

                foreach (AttributeInfo attrInfo in list)
                {
                    // Skip attributes already added to first line.
                    if (firstLineList.Contains(attrInfo))
                    {
                        continue;
                    }

                    // Attributes with markup extension, always put on new line
                    if (attrInfo.IsMarkupExtension && this.options.FormatMarkupExtension)
                    {
                        if (currentLineBuffer.Length > 0)
                        {
                            attributeLines.Add(currentLineBuffer.ToString());
                            currentLineBuffer.Length = 0;
                            attributeCountInCurrentLineBuffer = 0;
                        }

                        attributeLines.Add(
                            this.attributeInfoFormatter.ToMultiLineString(attrInfo, attributeIndentationString));
                    }
                    else
                    {
                        string pendingAppend = this.attributeInfoFormatter.ToSingleLineString(attrInfo, xamlLanguageOptions);
                        var actualPendingAppend = this.xmlEscapingService.RestoreXmlnsAliasesBypass(pendingAppend);
                        xmlnsAliasesBypassLengthInCurrentLine += pendingAppend.Length - actualPendingAppend.Length;

                        bool isAttributeCharLengthExceeded = (attributeCountInCurrentLineBuffer > 0)
                            && (this.options.MaxAttributeCharactersPerLine > 0)
                            && ((currentLineBuffer.Length + pendingAppend.Length - xmlnsAliasesBypassLengthInCurrentLine) > this.options.MaxAttributeCharactersPerLine);

                        bool isAttributeCountExceeded = (this.options.MaxAttributesPerLine > 0)
                            && ((attributeCountInCurrentLineBuffer + 1) > this.options.MaxAttributesPerLine);

                        bool isAttributeRuleGroupChanged = this.options.PutAttributeOrderRuleGroupsOnSeparateLines
                            && (lastAttributeInfo != null)
                            && (lastAttributeInfo.OrderRule.Group != attrInfo.OrderRule.Group);

                        if ((currentLineBuffer.Length > 0)
                            && (forceLineBreakInAttributes || isAttributeCharLengthExceeded || isAttributeCountExceeded || isAttributeRuleGroupChanged))
                        {
                            attributeLines.Add(currentLineBuffer.ToString());
                            currentLineBuffer.Length = 0;
                            attributeCountInCurrentLineBuffer = 0;
                            xmlnsAliasesBypassLengthInCurrentLine = 0;
                        }

                        currentLineBuffer.AppendFormat(CultureInfo.InvariantCulture, "{0} ", pendingAppend);
                        attributeCountInCurrentLineBuffer++;
                        xmlnsAliasesBypassLengthInCurrentLine += pendingAppend.Length - actualPendingAppend.Length;
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
                    if ((i == 0) && (this.options.KeepFirstAttributeOnSameLine || (firstLineList.Count > 0)))
                    {
                        output.Append(' ').Append(attributeLines[i].Trim());
                    }
                    else
                    {
                        output.Append(Environment.NewLine)
                            .Append(this.indentService.Normalize(attributeIndentationString + attributeLines[i].Trim()));
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

            if (this.options.OrderAttributesByName)
            {
                if (x.AttributeHasIgnoredNamespace && y.AttributeHasIgnoredNamespace)
                {
                    return String.Compare(x.AttributeNameWithoutNamespace, y.AttributeNameWithoutNamespace, StringComparison.Ordinal);
                }
                // If we have attribute with ignored namespace, we want to compare it by full name
                // if it is compared with analogical attribute without this namespace.
                else if (x.AttributeHasIgnoredNamespace && ! String.Equals(x.AttributeNameWithoutNamespace, y.Name, StringComparison.InvariantCulture))
                {
                    return String.Compare(x.AttributeNameWithoutNamespace, y.Name, StringComparison.Ordinal);
                }
                // If we have attribute with ignored namespace, we want to compare it by full name
                // if it is compared with analogical attribute without this namespace.
                else if (y.AttributeHasIgnoredNamespace && ! String.Equals(y.AttributeNameWithoutNamespace, x.Name, StringComparison.InvariantCulture))
                {
                    return String.Compare(x.Name, y.AttributeNameWithoutNamespace, StringComparison.Ordinal);
                }
                else
                {
                    return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
                }
            }
            return 0;
        }

        private string GetAttributeIndetationString(XmlReader xmlReader)
        {
            if (this.options.AttributeIndentation == 0)
            {
                if (this.options.KeepFirstAttributeOnSameLine)
                {
                    return this.indentService.GetIndentString(xmlReader.Depth, (xmlReader.Name.Length + 2));
                }
                else
                {
                    return this.indentService.GetIndentString(xmlReader.Depth + 1);
                }
            }
            else
            {
                return this.indentService.GetIndentString(xmlReader.Depth, this.options.AttributeIndentation);
            }
        }

        private bool IsFirstLineAttribute(string attributeName)
        {
            return this.firstLineAttributes.Contains(attributeName);
        }

        private bool IsNoLineBreakElement(string elementName)
        {
            return this.noNewLineElementsList.Contains(elementName);
        }
    }
}