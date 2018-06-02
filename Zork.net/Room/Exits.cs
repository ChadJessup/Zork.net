using System.Collections.Generic;

namespace Zork.Core.Room
{
    public class Exits
    {
        public int Count { get; set; }
        public List<int> Travel { get; } = new List<int>(900);
    }
}
