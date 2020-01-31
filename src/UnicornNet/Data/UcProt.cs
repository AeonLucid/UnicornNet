using System;

namespace UnicornNet.Data
{
    [Flags]
    public enum UcProt
    {
        UC_PROT_NONE = 0,
        UC_PROT_READ = 1,
        UC_PROT_WRITE = 2,
        UC_PROT_EXEC = 4,
        UC_PROT_ALL = 7,
    }
}