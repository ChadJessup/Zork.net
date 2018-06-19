using System;

namespace Zork.Core
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
        AIR = 2048,
        WATER = 4096,
        LAND = 8192,
        LIGHT = 16384,
        SEEN = 32768,
    }
}
