using System.Collections.Generic;
using System.Linq;

namespace Zork.Core
{
    public class Adventurer
    {
        public ActorIds Id { get; set; }
        public Room CurrentRoom { get; set; }
        public int Score { get; set; }
        public int VehicleId { get; set; }
        public List<Object> HeldObjects { get; } = new List<Object>();
        public ObjectIds ObjectId { get; set; }
        public int Action { get; set; }
        public int Strength { get; set; }
        public int Flag { get; set; }

        public bool HasObject(ObjectIds objId) => this.HeldObjects.Any(co => co.IsOrHasObject(objId));
    }
}
