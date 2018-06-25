using System.Collections.Generic;

namespace Zork.Core
{
    public class Room
    {
        public RoomIds Id { get; set; }
        public int Description1 { get; set; }
        public int Description2 { get; set; }
        public int Exit { get; set; }
        public int Action { get; set; }
        public int Score { get; set; }
        public RoomFlags Flags { get; set;}
        public List<int> Travel { get; set; }

        public override string ToString() => $"{Id} - {Flags}";
        public override int GetHashCode() => this.Id.GetHashCode();
    }
}
