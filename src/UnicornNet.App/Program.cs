using System;
using UnicornNet.Data;
using UnicornNet.Registers;

namespace UnicornNet.App
{
    internal class Program
    {   
        private static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var version = Unicorn.GetVersion();
            
            Console.WriteLine($"Running Unicorn version: {version.Major}.{version.Minor}");
            
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

                Console.WriteLine("- HookCode");
                unicorn.HookCode((uc, address1, size, data) =>
                {
                    Console.WriteLine("Code..");    
                });
                
                Console.WriteLine("- MemMap");
                unicorn.MemMap(address, memSize);
                
                Console.WriteLine("- MemWrite");
                unicorn.MemWrite(address, codeBytes);
                
                Console.WriteLine("- RegWrite");
                unicorn.RegWrite(UcArm64Reg.UC_ARM64_REG_SP, address + memSize);
                
                Console.WriteLine("- EmuStart");
                unicorn.EmuStart(address, address + (ulong) codeBytes.Length);
            }
            
            Console.WriteLine("Stop");
        }
    }
}