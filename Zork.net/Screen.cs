using System.Collections.Generic;

namespace Zork.Core
{
    public class Screen
    {
        public int fromdr { get; set; } = 0;
        public int scolrm { get; set; } = 0;
        public int scolac { get; set; } = 0;
        public List<int> scoldr { get; } = new List<int>(8) { 1024, 153, 5120, 154, 3072, 152, 7168, 151 };
        public List<int> scolwl { get; } = new List<int>(12) { 151, 207, 3072, 152, 208, 7168, 153, 206, 5120, 154, 205, 1024 };
    }
}
