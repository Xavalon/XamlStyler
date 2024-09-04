// (c) Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.Options
{
    /// <summary>
    /// Type of end of line
    /// </summary>
    public enum EndOfLine
    {
        /// <summary>
        /// Uses <see cref="System.Environment.NewLine"/>.
        /// </summary>
        Default,
        /// <summary>
        /// The line feed `\n` character.
        /// </summary>
        LF,
        /// <summary>
        /// The carriage return line feed `\r\n` sequence.
        /// </summary>
        CRLF,
        /// <summary>
        /// The carriage return `\r` character.
        /// </summary>
        CR,
    }
}