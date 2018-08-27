
using Zork.Core.Attributes;

namespace Zork.Core.Rooms
{
    [Room(RoomIds.WestOfHouse)]
    public class WestOfHouseRoom : Room
    {
        public WestOfHouseRoom(Room room) : base(room)
        {
        }

        public override bool RoomAction(Game game)
        {
            return false;
        }
    }
}
