using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnicornNet.Data;

namespace UnicornNet
{
    public class Unicorn : IDisposable
    {
        private readonly Dictionary<int, UnicornCallbackData> _callbacks;
        private int _callbackId;

        public Unicorn(UcArch arch, UcMode mode)
        {
            _callbacks = new Dictionary<int, UnicornCallbackData>();
            _callbackId = 0;
            
            var result = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            var err = UcNative.UcOpen(arch, mode, result);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException($"Failed to create native Unicorn instance, error {err}.", err);
            }
            
            Handle = (IntPtr) Marshal.PtrToStructure(result, typeof(IntPtr));
        }
        
        public IntPtr Handle { get; }

        public static UcVersion GetVersion()
        {
            var result = UcNative.UcVersion(IntPtr.Zero, IntPtr.Zero);

            var major = (byte) ((result >> 24) & 0xFF);
            var minor = (byte) ((result >> 16) & 0xFF);
            var patch = (byte) ((result >> 8) & 0xFF);
            var extra = (byte) ((result) & 0xFF);
            
            return new UcVersion(major, minor, patch, extra);
        }

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

        public void MemWrite(ulong address, Stream stream, int size)
        {
            const int bufferSize = 8192;

            var remaining = size;
            var buffer = new byte[bufferSize];
            
            while (remaining > 0)
            {
                var target = Math.Min(remaining, bufferSize);
                var read = stream.Read(buffer, 0, target);
                if (read != target)
                {
                    throw new UcException("UnicornNet: Unable to read correct amount from stream.", UcErr.UC_ERR_ARG);
                }
                
                MemWrite(address, buffer, (ulong) target);

                address += (ulong) read;
                remaining -= read;
            }

            if (remaining != 0)
            {
                throw new UcException("UnicornNet: Unable to read full size from stream.", UcErr.UC_ERR_ARG);
            }
        }

        public void MemWrite(ulong address, byte[] bytes)
        {
            MemWrite(address, bytes, (ulong) bytes.Length);
        }
        
        public unsafe void MemWrite(ulong address, byte[] bytes, ulong length)
        {
            fixed (byte* pBytes = bytes)
            {
                var err = UcNative.UcMemWrite(Handle, address, pBytes, length);
                if (err != UcErr.UC_ERR_OK)
                {
                    throw new UcException(err);
                }
            }
        }

        public unsafe void MemWrite(ulong address, Span<byte> bytes)
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

        public unsafe byte[] MemRead(ulong address, ulong size)
        {
            var result = new byte[size];
            
            fixed (byte* pBytes = result)
            {
                var err = UcNative.UcMemRead(Handle, address, pBytes, size);
                if (err != UcErr.UC_ERR_OK)
                {
                    throw new UcException(err);
                }
            }

            return result;
        }

        public unsafe void MemRead(ulong address, ulong size, Span<byte> dest)
        {
            fixed (byte* destPtr = dest)
            {
                var err = UcNative.UcMemRead(Handle, address, destPtr, size);
                if (err != UcErr.UC_ERR_OK)
                {
                    throw new UcException(err);
                }
            }
        }

        public void EmuStart(ulong begin, ulong until, ulong timeout = 0, ulong size = 0)
        {
            var err = UcNative.UcEmuStart(Handle, begin, until, timeout, size);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public void EmuStop()
        {
            var err = UcNative.UcEmuStop(Handle);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        private IntPtr HookAdd(UcHookType type, Delegate callback, Delegate userCallback, object userData = null, ulong begin = 1, ulong end = 0)
        {
            var callbackId = _callbackId++;
            var callbackData = new UnicornCallbackData(callback, userCallback, userData);
            var err = UcNative.UcHookAdd(Handle, out var result, type, callback, new IntPtr(callbackId), begin, end);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }

            _callbacks.Add(callbackId, callbackData);
            
            return result;
        }

        public IntPtr HookCode(CallbackHookCodeUser callback, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd(UcHookType.UC_HOOK_CODE, (CallbackHookCode) HookCodeCallback, callback, userData, begin, end);
        }

        public IntPtr HookBlock(CallbackHookCodeUser callback, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd(UcHookType.UC_HOOK_BLOCK, (CallbackHookCode) HookCodeCallback, callback, userData, begin, end);
        }

        private void HookCodeCallback(IntPtr uc, ulong address, int size, IntPtr userData)
        {
            var callbackData = _callbacks[userData.ToInt32()];
            ((CallbackHookCodeUser) callbackData.UserCallback)(this, address, size, callbackData.UserData);
        }

        public IntPtr HookIntr(CallbackHookIntrUser callback, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd(UcHookType.UC_HOOK_INTR, (CallbackHookIntr) HookIntrCallback, callback, userData, begin, end);
        }

        private void HookIntrCallback(IntPtr uc, uint intno, IntPtr userData)
        {
            var callbackData = _callbacks[userData.ToInt32()];
            ((CallbackHookIntrUser) callbackData.UserCallback)(this, intno, callbackData.UserData);
        }

        public IntPtr HookInsnInvalid(CallbackHookInsnInvalidUser callback, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd(UcHookType.UC_HOOK_INSN_INVALID, (CallbackHookInsnInvalid) HookInsnInvalidCallback, callback, userData, begin, end);
        }

        private bool HookInsnInvalidCallback(IntPtr uc, IntPtr userData)
        {
            var callbackData = _callbacks[userData.ToInt32()];
            return ((CallbackHookInsnInvalidUser) callbackData.UserCallback)(this, callbackData.UserData);
        }

        public IntPtr HookMem(CallbackHookMemUser callback, UcHookTypeMem hookType, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd((UcHookType) hookType, (CallbackHookMem) HookMemCallback, callback, userData, begin, end);
        }

        private void HookMemCallback(IntPtr uc, UcMemType type, ulong address, int size, long value, IntPtr userData)
        {
            var callbackData = _callbacks[userData.ToInt32()];
            ((CallbackHookMemUser) callbackData.UserCallback)(this, type, address, size, value, callbackData.UserData);
        }

        public IntPtr HookMemEvent(CallbackEventMemUser callback, UcHookTypeMemInvalid hookType, object userData = null, ulong begin = 1, ulong end = 0)
        {
            return HookAdd((UcHookType) hookType, (CallbackEventMem) HookMemEventCallback, callback, userData, begin, end);
        }

        private bool HookMemEventCallback(IntPtr uc, UcMemType type, ulong address, int size, long value, IntPtr userData)
        {
            var callbackData = _callbacks[userData.ToInt32()];
            return ((CallbackEventMemUser) callbackData.UserCallback)(this, type, address, size, value, callbackData.UserData);
        }

        public void HookDel(IntPtr hookHandle)
        {
            var err = UcNative.UcHookDel(Handle, hookHandle);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public void MemMap(ulong address, ulong size, UcProt prot = UcProt.UC_PROT_ALL)
        {
            var err = UcNative.UcMemMap(Handle, address, size, prot);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public void MemMapPtr(ulong address, ulong size, IntPtr ptr, UcProt perms = UcProt.UC_PROT_ALL)
        {
            var err = UcNative.UcMemMapPtr(Handle, address, size, perms, ptr);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public void MemUnmap(ulong address, ulong size)
        {
            var err = UcNative.UcMemUnmap(Handle, address, size);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public void MemProtect(ulong address, ulong size, UcProt perms = UcProt.UC_PROT_ALL)
        {
            var err = UcNative.UcMemProtect(Handle, address, size, perms);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }
        
        public IEnumerable<UcMemRegion> MemRegions()
        {
            var err = UcNative.UcMemRegions(Handle, out var regions, out var count);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }

            var size = Marshal.SizeOf<UcMemRegion>();
            var result = new UcMemRegion[count];

            for (var i = 0; i < count; i++)
            {
                yield return Marshal.PtrToStructure<UcMemRegion>(regions + (i * size));
            }
        }

        public IntPtr ContextAlloc()
        {
            var err = UcNative.UcContextAlloc(Handle, out var context);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }

            return context;
        }

        /// <summary>
        ///     Free the memory allocated by uc_context_alloc & uc_mem_regions.
        /// </summary>
        /// <param name="mem">
        ///     memory allocated by uc_context_alloc (returned in *context),
        ///     or by uc_mem_regions (returned in *regions)
        /// </param>
        public void Free(IntPtr mem)
        {
            var err = UcNative.UcFree(mem);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public void ContextSave(IntPtr context)
        {
            var err = UcNative.UcContextSave(Handle, context);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public void ContextRestore(IntPtr context)
        {
            var err = UcNative.UcContextRestore(Handle, context);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException(err);
            }
        }

        public ulong ContextSize(IntPtr context)
        {
            return UcNative.UcContextSize(Handle);
        }

        public void Dispose()
        {
            var err = UcNative.UcClose(Handle);
            if (err != UcErr.UC_ERR_OK)
            {
                throw new UcException($"Failed to close native Unicorn instance, error {err}.", err);
            }
        }

        public delegate void CallbackHookCode(IntPtr uc, ulong address, int size, IntPtr userData);
        public delegate void CallbackHookCodeUser(Unicorn uc, ulong address, int size, object userData);

        public delegate void CallbackHookIntr(IntPtr uc, uint intno, IntPtr userData);
        public delegate void CallbackHookIntrUser(Unicorn uc, uint intno, object userData);
        
        public delegate bool CallbackHookInsnInvalid(IntPtr uc, IntPtr userData);
        public delegate bool CallbackHookInsnInvalidUser(Unicorn uc, object userData);
        
        public delegate void CallbackHookMem(IntPtr uc, UcMemType type, ulong address, int size, long value, IntPtr userData);
        public delegate void CallbackHookMemUser(Unicorn uc, UcMemType type, ulong address, int size, long value, object userData);
        
        public delegate bool CallbackEventMem(IntPtr uc, UcMemType type, ulong address, int size, long value, IntPtr userData);
        public delegate bool CallbackEventMemUser(Unicorn uc, UcMemType type, ulong address, int size, long value, object userData);
    }
}