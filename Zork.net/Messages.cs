using System.Collections.Generic;

namespace Zork.Core
{
    public class Messages
    {
        public int Count { get; set; }
        public int mrloc { get; set; }
        public List<int> rtext { get; set; } = new List<int>(1050);
        public List<string> text { get; set; } = new List<string>(1050);
    }
}
