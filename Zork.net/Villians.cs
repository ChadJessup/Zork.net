using System.Collections.Generic;

namespace Zork.Core
{
    public class Villians
    {
        public int Count { get; set; }
        public List<int> villns { get; } = new List<int>(4);
        public List<int> vprob { get; } = new List<int>(4);
        public List<int> vopps { get; } = new List<int>(4);
        public List<int> vbest { get; } = new List<int>(4);
        public List<int> vmelee { get; } = new List<int>(4);
    }
}
