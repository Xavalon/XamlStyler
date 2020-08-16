// (c) Xavalon. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.DocumentProcessors
{
    internal class WhitespaceDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;

        public WhitespaceDocumentProcessor(IStylerOptions options)
        {
            this.options = options;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            var hasNewline = xmlReader.Value.Contains('\n');

            if (elementProcessContext.Current.IsSignificantWhiteSpace && hasNewline)
            {
                elementProcessContext.Current.IsSignificantWhiteSpace = false;
            }

            if (hasNewline && !elementProcessContext.Current.IsPreservingSpace)
            {
                // For WhiteSpaces contain linefeed, trim all spaces/tab，
                // since the intent of this whitespace node is to break line,
                // and preserve the line feeds
                output.Append(xmlReader.Value
                    .Replace(" ", String.Empty)
                    .Replace("\t", String.Empty)
                    .Replace("\r", String.Empty)
                    .Replace("\n", options.NewLine));
            }
            else
            {
                // Preserve "pure" WhiteSpace between elements
                // e.g.,
                //   <TextBlock>
                //     <Run>A</Run> <Run>
                //      B
                //     </Run>
                //  </TextBlock>
                output.Append(xmlReader.Value.Replace("\n", options.NewLine));
            }
        }
    }
}