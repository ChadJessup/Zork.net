namespace Zork.Core
{
    public class Object
    {
        public ObjectIds Id { get; set; }
        public int Description1 { get; set; }
        public int Description2 { get; set; }
        public int odesco { get; set; }
        public int oactio { get; set; }
        public ObjectFlags Flag1 { get; set; }
        public ObjectFlags2 Flag2 { get; set; }
        public int ofval { get; set; }
        public int otval { get; set; }
        public int Size { get; set; }
        public int ocapac { get; set; }
        public RoomIds Room { get; set; }
        public ActorIds Adventurer { get; set; }
        public ObjectIds Container { get; set; }
        public int oread { get; set; }

        public override string ToString() => $"{this.Id} at {this.Room} and held by {this.Adventurer}";
    }
}