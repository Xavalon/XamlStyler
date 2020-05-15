// (c) Xavalon. All rights reserved.

using System;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.Parser;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class ProcessInstructionDocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService indentService;

        public ProcessInstructionDocumentProcessor(IndentService indentService)
        {
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.Mixed);

            string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);

            if (!output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            output.Append($"{currentIndentString}<?{xmlReader.Name} {xmlReader.Value}?>");
        }
    }
}