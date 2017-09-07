// © Xavalon. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.DocumentManipulation;
using Xavalon.XamlStyler.Core.DocumentProcessors;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.MarkupExtensions.Formatter;
using Xavalon.XamlStyler.Core.MarkupExtensions.Parser;
using Xavalon.XamlStyler.Core.Model;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core
{
    public class StylerService
    {
        private readonly DocumentManipulationService documentManipulationService;
        private readonly Dictionary<XmlNodeType, IDocumentProcessor> documentProcessors;
        private readonly XmlEscapingService xmlEscapingService;

        public StylerService(IStylerOptions options)
        {
            this.xmlEscapingService = new XmlEscapingService();
            this.documentManipulationService = new DocumentManipulationService(options);

            var indentService = new IndentService(options);
            var markupExtensionFormatter = new MarkupExtensionFormatter(options.NoNewLineMarkupExtensions.ToList());
            var attributeInfoFactory = new AttributeInfoFactory(new MarkupExtensionParser(), new AttributeOrderRules(options));
            var attributeInfoFormatter = new AttributeInfoFormatter(markupExtensionFormatter, indentService);

            this.documentProcessors = new Dictionary<XmlNodeType, IDocumentProcessor>
            {
                // { XmlNodeType.None, null },
                { XmlNodeType.Element, new ElementDocumentProcessor(options, attributeInfoFactory, attributeInfoFormatter, indentService) },
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

                // Format it to a string.
                var format = this.Format(manipulatedDocument);

                // Restore escaped xml entity references.
                xamlOutput = this.xmlEscapingService.UnescapeDocument(format);
            }

            return xamlOutput;
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