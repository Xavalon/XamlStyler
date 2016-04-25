using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XamlStyler.Core.DocumentManipulation;
using XamlStyler.Core.DocumentProcessors;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.MarkupExtensions.Formatter;
using XamlStyler.Core.MarkupExtensions.Parser;
using XamlStyler.Core.Model;
using XamlStyler.Core.Options;
using XamlStyler.Core.Services;

namespace XamlStyler.Core
{
    public class StylerService
    {
        private readonly DocumentManipulationService _documentManipulationService;
        private readonly Dictionary<XmlNodeType, IDocumentProcessor> _documentProcessors;
        private readonly XmlEscapingService _xmlEscapingService;

        public StylerService(IStylerOptions options)
        {
            _xmlEscapingService = new XmlEscapingService();
            _documentManipulationService = new DocumentManipulationService(options);

            var indentService = new IndentService(options.IndentWithTabs, options.IndentSize);
            var markupExtensionFormatter = new MarkupExtensionFormatter(options.NoNewLineMarkupExtensions.ToList());
            var attributeInfoFactory = new AttributeInfoFactory(new MarkupExtensionParser(), new AttributeOrderRules(options));
            var attributeInfoFormatter = new AttributeInfoFormatter(markupExtensionFormatter,indentService);

            _documentProcessors = new Dictionary<XmlNodeType, IDocumentProcessor>
            {
                //{XmlNodeType.None, null},
                {XmlNodeType.Element, new ElementDocumentProcessor(options, attributeInfoFactory, attributeInfoFormatter, indentService)},
                //{XmlNodeType.Attribute, null},
                {XmlNodeType.Text, new TextDocumentProcessor(indentService)},
                {XmlNodeType.CDATA, new CDATADocumentProcessor(indentService)},
                //{XmlNodeType.EntityReference, null},
                //{XmlNodeType.Entity, null},
                {XmlNodeType.ProcessingInstruction, new ProcessInstructionDocumentProcessor(indentService)},
                {XmlNodeType.Comment, new CommentDocumentProcessor(options, indentService)},
                //{XmlNodeType.Document, null},
                //{XmlNodeType.DocumentType, null},
                //{XmlNodeType.DocumentFragment, null},
                //{XmlNodeType.Notation, null},
                {XmlNodeType.Whitespace, new WhitespaceDocumentProcessor()},
                {XmlNodeType.SignificantWhitespace, new SignificantWhitespaceDocumentProcessor()},
                {XmlNodeType.EndElement, new EndElementDocumentProcessor(options,indentService)},
                //{XmlNodeType.EndEntity, null},
                //ignoring xml declarations for Xamarin support
                {XmlNodeType.XmlDeclaration, new XmlDeclarationDocumentProcessor()}
            };
        }

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="xamlSource"></param>
        /// <returns></returns>
        public string StyleDocument(string xamlSource)
        {
            // Escape all xml entity references to ensure that they are output exactly as given.
            var escapedDocument = _xmlEscapingService.EscapeDocument(xamlSource);
            // parse XDocument
            var xDocument = XDocument.Parse(escapedDocument, LoadOptions.PreserveWhitespace);
            // Manipulate the document tree;
            var manipulatedDocument = _documentManipulationService.ManipulateDocument(xDocument);
            // Format it to a string
            var format = Format(manipulatedDocument);
            // Restore escaped xml entity references
            return _xmlEscapingService.UnescapeDocument(format);
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
                        if (_documentProcessors.TryGetValue(xmlReader.NodeType, out processor))
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