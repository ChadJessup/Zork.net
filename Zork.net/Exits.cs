using System.Collections.Generic;

namespace Zork.Core
{
    public class Exits
    {
        public int Count { get; set; }
        public List<int> Travel { get; set; } = new List<int>(900);
    }
}
