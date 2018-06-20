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

        // I think this is for no walls...was RNWALL
        NOWALL = 32,
        HOUSE = 64,
        RBUCK = 128,
        RMUNG = 256,
        RFILL = 512,
        SACRED = 1024,
        AIR = 2048,
        WATER = 4096,
        LAND = 8192,
        LIGHT = 16384,
        SEEN = 32768,
    }
}
