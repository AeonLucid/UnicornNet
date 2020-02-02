using System;

namespace UnicornNet.Data
{
    [Flags]
    public enum UcHookTypeMem
    {
        // Hook memory read events.
        UC_HOOK_MEM_READ = 1 << 10,
        // Hook memory write events.
        UC_HOOK_MEM_WRITE = 1 << 11,
        // Hook memory fetch for execution events
        UC_HOOK_MEM_FETCH = 1 << 12,
        // Hook memory read events, but only successful access.
        // The callback will be triggered after successful read.
        UC_HOOK_MEM_READ_AFTER = 1 << 13
    }
}