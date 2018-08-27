using System;
using System.Linq;

namespace Zork.Core
{
    public static class RoomHandler
    {
        public static bool RoomDescription(Game game, Verbosity verbosity) => RoomDescription(verbosity, game);
        public static bool RoomDescription(Verbosity verbosity, Game game)
        {
            bool ret_val = true;
            string message;
            var room = game.Rooms[game.Player.Here];

            // FULL= 0/1/2/3= SHORT/OBJ/ROOM/FULL

            // !IF DIRECTION,
            if (game.ParserVectors.DirectObject < (ObjectIds)XSearch.xmin)
            {
                goto L50;
            }

            // !SAVE AND
            game.Screen.FromDirection = (int)game.ParserVectors.DirectObject;

            // !CLEAR.
            game.ParserVectors.DirectObject = 0;

            L50:
            // !PLAYER JUST MOVE?
            if (game.Player.Here == game.Adventurers[ActorIds.Player].CurrentRoom.Id)
            {
                goto L100;
            }

            // !NO, JUST SAY DONE.
            MessageHandler.Speak(2, game);

            // !SET UP WALK IN ACTION.
            game.ParserVectors.prsa = VerbId.WalkIn;

            return ret_val;

            L100:
            // !LIT?
            if (RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L300;
            }

            // !WARN OF GRUE.
            MessageHandler.Speak(430, game);

            ret_val = false;
            return ret_val;

            L300:

            // !OBJ ONLY?
            if (verbosity == Verbosity.ObjectsOnly)
            {
                goto L600;
            }

            // !ASSUME SHORT DESC.
            message = room.ShortDescription;

            if (verbosity == Verbosity.Short && (game.Flags.SuperBriefDescriptions ||
                (room.Flags.HasFlag(RoomFlags.SEEN) && game.Flags.BriefDescriptions)))
            {
                goto L400;
            }

            //  The next line means that when you request VERBOSE mode, you
            //  only get long room descriptions 20% of the time. I don't either
            //  like or understand this, so the mod. ensures VERBOSE works
            //  all the time.			jmh@ukc.ac.uk 22/10/87

            // & .AND.(BRIEFF.OR.PROB(80,80)))))       GO TO 400
            message = room.Description;

            // !IF GOT DESC, SKIP.
            if (!string.IsNullOrWhiteSpace(message) || room.Action == 0)
            {
                goto L400;
            }

            // !PRETEND LOOK AROUND.
            game.ParserVectors.prsa = VerbId.Look;

            // !ROOM HANDLES, NEW DESC?
            if (!RunRoomAction(room, game))
            {
                goto L100;
            }

            // !NOP PARSER.
            game.ParserVectors.prsa = VerbId.foow;

            goto L500;

            L400:
            // !OUTPUT DESCRIPTION.
            MessageHandler.Speak(message, game);

            L500:
            if (game.Adventurers[game.Player.Winner].VehicleId != 0)
            {
                MessageHandler.rspsub_(431, game.Objects[(ObjectIds)game.Adventurers[game.Player.Winner].VehicleId].Description2Id, game);
            }

            L600:
            //if (verbosity != Verbosity.RoomAndContents)
            {
                RoomHandler.PrintRoomContents(verbosity, game.Player.Here, game);
            }

            game.Rooms[game.Player.Here].Flags |= RoomFlags.SEEN;
            // !ANYTHING MORE?
            if (verbosity != Verbosity.Short || room.Action == 0)
            {
                return ret_val;
            }

            game.ParserVectors.prsa = VerbId.WalkIn;

            // !GIVE HIM A SURPISE.
            if (!RunRoomAction(room, game))
            {
                goto L100;
            }

            // !ROOM HANDLES, NEW DESC?
            game.ParserVectors.prsa = VerbId.foow;

            return ret_val;
        }

        /// <summary>
        /// princr_ - PRINT CONTENTS OF ROOM
        /// </summary>
        /// <param name="verbosity"></param>
        /// <param name="roomId"></param>
        /// <param name="game"></param>
        public static void PrintRoomContents(Verbosity verbosity, RoomIds roomId, Game game)
        {
            bool full;
            int k;
            int j = 329;

            var room = game.Rooms[roomId];

            foreach (var obj in room.Objects.Where(o => o.IsVisible && !o.Flag1.HasFlag(ObjectFlags.HasNoDescription)))
            {
                string message = "";

                switch (verbosity)
                {
                    case Verbosity.Full:
                        message = obj.LongDescription;
                        break;
                    case Verbosity.Short:
                        message = obj.ShortDescription;
                        break;
                    case Verbosity.ObjectsOnly:
                        message = obj.Description;
                        break;
                    case Verbosity.RoomAndContents:
                        message = obj.LongDescription;
                        break;
                    default:
                        message = obj.Description;
                        break;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = obj.Description;
                }

                MessageHandler.Speak(message, game);
            }

            foreach (var obj in room.Objects.Where(o => o.IsContainer && o.CanSeeInside))
            {
                if (obj.ContainedObjects.Any())
                {
                    ObjectHandler.PrintDescription(obj.Id, 573, game);
                }
            }

            return;

            // !ASSUME SUPERBRIEF FORMAT.
            for (ObjectIds i = 0; i < (ObjectIds)game.Objects.Count; ++i)
            {
                // !LOOP ON OBJECTS
                if (!ObjectHandler.IsInRoom(roomId, i, game)
                    || (game.Objects[i].Flag1 & (int)ObjectFlags.IsVisible + ObjectFlags.HasNoDescription) != ObjectFlags.IsVisible
                    || (i == (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId))
                {
                    goto L500;
                }

                if (!(full) && (game.Flags.SuperBriefDescriptions || game.Flags.BriefDescriptions && (game.Rooms[game.Player.Here].Flags & RoomFlags.SEEN) != 0))
                {
                    goto L200;
                }

                // DO LONG DESCRIPTION OF OBJECT

                k = game.Objects[i].LongDescriptionId;
                // !GET UNTOUCHED
                if (k == 0 || game.Objects[i].Flag2.HasFlag(ObjectFlags2.WasTouched))
                {
                    k = game.Objects[i].Description1Id;
                }

                // !DESCRIBE
                MessageHandler.Speak(k, game);

                // DO SHORT DESCRIPTION OF OBJECT
                goto L500;

                L200:
                // !YOU CAN SEE IT
                MessageHandler.rspsub_(j, game.Objects[i].Description2Id, game);
                j = 502;

                L500:
                ;
            }

            // NOW LOOP TO PRINT CONTENTS OF OBJECTS IN ROOM
            for (ObjectIds i = 0; i < (ObjectIds)game.Objects.Count; ++i)
            {
                // !LOOP ON OBJECTS
                if (!ObjectHandler.IsInRoom(roomId, i, game) || (game.Objects[i].Flag1 & (int)ObjectFlags.IsVisible + ObjectFlags.HasNoDescription) != ObjectFlags.IsVisible)
                {
                    goto L1000;
                }

                if ((game.Objects[i].Flag2 & ObjectFlags2.IsActor) != 0)
                {
                    AdventurerHandler.PrintContents(ObjectHandler.GetActor(i, game), game);
                }

                // OBJECT IS NOT EMPTY AND IS OPEN OR TRANSPARENT
                if ((game.Objects[i].Flag1 & ObjectFlags.IsTransparent) == 0
                    && (game.Objects[i].Flag2 & ObjectFlags2.IsOpen) == 0
                    || ObjectHandler.IsObjectEmpty(i, game))
                {
                    goto L1000;
                }

                j = 573;
                // !TROPHY CASE?
                if (i != ObjectIds.TrophyCase)
                {
                    goto L600;
                }

                j = 574;
                if ((game.Flags.BriefDescriptions || game.Flags.SuperBriefDescriptions) && !(full))
                {
                    goto L1000;
                }

                L600:
                ObjectHandler.PrintDescription(i, j, game);
                // !PRINT CONTENTS

                L1000:
                ;
            }
        }

        public static Room GetRoomThatContainsObject(ObjectIds objId, Game game)
        {
            if (objId == ObjectIds.Nothing)
            {
                return null;
            }

            return game.Rooms.Values.FirstOrDefault(r => r.HasObject(objId));
        }

        public static bool IsRoomLit(RoomIds roomId, Game game)
        {
            bool ret_val = true;

            ActorIds oa;

            // !ASSUME WINS
            if ((game.Rooms[roomId].Flags.HasFlag(RoomFlags.LIGHT)))
            {
                return ret_val;
            }

            // !LOOK FOR LIT OBJ
            foreach (var obj in game.Objects.Values)
            {
                // !IN ROOM?
                if (ObjectHandler.IsInRoom(roomId, obj.Id, game))
                {
                    goto L100;
                }

                oa = obj.Adventurer;
                // !NO
                if (oa <= 0)
                {
                    goto L1000;
                }

                //!ON ADV?
                if (game.Adventurers[oa].CurrentRoom.Id != roomId)
                {
                    goto L1000;
                }

                // !ADV IN ROOM?

                // OBJ IN ROOM OR ON ADV IN ROOM

                L100:
                if ((obj.Flag1.HasFlag(ObjectFlags.IsOn)))
                {
                    return ret_val;
                }

                // OBJ IS VISIBLE AND OPEN OR TRANSPARENT
                if ((!obj.Flag1.HasFlag(ObjectFlags.IsVisible))
                    || (!obj.Flag1.HasFlag(ObjectFlags.IsTransparent))
                    && (!obj.Flag2.HasFlag(ObjectFlags2.IsOpen)))
                {
                    goto L1000;
                }

                foreach (var containerObj in game.Objects.Values)
                {
                    if (containerObj.Container == obj.Id && (containerObj.Flag1.HasFlag(ObjectFlags.IsOn)))
                    {
                        return ret_val;
                    }

                    // L500:
                }
                L1000:
                ;
            }

            ret_val = false;
            return ret_val;
        }

        /// <summary>
        /// rappli_ - ROUTING ROUTINE FOR ROOM APPLICABLES
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static bool RunRoomAction(Room room, Game game)
        {
            // Custom room types have their own complex actions...
            if (room.GetType() != typeof(Room))
            {
                return room.RoomAction(game);
            }

            // But sometimes, we just want a simple action on a room...
            if (room.DoAction != null)
            {
                return room.DoAction.Invoke(game);
            }

            // And, old code that will eventually be deprecated...

            const int newrms = 38;

            // System generated locals
            bool ret_val = true;

            // !ASSUME WINS.
            // !IF ZERO, WIN.
            if (room.Action == 0)
            {
                return ret_val;
            }

            // !IF OLD, PROCESSOR 1.
            if (room.Action < newrms)
            {
                ret_val = RoomHandler.RunRoomAction1(room, game);
            }

            // !IF NEW, PROCESSOR 2.
            if (room.Action >= newrms)
            {
                ret_val = RoomHandler.RunRoomAction2(room, game);
            }

            return ret_val;
        }

        public static bool RunRoomAction1(Room room, Game game)
        {
            int i__1, i__2;
            bool ret_val = true;

            bool f;
            ObjectIds i;
            ObjectIds j;

            // !USUALLY IGNORED.
            if (room.Action == 0)
            {
                return ret_val;
            }

            // !RETURN IF NAUGHT.

            // !SET TO FALSE FOR

            // !NEW DESC NEEDED.

            switch (room.Action)
            {
                case 5: goto MAZE11;
                case 6: goto CLEARING;
                case 7: goto RESERVOIRSOUTH;
                case 8: goto RESERVOIR;
                case 9: goto RESERVOIRNORTH;
                case 10: goto GLACIERROOM;
                case 11: goto FORESTROOM;
                case 12: goto MIRRORROOM;
                case 13: goto CAVE2ROOM;
                case 14: goto BOOMROOM;
                case 15: goto NOOBJS;
                case 16: goto MACHINEROOM;
                case 17: goto BATROOM;
                case 18: goto DOMEROOM;
                case 19: goto TORCHROOM;
                case 20: goto CAROUSELROOM;
                case 21: goto LLDROOM;
                case 22: goto LLD2ROOM;
                case 23: goto DAMROOM;
                case 24: goto TREEROOM;
                case 25: goto CYCLOPSROOM;
                case 26: goto BANKBOX;
                case 27: goto TREASURE;
                case 28: goto CLIFF;
                case 29: goto RIVER4;
                case 30: goto OVERFALLS;
                case 31: goto BEACHROOM;
                case 32: goto TCAVE;
                case 33: goto FALLSROOM;
                case 34: goto L34000;
                case 35: goto SAFEROOM;
                case 36: goto MAGNETROOM;
                case 37: goto CAGEROOM;
            }

            throw new InvalidOperationException("1");
            //bug_(1, ri);


            // R5--	MAZE11.  DESCRIBE STATE OF GRATING.

            MAZE11:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(23, game);
            // !DESCRIBE.
            i = (ObjectIds)24;
            // !ASSUME LOCKED.
            if (game.Flags.IsGrateUnlocked)
            {
                i = (ObjectIds)26;
            }

            // !UNLOCKED?
            if (game.Objects[ObjectIds.Grate].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                i = (ObjectIds)25;
            }
            // !OPEN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE GRATE.
            return ret_val;

            // R6--	CLEARING.  DESCRIBE CLEARING, MOVE LEAVES.

            CLEARING:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L6500;
            }

            // !DESCRIBE.
            MessageHandler.Speak(27, game);

            if (game.Switches.rvclr == 0)
            {
                return ret_val;
            }

            // !LEAVES MOVED?
            // !YES, ASSUME GRATE CLOSED.
            i = (ObjectIds)28;

            // !OPEN?
            if (game.Objects[ObjectIds.Grate].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                i = (ObjectIds)29;
            }

            // !DESCRIBE GRATE.
            MessageHandler.Speak(i, game);

            return ret_val;

            L6500:
            if (game.Switches.rvclr != 0 || ObjectHandler.IsObjectInRoom(ObjectIds.leave, RoomIds.ForestClearing, game)
                && (game.ParserVectors.prsa != VerbId.Move || game.ParserVectors.DirectObject != ObjectIds.leave))
            {
                return ret_val;
            }

            // !MOVE LEAVES, REVEAL GRATE.
            MessageHandler.Speak(30, game);
            // !INDICATE LEAVES MOVED.
            game.Switches.rvclr = 1;
            return ret_val;
            // RAPPL1, PAGE 4

            // R7--	RESERVOIR SOUTH.  DESCRIPTION DEPENDS ON LOW TIDE FLAG.

            RESERVOIRSOUTH:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !ASSUME FULL.
            i = (ObjectIds)31;
            // !IF LOW TIDE, EMPTY.
            if (game.Flags.IsLowTide)
            {
                i = (ObjectIds)32;
            }

            // !DESCRIBE.
            MessageHandler.Speak(i, game);
            // !DESCRIBE EXITS.
            MessageHandler.Speak(33, game);
            return ret_val;

            // R8--	RESERVOIR.  STATE DEPENDS ON LOW TIDE FLAG.

            RESERVOIR:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !ASSUME FULL.
            i = (ObjectIds)34;
            // !IF LOW TIDE, EMTPY.
            if (game.Flags.IsLowTide)
            {
                i = (ObjectIds)35;
            }

            // !DESCRIBE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // R9--	RESERVOIR NORTH.  ALSO DEPENDS ON LOW TIDE FLAG.

            RESERVOIRNORTH:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !YOU GET THE IDEA.
            i = (ObjectIds)36;
            if (game.Flags.IsLowTide)
            {
                i = (ObjectIds)37;
            }

            MessageHandler.Speak(i, game);
            MessageHandler.Speak(38, game);

            return ret_val;

            // R10--	GLACIER ROOM.  STATE DEPENDS ON MELTED, VANISHED FLAGS.

            GLACIERROOM:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !BASIC DESCRIPTION.
            MessageHandler.Speak(39, game);
            // !ASSUME NO CHANGES.
            i = 0;
            // !PARTIAL MELT?
            if (game.Flags.IsGlacierPartiallyMelted)
            {
                i = (ObjectIds)40;
            }

            // !COMPLETE MELT?
            if (game.Flags.IsGlacierFullyMelted)
            {
                i = (ObjectIds)41;
            }

            // !DESCRIBE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // R11--	FOREST ROOM

            FORESTROOM:
            // !IF WALK IN, BIRDIE.
            if (game.ParserVectors.prsa == VerbId.WalkIn)
            {
                game.Clock[ClockId.cevfor].Flags = true;
            }

            return ret_val;

            // R12--	MIRROR ROOM.  STATE DEPENDS ON MIRROR INTACT.

            MIRRORROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !DESCRIBE.
            MessageHandler.Speak(42, game);

            // !IF BROKEN, NASTY REMARK.
            if (game.Flags.IsMirrorBroken)
            {
                MessageHandler.Speak(43, game);
            }

            return ret_val;
            // RAPPL1, PAGE 5

            // R13--	CAVE2 ROOM.  BLOW OUT CANDLES WITH 50% PROBABILITY.

            CAVE2ROOM:
            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                return ret_val;
            }

            // !WALKIN?
            if (prob_(game, 50, 50) ||
                game.Objects[ObjectIds.Candle].Adventurer != game.Player.Winner ||
                !game.Objects[ObjectIds.Candle].Flag1.HasFlag(ObjectFlags.IsOn))
            {
                return ret_val;
            }

            game.Objects[ObjectIds.Candle].Flag1 &= ~ObjectFlags.IsOn;
            // !TELL OF WINDS.
            MessageHandler.Speak(47, game);

            // !HALT CANDLE COUNTDOWN.
            game.Clock[ClockId.CandleClock].Flags = false;

            return ret_val;

            // R14--	BOOM ROOM.  BLOW HIM UP IF CARRYING FLAMING OBJECT.

            BOOMROOM:
            j = (ObjectIds)game.Objects[ObjectIds.Candle].Description2Id;
            // !ASSUME CANDLE.
            if (game.Objects[ObjectIds.Candle].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Candle].Flag1.HasFlag(ObjectFlags.IsOn))
            {
                goto L14100;
            }

            j = (ObjectIds)game.Objects[ObjectIds.Torch].Description2Id;

            // !ASSUME TORCH.
            if (game.Objects[ObjectIds.Torch].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Torch].Flag1.HasFlag(ObjectFlags.IsOn))
            {
                goto L14100;
            }

            j = (ObjectIds)game.Objects[ObjectIds.Match].Description2Id;

            if (game.Objects[ObjectIds.Match].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Match].Flag1.HasFlag(ObjectFlags.IsOn))
            {
                goto L14100;
            }

            return ret_val;
            // !SAFE

            L14100:
            // !TURN ON?
            if (game.ParserVectors.prsa != VerbId.TurnOn)
            {
                goto L14200;
            }

            // !BOOM
            MessageHandler.rspsub_(294, (int)j, game);
            // !
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            L14200:
            // !WALKIN?
            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                return ret_val;
            }

            // !BOOM
            MessageHandler.rspsub_(295, (int)j, game);

            // !
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            // R15--	NO-OBJS.  SEE IF EMPTY HANDED, SCORE LIGHT SHAFT.

            NOOBJS:
            // !ASSUME TRUE.
            game.Flags.empthf = true;

            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !SEE IF CARRYING.
                if (game.Objects[i].Adventurer == game.Player.Winner)
                {
                    game.Flags.empthf = false;
                }
                // L15100:
            }

            if (game.Player.Here != RoomIds.BottomOfShaft || !RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                return ret_val;
            }

            AdventurerHandler.ScoreUpdate(game, game.State.LightShaft);

            // !SCORE LIGHT SHAFT.
            game.State.LightShaft = 0;

            // !NEVER AGAIN.
            return ret_val;
            // RAPPL1, PAGE 6

            // R16--	MACHINE ROOM.  DESCRIBE MACHINE.

            MACHINEROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !ASSUME LID CLOSED.
            i = (ObjectIds)46;

            // !IF OPEN, OPEN.
            if (game.Objects[ObjectIds.Machine].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                i = (ObjectIds)12;
            }

            // !DESCRIBE.
            MessageHandler.rspsub_(45, (int)i, game);

            return ret_val;

            // R17--	BAT ROOM.  UNLESS CARRYING GARLIC, FLY AWAY WITH ME...

            BATROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L17500;
            }

            // !DESCRIBE ROOM.
            MessageHandler.Speak(48, game);
            if (game.Objects[ObjectIds.Garlic].Adventurer == game.Player.Winner)
            {
                MessageHandler.Speak(49, game);
            }

            // !BAT HOLDS NOSE.
            return ret_val;

            L17500:
            if (game.ParserVectors.prsa != VerbId.WalkIn ||
                game.Objects[ObjectIds.Garlic].Adventurer == game.Player.Winner)
            {
                return ret_val;
            }

            // !TIME TO FLY, JACK.
            MessageHandler.Speak(50, game);

            // !SELECT RANDOM DEST.
            f = AdventurerHandler.MoveToNewRoom(game, (RoomIds)bats.batdrp[game.rnd_(9)], game.Player.Winner);

            // !INDICATE NEW DESC NEEDED.
            ret_val = false;
            return ret_val;

            // R18--	DOME ROOM.  STATE DEPENDS ON WHETHER ROPE TIED TO RAILING.

            DOMEROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L18500;
            }

            // !DESCRIBE.
            MessageHandler.Speak(51, game);

            // !IF ROPE, DESCRIBE.
            if (game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                MessageHandler.Speak(52, game);
            }

            return ret_val;

            L18500:
            // !DID HE JUMP???
            if (game.ParserVectors.prsa == VerbId.Leap)
            {
                AdventurerHandler.jigsup_(game, 53);
            }

            return ret_val;

            // R19--	TORCH ROOM.  ALSO DEPENDS ON WHETHER ROPE TIED TO RAILING.

            TORCHROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !DESCRIBE.
            MessageHandler.Speak(54, game);

            // !IF ROPE, DESCRIBE.
            if (game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                MessageHandler.Speak(55, game);
            }

            return ret_val;

            // R20--	CAROUSEL ROOM.  SPIN HIM OR KILL HIM.

            CAROUSELROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L20500;
            }

            // !DESCRIBE.
            MessageHandler.Speak(56, game);

            // !IF NOT FLIPPED, SPIN.
            if (!game.Flags.IsCarouselOff)
            {
                MessageHandler.Speak(57, game);
            }

            return ret_val;

            L20500:
            if (game.ParserVectors.prsa == VerbId.WalkIn && game.Flags.carozf)
            {
                AdventurerHandler.jigsup_(game, 58);
            }

            // !WALKED IN.
            return ret_val;
            // RAPPL1, PAGE 7

            // R21--	LLD ROOM.  HANDLE EXORCISE, DESCRIPTIONS.

            LLDROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L21500;
            }

            // !DESCRIBE.
            MessageHandler.Speak(59, game);
            // !IF NOT VANISHED, GHOSTS.
            if (!game.Flags.lldf)
            {
                MessageHandler.Speak(60, game);
            }

            return ret_val;

            L21500:
            // !EXORCISE?
            if (game.ParserVectors.prsa != VerbId.Exorcise)
            {
                return ret_val;
            }

            if (game.Objects[ObjectIds.Bell].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Book].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Candle].Adventurer == game.Player.Winner &&
                game.Objects[ObjectIds.Candle].Flag1.HasFlag(ObjectFlags.IsOn))
            {
                goto L21600;
            }

            MessageHandler.Speak(62, game);
            // !NOT EQUIPPED.
            return ret_val;

            L21600:
            // !GHOST HERE?
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Ghost, game.Player.Here, game))
            {
                goto L21700;
            }

            // !NOPE, EXORCISE YOU.
            AdventurerHandler.jigsup_(game, 61);

            return ret_val;

            L21700:
            // !VANISH GHOST.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Ghost, 63, 0, 0, 0, game);
            // !OPEN GATE.
            game.Flags.lldf = true;
            return ret_val;

            // R22--	LLD2-ROOM.  IS HIS HEAD ON A POLE?

            LLD2ROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            MessageHandler.Speak(64, game);
            // !DESCRIBE.
            if (game.Flags.IsHeadOnPole)
            {
                MessageHandler.Speak(65, game);
            }

            // !ON POLE?
            return ret_val;

            // R23--	DAM ROOM.  DESCRIBE RESERVOIR, PANEL.

            DAMROOM:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !DESCRIBE.
            MessageHandler.Speak(66, game);
            i = (ObjectIds)67;
            if (game.Flags.IsLowTide)
            {
                i = (ObjectIds)68;
            }

            // !DESCRIBE RESERVOIR.
            MessageHandler.Speak(i, game);
            // !DESCRIBE PANEL.
            MessageHandler.Speak(69, game);

            // !BUBBLE IS GLOWING.
            if (game.Flags.gatef)
            {
                MessageHandler.Speak(70, game);
            }

            return ret_val;

            // R24--	TREE ROOM

            TREEROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            MessageHandler.Speak(660, game);
            // !DESCRIBE.
            i = (ObjectIds)661;
            // !SET FLAG FOR BELOW.
            for (j = (ObjectIds)1; j <= (ObjectIds)game.Objects.Count; ++j)
            {
                // !DESCRIBE OBJ IN FORE3.
                if (!ObjectHandler.IsObjectInRoom(j, RoomIds.Forest3, game) || j == ObjectIds.ftree)
                {
                    goto L24200;
                }

                MessageHandler.Speak(i, game);
                // !SET STAGE,
                i = 0;
                MessageHandler.rspsub_(502, game.Objects[j].Description2Id, game);
                // !DESCRIBE.
                L24200:
                ;
            }

            return ret_val;
            // RAPPL1, PAGE 8

            // R25--	CYCLOPS-ROOM.  DEPENDS ON CYCLOPS STATE, ASLEEP FLAG, MAGIC FLAG.

            CYCLOPSROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            MessageHandler.Speak(606, game);
            // !DESCRIBE.
            i = (ObjectIds)607;

            // !ASSUME BASIC STATE.
            if (game.Switches.rvcyc > 0)
            {
                i = (ObjectIds)608;
            }

            // !>0?  HUNGRY.
            if (game.Switches.rvcyc < 0)
            {
                i = (ObjectIds)609;
            }

            // !<0?  THIRSTY.
            if (game.Flags.cyclof)
            {
                i = (ObjectIds)610;
            }

            // !ASLEEP?
            if (game.Flags.IsDoorToCyclopsRoomOpen)
            {
                i = (ObjectIds)611;
            }

            // !GONE?
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            if (!game.Flags.cyclof && game.Switches.rvcyc != 0)
            {
                i__1 = Math.Abs(game.Switches.rvcyc) + 193;
                MessageHandler.Speak(i__1, game);
            }

            return ret_val;

            // R26--	BANK BOX ROOM.

            BANKBOX:
            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                return ret_val;
            }
            // !SURPRISE HIM.
            for (i = (ObjectIds)1; i <= (ObjectIds)8; i += 2)
            {
                // !SCOLRM DEPENDS ON
                if (game.Screen.FromDirection == (int)game.Screen.scoldr[(int)i - 1])
                {
                    game.Screen.scolrm = game.Screen.scoldr[(int)i];
                }
                // L26100:
            }

            // !ENTRY DIRECTION.
            return ret_val;

            // R27--	TREASURE ROOM.

            TREASURE:
            if (game.ParserVectors.prsa != VerbId.WalkIn || !game.Hack.IsThiefActive)
            {
                return ret_val;
            }

            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.Thief, game).Id != game.Player.Here)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 82, game.Player.Here, 0, 0, game);
            }

            game.Hack.ThiefPosition = game.Player.Here;
            // !RESET SEARCH PATTERN.
            game.Objects[ObjectIds.Thief].Flag2 |= ObjectFlags2.IsFighting;
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.chali, game).Id == game.Player.Here)
            {
                game.Objects[ObjectIds.chali].Flag1 &= ~ObjectFlags.IsTakeable;
            }

            // 	VANISH EVERYTHING IN ROOM

            j = 0;
            // !ASSUME NOTHING TO VANISH.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                if (i == ObjectIds.chali || i == ObjectIds.Thief ||
                    !ObjectHandler.IsObjectInRoom(i, game.Player.Here, game))
                {
                    goto L27200;
                }

                j = (ObjectIds)83;
                // !FLAG BYEBYE.
                game.Objects[i].Flag1 &= ~ObjectFlags.IsVisible;
                L27200:
                ;
            }

            // !DESCRIBE.
            MessageHandler.Speak(j, game);
            return ret_val;

            // R28--	CLIFF FUNCTION.  SEE IF CARRYING INFLATED BOAT.

            CLIFF:
            game.Flags.deflaf = game.Objects[ObjectIds.rboat].Adventurer != game.Player.Winner;
            // !TRUE IF NOT CARRYING.
            return ret_val;
            // RAPPL1, PAGE 9

            // R29--	RIVR4 ROOM.  PLAY WITH BUOY.

            RIVER4:
            if (!game.Flags.buoyf || game.Objects[ObjectIds.Buoy].Adventurer != game.Player.Winner)
            {
                return ret_val;
            }

            MessageHandler.Speak(84, game);
            // !GIVE HINT,
            game.Flags.buoyf = false;
            // !THEN DISABLE.
            return ret_val;

            // R30--	OVERFALLS.  DOOM.

            OVERFALLS:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                AdventurerHandler.jigsup_(game, 85);
            }
            // !OVER YOU GO.
            return ret_val;

            // R31--	BEACH ROOM.  DIG A HOLE.

            BEACHROOM:
            if (game.ParserVectors.prsa != VerbId.Dig || game.ParserVectors.DirectObject != ObjectIds.Shovel)
            {
                return ret_val;
            }

            // !INCREMENT DIG STATE.
            ++game.Switches.rvsnd;
            switch (game.Switches.rvsnd)
            {
                case 1: goto L31100;
                case 2: goto L31100;
                case 3: goto L31100;
                case 4: goto L31400;
                case 5: goto L31500;
            }
            // !PROCESS STATE.
            //bug_(2, game.Switch.rvsnd);
            throw new InvalidOperationException("2");

            L31100:
            i__1 = game.Switches.rvsnd + 85;
            MessageHandler.Speak(i__1, game);
            // !1-3... DISCOURAGE HIM.
            return ret_val;

            L31400:
            i = (ObjectIds)89;
            // !ASSUME DISCOVERY.
            if (game.Objects[ObjectIds.statu].Flag1.HasFlag(ObjectFlags.IsVisible))
            {
                i = (ObjectIds)88;
            }

            MessageHandler.Speak(i, game);
            game.Objects[ObjectIds.statu].Flag1 |= ObjectFlags.IsVisible;
            return ret_val;

            L31500:
            game.Switches.rvsnd = 0;
            // !5... SAND COLLAPSES
            AdventurerHandler.jigsup_(game, 90);
            // !AND SO DOES HE.
            return ret_val;

            // R32--	TCAVE ROOM.  DIG A HOLE IN GUANO.

            TCAVE:
            if (game.ParserVectors.prsa != VerbId.Dig || game.ParserVectors.DirectObject != ObjectIds.Shovel)
            {
                return ret_val;
            }

            i = (ObjectIds)91;
            // !ASSUME NO GUANO.
            if (!ObjectHandler.IsObjectInRoom(ObjectIds.guano, game.Player.Here, game))
            {
                goto L32100;
            }

            // !IS IT HERE?
            // Computing MIN
            i__1 = 4;
            i__2 = game.Switches.rvgua + 1;

            game.Switches.rvgua = Math.Min(i__1, i__2);
            // !YES, SET NEW STATE.
            i = (ObjectIds)game.Switches.rvgua + 91;
            // !GET NASTY REMARK.
            L32100:
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;

            // R33--	FALLS ROOM

            FALLSROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(96, game);
            // !DESCRIBE.
            i = (ObjectIds)97;
            // !ASSUME NO RAINBOW.
            if (game.Flags.IsRainbowOn)
            {
                i = (ObjectIds)98;
            }

            // !GOT ONE?
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;
            // RAPPL1, PAGE 10

            // R34--	LEDGE FUNCTION.  LEDGE CAN COLLAPSE.

            L34000:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !DESCRIBE.
            MessageHandler.Speak(100, game);
            // !ASSUME SAFE ROOM OK.
            i = (ObjectIds)102;

            if ((game.Rooms[RoomIds.Safe].Flags & RoomFlags.RMUNG) != 0)
            {
                i = (ObjectIds)101;
            }

            // !DESCRIBE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // R35--	SAFE ROOM.  STATE DEPENDS ON WHETHER SAFE BLOWN.

            SAFEROOM:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !DESCRIBE.
            MessageHandler.Speak(104, game);
            // !ASSUME OK.
            i = (ObjectIds)105;
            // !BLOWN?
            if (game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)106;
            }

            // !DESCRIBE.
            MessageHandler.Speak(i, game);
            return ret_val;

            // R36--	MAGNET ROOM.  DESCRIBE, CHECK FOR SPINDIZZY DOOM.

            MAGNETROOM:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L36500;
            }

            // !LOOK?
            MessageHandler.Speak(107, game);
            // !DESCRIBE.
            return ret_val;

            L36500:
            if (game.ParserVectors.prsa != VerbId.WalkIn || !game.Flags.IsCarouselOff)
            {
                return ret_val;
            }

            // !WALKIN ON FLIPPED?
            if (game.Flags.carozf)
            {
                goto L36600;
            }

            // !ZOOM?
            MessageHandler.Speak(108, game);
            // !NO, SPIN HIS COMPASS.
            return ret_val;

            L36600:
            // !SPIN HIS INSIDES.
            i = (ObjectIds)58;
            // !SPIN ROBOT.
            if (game.Player.Winner != ActorIds.Player)
            {
                i = (ObjectIds)99;
            }

            // !DEAD.
            AdventurerHandler.jigsup_(game, (int)i);

            return ret_val;

            // R37--	CAGE ROOM.  IF SOLVED CAGE, MOVE TO OTHER CAGE ROOM.

            CAGEROOM:
            if (game.Flags.cagesf)
            {
                f = AdventurerHandler.MoveToNewRoom(game, RoomIds.cager, game.Player.Winner);
            }

            // !IF SOLVED, MOVE.
            return ret_val;
        }

        public static bool RunRoomAction2(Room room, Game game)
        {
            const int newrms = 38;

            //  System generated locals
            int i__1;
            bool ret_val;

            int i;
            ObjectIds j;

            ret_val = true;
            switch (room.Action - newrms + 1)
            {
                case 1: goto L38000;
                case 2: goto L39000;
                case 3: goto L40000;
                case 4: goto L41000;
                case 5: goto L42000;
                case 6: goto L43000;
                case 7: goto L44000;
                case 8: goto L45000;
                case 9: goto L46000;
                case 10: goto L47000;
                case 11: goto L48000;
                case 12: goto L49000;
                case 13: goto L50000;
                case 14: goto L51000;
                case 15: goto L52000;
                case 16: goto L53000;
                case 17: goto L54000;
                case 18: goto L55000;
                case 19: goto L56000;
                case 20: goto Nirvana;
                case 21: goto L58000;
                case 22: goto L59000;
                case 23: goto L60000;
            }

            //bug_(70, ri);
            throw new InvalidOperationException("70");

            return ret_val;

            //  R38--	MIRROR D ROOM

            L38000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                lookto_(game, (int)RoomIds.FrontDoor, (int)RoomIds.mrg, 0, 682, 681);
            }
            return ret_val;

            //  R39--	MIRROR G ROOM

            L39000:
            if (game.ParserVectors.prsa == VerbId.WalkIn)
            {
                AdventurerHandler.jigsup_(game, 685);
            }
            return ret_val;

            //  R40--	MIRROR C ROOM

            L40000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                lookto_(game, (int)RoomIds.mrg, (int)RoomIds.mrb, 683, 0, 681);
            }
            return ret_val;

            //  R41--	MIRROR B ROOM

            L41000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                lookto_(game, (int)RoomIds.mrc, (int)RoomIds.mra, 0, 0, 681);
            }
            return ret_val;

            //  R42--	MIRROR A ROOM

            L42000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                lookto_(game, (int)RoomIds.mrb, 0, 0, 684, 681);
            }
            return ret_val;
            //  RAPPL2, PAGE 3

            //  R43--	MIRROR C EAST/WEST

            L43000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 683);
            }
            return ret_val;

            //  R44--	MIRROR B EAST/WEST

            L44000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 686);
            }
            return ret_val;

            //  R45--	MIRROR A EAST/WEST

            L45000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 687);
            }
            return ret_val;

            //  R46--	INSIDE MIRROR

            L46000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(688, game);
            // !DESCRIBE

            //  NOW DESCRIBE POLE STATE.

            //  CASES 1,2--	MDIR=270 & MLOC=MRB, POLE IS UP OR IN HOLE
            //  CASES 3,4--	MDIR=0 V MDIR=180, POLE IS UP OR IN CHANNEL
            //  CASE 5--	POLE IS UP

            i = 689;
            // !ASSUME CASE 5.
            if (game.Switches.mdir == 270 && game.Switches.mloc == RoomIds.mrb)
            {
                i = Math.Min(game.Switches.poleuf, 1) + 690;
            }

            if (game.Switches.mdir % 180 == 0)
            {
                i = Math.Min(game.Switches.poleuf, 1) + 692;
            }

            MessageHandler.Speak(i, game);
            // !DESCRIBE POLE.
            i__1 = game.Switches.mdir / 45 + 695;
            MessageHandler.rspsub_(694, i__1, game);
            // !DESCRIBE ARROW.
            return ret_val;
            //  RAPPL2, PAGE 4

            //  R47--	MIRROR EYE ROOM

            L47000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 704;
            // !ASSUME BEAM STOP.
            for (j = (ObjectIds)1; j <= (ObjectIds)game.Objects.Count; ++j)
            {
                if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game) && j != ObjectIds.rbeam)
                {
                    goto L47200;
                }
                //  L47100:
            }

            i = 703;
            L47200:
            MessageHandler.rspsub_(i, game.Objects[j].Description2Id, game);
            // !DESCRIBE BEAM.
            lookto_(game, (int)RoomIds.mra, 0, 0, 0, 0);
            // !LOOK NORTH.
            return ret_val;

            //  R48--	INSIDE CRYPT

            L48000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 46;
            // !CRYPT IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.Tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }

            MessageHandler.rspsub_(705, i, game);
            return ret_val;

            //  R49--	SOUTH CORRIDOR

            L49000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            MessageHandler.Speak(706, game);
            // !DESCRIBE.
            i = 46;
            // !ODOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.odoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }

            if (game.Switches.LeftCell == 4)
            {
                MessageHandler.rspsub_(707, i, game);
            }

            // !DESCRIBE ODOOR IF THERE.
            return ret_val;

            //  R50--	BEHIND DOOR

            L50000:
            // !WALK IN?
            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                goto L50100;
            }

            // !MASTER FOLLOWS.
            game.Clock[ClockId.cevfol].Flags = true;
            game.Clock[ClockId.cevfol].Ticks = -1;
            return ret_val;

            L50100:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            i = 46;
            // !QDOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.qdoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }

            MessageHandler.rspsub_(708, i, game);
            return ret_val;
            //  RAPPL2, PAGE 5

            //  R51--	FRONT DOOR

            L51000:
            if (game.ParserVectors.prsa == VerbId.WalkIn)
            {
                game.Clock[ClockId.cevfol].Ticks = 0;
            }

            // !IF EXITS, KILL FOLLOW.
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }

            // !LOOK?
            lookto_(game, 0, (int)RoomIds.mrd, 709, 0, 0);
            // !DESCRIBE SOUTH.
            i = 46;
            // !PANEL IS OPEN/CLOSED.
            if (game.Flags.inqstf)
            {
                i = 12;
            }
            // !OPEN IF INQ STARTED.
            j = (ObjectIds)46;
            // !QDOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.qdoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                j = (ObjectIds)12;
            }

            MessageHandler.rspsb2_(710, i, (int)j, game);

            return ret_val;

            //  R52--	NORTH CORRIDOR

            L52000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 46;
            if ((game.Objects[ObjectIds.CellDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }
            // !CDOOR IS OPEN/CLOSED.
            MessageHandler.rspsub_(711, i, game);
            return ret_val;

            //  R53--	PARAPET

            L53000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                i__1 = game.Switches.pnumb + 712;
                MessageHandler.rspsub_(712, i__1, game);
            }
            return ret_val;

            //  R54--	CELL

            L54000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 721;
            // !CDOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.CellDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 722;
            }

            MessageHandler.Speak(i, game);
            i = 46;
            // !ODOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.odoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }
            if (game.Switches.LeftCell == 4)
            {
                MessageHandler.rspsub_(723, i, game);
            }
            // !DESCRIBE.
            return ret_val;

            //  R55--	PRISON CELL

            L55000:
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                MessageHandler.Speak(724, game);
            }
            // !LOOK?
            return ret_val;

            //  R56--	NIRVANA CELL

            L56000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 46;
            // !ODOOR IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.odoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }
            MessageHandler.rspsub_(725, i, game);
            return ret_val;
            //  RAPPL2, PAGE 6

            //  R57--	NIRVANA AND END OF GAME

            Nirvana:
            // !WALKIN?
            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                return ret_val;
            }

            MessageHandler.Speak(726, game);
            //  moved to exit routine	CLOSE(DBCH)
            AdventurerHandler.PrintScore(game, false);
            //exit_();
            throw new ApplicationException("Exit");

            //  R58--	TOMB ROOM

            L58000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 46;
            // !TOMB IS OPEN/CLOSED.
            if ((game.Objects[ObjectIds.Tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = 12;
            }
            MessageHandler.rspsub_(792, i, game);
            return ret_val;

            //  R59--	PUZZLE SIDE ROOM

            L59000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 861;
            // !ASSUME DOOR CLOSED.
            if (game.Flags.cpoutf)
            {
                i = 862;
            }
            // !OPEN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;

            //  R60--	PUZZLE ROOM

            L60000:
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return ret_val;
            }
            // !LOOK?
            if (game.Flags.cpushf)
            {
                goto L60100;
            }

            // !STARTED PUZZLE?
            MessageHandler.Speak(868, game);
            // !NO, DESCRIBE.
            if ((game.Objects[ObjectIds.warni].Flag2 & ObjectFlags2.WasTouched) != 0)
            {
                MessageHandler.Speak(869, game);
            }

            return ret_val;

            L60100:
            dso7.cpinfo_(game, 880, game.Switches.cphere);
            // !DESCRIBE ROOM.
            return ret_val;
        }

        public static bool prob_(Game game, int g, int b)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            int i;

            i = g;
            // !ASSUME GOOD LUCK.
            if (game.Flags.HasBadLuck)
            {
                i = b;
            }
            // !IF BAD, TOO BAD.
            ret_val = game.rnd_(100) < i;
            // !COMPUTE.
            return ret_val;
        }

        /// <summary>
        /// opncls_ - Process open/close for doors
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="openStringId"></param>
        /// <param name="closeStringId"></param>
        /// <returns></returns>
        public static bool OpenCloseDoor(ObjectIds obj, int openStringId, int closeStringId, Game game)
        {
            int i__1;
            // !ASSUME WINS.
            bool ret_val = true;

            // !CLOSE?
            if (game.ParserVectors.prsa == VerbId.Close)
            {
                goto L100;
            }

            // !OPEN?
            if (game.ParserVectors.prsa == VerbId.Open)
            {
                goto L50;
            }

            // !LOSE
            ret_val = false;
            return ret_val;

            L50:
            if (game.Objects[obj].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                goto L200;
            }

            // !OPEN... IS IT?
            MessageHandler.Speak(openStringId, "", game);
            game.Objects[obj].Flag2 |= ObjectFlags2.IsOpen;
            return ret_val;

            L100:
            if (!((game.Objects[obj].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto L200;
            }

            // !CLOSE... IS IT?
            MessageHandler.Speak(closeStringId, "", game);
            game.Objects[obj].Flag2 &= ~ObjectFlags2.IsOpen;
            return ret_val;

            L200:
            i__1 = game.rnd_(3) + 125;
            MessageHandler.Speak(i__1, game);
            // !DUMMY.
            return ret_val;
        }

        /// <summary>
        /// ewtell_ - Describe E/W Narrow Rooms
        /// </summary>
        /// <param name="game"></param>
        /// <param name="rm"></param>
        /// <param name="st"></param>
        public static void DescribeEastWestNarrowRooms(Game game, RoomIds rm, int st)
        {
            int i__1;

            // Local variables
            int i;
            bool m1;

            // NOTE THAT WE ARE EAST OR WEST OF MIRROR, AND
            // MIRROR MUST BE N-S.

            m1 = game.Switches.mdir + (rm - RoomIds.mrae) % 2 * 180 == 180;
            i = (rm - RoomIds.mrae) % 2 + 819;
            // !GET BASIC E/W STRING.
            if (m1 && !game.Flags.mr1f || !m1 && !game.Flags.mr2f)
            {
                i += 2;
            }

            MessageHandler.Speak(i, game);
            if (m1 && game.Flags.mropnf)
            {
                i__1 = (i - 819) / 2 + 823;
                MessageHandler.Speak(i__1, game);
            }

            MessageHandler.Speak(825, game);
            MessageHandler.Speak(st, game);
        }

        /// <summary>
        /// lookto_ - Describe view in mirror hallway.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="nrm"></param>
        /// <param name="srm"></param>
        /// <param name="nt"></param>
        /// <param name="st"></param>
        /// <param name="ht"></param>
        public static void lookto_(Game game, int nrm, int srm, int nt, int st, int ht)
        {
            int i__1;
            int i, m1, dir, mrbf;

            MessageHandler.Speak(ht, game);
            // !DESCRIBE HALL.
            MessageHandler.Speak(nt, game);
            // !DESCRIBE NORTH VIEW.
            MessageHandler.Speak(st, game);
            // !DESCRIBE SOUTH VIEW.
            dir = 0;
            // !ASSUME NO DIRECTION.
            i__1 = game.Switches.mloc - game.Player.Here;
            if (Math.Abs(i__1) != 1)
            {
                goto L200;
            }

            // !MIRROR TO N OR S?
            // !DIR=N/S.
            if (game.Switches.mloc == (RoomIds)nrm)
            {
                dir = 695;
            }

            if (game.Switches.mloc == (RoomIds)srm)
            {
                dir = 699;
            }

            // !MIRROR N-S?
            if (game.Switches.mdir % 180 != 0)
            {
                goto L100;
            }

            // !YES, HE SEES PANEL
            MessageHandler.rspsub_(game, 847, dir);
            // !AND NARROW ROOMS.
            MessageHandler.rspsb2_(848, dir, dir, game);

            goto L200;

            L100:
            // !WHICH MIRROR?
            m1 = IsMirrorHere(game, game.Player.Here);
            // !ASSUME INTACT.
            mrbf = 0;

            if (m1 == 1 && !game.Flags.mr1f || m1 == 2 && !game.Flags.mr2f)
            {
                mrbf = 1;
            }

            i__1 = mrbf + 849;
            MessageHandler.rspsub_(game, i__1, dir);
            // !DESCRIBE.
            if (m1 == 1 && game.Flags.mropnf)
            {
                i__1 = mrbf + 823;
                MessageHandler.Speak(i__1, game);
            }

            if (mrbf != 0)
            {
                MessageHandler.Speak(851, game);
            }

            L200:
            i = 0;

            // !ASSUME NO MORE TO DO.
            if (nt == 0 && (dir == 0 || dir == 699))
            {
                i = 852;
            }

            if (st == 0 && (dir == 0 || dir == 695))
            {
                i = 853;
            }

            if (nt + st + dir == 0)
            {
                i = 854;
            }

            if (ht != 0)
            {
                MessageHandler.Speak(i, game);
            }
            // !DESCRIBE HALLS.
        }

        /// <summary>
        /// mrhere_ - Is Mirror here?
        /// </summary>
        /// <param name="game"></param>
        /// <param name="rm"></param>
        /// <returns></returns>
        public static int IsMirrorHere(Game game, RoomIds rm)
        {
            // System generated locals
            int ret_val, i__1;

            // RM IS AN E-W ROOM, MIRROR MUST BE N-S (MDIR= 0 OR 180)
            if (rm < RoomIds.mrae || rm > RoomIds.mrdw)
            {
                goto L100;
            }

            ret_val = 1;
            // !ASSUME MIRROR 1 HERE.
            if ((rm - RoomIds.mrae) % 2 == game.Switches.mdir / 180)
            {
                ret_val = 2;
            }

            return ret_val;

            // RM IS NORTH OR SOUTH OF MIRROR.  IF MIRROR IS N-S OR NOT
            // WITHIN ONE ROOM OF RM, LOSE.

            L100:
            ret_val = 0;
            i__1 = game.Switches.mloc - rm;
            if (Math.Abs(i__1) != 1 || game.Switches.mdir % 180 == 0)
            {
                return ret_val;
            }

            // RM IS WITHIN ONE OF MLOC, AND MDIR IS E-W

            ret_val = 1;
            if (rm < game.Switches.mloc && game.Switches.mdir < 180 || rm > game.Switches.mloc && game.Switches.mdir > 180)
            {
                ret_val = 2;
            }

            return ret_val;
        }
    }
}