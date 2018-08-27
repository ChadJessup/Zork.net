using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zork.Core
{
    public class Room : IComparable, IComparable<Room>
    {
        public Room() { }
        public Room(Room room)
        {
            this.Id = room.Id;
            this.Name = room.Name;
            this.Description = room.Description;
            this.ShortDescription = room.ShortDescription;
            this.Exit = room.Exit;
            this.Action = room.Action;
            this.Score = room.Score;
            this.Flags = room.Flags;
            this.Travel = room.Travel;
            this.Villians = room.Villians;
            this.Adventurers = room.Adventurers;
            this.Objects = room.Objects;

            this.DoAction = room.DoAction;
        }

        public RoomIds Id { get; set; }
        public string Name { get; set; }
        public int Description1Id { get; set; }
        public int Description2Id { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int Exit { get; set; }
        public int Action { get; set; }
        public int Score { get; set; }
        public RoomFlags Flags { get; set;}
        public List<int> Travel { get; set; }

        public int NumericId => (int)this.Id;

        /// <summary>
        /// Container for any villians that might be in this room.
        /// </summary>
        public List<Villian> Villians { get; set; } = new List<Villian>();

        /// <summary>
        /// Container for Adventurers that are in this room.
        /// </summary>
        public List<Adventurer> Adventurers { get; set; } = new List<Adventurer>();

        public bool HasAdventurer(ActorIds actorId) => this.Adventurers.Any(a => a.Id == actorId);
        public Adventurer GetAdventurer(ActorIds actorId) => this.Adventurers.FirstOrDefault(a => a.Id == actorId);

        /// <summary>
        /// Container for objects that are in this room.
        /// </summary>
        public List<Object> Objects { get; set; } = new List<Object>();

        /// <summary>
        /// Unique room action.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public Func<Game, bool> DoAction { get; set; }

        public virtual bool RoomAction(Game game) => false;

        public bool HasObject(ObjectIds objId) => this.Objects.Any(o => o.IsOrHasObject(objId)) || this.Adventurers.Any(a => a.HasObject(objId));
        public Object GetObject(ObjectIds objId) => this.Objects.FirstOrDefault(o => o.Id == objId);

        public override string ToString() => $"#{this.NumericId} {this.Name}: Id: {this.Id} - {this.Flags} - {"items".ToQuantity(this.Objects.Count)}";
        public override int GetHashCode() => this.Id.GetHashCode();

        public int CompareTo(Room other) => ((int)this.Id).CompareTo((int)other.Id);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Room other = obj as Room;
            return this.CompareTo(other);
        }
    }
}
