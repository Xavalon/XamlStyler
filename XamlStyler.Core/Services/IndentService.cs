using System;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Core.Services
{
    public class IndentService
    {
        private readonly bool _indentWithTabs;
        private readonly int _indentSize;
        private readonly AttributeIndentationStyle _attributeIndentationStyle;

        public IndentService(IStylerOptions options)
        {
            _indentWithTabs = options.IndentWithTabs;
            _indentSize = options.IndentSize;
            _attributeIndentationStyle = options.AttributeIndentationStyle;
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
                switch (_attributeIndentationStyle)
                {
                    case AttributeIndentationStyle.Mixed:
                        return new string('\t', depth + (additionalSpaces / _indentSize)) + new string(' ', (additionalSpaces % _indentSize));
                    case AttributeIndentationStyle.Spaces:
                        return new string('\t', depth) + new string(' ', additionalSpaces);
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

            return new string(' ', (depth*_indentSize) + additionalSpaces);
        }

        /// <summary>
        /// Replace blocks of "indentsize" consecutive spaces with a tab, but only at the beginning of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string Normalize(string line)
        {
            // Only do this if indenting attributes with mixed tabs & spaces
            if (_indentWithTabs && _attributeIndentationStyle == AttributeIndentationStyle.Mixed)
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