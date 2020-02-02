using System.Runtime.InteropServices;

namespace UnicornNet.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UcMemRegion
    {
        public ulong Begin { get; }
        public ulong End { get; }
        public UcProt Perms { get; }
    }
}