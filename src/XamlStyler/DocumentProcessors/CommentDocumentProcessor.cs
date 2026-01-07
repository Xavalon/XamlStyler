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
    /// <summary>
    /// Formats XML comment nodes emitted by the XAML writer.
    ///
    /// This processor preserves the original formatting behavior by default,
    /// but can optionally expand "commented-out XAML" into a multi-line block comment
    /// when <see cref="IStylerOptions.TreatCommentWithTagAsMultiline"/> is enabled.
    /// </summary>
    internal class CommentDocumentProcessor : IDocumentProcessor
    {
        private readonly IStylerOptions options;
        private readonly IndentService indentService;

        public CommentDocumentProcessor(IStylerOptions options, IndentService indentService)
        {
            this.options = options;
            this.indentService = indentService;
        }

        /// <summary>
        /// Represents the internal state used while formatting a commented-out XAML block.
        /// </summary>
        private enum PendingState
        {
            /// <summary>No pending alignment context.</summary>
            None,
            /// <summary>
            /// A start tag has begun but has not yet been closed,
            /// so subsequent attribute lines must be formatted with hanging alignment.
            /// </summary>
            OpenStartTagAligned
        }

        /// <summary>
        /// Holds state for formatting a multi-line comment containing XAML tags.
        /// </summary>
        private sealed class CommentFormatState
        {
            /// <summary>Current pending mode.</summary>
            public PendingState State = PendingState.None;

            /// <summary>
            /// Hanging alignment prefix for attributes (e.g. "&lt;Image " => 7 spaces).
            /// Used only when <see cref="State"/> is <see cref="PendingState.OpenStartTagAligned"/>.
            /// </summary>
            public string AlignPrefix = string.Empty;
        }

        /// <summary>
        /// Entry point called by the styling pipeline when an XML comment node is encountered.
        ///
        /// - Keeps all existing behavior unchanged by default.
        /// - When TreatCommentWithTagAsMultiline is enabled, expands commented-out XAML into:
        ///   <!--
        ///     ...
        ///   -->
        /// - Also expands single-line tag comments (e.g. &lt;!--&lt;Setter ... /&gt;--&gt;) into a block form.
        /// </summary>
        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypes.Mixed);

            string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
            string content = xmlReader.Value;

            // Ensure a line break before writing a comment if the output is not currently at a newline.
            if ((output.Length > 0) && !output.IsNewLine())
            {
                output.Append(Environment.NewLine);
            }

            // "Tag-looking" comment content
            if (content.Contains("<") && content.Contains(">"))
            {
                // Multi-line commented-out XAML block
                bool isMultilineCommentWithTag =
                    this.options.TreatCommentWithTagAsMultiline
                    && !string.IsNullOrEmpty(content)
                    && content.StartsWith("<", StringComparison.Ordinal)
                    && (content.IndexOf('\n') >= 0 || content.IndexOf('\r') >= 0);

                if (isMultilineCommentWithTag)
                {
                    WriteCommentWithTagAsBlock(xmlReader, output, currentIndentString, content);
                    return;
                }

                // Single-line tag comment (e.g. <!--<Setter ... />-->)
                bool isSingleLineCommentWithTag =
                    this.options.TreatCommentWithTagAsMultiline
                    && !string.IsNullOrEmpty(content)
                    && content.StartsWith("<", StringComparison.Ordinal)
                    && (content.IndexOf('\n') < 0 && content.IndexOf('\r') < 0);

                if (isSingleLineCommentWithTag)
                {
                    output.Append(currentIndentString).Append("<!--").Append(Environment.NewLine);
                    output.Append(currentIndentString).Append(content.Trim()).Append(Environment.NewLine);
                    output.Append(currentIndentString).Append("-->");
                    return;
                }

                // Original behavior (must remain unchanged)
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
                FixIndentBeforeCommentClose(output, this.options, currentIndentString);
            }
            // Region directives are kept as a compact single-line comment.
            else if (content.Contains("#region") || content.Contains("#endregion"))
            {
                output.Append(currentIndentString).Append("<!--").Append(content.Trim()).Append("-->");
            }
            // Multi-line plain comment (without tags): keep existing block style.
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
            // Single-line plain comment: keep existing inline form.
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

        // =====================================================================
        // State machine version (structured, readable, extensible)
        // =====================================================================
        /// <summary>
        /// Formats a multi-line comment that contains XAML tags into a well-indented block comment.
        ///
        /// This uses a small state machine to support:
        /// - hanging-aligned attribute lines for open start tags (e.g. &lt;Image Width="20")
        /// - reconstructing the special two-line pattern:
        ///     &lt;Image
        ///       Width="20" Height="20" ... /&gt;
        ///   into the expected aligned multi-line representation.
        ///
        /// The output format is:
        ///   <!--
        ///   <tag ...>
        ///   -->
        /// </summary>
        private void WriteCommentWithTagAsBlock(
            XmlReader xmlReader,
            StringBuilder output,
            string currentIndentString,
            string content)
        {
            output.Append(currentIndentString).Append("<!--").Append(Environment.NewLine);

            var oneIndent = this.indentService.GetIndentString(1);

            // Normalize lines (LINQ-free)
            var lines = new System.Collections.Generic.List<string>(32);
            foreach (var l in content.GetLines())
            {
                var t = l.TrimEnd();
                if (string.IsNullOrWhiteSpace(t)) continue;
                lines.Add(t);
            }

            var st = new CommentFormatState();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].TrimStart();

                // 1) If we are in "open start tag" mode, attribute lines must be aligned
                if (st.State == PendingState.OpenStartTagAligned)
                {
                    if (!line.StartsWith("<", StringComparison.Ordinal) && line.IndexOf('=') >= 0)
                    {
                        bool closedHere;
                        var attrs = ExtractAttributes(line, out closedHere);

                        if (attrs.Length == 0)
                        {
                            // fallback: treat as regular text line
                            output.Append(currentIndentString).Append(oneIndent).Append(line).Append(Environment.NewLine);

                            if (EndsWithTagClose(line))
                                ResetState(st);

                            continue;
                        }

                        for (int ai = 0; ai < attrs.Length; ai++)
                        {
                            output.Append(currentIndentString).Append(st.AlignPrefix).Append(attrs[ai]).Append(Environment.NewLine);
                        }

                        if (closedHere)
                            ResetState(st);

                        continue;
                    }

                    // If a new tag starts while pending, reset (safe side)
                    if (line.StartsWith("<", StringComparison.Ordinal))
                    {
                        ResetState(st);
                        // fall through to process as a normal line
                    }
                }

                // 2) "<Tag" header + next line attributes (two-line pattern)
                if (IsStartTagHeaderOnly(line) && i + 1 < lines.Count)
                {
                    var next = lines[i + 1].TrimStart();

                    if (!next.StartsWith("<", StringComparison.Ordinal) && next.IndexOf('=') >= 0)
                    {
                        string tagName = TryGetTagNameFromStartTagLine(line);
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            bool closedHere;
                            var attrs = ExtractAttributes(next, out closedHere);
                            if (attrs.Length > 0)
                            {
                                string alignPrefix = new string(' ', ("<" + tagName + " ").Length);

                                // First line: <Tag attr1
                                output.Append(currentIndentString).Append("<").Append(tagName).Append(" ").Append(attrs[0]).Append(Environment.NewLine);

                                // Remaining attributes
                                for (int ai = 1; ai < attrs.Length; ai++)
                                {
                                    output.Append(currentIndentString).Append(alignPrefix).Append(attrs[ai]).Append(Environment.NewLine);
                                }

                                i++; // consume next line

                                if (!closedHere)
                                {
                                    st.State = PendingState.OpenStartTagAligned;
                                    st.AlignPrefix = alignPrefix;
                                }

                                continue;
                            }
                        }
                    }
                }

                // 3) Start tag with attributes but not closed: "<Tag attr1"
                if (line.StartsWith("<", StringComparison.Ordinal)
                    && line.IndexOf('=') >= 0
                    && line.IndexOf('>') < 0)
                {
                    string tagName = TryGetTagNameFromStartTagLine(line);
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        st.State = PendingState.OpenStartTagAligned;
                        st.AlignPrefix = new string(' ', ("<" + tagName + " ").Length);
                    }

                    output.Append(currentIndentString).Append(line).Append(Environment.NewLine);
                    continue;
                }

                // 4) Closed single-line start tag: "<Tag a=.. b=.. />" -> aligned multi-line
                if (line.StartsWith("<", StringComparison.Ordinal)
                    && TryFormatStartTagAlignedAttributes(line, out var formatted))
                {
                    for (int fi = 0; fi < formatted.Length; fi++)
                    {
                        output.Append(currentIndentString).Append(formatted[fi]).Append(Environment.NewLine);
                    }
                    continue;
                }

                // 5) Default: tags keep indent, text lines get oneIndent
                bool isTagLine = line.StartsWith("<", StringComparison.Ordinal);
                output.Append(currentIndentString);
                if (!isTagLine) output.Append(oneIndent);
                output.Append(line).Append(Environment.NewLine);
            }

            output.Append(currentIndentString).Append("-->");
        }

        /// <summary>
        /// Resets the in-progress formatting state.
        /// Used when the formatter finishes a start tag (encounters "/&gt;" or "&gt;"),
        /// or when it sees a new tag while still in an "open start tag" state.
        /// </summary>
        private static void ResetState(CommentFormatState st)
        {
            st.State = PendingState.None;
            st.AlignPrefix = string.Empty;
        }

        /// <summary>
        /// Determines whether a line represents a "start tag header only" line,
        /// such as "&lt;Image" (no attributes and not yet closed).
        ///
        /// This is used to detect a two-line pattern:
        ///   &lt;Image
        ///     Width="20" Height="20" ... /&gt;
        /// so that it can be reconstructed into the aligned multi-line form.
        /// </summary>
        private static bool IsStartTagHeaderOnly(string line)
        {
            // "<Image" etc: start tag header without attributes and not closed
            if (string.IsNullOrEmpty(line)) return false;
            if (!line.StartsWith("<", StringComparison.Ordinal)) return false;
            if (line.StartsWith("</", StringComparison.Ordinal)) return false;
            if (line.StartsWith("<?", StringComparison.Ordinal)) return false;
            if (line.StartsWith("<!", StringComparison.Ordinal)) return false;

            return line.IndexOf('=') < 0 && line.IndexOf('>') < 0;
        }

        // ---------------------------------------------------------------------
        // Existing helpers kept (behavior identical)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Post-processing helper that fixes cases where "--&gt;" is preceded by tabs only.
        ///
        /// Some inputs can produce:
        ///   \n\t\t\t--&gt;
        /// which is replaced with:
        ///   \n{currentIndentString}--&gt;
        ///
        /// This method runs only when TreatCommentWithTagAsMultiline is enabled,
        /// and is designed to be as conservative as possible to avoid changing
        /// existing behavior.
        /// </summary>
        private static void FixIndentBeforeCommentClose(
            StringBuilder output,
            IStylerOptions options,
            string currentIndentString)
        {
            if (!options.TreatCommentWithTagAsMultiline)
                return;

            int closePos = output.Length - 3;
            if (closePos < 0) return;

            int nl = LastIndexOf(output, '\n', closePos);
            if (nl < 0) return;

            int segStart = nl + 1;
            int segEnd = closePos;

            if (segEnd <= segStart) return;

            bool hasTab = false;
            for (int i = segStart; i < segEnd; i++)
            {
                if (output[i] == '\t')
                    hasTab = true;
                else
                    return;
            }

            if (!hasTab) return;

            output.Remove(segStart, segEnd - segStart);
            output.Insert(segStart, currentIndentString);
        }

        /// <summary>
        /// Finds the last occurrence of <paramref name="ch"/> in <paramref name="sb"/>,
        /// searching backward from <paramref name="startIndex"/>.
        /// </summary>
        private static int LastIndexOf(StringBuilder sb, char ch, int startIndex)
        {
            for (int i = startIndex; i >= 0; i--)
            {
                if (sb[i] == ch) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns true if the given line ends a tag with "&gt;" or "/&gt;" (ignoring trailing whitespace).
        /// Used to determine whether the current open start tag is closed on this line.
        /// </summary>
        private static bool EndsWithTagClose(string s)
        {
            var t = s.TrimEnd();
            return t.EndsWith("/>", StringComparison.Ordinal)
                || t.EndsWith(">", StringComparison.Ordinal);
        }

        /// <summary>
        /// Extracts the tag name from a start-tag line, e.g.:
        ///   "&lt;Image Width="20"" => "Image"
        ///
        /// Returns an empty string if the input is not a start tag,
        /// or if a name cannot be determined.
        ///
        /// NOTE: Implemented without nullable reference types for C# 7.3 compatibility.
        /// </summary>
        private static string TryGetTagNameFromStartTagLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            int i = 0;
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            if (i >= s.Length || s[i] != '<') return string.Empty;

            i++;
            if (i < s.Length && (s[i] == '/' || s[i] == '?' || s[i] == '!'))
                return string.Empty;

            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            int start = i;

            while (i < s.Length)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c) || c == '>' || c == '/')
                    break;
                i++;
            }

            return (i > start) ? s.Substring(start, i - start) : string.Empty;
        }

        /// <summary>
        /// Extracts attribute tokens (only tokens containing '=') from a single line,
        /// respecting quoted values.
        ///
        /// Example input:
        ///   Width="20" Height="20" Margin="4,0,12,0" ... /&gt;
        /// Output:
        ///   ["Width="20"", "Height="20"", "Margin="4,0,12,0"", ..., "Source="..." /&gt;"]
        ///
        /// Also detects whether the tag is closed on this line ("/&gt;" or "&gt;") and:
        /// - removes the closing from the parse input
        /// - appends it to the last returned attribute token (so the closing appears on the last line)
        /// </summary>
        private static string[] ExtractAttributes(string line, out bool closedHere)
        {
            closedHere = false;
            if (string.IsNullOrEmpty(line))
                return new string[0];

            string trimmed = line.Trim();
            string closing = string.Empty;

            if (trimmed.EndsWith("/>", StringComparison.Ordinal))
            {
                closing = "/>";
                closedHere = true;
                trimmed = trimmed.Substring(0, trimmed.Length - 2).TrimEnd();
            }
            else if (trimmed.EndsWith(">", StringComparison.Ordinal))
            {
                closing = ">";
                closedHere = true;
                trimmed = trimmed.Substring(0, trimmed.Length - 1).TrimEnd();
            }

            var attrs = new System.Collections.Generic.List<string>(8);

            var sb = new StringBuilder();
            bool inQuotes = false;
            bool hasEquals = false;

            for (int i = 0; i < trimmed.Length; i++)
            {
                char c = trimmed[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    sb.Append(c);
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0)
                    {
                        if (hasEquals)
                            attrs.Add(sb.ToString());

                        sb.Length = 0;
                        hasEquals = false;
                    }
                    continue;
                }

                if (!inQuotes && c == '=')
                    hasEquals = true;

                sb.Append(c);
            }

            if (sb.Length > 0 && hasEquals)
                attrs.Add(sb.ToString());

            if (attrs.Count == 0)
                return new string[0];

            if (!string.IsNullOrEmpty(closing))
            {
                int last = attrs.Count - 1;
                attrs[last] = attrs[last] + " " + closing;
            }

            return attrs.ToArray();
        }

        /// <summary>
        /// Formats a closed single-line start tag into a multi-line aligned representation.
        ///
        /// Example input:
        ///   &lt;Image Width="20" Height="20" Margin="..." /&gt;
        /// Output lines:
        ///   &lt;Image Width="20"
        ///          Height="20"
        ///          Margin="..."
        ///          ... /&gt;
        ///
        /// Returns false if:
        /// - no attributes exist
        /// - the tag is not closed on the same line
        /// - fewer than two attributes exist (no benefit from splitting)
        /// </summary>
        private static bool TryFormatStartTagAlignedAttributes(string trimmedStart, out string[] lines)
        {
            lines = new string[0];
            if (trimmedStart.IndexOf('=') < 0) return false;

            bool closedHere;
            var attrs = ExtractAttributes(trimmedStart, out closedHere);
            if (!closedHere || attrs.Length < 2) return false;

            string tagName = TryGetTagNameFromStartTagLine(trimmedStart);
            if (string.IsNullOrEmpty(tagName)) return false;

            string alignPrefix = new string(' ', ("<" + tagName + " ").Length);

            var result = new string[attrs.Length];
            result[0] = "<" + tagName + " " + attrs[0];
            for (int i = 1; i < attrs.Length; i++)
            {
                result[i] = alignPrefix + attrs[i];
            }

            lines = result;
            return true;
        }
    }
}
