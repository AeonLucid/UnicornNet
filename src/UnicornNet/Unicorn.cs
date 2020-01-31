using System;
using System.Runtime.InteropServices;
using UnicornNet.Data;

namespace UnicornNet
{
    public class Unicorn : IDisposable
    {
        public Unicorn(UcArch arch, UcMode mode)
        {
            var result = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            var err = UcNative.UcOpen(arch, mode, result);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException($"Failed to create native Unicorn instance, error {err}.");
            }
            
            Handle = (IntPtr) Marshal.PtrToStructure(result, typeof(IntPtr));
        }
        
        public IntPtr Handle { get; }

        public ulong Query(UcQueryType type)
        {
            var err = UcNative.UcQuery(Handle, type, out var result);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }

            return result;
        }

        public UcErr Errno()
        {
            return UcNative.UcErrno(Handle);
        }

        public void RegWrite(int registerId, ulong data)
        {
            var err = UcNative.UcRegWrite(Handle, registerId, ref data);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public ulong RegRead(int registerId)
        {
            var err = UcNative.UcRegRead(Handle, registerId, out var result);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }

            return result;
        }

        public unsafe void MemWrite(ulong address, byte[] bytes)
        {
            fixed (byte* pBytes = bytes)
            {
                var err = UcNative.UcMemWrite(Handle, address, pBytes, (ulong) bytes.Length);
                if (err != UcErr.UC_ERR_OK)
                {
                    throw new UcException(err);
                }
            }
        }
        
        // MemRead

        public void EmuStart(ulong begin, ulong until, ulong timeout = 0, ulong size = 0)
        {
            var err = UcNative.UcEmuStart(Handle, begin, until, timeout, size);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public void MemMap(ulong address, ulong memSize, UcProt prot = UcProt.UC_PROT_ALL)
        {
            var err = UcNative.UcMemMap(Handle, address, memSize, prot);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public void Dispose()
        {
            var err = UcNative.UcClose(Handle);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException($"Failed to close native Unicorn instance, error {err}.");
            }
        }
    }
}