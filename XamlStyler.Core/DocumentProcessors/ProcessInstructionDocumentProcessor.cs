// © Xavalon. All rights reserved.

using System;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Parser;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.DocumentProcessors
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
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

            string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);

            if (!output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            output.Append(currentIndentString).Append("<?Mapping ").Append(xmlReader.Value).Append(" ?>");
        }
    }
}