using System.Collections.Generic;

namespace Zork.Core
{
    public class Screen
    {
        public int fromdr { get; set; } = 0;
        public RoomIds scolrm { get; set; } = 0;
        public RoomIds scolac { get; set; } = 0;
        public List<RoomIds> scoldr { get; } = new List<RoomIds>(8)
        {
            (RoomIds)1024,
            (RoomIds)153,
            (RoomIds)5120,
            (RoomIds)154,
            (RoomIds)3072,
            (RoomIds)152,
            (RoomIds)7168,
            (RoomIds)151
        };

        public List<int> scolwl { get; } = new List<int>(12) { 151, 207, 3072, 152, 208, 7168, 153, 206, 5120, 154, 205, 1024 };
    }
}
