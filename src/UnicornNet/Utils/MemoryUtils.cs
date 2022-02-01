using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace UnicornNet.Utils
{
    public static class MemoryUtils
    {
        public const ulong UnicornPageSize = 0x1000;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignPageUp(ulong size)
        {
            return (size + UnicornPageSize - 1) & ~(UnicornPageSize - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignPageDown(ulong size)
        {
            return size & ~(UnicornPageSize - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(ulong value)
        {
            var bytes = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(bytes, value);
            return bytes;
        }
    }
}