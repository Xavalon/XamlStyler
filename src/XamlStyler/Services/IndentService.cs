// (c) Xavalon. All rights reserved.

using System;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Services
{
    public class IndentService
    {
        private readonly bool indentWithTabs;
        private readonly int indentSize;
        private readonly AttributeIndentationStyle attributeIndentationStyle;

        public IndentService(IStylerOptions options)
        {
            this.indentWithTabs = options.IndentWithTabs ?? false;
            this.indentSize = options.IndentSize;
            this.attributeIndentationStyle = options.AttributeIndentationStyle;
        }

        public string GetIndentString(int depth)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if (this.indentWithTabs)
            {
                return new string('\t', depth);
            }

            return new string(' ', (depth * this.indentSize));
        }

        public string GetIndentString(int depth, int additionalSpaces)
        {
            if (depth < 0)
            {
                depth = 0;
            }

            if (additionalSpaces < 0)
            {
                additionalSpaces = 0;
            }

            if (this.indentWithTabs)
            {
                switch (this.attributeIndentationStyle)
                {
                    case AttributeIndentationStyle.Mixed:
                        return new String('\t', depth + (additionalSpaces / this.indentSize)) + new String(' ', (additionalSpaces % this.indentSize));
                    case AttributeIndentationStyle.Spaces:
                        return new String('\t', depth) + new String(' ', additionalSpaces);
                    default:
                        throw new NotImplementedException();
                }

            }

            return new String(' ', (depth * this.indentSize) + additionalSpaces);
        }

        /// <summary>
        /// Replace blocks of "indentsize" consecutive spaces with a tab, but only at the beginning of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string Normalize(string line)
        {
            // Only do this if indenting attributes with mixed tabs & spaces
            if (this.indentWithTabs && (this.attributeIndentationStyle == AttributeIndentationStyle.Mixed))
            {
                int runningSpaces = 0;
                for (int position = 0; position < line.Length; position++)
                {
                    switch (line[position])
                    {
                        case ' ':
                            runningSpaces++;
                            if (runningSpaces == this.indentSize)
                            {
                                line = line.Substring(0, position + 1 - runningSpaces)
                                    + '\t' + line.Substring(position + 1);
                                position -= runningSpaces - 1;
                                runningSpaces = 0;
                            }

                            break;

                        case '\t':
                            if (runningSpaces != 0)
                            {
                                return line;
                            }

                            break;

                        default:
                            return line;
                    }
                }
            }

            return line;
        }
    }
}