using System.Collections.Generic;

namespace Zork.Core.Room
{
    public class Rooms2
    {
        public int Count { get; set; }
        public List<int> Rooms { get; } = new List<int>(20);
        public List<int> RRoom { get; } = new List<int>(20);
    }
}
