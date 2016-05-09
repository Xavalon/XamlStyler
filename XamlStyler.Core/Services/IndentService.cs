// © Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.Core.Services
{
    public class IndentService
    {
        private readonly bool indentWithTabs;
        private readonly int indentSize;

        public IndentService(bool indentWithTabs, int indentSize)
        {
            this.indentWithTabs = indentWithTabs;
            this.indentSize = indentSize;
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

            return new string(' ', depth * this.indentSize);
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
                return new string('\t', depth + (additionalSpaces / this.indentSize)) + new string(' ', (additionalSpaces % this.indentSize));
            }

            return new string(' ', (depth * this.indentSize) + additionalSpaces);
        }

        /// <summary>
        /// Replace blocks of "indentsize" consecutive spaces with a tab, but only at the beginning of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string Normalize(string line)
        {
            // Only do this if indenting with tabs
            if (this.indentWithTabs)
            {
                int runningSpaces = 0;
                for (int pos = 0; pos < line.Length; pos++)
                {
                    switch (line[pos])
                    {
                        case ' ':
                            runningSpaces++;
                            if (runningSpaces == this.indentSize)
                            {
                                line = line.Substring(0, pos + 1 - runningSpaces) + '\t' + line.Substring(pos + 1);
                                pos -= runningSpaces - 1;
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