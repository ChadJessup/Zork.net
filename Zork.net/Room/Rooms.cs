using System.Collections.Generic;

namespace Zork.Core.Room
{
    public class Rooms
    {
        public int Count { get; set; }
        public List<int> RoomDescriptions1 { get; } = new List<int>(200);
        public List<int> RoomDescriptions2 { get; } = new List<int>(200);
        public List<int> RoomExits { get; } = new List<int>(200);
        public List<int> RoomActions { get; } = new List<int>(200);
        public List<int> RoomValues { get; } = new List<int>(200);
        public List<int> RoomFlags { get; } = new List<int>(200);
    }
}
