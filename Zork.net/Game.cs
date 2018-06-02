using Zork.Core.Clock;
using Zork.Core.Helpers;
using Zork.Core.Object;
using Zork.Core.Player;
using Zork.Core.Room;

namespace Zork.Core
{
    public class Game
    {
        public Star Star { get; } = new Star();
        public Exits Exits { get; } = new Exits();
        public Rooms Rooms { get; } = new Rooms();
        public Rooms2 Rooms2 { get; } = new Rooms2();
        public Objects Objects { get; } = new Objects();
        public Villians Villians { get; } = new Villians();
        public Messages Messages { get; } = new Messages();
        public PlayerState State { get; } = new PlayerState();
        public ClockEvents Clock { get; } = new ClockEvents();
        public Adventurer Adventurers { get; } = new Adventurer();

        public static Game Initialize() => DataLoader.LoadDataFile();
    }
}
