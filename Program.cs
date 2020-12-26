using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Synacor_Challenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Email: raz@razfriman.com
            // Password: uE4R9py32fgVvJbrZ@*cYt9H

            var code1 = ParseArchSpecCode();
            Console.WriteLine(code1);

            var code2 = ParseChallengeTextCode();
            Console.WriteLine(code2);

            var data = File.ReadAllBytes("challenge.bin");
            var vm = new VirtualMachine(data);
            Console.WriteLine("Done!");
        }

        public static string ParseArchSpecCode()
        {
            var text = File.ReadAllText("arch-spec.md");
            var match = Regex.Match(text, @"Here's a code for the challenge website: (.+?)\W");
            var code = match.Groups[1].Value;
            return code;
        }

        public static string ParseChallengeTextCode()
        {
            var lines = File.ReadAllLines("challenge.bin", Encoding.Unicode);
            var unicodeCode = lines[2][74..];
            var code = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.ASCII,
                Encoding.Unicode.GetBytes(unicodeCode)));
            return code;
        }
    }
}