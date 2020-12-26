
using System.Collections.Generic;
using System.IO;
using System;
using System.Buffers;

namespace Synacor_Challenge
{
    public class VirtualMachine
    {
        public Stack<short> Stack { get; set; } = new Stack<short>();

        public short[] Data { get; set; }

        public byte[] ByteData { get; set; }

        public MemoryStream Stream { get; set; }

        public BinaryReader Reader { get; set; }

        public VirtualMachine(byte[] byteData)
        {
            Data = new short[(int)Math.Ceiling((double)byteData.Length / 2)];
            Buffer.BlockCopy(byteData, 0, Data, 0, byteData.Length);
            ByteData = byteData;
            Stream = new MemoryStream(ByteData);
            Reader = new BinaryReader(Stream);
        }

        public void Init(byte[] data)
        {


        }
    }
}