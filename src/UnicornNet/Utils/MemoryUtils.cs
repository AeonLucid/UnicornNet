using System.Buffers.Binary;

namespace UnicornNet.Utils
{
    public static class MemoryUtils
    {
        private const ulong UnicornPageSize = 0x1000;
        
        public static ulong AlignPageUp(ulong size)
        {
            return (size + UnicornPageSize - 1) & ~(UnicornPageSize - 1);
        }

        public static ulong AlignPageDown(ulong size)
        {
            return size & ~(UnicornPageSize - 1);
        }

        public static byte[] ToBytes(ulong value)
        {
            var bytes = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(bytes, value);
            return bytes;
        }
    }
}