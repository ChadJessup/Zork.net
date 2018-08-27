using Newtonsoft.Json;
using Semver;
using System;
using System.Collections.Generic;

namespace Zork.Core
{
    public class Game
    {
        public Game() { }

        public Game(byte[] bytes)
        {
            this.Initialize(bytes);
        }

        private void Initialize(byte[] bytes)
        {
            this.Random = new Random(this.RandomSeed);
            this.Data = bytes;
            this.State.LightShaft = 10;
            this.State.MaxScore = this.State.LightShaft;
            this.State.egscor = 0;
            this.State.egmxsc = 0;
            this.State.MaxLoad = 100;
            this.State.RawScore = 0;
            this.State.Deaths = 0;
            this.State.Moves = 0;
            this.Time.pltime = 0;
            this.State.HelloSailor = 0;
            this.ParserVectors.prsa = 0;

            this.Hack.WasThiefIntroduced = false;

            // TODO: chadj - 6/24/18 - turning off thief during refactor of obj/rooms
            this.Hack.IsThiefActive = false;
            this.Hack.IsSwordActive = false;
            this.Hack.SwordStatus = 0;

            this.Star.mbase = 0;

            this.Flags.buoyf = true;
            this.Flags.egyptf = true;
            this.Flags.mr1f = true;
            this.Flags.mr2f = true;
            this.Flags.follwf = true;

            this.Switches.MatchCount = 4;
            this.Switches.LeftCell = 1;
            this.Switches.pnumb = 1;
            this.Switches.mdir = 270;
            this.Switches.mloc = RoomIds.mrb;
            this.Switches.cphere = 10;

            this.IsRunning = true;
        }

        public SemVersion Version { get; set; } = new SemVersion(3, 0, 0);
        public int RandomSeed { get; set; } = DateTime.Now.Millisecond;

        public Dictionary<ActorIds, Adventurer> Adventurers { get; set; } = new Dictionary<ActorIds, Adventurer>();
        public Dictionary<ObjectIds, Villian> Villians { get; set; } = new Dictionary<ObjectIds, Villian>();
        public Dictionary<ClockId, ClockEvent> Clock { get; set; } = new Dictionary<ClockId, ClockEvent>();
        public Dictionary<ObjectIds, Object> Objects { get; set; } = new Dictionary<ObjectIds, Object>();
        public Dictionary<RoomIds, Room> Rooms { get; set;  } = new Dictionary<RoomIds, Room>();

        public Time Time { get; } = new Time();
        public Star Star { get; } = new Star();
        public Last Last { get; } = new Last();
        public Hack Hack { get; } = new Hack();
        public Flags Flags { get; } = new Flags();
        public Random Random { get; set; }
        public Exits Exits { get; } = new Exits();
        public Switches Switches { get; } = new Switches();
        public Screen Screen { get; } = new Screen();
        public Rooms2 Rooms2 { get; } = new Rooms2();
        public Player Player { get; } = new Player();
        public Syntax Syntax { get; } = new Syntax();
        public CurrentExit CurrentExit { get; } = new CurrentExit();
        public Orphans Orphans { get; } = new Orphans();
        public Messages Messages { get; } = new Messages();
        public PlayerState State { get; } = new PlayerState();

        public hyper_ hyper_ { get; } = new hyper_();

        public ParserVectors ParserVectors { get; } = new ParserVectors();

        public pv pv_1 { get; set; } = new pv();
        public objvec ObjectVector { get; set; } = new objvec();
        public prpvec prpvec { get; set; } = new prpvec();

        public byte[] Data { get; set; } = new byte[1];
        public int DataPosition { get; set; }

        public int astag { get; set; } = 32768;

        /// <summary>
        /// Gets or sets the method that provides input.
        /// </summary>
        public Func<string> ReadInput { get; set; }

        /// <summary>
        /// Gets or sets the method that displays the text output.
        /// </summary>
        public Action<string> WriteOutput { get; set; }

        public EventHandler<MovedEventArgs> MoveOccurred { get; set; }

        public static Game Initialize(bool useJson = false) => DataLoader.LoadDataFile(useJson: useJson);

        public int rnd_(int maxVal) => this.Random.Next(maxVal);

        public bool IsRunning { get; set; } = true;

        public string CurrentRoomName => this.Rooms[this.Player.Here].Name;
        public int CurrentScore => this.State.RawScore;
        public int MovesCount => this.State.Moves;

        public Queue<string> QueuedCommands = new Queue<string>();

        public void Exit()
        {
            this.WriteOutput("The game is over.");
            this.IsRunning = false;
        }

        public void Play()
        {
            MessageHandler.Speak(1, this);

            bool result = RoomHandler.RoomDescription(Verbosity.Full, this);

            bool f = false;
            int i = 0;

            while (this.IsRunning)
            {
                L100:
                if (!this.IsRunning)
                {
                    // Game is over, exit this game loop.
                    continue;
                }

                this.Player.Winner = ActorIds.Player;
                this.Player.TelFlag = false;

                string input = string.Empty;

                if (this.ParserVectors.prscon <= 1)
                {
                    input = Parser.ReadLine(this, 1);

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }

                    this.ParserVectors.prscon = 1;
                }

                ++this.State.Moves;
                this.ParserVectors.prswon = Parser.Parse(input, true, this);

                if (!this.ParserVectors.prswon)
                {
                    goto ENDOFMOVE;
                }

                if (xvehic_(1))
                {
                    goto ENDOFMOVE;
                }

                if (this.ParserVectors.prsa == VerbId.Tell)
                {
                    //goto L2000;
                }

                L300:
                if (this.ParserVectors.DirectObject == ObjectIds.valua ||
                    this.ParserVectors.DirectObject == ObjectIds.every)
                {
                    goto L900;
                }

                if (!Parser.ProcessVerb(input, this.ParserVectors.prsa, this))
                {
                    goto ENDOFMOVE;
                }

                L350:
                if (!Flags.echof && this.Player.Here == RoomIds.echor)
                {
                    goto L1000;
                }

                f = RoomHandler.RunRoomAction(this.Rooms[this.Player.Here], this);

                ENDOFMOVE:
                // !DO END OF MOVE.
                xendmv_(this.Player.TelFlag);

                if (!RoomHandler.IsRoomLit(this.Player.Here, this))
                {
                    this.ParserVectors.prscon = 1;
                }

                goto L100;

                L900:
                dverb1.valuac_(this, ObjectIds.valua);
                goto L350;
                // GAME, PAGE 3

                // SPECIAL CASE-- ECHO ROOM.
                // IF INPUT IS NOT 'ECHO' OR A DIRECTION, JUST ECHO.

                L1000:
                input = Parser.ReadLine(this, 0);

                // !CHARGE FOR MOVES.
                ++this.State.Moves;

                if (input.Equals("ECHO"))
                {
                    goto L1300;
                }

                MessageHandler.Speak(571, this);

                // !KILL THE ECHO.
                Flags.echof = true;
                this.Objects[ObjectIds.Bar].Flag2 &= ~ObjectFlags2.SCRDBT;

                this.ParserVectors.prswon = true;
                // !FAKE OUT PARSER.
                this.ParserVectors.prscon = 1;
                // !FORCE NEW INPUT.
                goto ENDOFMOVE;

                L1300:
                this.ParserVectors.prswon = Parser.Parse(input, false, this);
                if (!this.ParserVectors.prswon || this.ParserVectors.prsa != VerbId.Walk)
                {
                    goto L1400;
                }

                if (dso3.FindExit(this, (int)this.ParserVectors.DirectObject, this.Player.Here))
                {
                    goto L300;
                }

                // !VALID EXIT?

                L1400:
                MessageHandler.more_output(this, input);
                this.Player.TelFlag = true;
                // !INDICATE OUTPUT.
                goto L1000;
                // !MORE ECHO ROOM.
                // GAME, PAGE 4

                // SPECIAL CASE-- TELL <ACTOR>, NEW COMMAND
                // NOTE THAT WE CANNOT BE IN THE ECHO ROOM.

                L2000:
                if ((this.Objects[this.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsActor) != 0)
                {
                    goto L2100;
                }

                MessageHandler.Speak(602, this);
                // !CANT DO IT.
                goto L350;
                // !VAPPLI SUCCEEDS.

                L2100:
                this.Player.Winner = ObjectHandler.GetActor(this.ParserVectors.DirectObject, this);
                // !NEW PLAYER.
                this.Player.Here = this.Adventurers[this.Player.Winner].CurrentRoom.Id;

                // !NEW LOCATION.
                if (this.ParserVectors.prscon <= 1)
                {
                    goto L2700;
                }

                // !ANY INPUT?
                if (Parser.Parse(input, true, this))
                {
                    goto L2150;
                }

                L2700:
                i = 341;
                // !FAILS.
                if (this.Player.TelFlag)
                {
                    i = 604;
                }
                // !GIVE RESPONSE.
                MessageHandler.Speak(i, this);

                L2600:
                this.Player.Winner = ActorIds.Player;
                // !RESTORE STATE.
                this.Player.Here = this.Adventurers[this.Player.Winner].CurrentRoom.Id;
                goto L350;

                L2150:
                if (Actors.aappli_(this, this.Adventurers[this.Player.Winner].Action))
                {
                    goto L2400;
                }

                // !ACTOR HANDLE?
                //if (xvehic_(1))
                //{
                //    goto L2400;
                //}

                // !VEHICLE HANDLE?
                if (this.ParserVectors.DirectObject == ObjectIds.valua || this.ParserVectors.DirectObject == ObjectIds.every)
                {
                    goto L2900;
                }
                if (!Parser.ProcessVerb(input, this.ParserVectors.prsa, this))
                {
                    goto L2400;
                }
                // !VERB HANDLE?
                // L2350:
                f = RoomHandler.RunRoomAction(this.Rooms[this.Player.Here], this);

                L2400:
                xendmv_(this.Player.TelFlag);
                // !DO END OF MOVE.
                goto L2600;
                // !DONE.

                L2900:
                dverb1.valuac_(this, ObjectIds.valua);
                // !ALL OR VALUABLES.
                goto L350;
            }
        }

        // XENDMV-	EXECUTE END OF MOVE FUNCTIONS.
        public void xendmv_(bool flag)
        {
            bool f;

            // !DEFAULT REMARK.
            if (!(flag))
            {
                MessageHandler.Speak(341, this);
            }

            // !THIEF DEMON.
            if (this.Hack.IsThiefActive)
            {
                Actors.thiefd_(this);
            }

            // !FIGHT DEMON.
            if (this.ParserVectors.prswon)
            {
                DemonHandler.Fight(this);
            }

            // !SWORD DEMON.
            if (this.Hack.IsSwordActive)
            {
                DemonHandler.swordd_(this);
            }

            // !CLOCK DEMON.
            if (this.ParserVectors.prswon)
            {
                f = ClockEvents.clockd_(this);
            }

            // !VEHICLE READOUT.
            if (this.ParserVectors.prswon)
            {
                f = xvehic_(2);
            }

            this.MoveOccurred?.Invoke(this, new MovedEventArgs(this));
        }

        // XVEHIC- EXECUTE VEHICLE FUNCTION
        public bool xvehic_(int n)
        {
            bool ret_val;
            ObjectIds av;

            // !ASSUME LOSES.
            ret_val = false;
            // !GET VEHICLE.
            av = (ObjectIds)this.Adventurers[this.Player.Winner].VehicleId;
            if (av != 0)
            {
                ret_val = ObjectHandler.DoObjectSpecialAction(this.Objects[av], n, this);
            }

            return ret_val;
        }
    }

    public class hyper_
    {
        public int hfactr { get; set; } = 500;
    }

    public class CurrentExit
    {
        public int ExitType { get; set; }
        public RoomIds xroom1 { get; set; }
        public int xstrng { get; set; }
        public int xactio { get; set; }
        public ObjectIds xobj { get; set; }
    }
}
