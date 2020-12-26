using System.Collections.Generic;
using System.IO;
using System;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Collections.Immutable;

namespace SynacorChallenge
{
    public enum Opcode
    {
        // halt: 0
        // stop execution and terminate the program
        Halt = 0,

        // set: 1 a b
        // set register <a> to the value of <b>
        Set = 1,

        //   push: 2 a
        //   push <a> onto the stack
        Push = 2,

        //   pop: 3 a
        //   remove the top element from the stack and write it into <a>; empty stack = error
        Pop = 3,

        //   eq: 4 a b c
        //   set <a> to 1 if <b> is equal to <c>; set it to 0 otherwise
        Eq = 4,

        //   gt: 5 a b c
        //   set <a> to 1 if <b> is greater than <c>; set it to 0 otherwise
        Gt = 5,

        //   jmp: 6 a
        //   jump to <a>
        Jmp = 6,

        //   jt: 7 a b
        //   if <a> is nonzero, jump to <b>
        Jt = 7,

        //   jf: 8 a b
        //   if <a> is zero, jump to <b>
        Jf = 8,

        //   add: 9 a b c
        //   assign into <a> the sum of <b> and <c> (modulo 32768)
        Add = 9,

        //   mult: 10 a b c
        //   store into <a> the product of <b> and <c> (modulo 32768)
        Mult = 10,

        //   mod: 11 a b c
        //   store into <a> the remainder of <b> divided by <c>
        Mod = 11,

        //   and: 12 a b c
        //   stores into <a> the bitwise and of <b> and <c>
        And = 12,

        //   or: 13 a b c
        //   stores into <a> the bitwise or of <b> and <c>
        Or = 13,

        //   not: 14 a b
        //   stores 15-bit bitwise inverse of <b> in <a>
        Not = 14,

        //   rmem: 15 a b
        //   read memory at address <b> and write it to <a>
        Rmem = 15,

        //   wmem: 16 a b
        //   write the value from <b> into memory at address <a>
        Wmem = 16,

        //   call: 17 a
        //   write the address of the next instruction to the stack and jump to <a>
        Call = 17,

        //   ret: 18
        //   remove the top element from the stack and jump to it; empty stack = halt
        Ret = 18,

        //   out: 19 a
        //   write the character represented by ascii code <a> to the terminal
        Out = 19,

        //   in: 20 a
        //   read a character from the terminal and write its ascii code to <a>; it can be assumed that once input starts, it will continue until a newline is encountered; this means that you can safely read whole lines from the keyboard and trust that they will be fully read
        In = 20,

        //   noop: 21
        //   no operation
        Noop = 21,
    }

    public class VirtualMachine
    {
        public ushort[] Registers { get; set; } = new ushort[8];
        public static int Modulus = 32768;
        public Stack<ushort> Stack { get; set; } = new Stack<ushort>();

        public ushort[] Data { get; set; }

        public Queue<ushort> Inputs { get; private init; } = new();
        public ushort InstructionPointer;

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
        private Opcode GetOpcode(ushort addr) => (Opcode) Data[addr];

        public ImmutableList<ushort> Run() => Run(Array.Empty<ushort>());

        public ImmutableList<ushort> Run(params ushort[] input)
        {
            foreach (var i in input)
            {
                Inputs.Enqueue(i);
            }

            var output = new List<ushort>();
            while (true)
            {
                var opcode = GetOpcode(InstructionPointer);
                var oldIp = InstructionPointer;
                Console.WriteLine($"{InstructionPointer} {opcode} ({Data[InstructionPointer+1]}, {Data[InstructionPointer+2]}, {Data[InstructionPointer+3]})");
                switch (opcode)
                {
                    case Opcode.Noop:
                        InstructionPointer += 1;
                        break;
                    case Opcode.Out:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        output.Add(a);
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Jmp:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        InstructionPointer = a;
                        break;
                    }
                    case Opcode.Jt:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        var b = GetArgument(InstructionPointer + 2);
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
                        InstructionPointer += 3;
                        break;
                    }
                    case Opcode.Halt:
                        return output.ToImmutableList();
                    case Opcode.Add:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b + c));
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Push:
                    {
                        var a = GetArgument(InstructionPointer + 1);
                        Stack.Push(a);
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Pop:
                    {
                        var pop = Stack.Pop();
                        var a = InstructionPointer + 1;
                        SetValue(a, pop);
                        InstructionPointer += 2;
                        break;
                    }
                    case Opcode.Eq:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, b == c ? 1 : 0);
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Gt:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, b > c ? 1 : 0);
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Mult:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b * c));
                        InstructionPointer += 4;
                        break;
                    }
                    // case Opcode.Mod:
                    case Opcode.And:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b & c));
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Or:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        var c = GetArgument(InstructionPointer + 3);
                        SetValue(a, (ushort) (b | c));
                        InstructionPointer += 4;
                        break;
                    }
                    case Opcode.Not:
                    {
                        var a = InstructionPointer + 1;
                        var b = GetArgument(InstructionPointer + 2);
                        SetValue(a, (ushort) (~b & 0b0111_1111_1111_1111));
                        InstructionPointer += 3;
                        break;
                    }
                    // case Opcode.Rmem:
                    // case Opcode.Wmem:
                    case Opcode.Call:
                    {
                        //write the address of the next instruction to the stack and jump to <a>
                        var a = GetArgument(InstructionPointer + 1);
                        Stack.Push((ushort) (InstructionPointer + 1));
                        InstructionPointer = a;
                        break;
                    }
                    // case Opcode.Ret:
                    // case Opcode.In:
                    //     InstructionPointer += 1;
                    //     break;
                    default:
                        // Console.WriteLine($"Unknown {nameof(opcode)}: {opcode}");
                        throw new Exception($"Unknown ${nameof(opcode)}: ${opcode}");
                        return output.ToImmutableList();
                }
            }

            return output.ToImmutableList();
        }

        private void SetValue(int destAddr, ushort value)
        {
            var destValue = Data[destAddr];

            if (destValue >= 0 && destValue <= 32767)
            {
                Data[destValue] = value;
            }
            else if (destValue >= 32768 && destValue <= 32775)
            {
                var register = destValue - 32768;
                Registers[register] = value;
            }
        }

        public ushort GetArgument(int address)
        {
            var value = Data[address];
            return value switch
            {
                ushort literal when value >= 0 && value <= 32767 => literal,
                ushort register when value >= 32768 && value <= 32775 => Registers[register - 32768],
                _ => throw new Exception($"numbers 32776..65535 are invalid. Value: ${value}")
            };
        }
    }
}