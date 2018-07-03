using System.Collections.Generic;
using System.Linq;

namespace Zork.Core
{
    public class Object
    {
        public ObjectIds Id { get; set; }
        public int Description1 { get; set; }
        public int Description2 { get; set; }
        public int odesco { get; set; }
        public int Action { get; set; }
        public ObjectFlags Flag1 { get; set; }
        public ObjectFlags2 Flag2 { get; set; }
        public int Value { get; set; }
        public int otval { get; set; }
        public int Size { get; set; }
        public int Capacity { get; set; }
      //  public RoomIds Room { get; set; }
        public ActorIds Adventurer { get; set; }
        public ObjectIds Container { get; set; }
        public int oread { get; set; }

        public bool CanSeeInside => this.IsContainer && this.Flag1.HasFlag(ObjectFlags.IsTransparent) || this.Flag2.HasFlag(ObjectFlags2.IsOpen);
        public bool IsContainer => this.Capacity != 0;
        public int Weight => this.Size + this.ContainedObjects.Sum(co => co.Size);
        public bool IsOrHasObject(ObjectIds objId) => this.Id == objId || this.ContainedObjects.Any(co => co.IsOrHasObject(objId));

        /// <summary>
        /// Objects can be containers, this is the collection for the contained objects.
        /// </summary>
        public List<Object> ContainedObjects { get; set; } = new List<Object>();
        public bool IsVisible => this.Flag1.HasFlag(ObjectFlags.IsVisible);

        public override string ToString() => $"{this.Id} held by {this.Adventurer}";
    }
}