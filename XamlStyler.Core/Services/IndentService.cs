namespace Xavalon.XamlStyler.Core.Services
{
    public class IndentService
    {
        private readonly bool _indentWithTabs;
        private readonly int _indentSize;

        public IndentService(bool indentWithTabs, int indentSize)
        {
            _indentWithTabs = indentWithTabs;
            _indentSize = indentSize;
        }

        public string GetIndentString(int depth)
        {
            if (depth < 0) depth = 0;

            if (_indentWithTabs)
            {
                return new string('\t', depth);
            }

            return new string(' ', depth * _indentSize);
        }

        public string GetIndentString(int depth, int additionalSpaces)
        {
            if (depth < 0) depth = 0;
            if (additionalSpaces < 0) additionalSpaces = 0;

            if (_indentWithTabs)
            {
                return new string('\t', depth + (additionalSpaces / _indentSize)) + new string(' ', (additionalSpaces % _indentSize));
            }

            return new string(' ', (depth * _indentSize) + additionalSpaces);
        }

        /// <summary>
        /// Replace blocks of "indentsize" consecutive spaces with a tab, but only at the beginning of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string Normalize(string line)
        {
            // Only do this if indenting with tabs
            if (_indentWithTabs)
            {
                int runningSpaces = 0;
                for (int pos = 0; pos < line.Length; pos++)
                {
                    switch (line[pos])
                    {
                        case ' ':
                            runningSpaces++;
                            if (runningSpaces == _indentSize)
                            {
                                line = line.Substring(0, pos + 1 - runningSpaces) + '\t' + line.Substring(pos + 1);
                                pos -= runningSpaces - 1;
                                runningSpaces = 0;
                            }
                            break;

                        case '\t':
                            if (runningSpaces != 0)
                                return line;
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