using System;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Core.Parser;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.DocumentProcessors
{
    internal class CommentDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions _options;
        private readonly IndentService _indentService;

        public CommentDocumentProcessor(IStylerOptions options, IndentService indentService)
        {
            _options = options;
            _indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MIXED);

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
                output.Append(currentIndentString).Append("<!--").Append(content.Trim()).Append("-->");
            }
            else if (content.Contains("\n"))
            {
                output.Append(currentIndentString).Append("<!--");

                var contentIndentString = _indentService.GetIndentString(xmlReader.Depth + 1);
                foreach (var line in content.Trim().GetLines())
                {
                    output.Append(Environment.NewLine).Append(contentIndentString).Append(line.Trim());
                }

                output.Append(Environment.NewLine).Append(currentIndentString).Append("-->");
            }
            else
            {
                output
                    .Append(currentIndentString)
                    .Append("<!--")
                    .Append(' ', _options.CommentSpaces)
                    .Append(content.Trim())
                    .Append(' ', _options.CommentSpaces).Append("-->");
            }
        }
    }
}