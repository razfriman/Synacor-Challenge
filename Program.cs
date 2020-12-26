using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SynacorChallenge
{
    public class Program
    {
        public static void Main()
        {
            var code1 = ParseArchSpecCode();
            Console.WriteLine(code1);

            var data = File.ReadAllBytes("challenge.bin");
            var vm = new VirtualMachine(data);
            var output = vm.Run();
            var code2 = output.Ascii[121..133];
            Console.WriteLine(code2);
            var code3 = output.Ascii[228..240];
            Console.WriteLine(code3);

            vm.Run("take tablet");
            var code4 = vm.Run("use tablet").Ascii[29..41];
            Console.WriteLine(code4);

            vm.Run("look", true);
            vm.RunInteractive();
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