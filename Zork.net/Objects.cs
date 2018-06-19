using System.Collections.Generic;

namespace Zork.Core
{
    public class Objects
    {
        public int Count { get; set; }
        public List<int> odesc1 = new List<int>(220);
        public List<int> odesc2 = new List<int>(220);
        public List<int> odesco = new List<int>(220);
        public List<int> oactio = new List<int>(220);
        public List<ObjectFlags> oflag1 = new List<ObjectFlags>(220);
        public List<ObjectFlags2> oflag2 = new List<ObjectFlags2>(220);
        public List<int> ofval = new List<int>(220);
        public List<int> otval = new List<int>(220);
        public List<int> osize = new List<int>(220);
        public List<int> ocapac = new List<int>(220);
        public List<int> oroom = new List<int>(220);
        public List<int> oadv = new List<int>(220);
        public List<int> ocan = new List<int>(220);
        public List<int> oread = new List<int>(220);
    }
}
