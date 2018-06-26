using Humanizer;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Container for objects that are in this room.
        /// </summary>
        public List<Object> Objects { get; set; } = new List<Object>();

        /// <summary>
        /// Container for Adventurers that are in this room.
        /// </summary>
        public List<Adventurer> Adventurers { get; set; } = new List<Adventurer>();

        public bool HasObject(ObjectIds objId) => this.Objects.Any(o => o.IsOrHasObject(objId));

        public Object GetObject(ObjectIds objId) => this.Objects.FirstOrDefault(o => o.Id == objId);

        public override string ToString() => $"{this.Id} - {this.Flags} - {"items".ToQuantity(this.Objects.Count)}";
        public override int GetHashCode() => this.Id.GetHashCode();
    }
}
