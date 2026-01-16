// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;

namespace Xavalon.XamlStyler.Parser
{
    /// <summary>
    /// Parses the original XAML source to extract format metadata that is lost during XmlReader processing.
    /// </summary>
    public class OriginalFormatParser
    {
        private readonly Dictionary<string, Queue<OriginalFormatInfo>> elementFormatInfosByPath;
        private readonly Dictionary<string, Queue<OriginalFormatInfo>> elementFormatInfosByName;

        public OriginalFormatParser(string xamlSource)
        {
            this.elementFormatInfosByPath = new Dictionary<string, Queue<OriginalFormatInfo>>(StringComparer.Ordinal);
            this.elementFormatInfosByName = new Dictionary<string, Queue<OriginalFormatInfo>>(StringComparer.Ordinal);
            this.ParseOriginalFormat(xamlSource);
        }

        /// <summary>
        /// Gets the next format info for the specified element name.
        /// </summary>
        public OriginalFormatInfo GetNextFormatInfo(string elementName, string pathKey)
        {
            if (!string.IsNullOrEmpty(pathKey)
                && this.elementFormatInfosByPath.TryGetValue(pathKey, out var pathQueue)
                && pathQueue.Count > 0)
            {
                return pathQueue.Dequeue();
            }

            if (this.elementFormatInfosByName.TryGetValue(elementName, out var nameQueue) && nameQueue.Count > 0)
            {
                return nameQueue.Dequeue();
            }

            return null;
        }

        private void ParseOriginalFormat(string xamlSource)
        {
            if (string.IsNullOrEmpty(xamlSource))
            {
                return;
            }

            var pathStack = new Stack<string>();
            int index = 0;

            while (index < xamlSource.Length)
            {
                if (xamlSource[index] != '<')
                {
                    index++;
                    continue;
                }

                if (this.StartsWith(xamlSource, index, "<!--"))
                {
                    index = this.SkipUntil(xamlSource, index + 4, "-->");
                    continue;
                }

                if (this.StartsWith(xamlSource, index, "<?"))
                {
                    index = this.SkipUntil(xamlSource, index + 2, "?>");
                    continue;
                }

                if (this.StartsWith(xamlSource, index, "<![CDATA["))
                {
                    index = this.SkipUntil(xamlSource, index + 9, "]]>" );
                    continue;
                }

                if (this.StartsWith(xamlSource, index, "<!DOCTYPE"))
                {
                    index = this.SkipToChar(xamlSource, index + 9, '>') + 1;
                    continue;
                }

                if (this.StartsWith(xamlSource, index, "</"))
                {
                    index += 2;
                    this.ReadName(xamlSource, ref index);

                    if (pathStack.Count > 0)
                    {
                        pathStack.Pop();
                    }

                    index = this.SkipToChar(xamlSource, index, '>') + 1;
                    continue;
                }

                if (index + 1 < xamlSource.Length && xamlSource[index + 1] == '!')
                {
                    index = this.SkipToChar(xamlSource, index + 2, '>') + 1;
                    continue;
                }

                // Start tag
                index++;
                string elementName = this.ReadName(xamlSource, ref index);
                if (string.IsNullOrEmpty(elementName))
                {
                    continue;
                }

                string currentPathKey = pathStack.Count == 0
                    ? elementName
                    : $"{pathStack.Peek()}/{elementName}";

                var formatInfo = new OriginalFormatInfo
                {
                    ElementName = elementName
                };

                this.ParseAttributesAndTagEnd(xamlSource, ref index, formatInfo, out bool isSelfClosing);
                formatInfo.IsSelfClosing = isSelfClosing;

                this.AddFormatInfo(formatInfo, currentPathKey, elementName);

                if (!isSelfClosing)
                {
                    pathStack.Push(currentPathKey);
                }
            }
        }

        private void ParseAttributesAndTagEnd(string source, ref int index, OriginalFormatInfo formatInfo, out bool isSelfClosing)
        {
            isSelfClosing = false;
            bool sawLineBreak = false;
            int attributeIndex = 0;

            while (index < source.Length)
            {
                this.SkipWhitespace(source, ref index, ref sawLineBreak);

                if (index >= source.Length)
                {
                    break;
                }

                if (source[index] == '>')
                {
                    index++;
                    break;
                }

                if (source[index] == '/' && (index + 1 < source.Length) && source[index + 1] == '>')
                {
                    isSelfClosing = true;
                    index += 2;
                    break;
                }

                string attributeName = this.ReadName(source, ref index);
                if (string.IsNullOrEmpty(attributeName))
                {
                    index++;
                    continue;
                }

                if (sawLineBreak)
                {
                    formatInfo.AttributeLineBreakIndices.Add(attributeIndex);
                }

                attributeIndex++;
                sawLineBreak = false;

                this.SkipWhitespace(source, ref index, ref sawLineBreak);

                if (index < source.Length && source[index] == '=')
                {
                    index++;
                }

                this.SkipWhitespace(source, ref index, ref sawLineBreak);

                if (index < source.Length && (source[index] == '"' || source[index] == '\''))
                {
                    char quote = source[index];
                    index++;
                    while (index < source.Length && source[index] != quote)
                    {
                        index++;
                    }

                    if (index < source.Length)
                    {
                        index++;
                    }
                }
                else
                {
                    while (index < source.Length && !char.IsWhiteSpace(source[index]) && source[index] != '>' && source[index] != '/')
                    {
                        index++;
                    }
                }
            }

            formatInfo.AttributeCount = attributeIndex;
        }

        private void AddFormatInfo(OriginalFormatInfo formatInfo, string pathKey, string elementName)
        {
            if (!string.IsNullOrEmpty(pathKey))
            {
                if (!this.elementFormatInfosByPath.TryGetValue(pathKey, out var pathQueue))
                {
                    pathQueue = new Queue<OriginalFormatInfo>();
                    this.elementFormatInfosByPath[pathKey] = pathQueue;
                }

                pathQueue.Enqueue(formatInfo);
            }

            if (!this.elementFormatInfosByName.TryGetValue(elementName, out var nameQueue))
            {
                nameQueue = new Queue<OriginalFormatInfo>();
                this.elementFormatInfosByName[elementName] = nameQueue;
            }

            nameQueue.Enqueue(formatInfo);
        }

        private void SkipWhitespace(string source, ref int index, ref bool sawLineBreak)
        {
            while (index < source.Length && char.IsWhiteSpace(source[index]))
            {
                if (source[index] == '\r' || source[index] == '\n')
                {
                    sawLineBreak = true;

                    if (source[index] == '\r' && (index + 1 < source.Length) && source[index + 1] == '\n')
                    {
                        index++;
                    }
                }

                index++;
            }
        }

        private string ReadName(string source, ref int index)
        {
            int start = index;

            while (index < source.Length)
            {
                char c = source[index];
                if (char.IsWhiteSpace(c) || c == '>' || c == '/' || c == '=' || c == '?')
                {
                    break;
                }

                index++;
            }

            if (index == start)
            {
                return string.Empty;
            }

            return source.Substring(start, index - start);
        }

        private bool StartsWith(string source, int index, string value)
        {
            if (index + value.Length > source.Length)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (source[index + i] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        private int SkipUntil(string source, int index, string token)
        {
            int tokenLength = token.Length;

            while (index + tokenLength <= source.Length)
            {
                if (this.StartsWith(source, index, token))
                {
                    return index + tokenLength;
                }

                index++;
            }

            return source.Length;
        }

        private int SkipToChar(string source, int index, char target)
        {
            while (index < source.Length && source[index] != target)
            {
                index++;
            }

            return index;
        }
    }
}
