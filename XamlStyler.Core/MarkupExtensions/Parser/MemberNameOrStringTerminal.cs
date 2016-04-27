using System;
using Irony.Parsing;

namespace Xavalon.XamlStyler.Core.MarkupExtensions.Parser
{
    internal abstract class MemberNameOrStringTerminal : Terminal
    {
        protected MemberNameOrStringTerminal(string name) : base(name)
        {
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            return MatchQuoted(context, source) ?? MatchUnquoted(context, source);
        }

        private Token MatchQuoted(ParsingContext context, ISourceStream source)
        {
            char quoteChar = source.PreviewChar;
            if (quoteChar != '\'' && quoteChar != '"')
                return null;

            source.PreviewPosition++;
            while (!source.EOF())
            {
                if (source.PreviewChar == quoteChar)
                {
                    source.PreviewPosition++;
                    return source.CreateToken(this.OutputTerminal);
                }

                // Escaped?
                if (source.PreviewChar == '\\')
                {
                    // Consume next
                    ++source.PreviewPosition;
                }
                source.PreviewPosition++;
            }

            return context.CreateErrorToken("Unbalanced quoted string");
        }

        protected abstract bool IsMemberName { get; }

        private Token MatchUnquoted(ParsingContext context, ISourceStream source)
        {
            if (source.PreviewChar == '{')
            {
                // Member names can't start with {
                if (IsMemberName) return null;
                // Check for special {} at start of token indicating that this is a STRING token.
                if (source.NextPreviewChar != '}') return null;
                source.PreviewPosition +=2;
            }

            var runningBraceTotal = 0;
            // This variable tracks the position of the last non whitespace (or significant whitespace).
            var lastNonWhitespacePosition = source.PreviewPosition;
            while (!source.EOF())
            {
                bool isWhiteSpace = false;
                switch (source.PreviewChar)
                {
                    case '{':
                        runningBraceTotal++;
                        break;
                    case '}':
                        if (--runningBraceTotal < 0)
                            return CreateToken(source, lastNonWhitespacePosition);
                        break;
                    case ',':
                        if (runningBraceTotal == 0)
                            return CreateToken(source, lastNonWhitespacePosition);
                        break;
                    case '=':
                        if (runningBraceTotal == 0)
                        {
                            // Equal sign. Only allowed after MemberNames.
                            return IsMemberName
                                ? CreateToken(source, lastNonWhitespacePosition)
                                : null;
                        }
                        break;
                    case '\\':
                        source.PreviewPosition++;
                        break;
                    default:
                        isWhiteSpace = Char.IsWhiteSpace(source.PreviewChar);
                        break;
                }

                source.PreviewPosition++;

                if (!isWhiteSpace)
                    lastNonWhitespacePosition = source.PreviewPosition;
            }

            return context.CreateErrorToken("Unterminated string terminal");
        }

        protected Token CreateToken(ISourceStream source, int lastNonWhitespacePosition)
        {
            if (lastNonWhitespacePosition > source.Position)
            {
                // Remember position
                var pos = source.PreviewPosition;

                // Move position back to last non whitespace (or significant whitespace)
                source.PreviewPosition = lastNonWhitespacePosition;
                var token = source.CreateToken(this.OutputTerminal);

                // Restore position
                source.PreviewPosition = pos;
                return token;
            }
            return null;
        }
    }
}