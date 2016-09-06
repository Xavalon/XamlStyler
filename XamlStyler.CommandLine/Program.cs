using System;

namespace XamlStyler.CommandLine
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return 0;
            }
            return 0;
        }
        private static void PrintHelp()
        {
            var version = typeof(Program).Assembly.GetName().Version;

            Console.WriteLine($"XamlStyler Version: {version}, Command line utility for styling xaml");
            Console.WriteLine($"usage: XamlStyler <xaml-file> [-s <setings-file>]");
        }
    }
}
