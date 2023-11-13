// © Xavalon. All rights reserved.

namespace Xavalon.XamlStyler.Console
{
    public sealed class Logger
    {
        private readonly System.IO.TextWriter writer;
        private readonly LogLevel level;

        public Logger(System.IO.TextWriter writer, LogLevel level)
        {
            this.writer = writer;
            this.level = level;
        }

        public void Log(string line, LogLevel level = LogLevel.Default)
        {
            if (level <= this.level)
            {
                this.writer.WriteLine(line);
            }
        }
    }
}
