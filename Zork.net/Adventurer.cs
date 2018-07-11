using System;
using System.Collections.Generic;
using System.Linq;

namespace Zork.Core
{
    public class Adventurer : IComparable, IComparable<Adventurer>
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

        public void DropObject(Object obj)
        {
            obj.Adventurer = ActorIds.NoOne;
            this.HeldObjects.Remove(obj);
        }

        public void PickupObject(Object obj)
        {
            obj.Adventurer = this.Id;
            this.HeldObjects.Add(obj);
            // obj.Container = adventurer.ObjectId;
        }

        public int CompareTo(Adventurer other) => ((int)this.Id).CompareTo((int)other.Id);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Adventurer other = obj as Adventurer;
            return this.CompareTo(other);
        }
    }
}
