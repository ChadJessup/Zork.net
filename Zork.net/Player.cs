namespace Zork.Core
{
    public class Player
    {
        public ActorIds Winner { get; set; }
        public RoomIds Here { get; set; }
        public bool TelFlag { get; set; }
    }
}
