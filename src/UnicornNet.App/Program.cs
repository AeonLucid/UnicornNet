using UnicornNet.Data;
using UnicornNet.Registers;

namespace UnicornNet.App
{
    internal class Program
    {   
        private static void Main(string[] args)
        {
            using (var unicorn = new Unicorn(UcArch.UC_ARCH_ARM64, UcMode.UC_MODE_ARM))
            {
                const ulong address = 0x1000;
                const ulong memSize = 0x1000;
                
                var codeBytes = new byte[]
                {
                    0x01, 0x06, 0xa0, 0xd2, 
                    0x41, 0x10, 0x18, 0xd5, 
                    0xdf, 0x3f, 0x03, 0xd5
                };

                unicorn.MemMap(address, memSize);
                unicorn.MemWrite(address, codeBytes);
                unicorn.RegWrite(UcArm64Reg.UC_ARM64_REG_SP, address + memSize);
                unicorn.EmuStart(address, address + (ulong) codeBytes.Length);
            }
        }
    }
}