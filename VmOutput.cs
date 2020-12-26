using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SynacorChallenge
{
    public class VmOutput
    {
        public ImmutableList<ushort> Data { get; set; }

        public string Ascii => Encoding.Unicode.GetString(Data.SelectMany(BitConverter.GetBytes).ToArray());
    }
}