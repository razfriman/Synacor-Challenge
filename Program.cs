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

            vm.Run("doorway");
            vm.Run("north");
            vm.Run("north");
            vm.Run("bridge");
            vm.Run("continue");
            vm.Run("down");
            vm.Run("east");
            vm.Run("take empty lantern");
            vm.Run("take empty lantern");
            vm.Run("west");
            vm.Run("west");
            vm.Run("passage");
            vm.Run("ladder");
            vm.Run("west");
            vm.Run("south");
            vm.Run("north");
            vm.Run("take can");
            vm.Run("use can");
            vm.Run("use lantern");
            vm.Run("west");
            vm.Run("ladder");
            vm.Run("darkness");
            vm.Run("continue");
            vm.Run("west");
            vm.Run("west");
            vm.Run("west");
            vm.Run("west");
            vm.Run("north");
            vm.Run("take red coin");
            vm.Run("north");
            vm.Run("east");
            vm.Run("take concave coin");
            vm.Run("down");
            vm.Run("take corroded coin");
            vm.Run("up");
            vm.Run("west");
            vm.Run("west");
            vm.Run("take blue coin");
            vm.Run("up");
            vm.Run("take shiny coin");
            vm.Run("down");
            vm.Run("east");
            vm.Run("use blue coin");
            vm.Run("use red coin");
            vm.Run("use shiny coin");
            vm.Run("use concave coin");
            vm.Run("use corroded coin");
            vm.Run("north");
            vm.Run("take teleporter");
            vm.Run("use teleporter");
            vm.Run("take business card");
            vm.Run("take strange book");
            vm.Run("look strange book", true);
            vm.Run("look", true);
            var history = vm.AsmHistory;
            //history.ForEach(Console.WriteLine);
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