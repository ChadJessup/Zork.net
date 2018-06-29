using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public class Adventurer
    {
        public ActorIds Id { get; set; } = ActorIds.NoOne;
        public int Flags { get; set; }
        public int Score { get; set; }
        public int Strength { get; set; }
        public int VehicleId { get; set; }
        public RoomIds RoomId { get; set; } = RoomIds.NoWhere;
        public ObjectIds ObjectId { get; set; } = ObjectIds.Nothing;
        public List<Object> Objects { get; set; } = new List<Object>();
    }
}
