using System;

namespace UnicornNet.Data
{
    [Flags]
    public enum UcHookTypeMemInvalid
    {
        // Hook for memory read on unmapped memory
        UC_HOOK_MEM_READ_UNMAPPED = 1 << 4,
        // Hook for invalid memory write events
        UC_HOOK_MEM_WRITE_UNMAPPED = 1 << 5,
        // Hook for invalid memory fetch for execution events
        UC_HOOK_MEM_FETCH_UNMAPPED = 1 << 6,
        // Hook for memory read on read-protected memory
        UC_HOOK_MEM_READ_PROT = 1 << 7,
        // Hook for memory write on write-protected memory
        UC_HOOK_MEM_WRITE_PROT = 1 << 8,
        // Hook for memory fetch on non-executable memory
        UC_HOOK_MEM_FETCH_PROT = 1 << 9,
    }
}