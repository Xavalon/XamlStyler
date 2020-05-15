// (c) Xavalon. All rights reserved.

using System;
using Irony.Parsing;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    public class MarkupExtensionParser : IMarkupExtensionParser
    {
        private readonly Irony.Parsing.Parser parser;

#if DEBUG
        public ParseTree LastParseTree { get; private set; }

        public Exception LastException { get; private set; }

#endif

        public MarkupExtensionParser()
        {
            var grammar = new XamlMarkupExtensionGrammar();
            var language = new LanguageData(grammar);
            this.parser = new Irony.Parsing.Parser(language)
#if DEBUG
            {
                Context = { TracingEnabled = true }
            }
#endif
            ;
        }

        public bool TryParse(string sourceText, out MarkupExtension graph)
        {
            graph = null;

            try
            {
                ParseTree tree = this.parser.Parse(sourceText);
#if DEBUG
                // Save result tree for debugging purposes.
                this.LastParseTree = tree;
                this.LastException = null;
#endif
                if (tree.Status == ParseTreeStatus.Parsed)
                {
                    graph = MarkupExtension.Create(tree.Root);
                    return true;
                }
            }
#if DEBUG
            catch (Exception ex)
            {
                this.LastParseTree = null;
                this.LastException = ex;
            }
#else
            catch
            {
                // ignored
            }
#endif
            return false;
        }
    }
}