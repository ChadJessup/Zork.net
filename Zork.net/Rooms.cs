using System.Collections.Generic;

namespace Zork.Core
{
    public class Rooms
    {
        public int Count { get; set; }
        public List<int> Descriptions1 { get; } = new List<int>(200);
        public List<int> Descriptions2 { get; } = new List<int>(200);
        public List<int> Exits { get; } = new List<int>(200);
        public List<int> Actions { get; } = new List<int>(200);
        public List<int> Values { get; } = new List<int>(200);
        public List<RoomFlags> Flags { get; } = new List<RoomFlags>(200);
    }
}
