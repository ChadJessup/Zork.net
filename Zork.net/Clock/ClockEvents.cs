using System.Collections.Generic;

namespace Zork.Core.Clock
{
    public class ClockEvents
    {
        public int Count { get; set; }
        public List<int> Ticks { get; } = new List<int>(25);
        public List<int> Actions { get; } = new List<int>(25);
        public List<bool> Flags { get; } = new List<bool>(25);
    }
}
