using System;
using System.Collections.Generic;

namespace FubuCore.CommandLine
{
    public static class ConsoleWriter
    {
        private static int CONSOLE_WIDTH = 80;
        private static string HL = new string('-', CONSOLE_WIDTH);

        public static void Line()
        {
            Console.WriteLine();
        }

        public static void PrintHorizontalLine()
        {
            Console.WriteLine(HL);
        }

        public static void Write(string stuff)
        {
            BreakIntoLines(stuff)
                .Each(l=>Console.WriteLine(l));
        }
        public static void Write(string format, params object[] args)
        {
            var input = string.Format(format, args);
            Write(input);
        }

        private static string[] BreakIntoLines(string input)
        {
            if (string.IsNullOrEmpty(input)) return new string[0];

            var lines = new List<string>();


            while(input.Length > 0)
            {
                var chomp = input.Length > CONSOLE_WIDTH ? CONSOLE_WIDTH : input.Length;
                string c = input.Substring(0, chomp);
                lines.Add(c);
                input = input.Remove(0, chomp);
            }

            return lines.ToArray();
        }
    }
}