using System;

namespace Zork.Core.Room
{
    /// <summary>
    /// RoomFlags, original names ftw, will translate later when I know what they mean.
    /// </summary>
    [Flags]
    public enum RoomFlags
    {
        REND = 16,
        RNWALL = 32,
        RHOUSE = 64,
        RBUCK = 128,
        RMUNG = 256,
        RFILL = 512,
        RSACRD = 1024,
        RAIR = 2048,
        RWATER = 4096,
        RLAND = 8192,
        RLIGHT = 16384,
        RSEEN = 32768,
    }
}
