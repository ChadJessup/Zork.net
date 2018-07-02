using System.Collections.Generic;

namespace Zork.Core
{
    public class Adventurer
    {
        public ActorIds Id { get; set; }
        public Room CurrentRoom { get; set; }
        public int Score { get; set; }
        public int Vehicle { get; set; }
        public List<Object> HeldObjects { get; } = new List<Object>();
        public ObjectIds Object { get; set; }
        public int Action { get; set; }
        public int Strength { get; set; }
        public int Flag { get; set; }
    }
}
