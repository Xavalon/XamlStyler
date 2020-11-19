// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xavalon.XamlStyler.DocumentManipulation;
using Xavalon.XamlStyler.DocumentProcessors;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.MarkupExtensions.Formatter;
using Xavalon.XamlStyler.MarkupExtensions.Parser;
using Xavalon.XamlStyler.Model;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler
{
    public class StylerService
    {
        private readonly DocumentManipulationService documentManipulationService;
        private readonly IStylerOptions options;
        private readonly XmlEscapingService xmlEscapingService;
        private Dictionary<XmlNodeType, IDocumentProcessor> documentProcessors;
        private IList<string> ignoredNamespaces;

        public StylerService(IStylerOptions options)
        {
            this.xmlEscapingService = new XmlEscapingService();
            this.documentManipulationService = new DocumentManipulationService(options);

            this.options = options;
        }

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="xamlSource"></param>
        /// <returns></returns>
        public string StyleDocument(string xamlSource)
        {
            string xamlOutput = xamlSource;

            if (this.documentManipulationService.AllowProcessing)
            {
                // Escape all xml entity references to ensure that they are output exactly as given.
                var escapedDocument = this.xmlEscapingService.EscapeDocument(xamlSource);

                // Parse XDocument.
                var xDocument = XDocument.Parse(escapedDocument, LoadOptions.PreserveWhitespace);

                // Manipulate the document tree.
                var manipulatedDocument = this.documentManipulationService.ManipulateDocument(xDocument);

                // Find ignored namespaces in document.
                var ignoredNamespacesLocalNames = FindIgnoredNamespaces(manipulatedDocument, options.IgnoredNamespacesInOrdering);
                
                // Once we have ignored namespaces from first element,
                // we can apply styler configuration.
                ApplyOptions(ignoredNamespacesLocalNames);

                // Format it to a string.
                var format = this.Format(manipulatedDocument);

                // Restore escaped xml entity references.
                xamlOutput = this.xmlEscapingService.UnescapeDocument(format);
            }

            return xamlOutput;
        }

        private void ApplyOptions(IList<string> ignoredNamespacesLocalNames)
        {
            this.ignoredNamespaces = options.IgnoredNamespacesInOrdering;

            var indentService = new IndentService(options);
            var markupExtensionFormatter = new MarkupExtensionFormatter(options.NoNewLineMarkupExtensions.ToList());
            var attributeInfoFactory = new AttributeInfoFactory(new MarkupExtensionParser(), new AttributeOrderRules(options), ignoredNamespacesLocalNames);
            var attributeInfoFormatter = new AttributeInfoFormatter(markupExtensionFormatter, indentService);

            this.documentProcessors = new Dictionary<XmlNodeType, IDocumentProcessor>
            {
                // { XmlNodeType.None, null },
                { XmlNodeType.Element, new ElementDocumentProcessor(options, attributeInfoFactory, attributeInfoFormatter, indentService, xmlEscapingService) },
                // { XmlNodeType.Attribute, null },
                { XmlNodeType.Text, new TextDocumentProcessor(indentService) },
                { XmlNodeType.CDATA, new CDATADocumentProcessor(indentService) },
                // { XmlNodeType.EntityReference, null },
                // { XmlNodeType.Entity, null },
                { XmlNodeType.ProcessingInstruction, new ProcessInstructionDocumentProcessor(indentService) },
                { XmlNodeType.Comment, new CommentDocumentProcessor(options, indentService) },
                // { XmlNodeType.Document, null },
                // { XmlNodeType.DocumentType, null },
                // { XmlNodeType.DocumentFragment, null },
                // { XmlNodeType.Notation, null },
                { XmlNodeType.Whitespace, new WhitespaceDocumentProcessor() },
                { XmlNodeType.SignificantWhitespace, new SignificantWhitespaceDocumentProcessor() },
                { XmlNodeType.EndElement, new EndElementDocumentProcessor(options,indentService) },
                // { XmlNodeType.EndEntity, null },
                // ignoring xml declarations for Xamarin support
                { XmlNodeType.XmlDeclaration, new XmlDeclarationDocumentProcessor() }
            };
        }

        private static IList<string> FindIgnoredNamespaces(string xamlSource, string[] ignoredNamespaces)
        {
            using (var sourceReader = new StringReader(xamlSource))
            {
                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
                    // Try read first element
                    while (!xmlReader.Read() || xmlReader.NodeType != XmlNodeType.Element) { }
                    // Did not find any elements.
                    if (xmlReader.EOF)
                    {
                        return Array.Empty<string>();
                    }
                    // Try to move to first attribute.
                    if (!xmlReader.MoveToFirstAttribute())
                    {
                        // First element do not have any attributes.
                        return Array.Empty<string>();
                    }

                    IList<string> ignoredNamespacesLocalNames = new List<string>();
                    while (xmlReader.MoveToNextAttribute())
                    {
                        // Full namespace URI, it's stored in Value property.
                        var localName = xmlReader.LocalName;
                        var namespaceUri = xmlReader.Value.Replace('[' + localName + ']', "");

                        if (ignoredNamespaces.Contains(namespaceUri))
                        {
                            ignoredNamespacesLocalNames.Add(localName);
                        }
                    }

                    return ignoredNamespacesLocalNames;
                }
            }
        }

        private string Format(string xamlSource)
        {
            StringBuilder output = new StringBuilder();

            using (var sourceReader = new StringReader(xamlSource))
            {
                using (XmlReader xmlReader = XmlReader.Create(sourceReader))
                {
                    var elementProcessContext = new ElementProcessContext();

                    while (xmlReader.Read())
                    {
                        IDocumentProcessor processor;
                        if (this.documentProcessors.TryGetValue(xmlReader.NodeType, out processor))
                        {
                            processor.Process(xmlReader, output, elementProcessContext);
                        }
                        else
                        {
                            Trace.WriteLine($"Unprocessed NodeType: {xmlReader.NodeType} Name: {xmlReader.Name} Value: {xmlReader.Value}");
                        }
                    }
                }
            }

            return output.ToString();
        }
    }
}