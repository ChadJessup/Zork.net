using System;
using System.Linq;

namespace Zork.Core
{
    public static class RoomHandler
    {
        public static bool RoomDescription(Game game, int verbosity) => RoomDescription(verbosity, game);
        public static bool RoomDescription(int verbosity, Game game)
        {
            int full = verbosity;
            bool ret_val, L__1;
            int i, ra;

            // FULL= 0/1/2/3= SHORT/OBJ/ROOM/FULL

            ret_val = true;

            // !ASSUME WINS.
            if (game.ParserVectors.DirectObject < (ObjectIds)XSearch.xmin)
            {
                goto L50;
            }

            // !IF DIRECTION,
            game.Screen.fromdr = (int)game.ParserVectors.DirectObject;

            // !SAVE AND
            game.ParserVectors.DirectObject = 0;

            // !CLEAR.
            L50:
            if (game.Player.Here == game.Adventurers[ActorIds.Player].CurrentRoom.Id)
            {
                goto L100;
            }

            // !PLAYER JUST MOVE?
            MessageHandler.Speak(2, game);

            // !NO, JUST SAY DONE.
            game.ParserVectors.prsa = VerbIds.WalkIn;

            // !SET UP WALK IN ACTION.
            return ret_val;

            L100:
            if (RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L300;
            }

            // !LIT?
            MessageHandler.Speak(430, game);

            // !WARN OF GRUE.
            ret_val = false;
            return ret_val;

            L300:
            ra = game.Rooms[game.Player.Here].Action;

            // !GET ROOM ACTION.
            if (full == 1)
            {
                goto L600;
            }

            // !OBJ ONLY?
            i = game.Rooms[game.Player.Here].Description2;

            // !ASSUME SHORT DESC.
            if (full == 0 && (game.Flags.SuperBriefDescriptions || (game.Rooms[game.Player.Here].Flags.HasFlag(RoomFlags.SEEN) && game.Flags.BriefDescriptions)))
            {
                goto L400;
            }

            //  The next line means that when you request VERBOSE mode, you
            //  only get long room descriptions 20% of the time. I don't either
            //  like or understand this, so the mod. ensures VERBOSE works
            //  all the time.			jmh@ukc.ac.uk 22/10/87

            // & .AND.(BRIEFF.OR.PROB(80,80)))))       GO TO 400
            i = game.Rooms[game.Player.Here].Description1;

            // !USE LONG.
            if (i != 0 || ra == 0)
            {
                goto L400;
            }

            // !IF GOT DESC, SKIP.
            game.ParserVectors.prsa = VerbIds.Look;

            // !PRETEND LOOK AROUND.
            if (!RunRoomAction(ra, game))
            {
                goto L100;
            }

            // !ROOM HANDLES, NEW DESC?
            game.ParserVectors.prsa = VerbIds.foow;

            // !NOP PARSER.
            goto L500;

            L400:
            MessageHandler.Speak(i, game);

            // !OUTPUT DESCRIPTION.
            L500:
            if (game.Adventurers[game.Player.Winner].VehicleId != 0)
            {
                MessageHandler.rspsub_(431, game.Objects[(ObjectIds)game.Adventurers[game.Player.Winner].VehicleId].Description2, game);
            }

            L600:
            if (full != 2)
            {
                L__1 = full != 0;
                RoomHandler.PrintRoomContents(L__1, game.Player.Here, game);
            }

            game.Rooms[game.Player.Here].Flags |= RoomFlags.SEEN;
            if (full != 0 || ra == 0)
            {
                return ret_val;
            }

            // !ANYTHING MORE?
            game.ParserVectors.prsa = VerbIds.WalkIn;

            // !GIVE HIM A SURPISE.
            if (!RunRoomAction(ra, game))
            {
                goto L100;
            }

            // !ROOM HANDLES, NEW DESC?
            game.ParserVectors.prsa = VerbIds.foow;

            return ret_val;
        }

        /// <summary>
        /// princr_ - PRINT CONTENTS OF ROOM
        /// </summary>
        /// <param name="full"></param>
        /// <param name="roomId"></param>
        /// <param name="game"></param>
        public static void PrintRoomContents(bool full, RoomIds roomId, Game game)
        {
            int j, k;

            j = 329;

            var room = game.Rooms[roomId];

            foreach (var obj in room.Objects)
            {
                MessageHandler.Speak(obj.Description1, game);
            }

            foreach (var obj in room.Objects.Where(o => o.IsContainer && o.CanSeeInside))
            {
                foreach (var containedObject in obj.ContainedObjects)
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

                k = game.Objects[i].odesco;
                // !GET UNTOUCHED
                if (k == 0 || game.Objects[i].Flag2.HasFlag(ObjectFlags2.WasTouched))
                {
                    k = game.Objects[i].Description1;
                }

                // !DESCRIBE
                MessageHandler.Speak(k, game);

                // DO SHORT DESCRIPTION OF OBJECT
                goto L500;

                L200:
                // !YOU CAN SEE IT
                MessageHandler.rspsub_(j, game.Objects[i].Description2, game);
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

                if ((game.Objects[i].Flag2 & ObjectFlags2.ACTRBT) != 0)
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
        /// <param name="ri"></param>
        /// <returns></returns>
        public static bool RunRoomAction(int roomActionId, Game game)
        {
            const int newrms = 38;

            // System generated locals
            bool ret_val = true;

            // !ASSUME WINS.
            // !IF ZERO, WIN.
            if (roomActionId == 0)
            {
                return ret_val;
            }

            // !IF OLD, PROCESSOR 1.
            if (roomActionId < newrms)
            {
                ret_val = RoomHandler.RunRoomAction1(roomActionId, game);
            }

            // !IF NEW, PROCESSOR 2.
            if (roomActionId >= newrms)
            {
                ret_val = RoomHandler.RunRoomAction2(roomActionId, game);
            }

            return ret_val;
        }

        public static bool RunRoomAction1(int roomActionId, Game game)
        {
            int i__1, i__2;
            bool ret_val;

            // Local variables
            bool f;
            ObjectIds i;
            ObjectIds j;

            ret_val = true;
            // !USUALLY IGNORED.
            if (roomActionId == 0)
            {
                return ret_val;
            }

            // !RETURN IF NAUGHT.

            // !SET TO FALSE FOR

            // !NEW DESC NEEDED.
            switch (roomActionId)
            {
                case 1: goto EASTOFHOUSE;
                case 2: goto KITCHEN;
                case 3: goto LIVINGROOM;
                case 4: goto CELLAR;
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
                case 19: goto L19000;
                case 20: goto L20000;
                case 21: goto L21000;
                case 22: goto L22000;
                case 23: goto L23000;
                case 24: goto L24000;
                case 25: goto L25000;
                case 26: goto L26000;
                case 27: goto L27000;
                case 28: goto L28000;
                case 29: goto L29000;
                case 30: goto L30000;
                case 31: goto L31000;
                case 32: goto L32000;
                case 33: goto L33000;
                case 34: goto L34000;
                case 35: goto L35000;
                case 36: goto MAGNETROOM;
                case 37: goto CAGEROOM;
            }

            throw new InvalidOperationException("1");
            //bug_(1, ri);

            // R1--	EAST OF HOUSE.  DESCRIPTION DEPENDS ON STATE OF WINDOW

            EASTOFHOUSE:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }

            // !LOOK?
            i = (ObjectIds)13;
            // !ASSUME CLOSED.
            if ((game.Objects[ObjectIds.Window].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)12;
            }

            // !IF OPEN, AJAR.
            MessageHandler.rspsub_(11, (int)i, game);
            // !DESCRIBE.
            return ret_val;

            // R2--	KITCHEN.  SAME VIEW FROM INSIDE.

            KITCHEN:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }

            // !LOOK?
            i = (ObjectIds)13;
            // !ASSUME CLOSED.
            if ((game.Objects[ObjectIds.Window].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)12;
            }

            // !IF OPEN, AJAR.
            MessageHandler.rspsub_(14, (int)i, game);
            // !DESCRIBE.
            return ret_val;

            // R3--	LIVING ROOM.  DESCRIPTION DEPENDS ON MAGICF (STATE OF
            // 	DOOR TO CYCLOPS ROOM), RUG (MOVED OR NOT), DOOR (OPEN OR CLOSED)

            LIVINGROOM:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L3500;
            }

            // !LOOK?
            i = (ObjectIds)15;
            // !ASSUME NO HOLE.
            if (game.Flags.IsDoorToCyclopsRoomOpen)
            {
                i = (ObjectIds)16;
            }

            // !IF MAGICF, CYCLOPS HOLE.
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            i = (ObjectIds)(game.Switches.IsRugMoved + 17);
            // !ASSUME INITIAL STATE.
            if ((game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i += 2;
            }

            // !DOOR OPEN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;

            // 	NOT A LOOK WORD.  REEVALUATE TROPHY CASE.

            L3500:
            if (game.ParserVectors.prsa != VerbIds.Take && (game.ParserVectors.prsa != VerbIds.Put ||
                game.ParserVectors.IndirectObject != ObjectIds.TrophyCase))
            {
                return ret_val;
            }

            game.Adventurers[game.Player.Winner].Score = game.State.RawScore;
            // !SCORE TROPHY CASE.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !RETAIN RAW SCORE AS WELL.
                j = i;
                // !FIND OUT IF IN CASE.
                L3550:
                j = game.Objects[j].Container;
                // !TRACE OWNERSHIP.
                if (j == 0)
                {
                    goto L3600;
                }

                if (j != ObjectIds.TrophyCase)
                {
                    goto L3550;
                }

                // !DO ALL LEVELS.
                game.Adventurers[game.Player.Winner].Score += game.Objects[i].otval;
                L3600:
                ;
            }

            AdventurerHandler.ScoreUpdate(game, 0);
            // !SEE IF ENDGAME TRIG.
            return ret_val;
            // RAPPL1, PAGE 3

            // R4--	CELLAR.  SHUT DOOR AND BAR IT IF HE JUST WALKED IN.

            CELLAR:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L4500;
            }
            // !LOOK?
            MessageHandler.Speak(21, game);
            // !DESCRIBE CELLAR.
            return ret_val;

            L4500:
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
            {
                return ret_val;
            }

            // !WALKIN?
            if ((game.Objects[ObjectIds.TrapDoor].Flag2 & (int)ObjectFlags2.IsOpen + ObjectFlags2.WasTouched) != ObjectFlags2.IsOpen)
            {
                return ret_val;
            }

            game.Objects[ObjectIds.TrapDoor].Flag2 = (game.Objects[ObjectIds.TrapDoor].Flag2 | ObjectFlags2.WasTouched) & ~ObjectFlags2.IsOpen;
            // !SLAM AND BOLT DOOR.
            MessageHandler.Speak(22, game);
            return ret_val;

            // R5--	MAZE11.  DESCRIBE STATE OF GRATING.

            MAZE11:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if ((game.Objects[ObjectIds.Grate].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)25;
            }
            // !OPEN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE GRATE.
            return ret_val;

            // R6--	CLEARING.  DESCRIBE CLEARING, MOVE LEAVES.

            CLEARING:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L6500;
            }
            // !LOOK?
            MessageHandler.Speak(27, game);
            // !DESCRIBE.
            if (game.Switches.rvclr == 0)
            {
                return ret_val;
            }
            // !LEAVES MOVED?
            i = (ObjectIds)28;
            // !YES, ASSUME GRATE CLOSED.
            if ((game.Objects[ObjectIds.Grate].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)29;
            }
            // !OPEN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE GRATE.
            return ret_val;

            L6500:
            if (game.Switches.rvclr != 0
                || ObjectHandler.IsObjectInRoom(ObjectIds.leave, RoomIds.ForestClearing, game)
                && (game.ParserVectors.prsa != VerbIds.Move
                || game.ParserVectors.DirectObject != ObjectIds.leave))
            {
                return ret_val;
            }

            MessageHandler.Speak(30, game);
            // !MOVE LEAVES, REVEAL GRATE.
            game.Switches.rvclr = 1;
            // !INDICATE LEAVES MOVED.
            return ret_val;
            // RAPPL1, PAGE 4

            // R7--	RESERVOIR SOUTH.  DESCRIPTION DEPENDS ON LOW TIDE FLAG.

            RESERVOIRSOUTH:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa == VerbIds.WalkIn)
            {
                game.Clock.Flags[(int)ClockIndices.cevfor - 1] = true;
            }
            // !IF WALK IN, BIRDIE.
            return ret_val;

            // R12--	MIRROR ROOM.  STATE DEPENDS ON MIRROR INTACT.

            MIRRORROOM:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(42, game);
            // !DESCRIBE.
            if (game.Flags.IsMirrorBroken)
            {
                MessageHandler.Speak(43, game);
            }
            // !IF BROKEN, NASTY REMARK.
            return ret_val;
            // RAPPL1, PAGE 5

            // R13--	CAVE2 ROOM.  BLOW OUT CANDLES WITH 50% PROBABILITY.

            CAVE2ROOM:
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
            {
                return ret_val;
            }
            // !WALKIN?
            if (prob_(game, 50, 50)
                || game.Objects[ObjectIds.Candle].Adventurer != game.Player.Winner
                || !((game.Objects[ObjectIds.Candle].Flag1 & ObjectFlags.IsOn) != 0))
            {
                return ret_val;
            }

            game.Objects[ObjectIds.Candle].Flag1 &= ~ObjectFlags.IsOn;
            MessageHandler.Speak(47, game);
            // !TELL OF WINDS.
            game.Clock.Flags[(int)ClockIndices.cevcnd - 1] = false;
            // !HALT CANDLE COUNTDOWN.
            return ret_val;

            // R14--	BOOM ROOM.  BLOW HIM UP IF CARRYING FLAMING OBJECT.

            BOOMROOM:
            j = (ObjectIds)game.Objects[ObjectIds.Candle].Description2;
            // !ASSUME CANDLE.
            if (game.Objects[ObjectIds.Candle].Adventurer == game.Player.Winner
                && (game.Objects[ObjectIds.Candle].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L14100;
            }

            j = (ObjectIds)game.Objects[ObjectIds.Torch].Description2;
            // !ASSUME TORCH.
            if (game.Objects[ObjectIds.Torch].Adventurer == game.Player.Winner
                && (game.Objects[ObjectIds.Torch].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L14100;
            }

            j = (ObjectIds)game.Objects[ObjectIds.Match].Description2;
            if (game.Objects[ObjectIds.Match].Adventurer == game.Player.Winner
                && (game.Objects[ObjectIds.Match].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L14100;
            }

            return ret_val;
            // !SAFE

            L14100:
            if (game.ParserVectors.prsa != VerbIds.TurnOn)
            {
                goto L14200;
            }
            // !TURN ON?
            MessageHandler.rspsub_(294, (int)j, game);
            // !BOOM
            // !
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            L14200:
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
            {
                return ret_val;
            }
            // !WALKIN?
            MessageHandler.rspsub_(295, (int)j, game);
            // !BOOM
            // !
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            // R15--	NO-OBJS.  SEE IF EMPTY HANDED, SCORE LIGHT SHAFT.

            NOOBJS:
            game.Flags.empthf = true;
            // !ASSUME TRUE.
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
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
            AdventurerHandler.ScoreUpdate(game, game.State.ltshft);
            // !SCORE LIGHT SHAFT.
            game.State.ltshft = 0;
            // !NEVER AGAIN.
            return ret_val;
            // RAPPL1, PAGE 6

            // R16--	MACHINE ROOM.  DESCRIBE MACHINE.

            MACHINEROOM:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = (ObjectIds)46;
            // !ASSUME LID CLOSED.
            if ((game.Objects[ObjectIds.Machine].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)12;
            }
            // !IF OPEN, OPEN.
            MessageHandler.rspsub_(45, (int)i, game);
            // !DESCRIBE.
            return ret_val;

            // R17--	BAT ROOM.  UNLESS CARRYING GARLIC, FLY AWAY WITH ME...

            BATROOM:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L17500;
            }
            // !LOOK?
            MessageHandler.Speak(48, game);
            // !DESCRIBE ROOM.
            if (game.Objects[ObjectIds.Garlic].Adventurer == game.Player.Winner)
            {
                MessageHandler.Speak(49, game);
            }
            // !BAT HOLDS NOSE.
            return ret_val;

            L17500:
            if (game.ParserVectors.prsa != VerbIds.WalkIn
                || game.Objects[ObjectIds.Garlic].Adventurer == game.Player.Winner)
            {
                return ret_val;
            }

            // !TIME TO FLY, JACK.
            MessageHandler.Speak(50, game);
            // !SELECT RANDOM DEST.
            f = AdventurerHandler.moveto_(game, (RoomIds)bats.batdrp[game.rnd_(9)], game.Player.Winner);

            // !INDICATE NEW DESC NEEDED.
            ret_val = false;
            return ret_val;

            // R18--	DOME ROOM.  STATE DEPENDS ON WHETHER ROPE TIED TO RAILING.

            DOMEROOM:
            // !LOOK?
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa == VerbIds.Leap)
            {
                AdventurerHandler.jigsup_(game, 53);
            }
            // !DID HE JUMP???
            return ret_val;

            // R19--	TORCH ROOM.  ALSO DEPENDS ON WHETHER ROPE TIED TO RAILING.

            L19000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(54, game);
            // !DESCRIBE.
            if (game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                MessageHandler.Speak(55, game);
            }
            // !IF ROPE, DESCRIBE.
            return ret_val;

            // R20--	CAROUSEL ROOM.  SPIN HIM OR KILL HIM.

            L20000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L20500;
            }
            // !LOOK?
            MessageHandler.Speak(56, game);
            // !DESCRIBE.
            if (!game.Flags.IsCarouselOff)
            {
                MessageHandler.Speak(57, game);
            }
            // !IF NOT FLIPPED, SPIN.
            return ret_val;

            L20500:
            if (game.ParserVectors.prsa == VerbIds.WalkIn && game.Flags.carozf)
            {
                AdventurerHandler.jigsup_(game, 58);
            }
            // !WALKED IN.
            return ret_val;
            // RAPPL1, PAGE 7

            // R21--	LLD ROOM.  HANDLE EXORCISE, DESCRIPTIONS.

            L21000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L21500;
            }
            // !LOOK?
            MessageHandler.Speak(59, game);
            // !DESCRIBE.
            if (!game.Flags.lldf)
            {
                MessageHandler.Speak(60, game);
            }
            // !IF NOT VANISHED, GHOSTS.
            return ret_val;

            L21500:
            if (game.ParserVectors.prsa != VerbIds.exorcw)
            {
                return ret_val;
            }
            // !EXORCISE?
            if (game.Objects[ObjectIds.Bell].Adventurer == game.Player.Winner
                && game.Objects[ObjectIds.Book].Adventurer == game.Player.Winner
                && game.Objects[ObjectIds.Candle].Adventurer == game.Player.Winner
                && (game.Objects[ObjectIds.Candle].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L21600;
            }

            MessageHandler.Speak(62, game);
            // !NOT EQUIPPED.
            return ret_val;

            L21600:
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Ghost, game.Player.Here, game))
            {
                goto L21700;
            }
            // !GHOST HERE?
            AdventurerHandler.jigsup_(game, 61);
            // !NOPE, EXORCISE YOU.
            return ret_val;

            L21700:
            ObjectHandler.SetNewObjectStatus(ObjectIds.Ghost, 63, 0, 0, 0, game);
            // !VANISH GHOST.
            game.Flags.lldf = true;
            // !OPEN GATE.
            return ret_val;

            // R22--	LLD2-ROOM.  IS HIS HEAD ON A POLE?

            L22000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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

            L23000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(66, game);
            // !DESCRIBE.
            i = (ObjectIds)67;
            if (game.Flags.IsLowTide)
            {
                i = (ObjectIds)68;
            }
            MessageHandler.Speak(i, game);
            // !DESCRIBE RESERVOIR.
            MessageHandler.Speak(69, game);
            // !DESCRIBE PANEL.
            if (game.Flags.gatef)
            {
                MessageHandler.Speak(70, game);
            }
            // !BUBBLE IS GLOWING.
            return ret_val;

            // R24--	TREE ROOM

            L24000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(660, game);
            // !DESCRIBE.
            i = (ObjectIds)661;
            // !SET FLAG FOR BELOW.
            i__1 = game.Objects.Count;
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                // !DESCRIBE OBJ IN FORE3.
                if (!ObjectHandler.IsObjectInRoom(j, RoomIds.Forest3, game) || j == ObjectIds.ftree)
                {
                    goto L24200;
                }

                MessageHandler.Speak(i, game);
                // !SET STAGE,
                i = 0;
                MessageHandler.rspsub_(502, game.Objects[j].Description2, game);
                // !DESCRIBE.
                L24200:
                ;
            }
            return ret_val;
            // RAPPL1, PAGE 8

            // R25--	CYCLOPS-ROOM.  DEPENDS ON CYCLOPS STATE, ASLEEP FLAG, MAGIC FLAG.


            L25000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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

            L26000:
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
            {
                return ret_val;
            }
            // !SURPRISE HIM.
            for (i = (ObjectIds)1; i <= (ObjectIds)8; i += 2)
            {
                // !SCOLRM DEPENDS ON
                if (game.Screen.fromdr == (int)game.Screen.scoldr[(int)i - 1])
                {
                    game.Screen.scolrm = game.Screen.scoldr[(int)i];
                }
                // L26100:
            }
            // !ENTRY DIRECTION.
            return ret_val;

            // R27--	TREASURE ROOM.

            L27000:
            if (game.ParserVectors.prsa != VerbIds.WalkIn || !game.Hack.IsThiefActive)
            {
                return ret_val;
            }

            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.thief, game).Id != game.Player.Here)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.thief, 82, game.Player.Here, 0, 0, game);
            }

            game.Hack.ThiefPosition = game.Player.Here;
            // !RESET SEARCH PATTERN.
            game.Objects[ObjectIds.thief].Flag2 |= ObjectFlags2.FITEBT;
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.chali, game).Id == game.Player.Here)
            {
                game.Objects[ObjectIds.chali].Flag1 &= ~ObjectFlags.IsTakeable;
            }

            // 	VANISH EVERYTHING IN ROOM

            j = 0;
            // !ASSUME NOTHING TO VANISH.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                if (i == ObjectIds.chali || i == ObjectIds.thief || !ObjectHandler.IsObjectInRoom((ObjectIds)i, game.Player.Here, game))
                {
                    goto L27200;
                }

                j = (ObjectIds)83;
                // !FLAG BYEBYE.
                game.Objects[i].Flag1 &= ~ObjectFlags.IsVisible;
                L27200:
                ;
            }
            MessageHandler.Speak(j, game);
            // !DESCRIBE.
            return ret_val;

            // R28--	CLIFF FUNCTION.  SEE IF CARRYING INFLATED BOAT.

            L28000:
            game.Flags.deflaf = game.Objects[ObjectIds.rboat].Adventurer != game.Player.Winner;
            // !TRUE IF NOT CARRYING.
            return ret_val;
            // RAPPL1, PAGE 9

            // R29--	RIVR4 ROOM.  PLAY WITH BUOY.

            L29000:
            if (!game.Flags.buoyf || game.Objects[ObjectIds.buoy].Adventurer != game.Player.Winner)
            {
                return ret_val;
            }
            MessageHandler.Speak(84, game);
            // !GIVE HINT,
            game.Flags.buoyf = false;
            // !THEN DISABLE.
            return ret_val;

            // R30--	OVERFALLS.  DOOM.

            L30000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                AdventurerHandler.jigsup_(game, 85);
            }
            // !OVER YOU GO.
            return ret_val;

            // R31--	BEACH ROOM.  DIG A HOLE.

            L31000:
            if (game.ParserVectors.prsa != VerbIds.Dig || game.ParserVectors.DirectObject != ObjectIds.Shovel)
            {
                return ret_val;
            }
            ++game.Switches.rvsnd;
            // !INCREMENT DIG STATE.
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
            if ((game.Objects[ObjectIds.statu].Flag1 & ObjectFlags.IsVisible) != 0)
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

            L32000:
            if (game.ParserVectors.prsa != VerbIds.Dig || game.ParserVectors.DirectObject != ObjectIds.Shovel)
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

            L33000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(100, game);
            // !DESCRIBE.
            i = (ObjectIds)102;
            // !ASSUME SAFE ROOM OK.
            if ((game.Rooms[RoomIds.Safe].Flags & RoomFlags.RMUNG) != 0)
            {
                i = (ObjectIds)101;
            }
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;

            // R35--	SAFE ROOM.  STATE DEPENDS ON WHETHER SAFE BLOWN.

            L35000:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            MessageHandler.Speak(104, game);
            // !DESCRIBE.
            i = (ObjectIds)105;
            // !ASSUME OK.
            if (game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)106;
            }
            // !BLOWN?
            MessageHandler.Speak(i, game);
            // !DESCRIBE.
            return ret_val;

            // R36--	MAGNET ROOM.  DESCRIBE, CHECK FOR SPINDIZZY DOOM.

            MAGNETROOM:
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L36500;
            }
            // !LOOK?
            MessageHandler.Speak(107, game);
            // !DESCRIBE.
            return ret_val;

            L36500:
            if (game.ParserVectors.prsa != VerbIds.WalkIn || !game.Flags.IsCarouselOff)
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
            i = (ObjectIds)58;
            // !SPIN HIS INSIDES.
            if (game.Player.Winner != ActorIds.Player)
            {
                i = (ObjectIds)99;
            }
            // !SPIN ROBOT.
            AdventurerHandler.jigsup_(game, (int)i);
            // !DEAD.
            return ret_val;

            // R37--	CAGE ROOM.  IF SOLVED CAGE, MOVE TO OTHER CAGE ROOM.

            CAGEROOM:
            if (game.Flags.cagesf)
            {
                f = AdventurerHandler.moveto_(game, RoomIds.cager, game.Player.Winner);
            }
            // !IF SOLVED, MOVE.
            return ret_val;
        }

        public static bool RunRoomAction2(int ri, Game game)
        {
            const int newrms = 38;

            //  System generated locals
            int i__1;
            bool ret_val;

            int i;
            ObjectIds j;

            ret_val = true;
            switch (ri - newrms + 1)
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
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                lookto_(game, (int)RoomIds.FrontDoor, (int)RoomIds.mrg, 0, 682, 681);
            }
            return ret_val;

            //  R39--	MIRROR G ROOM

            L39000:
            if (game.ParserVectors.prsa == VerbIds.WalkIn)
            {
                AdventurerHandler.jigsup_(game, 685);
            }
            return ret_val;

            //  R40--	MIRROR C ROOM

            L40000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                lookto_(game, (int)RoomIds.mrg, (int)RoomIds.mrb, 683, 0, 681);
            }
            return ret_val;

            //  R41--	MIRROR B ROOM

            L41000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                lookto_(game, (int)RoomIds.mrc, (int)RoomIds.mra, 0, 0, 681);
            }
            return ret_val;

            //  R42--	MIRROR A ROOM

            L42000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                lookto_(game, (int)RoomIds.mrb, 0, 0, 684, 681);
            }
            return ret_val;
            //  RAPPL2, PAGE 3

            //  R43--	MIRROR C EAST/WEST

            L43000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 683);
            }
            return ret_val;

            //  R44--	MIRROR B EAST/WEST

            L44000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 686);
            }
            return ret_val;

            //  R45--	MIRROR A EAST/WEST

            L45000:
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                DescribeEastWestNarrowRooms(game, game.Player.Here, 687);
            }
            return ret_val;

            //  R46--	INSIDE MIRROR

            L46000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return ret_val;
            }
            // !LOOK?
            i = 704;
            // !ASSUME BEAM STOP.
            i__1 = game.Objects.Count;
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game) && j != ObjectIds.rbeam)
                {
                    goto L47200;
                }
                //  L47100:
            }

            i = 703;
            L47200:
            MessageHandler.rspsub_(i, game.Objects[j].Description2, game);
            // !DESCRIBE BEAM.
            lookto_(game, (int)RoomIds.mra, 0, 0, 0, 0);
            // !LOOK NORTH.
            return ret_val;

            //  R48--	INSIDE CRYPT

            L48000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
            {
                goto L50100;
            }

            // !MASTER FOLLOWS.
            game.Clock.Flags[(int)ClockIndices.cevfol - 1] = true;
            game.Clock.Ticks[(int)ClockIndices.cevfol - 1] = -1;
            return ret_val;

            L50100:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa == VerbIds.WalkIn)
            {
                game.Clock.Ticks[(int)ClockIndices.cevfol - 1] = 0;
            }

            // !IF EXITS, KILL FOLLOW.
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                i__1 = game.Switches.pnumb + 712;
                MessageHandler.rspsub_(712, i__1, game);
            }
            return ret_val;

            //  R54--	CELL

            L54000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                MessageHandler.Speak(724, game);
            }
            // !LOOK?
            return ret_val;

            //  R56--	NIRVANA CELL

            L56000:
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.WalkIn)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            if (game.ParserVectors.prsa != VerbIds.Look)
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
        /// <param name="so"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static bool OpenCloseDoor(ObjectIds obj, int so, int sc, Game game)
        {
            int i__1;
            // !ASSUME WINS.
            bool ret_val = true;

            // !CLOSE?
            if (game.ParserVectors.prsa == VerbIds.Close)
            {
                goto L100;
            }

            // !OPEN?
            if (game.ParserVectors.prsa == VerbIds.Open)
            {
                goto L50;
            }

            // !LOSE
            ret_val = false;
            return ret_val;

            L50:
            if ((game.Objects[obj].Flag2.HasFlag(ObjectFlags2.IsOpen)))
            {
                goto L200;
            }

            // !OPEN... IS IT?
            MessageHandler.Speak(so, game);
            game.Objects[obj].Flag2 |= ObjectFlags2.IsOpen;
            return ret_val;

            L100:
            if (!((game.Objects[obj].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto L200;
            }

            // !CLOSE... IS IT?
            MessageHandler.Speak(sc, game);
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

            MessageHandler.rspeak_(game, i);
            if (m1 && game.Flags.mropnf)
            {
                i__1 = (i - 819) / 2 + 823;
                MessageHandler.rspeak_(game, i__1);
            }

            MessageHandler.rspeak_(game, 825);
            MessageHandler.rspeak_(game, st);
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

            MessageHandler.Speak(game, ht);
            // !DESCRIBE HALL.
            MessageHandler.Speak(game, nt);
            // !DESCRIBE NORTH VIEW.
            MessageHandler.Speak(game, st);
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
            MessageHandler.rspsb2_(game, 848, dir, dir);

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
                MessageHandler.Speak(game, i__1);
            }

            if (mrbf != 0)
            {
                MessageHandler.Speak(game, 851);
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
                MessageHandler.Speak(game, i);
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