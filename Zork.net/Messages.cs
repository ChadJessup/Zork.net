using System.Collections.Generic;

namespace Zork.Core
{
    public class Messages
    {
        public int Count { get; set; }
        public int mrloc { get; set; }
        public List<int> rtext { get; } = new List<int>(1050);
    }
}
