using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Collections.Immutable;

namespace SynacorChallenge
{
    public class VirtualMachine
    {
        public ushort[] Registers { get; } = new ushort[8];
        public const int Modulus = 32768;
        public Stack<ushort> Stack { get; } = new();

        public ushort[] Data { get; }

        public Queue<ushort> Inputs { get; } = new();
        public ushort InstructionPointer;

        public List<string> AsmHistory { get; } = new();

        public VirtualMachine()
        {
        }

        public VirtualMachine(IEnumerable<ushort> data) => Data = data.ToArray();

        public VirtualMachine(string input) =>
            Data = Encoding.ASCII.GetBytes(input).Select(b => (ushort) b).ToArray();

        public VirtualMachine(byte[] byteData)
        {
            Data = new ushort[(int) Math.Ceiling((double) byteData.Length / 2)];
            Buffer.BlockCopy(byteData, 0, Data, 0, byteData.Length);
        }

        public bool Halted() => GetOpcode(InstructionPointer) == Opcode.Halt;
        public bool NeedsInput() => GetOpcode(InstructionPointer) == Opcode.In;
        private Opcode GetOpcode(ushort addr) => (Opcode) Data[addr];

        public VmOutput Run() => Run(Array.Empty<ushort>());

        public VmOutput Run(string s, bool printOutput = false)
        {
            if (printOutput)
            {
                Console.WriteLine("Command: " + s);
            }
            var inputs = s.Select(c => (ushort) c).ToList();
            inputs.Add('\n');
            var output = Run(inputs.ToArray());
            if (printOutput)
            {
                Console.WriteLine(output.Ascii);
            }

            return output;
        }

        public VmOutput Run(params ushort[] input)
        {
            foreach (var i in input)
            {
                Inputs.Enqueue(i);
            }

            var output = new List<ushort>();
            while (true)
            {
                if (InstructionPointer == 6027)
                {
                    // Console.WriteLine("at 6027");
                }
                var opcode = GetOpcode(InstructionPointer);
                switch (opcode)
                {
                    case Opcode.Noop:
                        AsmHistory.Add($"{InstructionPointer}: Noop");
                        InstructionPointer += 1;
                        break;
                    case Opcode.Out:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        output.Add(a);
                        AsmHistory.Add($"{InstructionPointer}: Out {a} ('{(char) a}')");
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Jmp:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        AsmHistory.Add($"{InstructionPointer}: Jmp {a}");
                        InstructionPointer = a;
                        break;
                    }
                    case Opcode.Jt:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        var b = GetArgument(InstructionPointer + 2);
                        AsmHistory.Add($"{InstructionPointer}: Jt {a} {b}");
                        if (a != 0)
                        {
                            InstructionPointer = b;
                        }
                        else
                        {
                            InstructionPointer += 3;
                        }

                        break;
                    }
                    case Opcode.Jf:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        var b = GetArgument(InstructionPointer + 2);
                        AsmHistory.Add($"{InstructionPointer}: Jf {a} {b}");
                        if (a == 0)
                        {
                            InstructionPointer = b;
                        }
                        else
                        {
                            InstructionPointer += 3;
                        }

                        break;
                    }
                    case Opcode.Set:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        SetValue(a, b);
                        AsmHistory.Add($"{InstructionPointer}: Set {a} {b}");
                        InstructionPointer += 3;
                        break;
                    }
                    case Opcode.Halt:
                        return new VmOutput {Data = output.ToImmutableList()};
                    case Opcode.Add:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) ((b + c) % Modulus));
                        AsmHistory.Add($"{InstructionPointer}: Add {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Push:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        Stack.Push(a);
                        AsmHistory.Add($"{InstructionPointer}: Push {a}");
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Pop:
                    {
                        var pop = Stack.Pop();
                        var a = InstructionPointer + 1;
                        SetValue(a, pop);
                        AsmHistory.Add($"{InstructionPointer}: Pop {a}");
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Eq:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, b == c ? 1 : 0);
                        AsmHistory.Add($"{InstructionPointer}: Eq {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Gt:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, b > c ? 1 : 0);
                        AsmHistory.Add($"{InstructionPointer}: Gt {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Mult:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b * c % Modulus));
                        AsmHistory.Add($"{InstructionPointer}: Mult {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Mod:
                    {
                        //store into <a> the remainder of <b> divided by <c>
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b % c));
                        AsmHistory.Add($"{InstructionPointer}: Mod {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.And:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b & c));
                        AsmHistory.Add($"{InstructionPointer}: And {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Or:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b | c));
                        AsmHistory.Add($"{InstructionPointer}: Or {a} {b} {c}");
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Not:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        SetValue(a, (ushort) (~b & 0b0111_1111_1111_1111));
                        AsmHistory.Add($"{InstructionPointer}: Not {a} {b}");
                        InstructionPointer += 3;
                        break;
                    }
                    case Opcode.Call:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        AsmHistory.Add($"{InstructionPointer}: Call {a}");
                        Stack.Push((ushort) (InstructionPointer + 2));
                        InstructionPointer = a;
                        break;
                    }
                    case Opcode.Rmem:
                    {
                        var a = InstructionPointer + 1;
                        var b = Data[GetArgument(InstructionPointer + 2)];
                        SetValue(a, b);
                        AsmHistory.Add($"{InstructionPointer}: Rmem {a} {b}");
                        InstructionPointer += 3;
                        break;
                    }
                    case Opcode.Wmem:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        var b = GetArgument(InstructionPointer + 2);
                        Data[a] = b;
                        AsmHistory.Add($"{InstructionPointer}: Wmem {a} {b}");
                        InstructionPointer += 3;
                        break;
                    }
                    case Opcode.Ret:
                    {
                        if (Stack.Count == 0)
                        {
                            AsmHistory.Add($"{InstructionPointer}: Ret");
                            return new VmOutput {Data = output.ToImmutableList()};
                        }

                        var pop = Stack.Pop();
                        AsmHistory.Add($"{InstructionPointer}: Ret {pop}");
                        InstructionPointer = pop;
                        break;
                    }
                    case Opcode.In:
                    {
                        if (Inputs.Any())
                        {
                            var a = InstructionPointer + 1;
                            var b = Inputs.Dequeue();
                            SetValue(a, b);
                            AsmHistory.Add($"{InstructionPointer}: In {a} {b} ('{(char) b}')");
                            InstructionPointer += 2;
                        }
                        else
                        {
                            return new VmOutput {Data = output.ToImmutableList()};
                        }

                        break;
                    }
                    default:
                        throw new Exception($"Unknown ${nameof(opcode)}: ${opcode}");
                }
            }
        }

        private void SetValue(int destAddr, ushort value)
        {
            var destValue = Data[destAddr];

            switch (destValue)
            {
                case >= 0 and <= 32767:
                    Data[destValue] = value;
                    break;
                case >= 32768 and <= 32775:
                {
                    var register = destValue - 32768;
                    Registers[register] = value;
                    break;
                }
            }
        }

        public ushort GetArgument(int address)
        {
            var value = Data[address];
            return value switch
            {
                { } literal when value <= 32767 => literal,
                { } register when value >= 32768 && value <= 32775 => Registers[register - 32768],
                _ => throw new Exception($"numbers 32776..65535 are invalid. Value: ${value}")
            };
        }

        public void RunInteractive()
        {
            while (!Halted())
            {
                var output = NeedsInput() ? Run(Console.ReadLine()) : Run();
                Console.WriteLine(output.Ascii);
            }
        }
    }
}