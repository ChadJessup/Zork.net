using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public class Adventurer
    {
        public int Count { get; set; }
        public List<int> Rooms { get; } = new List<int>();
        public List<int> Scores { get; } = new List<int>();
        public List<int> Vehicles { get; } = new List<int>();
        public List<int> Objects { get; } = new List<int>();
        public List<int> Actions { get; } = new List<int>();
        public List<int> astren { get; } = new List<int>();
        public List<int> Flags { get; } = new List<int>();
    }
}
