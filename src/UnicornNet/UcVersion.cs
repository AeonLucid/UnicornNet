namespace UnicornNet;

public readonly struct UcVersion
{
    public UcVersion(byte major, byte minor, byte patch, byte extra)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        Extra = extra;
    }
    
    public byte Major { get; }
    public byte Minor { get; }
    public byte Patch { get; }
    public byte Extra { get; }
}