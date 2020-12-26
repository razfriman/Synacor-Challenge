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
        Noop = 21
    }
}