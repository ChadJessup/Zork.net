using System;

namespace Zork.Core.Object
{
    [Flags]
    public enum ObjectFlags
    {
        VISIBT = 32768,
        READBT = 16384,
        TAKEBT = 8192,
        DOORBT = 4096,
        TRANBT = 2048,
        FOODBT = 1024,
        NDSCBT = 512,
        DRNKBT = 256,
        CONTBT = 128,
        LITEBT = 64,
        VICTBT = 32,
        BURNBT = 16,
        FLAMBT = 8,
        TOOLBT = 4,
        TURNBT = 2,
        ONBT   = 1,
        FINDBT = 32768,
        SLEPBT = 16384,
        SCRDBT = 8192,
        TIEBT  = 4096,
        CLMBBT = 2048,
        ACTRBT = 1024,
        WEAPBT = 512,
        FITEBT = 256,
        VILLBT = 128,
        STAGBT = 64,
        TRYBT  = 32,
        NOCHBT = 16,
        OPENBT = 8,
        TCHBT  = 4,
        VEHBT  = 2,
        SCHBT  = 1,
    }
}
