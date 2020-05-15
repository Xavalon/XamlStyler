// (c) Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Extensions;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Parser;
using Xavalon.XamlStyler.Services;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class CommentDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;
        private readonly IndentService indentService;

        public CommentDocumentProcessor(IStylerOptions options, IndentService indentService)
        {
            this.options = options;
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.Mixed);

            string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value;

            if ((output.Length > 0) && !output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            if (content.Contains("<") && content.Contains(">"))
            {
                output.Append(currentIndentString);
                output.Append("<!--");

                if (content.Contains("\n"))
                {
                    output.Append(String.Join(Environment.NewLine, content.GetLines().Select(_ => _.TrimEnd(' '))));

                    if (content.TrimEnd(' ').EndsWith("\n", StringComparison.Ordinal))
                    {
                        output.Append(currentIndentString);
                    }
                }
                else
                {
                    output.Append(content);
                }

                output.Append("-->");
            }
            else if (content.Contains("#region") || content.Contains("#endregion"))
            {
                output.Append(currentIndentString).Append("<!--").Append(content.Trim()).Append("-->");
            }
            else if (content.Contains("\n"))
            {
                output.Append(currentIndentString).Append("<!--");

                var contentIndentString = this.indentService.GetIndentString(xmlReader.Depth + 1);
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
                    .Append(' ', this.options.CommentSpaces)
                    .Append(content.Trim())
                    .Append(' ', this.options.CommentSpaces)
                    .Append("-->");
            }
        }
    }
}