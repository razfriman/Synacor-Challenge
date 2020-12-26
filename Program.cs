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
            vm.Run("west");
            vm.Run("west");
            vm.Run("passage");
            vm.Run("ladder");
            vm.Run("west");
            vm.Run("south");
            var code5 = vm.Run("north").Ascii[64..76];
            Console.WriteLine(code5);
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
            var code6 = vm.Run("use teleporter").Ascii[119..131];
            Console.WriteLine(code6);
            vm.Run("take business card");
            vm.Run("take strange book");
            vm.Run("look strange book");
            vm.Data[0x156d] = 6;
            vm.Data[0x1571] = 21;
            vm.Data[0x1572] = 21;
            vm.Registers[7] = SolveAckermann();
            var code7 = vm.Run("use teleporter").Ascii[397..409];
            Console.WriteLine(code7);
            vm.Run("north");
            vm.Run("north");
            vm.Run("north");
            vm.Run("north");
            vm.Run("north");
            vm.Run("north");
            vm.Run("north");
            vm.Run("east");
            vm.Run("take journal");
            vm.Run("west");
            vm.Run("north");
            vm.Run("north");
            vm.Run("take orb");
            vm.Run("look", true);
            // Vault
            //vm.AsmHistory.ForEach(Console.WriteLine);
            vm.RunInteractive();
            Console.WriteLine("Done!");
        }

        private static ushort SolveAckermann()
        {
            
            ushort x = 0;
            for (ushort hx = 0; hx <= 0xffff; hx++)
            {
                if (Teleporter.Ackermann(hx, 4, 1) == 6)
                {
                    x = hx;
                    break;
                }
            }

            return x;
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