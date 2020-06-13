// (c) Xavalon. All rights reserved.

using Irony.Parsing;
using System;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    internal abstract class MemberNameOrStringTerminal : Terminal
    {
        protected abstract bool IsMemberName { get; }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            return this.MatchQuoted(context, source) ?? this.MatchUnquoted(context, source);
        }

        protected MemberNameOrStringTerminal(string name) : base(name)
        {
        }

        protected Token CreateToken(ISourceStream source, int lastNonWhitespacePosition)
        {
            if (lastNonWhitespacePosition > source.Position)
            {
                // Remember position.
                var position = source.PreviewPosition;

                // Move position back to last non whitespace (or significant whitespace).
                source.PreviewPosition = lastNonWhitespacePosition;
                var token = source.CreateToken(this.OutputTerminal);

                // Restore position.
                source.PreviewPosition = position;
                return token;
            }

            return null;
        }

        private Token MatchQuoted(ParsingContext context, ISourceStream source)
        {
            char quoteChar = source.PreviewChar;
            if ((quoteChar != '\'') && (quoteChar != '"'))
            {
                return null;
            }

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

        private Token MatchUnquoted(ParsingContext context, ISourceStream source)
        {
            if (source.PreviewChar == '{')
            {
                // Member names can't start with {
                if (this.IsMemberName)
                {
                    return null;
                }

                // Check for special {} at start of token indicating that this is a STRING token.
                if (source.NextPreviewChar != '}')
                {
                    return null;
                }

                source.PreviewPosition += 2;
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
                        {
                            return this.CreateToken(source, lastNonWhitespacePosition);
                        }

                        break;

                    case ',':
                        if (runningBraceTotal == 0)
                        {
                            return this.CreateToken(source, lastNonWhitespacePosition);
                        }

                        break;

                    case '=':
                        if (runningBraceTotal == 0)
                        {
                            // Equal sign. Only allowed after MemberNames.
                            return this.IsMemberName
                                ? this.CreateToken(source, lastNonWhitespacePosition)
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
                {
                    lastNonWhitespacePosition = source.PreviewPosition;
                }
            }

            return context.CreateErrorToken("Unterminated string terminal");
        }
    }
}