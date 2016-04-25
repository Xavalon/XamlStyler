using System;
using System.Text;
using System.Xml;
using XamlStyler.Core.Extensions;
using XamlStyler.Core.Parser;
using XamlStyler.Core.Services;

namespace XamlStyler.Core.DocumentProcessors
{
    internal class ProcessInstructionDocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService _indentService;

        public ProcessInstructionDocumentProcessor(IndentService indentService)
        {
            _indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

            string currentIndentString = _indentService.GetIndentString(xmlReader.Depth);

            if (!output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            output.Append(currentIndentString).Append("<?Mapping ").Append(xmlReader.Value).Append(" ?>");
        }
    }
}