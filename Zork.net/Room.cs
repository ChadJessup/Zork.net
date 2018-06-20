using System.Collections.Generic;

namespace Zork.Core
{
    public class Room
    {
        public int Id { get; set; }
        public RoomIds RoomId { get; set; }
        public int Description1 { get; set; }
        public int Description2 { get; set; }
        public int Exit { get; set; }
        public int Action { get; set; }
        public int Score { get; set; }
        public RoomFlags Flags { get; set;}
        public List<int> Travel { get; set; }
    }
}
