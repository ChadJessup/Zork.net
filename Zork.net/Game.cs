using Zork.Core.Clock;
using Zork.Core.Helpers;
using Zork.Core.Object;
using Zork.Core.Room;

namespace Zork.Core
{
    public class Game
    {
        public Game(byte[] bytes) => this.Data = bytes;

        public Time Time { get; } = new Time();
        public Star Star { get; } = new Star();
        public Last Last { get; } = new Last();
        public Hack Hack { get; } = new Hack();
        public Flags Flags { get; } = new Flags();
        public Exits Exits { get; } = new Exits();
        public Rooms Rooms { get; } = new Rooms();
        public Screen Screen { get; } = new Screen();
        public Rooms2 Rooms2 { get; } = new Rooms2();
        public Player Player { get; } = new Player();
        public Syntax Syntax { get; } = new Syntax();
        public Objects Objects { get; } = new Objects();
        public Orphans Orphans { get; } = new Orphans();
        public Villians Villians { get; } = new Villians();
        public Messages Messages { get; } = new Messages();
        public PlayerState State { get; } = new PlayerState();
        public ClockEvents Clock { get; } = new ClockEvents();
        public Adventurer Adventurers { get; } = new Adventurer();

        // TODO: Figure out naming later...
        public ParserVector ParserVector { get; } = new ParserVector();
        public ParserVectors ParserVectors { get; } = new ParserVectors();

        public int DataPosition { get; set; }
        public byte[] Data { get; }

        public static Game Initialize() => DataLoader.LoadDataFile();

        public void Play()
        {
            MessageHandler.Speak(1, this);
            bool result = RoomHandler.RoomDescription(3, this);

            // L100
            while (true)
            {
                this.Player.Winner = (int)AIndices.player;
                this.Player.TelFlag = false;

                string input = string.Empty;

                if (this.ParserVectors.prscon <= 1)
                {
                    input = Parser.ReadLine(1);
                }

                ++this.State.Moves;
                this.ParserVectors.prswon = Parser.Parse(input, true, this);
            }
        }
    }
}
