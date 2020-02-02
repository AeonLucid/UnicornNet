using System;
using System.Runtime.InteropServices;
using UnicornNet.Data;

namespace UnicornNet
{
    public static class UcNative
    {
        /// <summary>
        ///     Return combined API version & major and minor version numbers.
        /// </summary>
        /// <param name="major">major number of API version</param>
        /// <param name="minor">minor number of API version</param>
        /// <returns>
        ///     hexical number as (major lt;lt; 8 | minor), which encodes bothmajor & minor versions.
        ///     NOTE: This returned value can be compared with version number made with macro UC_MAKE_VERSION
        /// </returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_version")]
        public static extern uint UcVersion(IntPtr major, IntPtr minor);
        // unsigned int uc_version(unsigned int *major, unsigned int *minor);
        
        /// <summary>
        ///     Determine if the given architecture is supported by this library.
        /// </summary>
        /// <param name="arch">architecture type (UC_ARCH_*)</param>
        /// <returns>True if this library supports the given arch.</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_arch_supported")]
        public static extern bool UcArchSupported(int arch);
        // bool uc_arch_supported(uc_arch arch);
        
        /// <summary>
        ///     Create new instance of unicorn engine.
        /// </summary>
        /// <param name="arch">architecture type (UC_ARCH_*)</param>
        /// <param name="mode">hardware mode. This is combined of UC_MODE_*</param>
        /// <param name="engine">pointer to uc_engine, which will be updated at return time</param>
        /// <returns>UC_ERR_OK on success, or other value on failure.</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_open")]
        public static extern UcErr UcOpen(UcArch arch, UcMode mode, IntPtr engine);
        // uc_err uc_open(uc_arch arch, uc_mode mode, uc_engine **uc);
        
        /// <summary>
        ///     Close a Unicorn engine instance.
        ///     NOTE: this must be called only when there is no longer any
        ///     usage of @uc. This API releases some of @uc's cached memory, thus
        ///     any use of the Unicorn API with @uc after it has been closed may
        ///     crash your application. After this, @uc is invalid, and is no
        ///     longer usable.
        /// </summary>
        /// <param name="engine">pointer to a handle returned by uc_open()</param>
        /// <returns>UC_ERR_OK on success, or other value on failure.</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_close")]
        public static extern UcErr UcClose(IntPtr engine);
        // uc_err uc_close(uc_engine *uc);
        
        /// <summary>
        ///     Query internal status of engine.
        /// </summary>
        /// <param name="engine">handle returned by uc_open()</param>
        /// <param name="ucQueryType">query type. See uc_query_type</param>
        /// <param name="result">save the internal status queried</param>
        /// <returns>error code of uc_err enum type (UC_ERR_*, see above)</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_query")]
        public static extern UcErr UcQuery(IntPtr engine, UcQueryType ucQueryType, out ulong result);
        // uc_err uc_query(uc_engine *uc, uc_query_type type, size_t *result);
        
        /// <summary>
        ///     Report the last error number when some API function fail.
        ///     Like glibc's errno, uc_errno might not retain its old value once accessed.
        /// </summary>
        /// <param name="engine">handle returned by uc_open()</param>
        /// <returns>error code of uc_err enum type (UC_ERR_*, see above)</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_errno")]
        public static extern UcErr UcErrno(IntPtr engine);
        // uc_err uc_errno(uc_engine *uc);
        
        /// <summary>
        ///     Return a string describing given error code.
        /// </summary>
        /// <param name="code">error code (see UC_ERR_* above)</param>
        /// <returns>returns a pointer to a string that describes the error code passed in the argument</returns>
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_strerror")]
        public static extern IntPtr UcStrerror(UcErr code);
        // const char *uc_strerror(uc_err code);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_reg_write")]
        public static extern UcErr UcRegWrite(IntPtr engine, int regid, ref ulong value);
        // uc_err uc_reg_write(uc_engine *uc, int regid, const void *value);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_reg_read")]
        public static extern UcErr UcRegRead(IntPtr engine, int regid, out ulong value);
        // uc_err uc_reg_read(uc_engine *uc, int regid, void *value);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_reg_write_batch")]
        public static extern UcErr UcRegWriteBatch(IntPtr engine, IntPtr regs, IntPtr vals, int count);
        // uc_err uc_reg_write_batch(uc_engine *uc, int *regs, void *const *vals, int count);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_reg_read_batch")]
        public static extern UcErr UcRegReadBatch(IntPtr engine, IntPtr regs, IntPtr vals, int count);
        // uc_err uc_reg_read_batch(uc_engine *uc, int *regs, void **vals, int count);

        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_write")]
        public static extern unsafe UcErr UcMemWrite(IntPtr engine, ulong address, byte* bytes, ulong size);
        // uc_err uc_mem_write(uc_engine *uc, uint64_t address, const void *bytes, size_t size);

        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_read")]
        public static extern unsafe UcErr UcMemRead(IntPtr engine, ulong address, byte* bytes, ulong size);
        // uc_err uc_mem_write(uc_engine *uc, uint64_t address, const void *bytes, size_t size);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_emu_start")]
        public static extern UcErr UcEmuStart(IntPtr engine, ulong begin, ulong until, ulong timeout, ulong size);
        // uc_err uc_emu_start(uc_engine *uc, uint64_t begin, uint64_t until, uint64_t timeout, size_t count);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_emu_stop")]
        public static extern UcErr UcEmuStop(IntPtr engine);
        // uc_err uc_emu_stop(uc_engine *uc);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_hook_add")]
        public static extern UcErr UcHookAdd(IntPtr engine, out IntPtr hookHandle, UcHookType type, Delegate callback, IntPtr userData, ulong begin, ulong end);
        // uc_err uc_hook_add(uc_engine *uc, uc_hook *hh, int type, void *callback, void *user_data, uint64_t begin, uint64_t end, ...);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_hook_del")]
        public static extern UcErr UcHookDel(IntPtr engine, IntPtr hookHandle);
        // uc_err uc_hook_del(uc_engine *uc, uc_hook hh);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_map")]
        public static extern UcErr UcMemMap(IntPtr engine, ulong address, ulong size, UcProt perms);
        // uc_err uc_mem_map(uc_engine *uc, uint64_t address, size_t size, uint32_t perms);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_map_ptr")]
        public static extern UcErr UcMemMapPtr(IntPtr engine, ulong address, ulong size, UcProt perms, IntPtr ptr);
        // uc_err uc_mem_map_ptr(uc_engine *uc, uint64_t address, size_t size, uint32_t perms, void *ptr);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_unmap")]
        public static extern UcErr UcMemUnmap(IntPtr engine, ulong address, ulong size);
        // uc_err uc_mem_unmap(uc_engine *uc, uint64_t address, size_t size);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_protect")]
        public static extern UcErr UcMemProtect(IntPtr engine, ulong address, ulong size, UcProt perms);
        // uc_err uc_mem_protect(uc_engine *uc, uint64_t address, size_t size, uint32_t perms);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_mem_regions")]
        public static extern UcErr UcMemRegions(IntPtr engine, out IntPtr regions, out uint count);
        // uc_err uc_mem_regions(uc_engine *uc, uc_mem_region **regions, uint32_t *count);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_context_alloc")]
        public static extern UcErr UcContextAlloc(IntPtr engine, out IntPtr context);
        // uc_err uc_context_alloc(uc_engine *uc, uc_context **context);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_free")]
        public static extern UcErr UcFree(IntPtr mem);
        // uc_err uc_free(void *mem);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_context_save")]
        public static extern UcErr UcContextSave(IntPtr engine, IntPtr context);
        // uc_err uc_context_save(uc_engine *uc, uc_context *context);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_context_restore")]
        public static extern UcErr UcContextRestore(IntPtr engine, IntPtr context);
        // uc_err uc_context_restore(uc_engine *uc, uc_context *context);
        
        [DllImport("Libs/x64/unicorn", EntryPoint = "uc_context_size")]
        public static extern ulong UcContextSize(IntPtr engine);
        // size_t uc_context_size(uc_engine *uc);
    }
}