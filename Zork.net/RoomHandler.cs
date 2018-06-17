using System;
using System.Linq;
using Zork.Core.Object;
using Zork.Core.Room;

namespace Zork.Core
{
    public static class RoomHandler
    {
        public static bool RoomDescription(int verbosity, Game game)
        {
            int full = verbosity;
            bool ret_val, L__1;
            int i, ra;

            // FULL= 0/1/2/3= SHORT/OBJ/ROOM/FULL

            ret_val = true;

            // !ASSUME WINS.
            if (game.ParserVectors.prso < (int)XSearch.xmin)
            {
                goto L50;
            }

            // !IF DIRECTION,
            game.Screen.fromdr = game.ParserVectors.prso;

            // !SAVE AND
            game.ParserVectors.prso = 0;

            // !CLEAR.
            L50:
            if (game.Player.Here == game.Adventurers.Rooms[(int)(AIndices.player - 1)])
            {
                goto L100;
            }

            // !PLAYER JUST MOVE?
            MessageHandler.Speak(2, game);

            // !NO, JUST SAY DONE.
            game.ParserVectors.prsa = (int)VIndices.walkiw;

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
            ra = game.Rooms.RoomActions[game.Player.Here - 1];

            // !GET ROOM ACTION.
            if (full == 1)
            {
                goto L600;
            }

            // !OBJ ONLY?
            i = game.Rooms.RoomDescriptions2[game.Player.Here - 1];

            // !ASSUME SHORT DESC.
            if (full == 0
                && (game.Flags.superf || (game.Rooms.RoomFlags[game.Player.Here - 1].HasFlag(RoomFlags.RSEEN)
                && game.Flags.brieff)))
            {
                goto L400;
            }

            //  The next line means that when you request VERBOSE mode, you
            //  only get long room descriptions 20% of the time. I don't either
            //  like or understand this, so the mod. ensures VERBOSE works
            //  all the time.			jmh@ukc.ac.uk 22/10/87

            // & .AND.(BRIEFF.OR.PROB(80,80)))))       GO TO 400
            i = game.Rooms.RoomDescriptions1[game.Player.Here - 1];

            // !USE LONG.
            if (i != 0 || ra == 0)
            {
                goto L400;
            }

            // !IF GOT DESC, SKIP.
            game.ParserVectors.prsa = (int)VIndices.lookw;

            // !PRETEND LOOK AROUND.
            if (!rappli_(ra, game))
            {
                goto L100;
            }

            // !ROOM HANDLES, NEW DESC?
            game.ParserVectors.prsa = (int)VIndices.foow;

            // !NOP PARSER.
            goto L500;

            L400:
            MessageHandler.Speak(i, game);

            // !OUTPUT DESCRIPTION.
            L500:
            if (game.Adventurers.Vehicles.Any() && game.Adventurers.Vehicles[game.Player.Winner - 1] != 0)
            {
                MessageHandler.rspsub_(431, game.Objects.odesc2[game.Adventurers.Vehicles[game.Player.Winner - 1] - 1], game);
            }

            L600:
            if (full != 2)
            {
                L__1 = full != 0;
                RoomHandler.PrintRoomContents(L__1, game.Player.Here, game);
            }

            game.Rooms.RoomFlags[(int)(game.Player.Here - 1)] |= RoomFlags.RSEEN;
            if (full != 0 || ra == 0)
            {
                return ret_val;
            }

            // !ANYTHING MORE?
            game.ParserVectors.prsa = (int)VIndices.walkiw;

            // !GIVE HIM A SURPISE.
            if (!rappli_(ra, game))
            {
                goto L100;
            }

            // !ROOM HANDLES, NEW DESC?
            game.ParserVectors.prsa = (int)VIndices.foow;

            return ret_val;
        }

        /// <summary>
        /// princr_ - PRINT CONTENTS OF ROOM */
        /// </summary>
        /// <param name="full"></param>
        /// <param name="roomId"></param>
        /// <param name="game"></param>
        public static void PrintRoomContents(bool full, int roomId, Game game)
        {
            int i__1, i__2;
            int i, j, k;

            j = 329;
            // !ASSUME SUPERBRIEF FORMAT.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP ON OBJECTS */
                if (!ObjectHandler.IsInRoom(roomId, i, game)
                    || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.VISIBT + ObjectFlags.NDSCBT) != ObjectFlags.VISIBT
                    || (game.Adventurers.Vehicles.Any() && i == game.Adventurers.Vehicles[game.Player.Winner - 1]))
                {
                    goto L500;
                }
                if (!(full) && (game.Flags.superf || game.Flags.brieff && (
                    game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RSEEN) != 0))
                {
                    goto L200;
                }

                // DO LONG DESCRIPTION OF OBJECT

                k = game.Objects.odesco[i - 1];
                // !GET UNTOUCHED
                if (k == 0 || (game.Objects.oflag2[i - 1] & ObjectFlags2.TCHBT) != 0)
                {
                    k = game.Objects.odesc1[i - 1];
                }

                MessageHandler.Speak(k, game);

                // !DESCRIBE
                goto L500;
                // DO SHORT DESCRIPTION OF OBJECT

                L200:
                MessageHandler.rspsub_(j, game.Objects.odesc2[i - 1], game);
                // !YOU CAN SEE IT
                j = 502;

                L500:
                ;
            }

            // NOW LOOP TO PRINT CONTENTS OF OBJECTS IN ROOM

            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP ON OBJECTS
                if (!ObjectHandler.IsInRoom(roomId, i, game) || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.VISIBT + ObjectFlags.NDSCBT) != ObjectFlags.VISIBT)
                {
                    goto L1000;
                }
                if ((game.Objects.oflag2[i - 1] & ObjectFlags2.ACTRBT) != 0)
                {
                    i__2 = ObjectHandler.GetActor(i, game);
                    AdventurerHandler.PrintContents(i__2, game);
                }
                if ((game.Objects.oflag1[i - 1] & ObjectFlags.TRANBT) == 0
                    && (game.Objects.oflag2[i - 1] & ObjectFlags2.OPENBT) == 0 || ObjectHandler.IsObjectEmpty(i, game))
                {
                    goto L1000;
                }

                // OBJECT IS NOT EMPTY AND IS OPEN OR TRANSPARENT

                j = 573;
                if (i != (int)ObjectIndices.tcase)
                {
                    goto L600;
                }
                // !TROPHY CASE? */
                j = 574;
                if ((game.Flags.brieff || game.Flags.superf) && !(full))
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

        public static bool IsRoomLit(int roomId, Game game)
        {
            // System generated locals
            int i__1, i__2;
            bool ret_val;

            // Local variables
            int i, j, oa;

            ret_val = true;

            // !ASSUME WINS
            if ((game.Rooms.RoomFlags[roomId - 1] & RoomFlags.RLIGHT) != 0)
            {
                return ret_val;
            }

            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOK FOR LIT OBJ
                if (ObjectHandler.IsInRoom(roomId, i, game))
                {
                    goto L100;
                }

                // !IN ROOM?
                oa = game.Objects.oadv[i - 1];
                // !NO
                if (oa <= 0)
                {
                    goto L1000;
                }

                //!ON ADV?
                if (game.Adventurers.Rooms[oa - 1] != roomId)
                {
                    goto L1000;
                }

                // !ADV IN ROOM?

                // OBJ IN ROOM OR ON ADV IN ROOM

                L100:
                if ((game.Objects.oflag1[i - 1] & ObjectFlags.ONBT) != 0)
                {
                    return ret_val;
                }

                if ((game.Objects.oflag1[i - 1] & ObjectFlags.VISIBT) == 0
                    || (game.Objects.oflag1[i - 1] & ObjectFlags.TRANBT) == 0
                    && (game.Objects.oflag2[i - 1] & ObjectFlags2.OPENBT) == 0)
                {
                    goto L1000;
                }

                // OBJ IS VISIBLE AND OPEN OR TRANSPARENT
                i__2 = game.Objects.Count;
                for (j = 1; j <= i__2; ++j)
                {
                    if (game.Objects.ocan[j - 1] == i && (game.Objects.oflag1[j - 1] & ObjectFlags.ONBT) != 0)
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
        /// ROUTING ROUTINE FOR ROOM APPLICABLES
        /// </summary>
        /// <param name="ri"></param>
        /// <returns></returns>
        public static bool rappli_(int ri, Game game)
        {
            const int newrms = 38;

            // System generated locals
            bool ret_val = true;

            // !ASSUME WINS.
            if (ri == 0)
            {
                return ret_val;
            }

            // !IF ZERO, WIN.
            if (ri < newrms)
            {
                ret_val = RoomHandler.rappl1_(ri, game);
            }

            // !IF OLD, PROCESSOR 1.
            if (ri >= newrms)
            {
                ret_val = rappl2_(ri, game);
            }

            // !IF NEW, PROCESSOR 2.
            return ret_val;
        }

        public static bool rappl1_(int ri, Game game)
        {
            int i__1, i__2;
            bool ret_val;

            // Local variables
            bool f;
            int i;
            int j;

            ret_val = true;
            // 						!USUALLY IGNORED.
            if (ri == 0)
            {
                return ret_val;
            }
            // 						!RETURN IF NAUGHT.

            // 						!SET TO FALSE FOR

            // 						!NEW DESC NEEDED.
            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L2000;
                case 3: goto L3000;
                case 4: goto L4000;
                case 5: goto L5000;
                case 6: goto L6000;
                case 7: goto L7000;
                case 8: goto L8000;
                case 9: goto L9000;
                case 10: goto L10000;
                case 11: goto L11000;
                case 12: goto L12000;
                case 13: goto L13000;
                case 14: goto L14000;
                case 15: goto L15000;
                case 16: goto L16000;
                case 17: goto L17000;
                case 18: goto L18000;
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
                case 36: goto L36000;
                case 37: goto L37000;
            }

            throw new InvalidOperationException("1");
            //bug_(1, ri);

            // R1--	EAST OF HOUSE.  DESCRIPTION DEPENDS ON STATE OF WINDOW

            L1000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 13;
            // 						!ASSUME CLOSED.
            if ((game.Objects.oflag2[(int)ObjectIndices.windo - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, AJAR.
            MessageHandler.rspsub_(11, i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R2--	KITCHEN.  SAME VIEW FROM INSIDE.

            L2000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 13;
            // 						!ASSUME CLOSED.
            if ((game.Objects.oflag2[(int)ObjectIndices.windo - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, AJAR.
            MessageHandler.rspsub_(14, i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R3--	LIVING ROOM.  DESCRIPTION DEPENDS ON MAGICF (STATE OF
            // 	DOOR TO CYCLOPS ROOM), RUG (MOVED OR NOT), DOOR (OPEN OR CLOSED)

            L3000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L3500;
            }
            // 						!LOOK?
            i = 15;
            // 						!ASSUME NO HOLE.
            if (game.Flags.magicf)
            {
                i = 16;
            }
            // 						!IF MAGICF, CYCLOPS HOLE.
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            i = game.Switch.orrug + 17;
            // 						!ASSUME INITIAL STATE.
            if ((game.Objects.oflag2[(int)ObjectIndices.door - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i += 2;
            }
            // 						!DOOR OPEN?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // 	NOT A LOOK WORD.  REEVALUATE TROPHY CASE.

            L3500:
            if (game.ParserVectors.prsa != (int)VIndices.takew && (game.ParserVectors.prsa != (int)VIndices.putw ||
                game.ParserVectors.prsi != (int)ObjectIndices.tcase))
            {
                return ret_val;
            }
            game.Adventurers.Scores[game.Player.Winner - 1] = game.State.rwscor;
            // 						!SCORE TROPHY CASE.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // 						!RETAIN RAW SCORE AS WELL.
                j = i;
                // 						!FIND OUT IF IN CASE.
                L3550:
                j = game.Objects.ocan[j - 1];
                // 						!TRACE OWNERSHIP.
                if (j == 0)
                {
                    goto L3600;
                }
                if (j != (int)ObjectIndices.tcase)
                {
                    goto L3550;
                }
                // 						!DO ALL LEVELS.
                game.Adventurers.Scores[game.Player.Winner - 1] += game.Objects.otval[i - 1];
                L3600:
                ;
            }

            AdventurerHandler.scrupd_(game, 0);
            // 						!SEE IF ENDGAME TRIG.
            return ret_val;
            // RAPPL1, PAGE 3

            // R4--	CELLAR.  SHUT DOOR AND BAR IT IF HE JUST WALKED IN.

            L4000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L4500;
            }
            // 						!LOOK?
            MessageHandler.Speak(21, game);
            // 						!DESCRIBE CELLAR.
            return ret_val;

            L4500:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw)
            {
                return ret_val;
            }

            // 						!WALKIN?
            if ((game.Objects.oflag2[(int)ObjectIndices.door - 1] & (int)ObjectFlags2.OPENBT + ObjectFlags2.TCHBT) != ObjectFlags2.OPENBT)
            {
                return ret_val;
            }

            game.Objects.oflag2[(int)ObjectIndices.door - 1] = (game.Objects.oflag2[(int)ObjectIndices.door - 1] | ObjectFlags2.TCHBT) & ~ObjectFlags2.OPENBT;
            MessageHandler.Speak(22, game);
            // 						!SLAM AND BOLT DOOR.
            return ret_val;

            // R5--	MAZE11.  DESCRIBE STATE OF GRATING.

            L5000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(23, game);
            // 						!DESCRIBE.
            i = 24;
            // 						!ASSUME LOCKED.
            if (game.Flags.grunlf)
            {
                i = 26;
            }
            // 						!UNLOCKED?
            if ((game.Objects.oflag2[(int)ObjectIndices.grate - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i = 25;
            }
            // 						!OPEN?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE GRATE.
            return ret_val;

            // R6--	CLEARING.  DESCRIBE CLEARING, MOVE LEAVES.

            L6000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L6500;
            }
            // 						!LOOK?
            MessageHandler.Speak(27, game);
            // 						!DESCRIBE.
            if (game.Switch.rvclr == 0)
            {
                return ret_val;
            }
            // 						!LEAVES MOVED?
            i = 28;
            // 						!YES, ASSUME GRATE CLOSED.
            if ((game.Objects.oflag2[(int)ObjectIndices.grate - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i = 29;
            }
            // 						!OPEN?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE GRATE.
            return ret_val;

            L6500:
            if (game.Switch.rvclr != 0
                || ObjectHandler.qhere_((int)ObjectIndices.leave, (int)RoomIndices.clear, game)
                && (game.ParserVectors.prsa != (int)VIndices.movew
                || game.ParserVectors.prso != (int)ObjectIndices.leave))
            {
                return ret_val;
            }

            MessageHandler.Speak(30, game);
            // 						!MOVE LEAVES, REVEAL GRATE.
            game.Switch.rvclr = 1;
            // 						!INDICATE LEAVES MOVED.
            return ret_val;
            // RAPPL1, PAGE 4

            // R7--	RESERVOIR SOUTH.  DESCRIPTION DEPENDS ON LOW TIDE FLAG.

            L7000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 31;
            // 						!ASSUME FULL.
            if (game.Flags.lwtidf)
            {
                i = 32;
            }
            // 						!IF LOW TIDE, EMPTY.
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            MessageHandler.Speak(33, game);
            // 						!DESCRIBE EXITS.
            return ret_val;

            // R8--	RESERVOIR.  STATE DEPENDS ON LOW TIDE FLAG.

            L8000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 34;
            // 						!ASSUME FULL.
            if (game.Flags.lwtidf)
            {
                i = 35;
            }
            // 						!IF LOW TIDE, EMTPY.
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R9--	RESERVOIR NORTH.  ALSO DEPENDS ON LOW TIDE FLAG.

            L9000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 36;
            // 						!YOU GET THE IDEA.
            if (game.Flags.lwtidf)
            {
                i = 37;
            }

            MessageHandler.Speak(i, game);
            MessageHandler.Speak(38, game);

            return ret_val;

            // R10--	GLACIER ROOM.  STATE DEPENDS ON MELTED, VANISHED FLAGS.

            L10000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(39, game);
            // 						!BASIC DESCRIPTION.
            i = 0;
            // 						!ASSUME NO CHANGES.
            if (game.Flags.glacmf)
            {
                i = 40;
            }
            // 						!PARTIAL MELT?
            if (game.Flags.glacrf)
            {
                i = 41;
            }
            // 						!COMPLETE MELT?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R11--	FOREST ROOM

            L11000:
            if (game.ParserVectors.prsa == (int)VIndices.walkiw)
            {
                game.Clock.Flags[(int)ClockIndices.cevfor - 1] = true;
            }
            // 						!IF WALK IN, BIRDIE.
            return ret_val;

            // R12--	MIRROR ROOM.  STATE DEPENDS ON MIRROR INTACT.

            L12000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(42, game);
            // 						!DESCRIBE.
            if (game.Flags.mirrmf)
            {
                MessageHandler.Speak(43, game);
            }
            // 						!IF BROKEN, NASTY REMARK.
            return ret_val;
            // RAPPL1, PAGE 5

            // R13--	CAVE2 ROOM.  BLOW OUT CANDLES WITH 50% PROBABILITY.

            L13000:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw)
            {
                return ret_val;
            }
            // 						!WALKIN?
            if (prob_(game, 50, 50) || game.Objects.oadv[(int)ObjectIndices.candl - 1] !=
                game.Player.Winner || !((game.Objects.oflag1[(int)ObjectIndices.candl - 1] & ObjectFlags.ONBT) != 0))
            {
                return ret_val;
            }
            game.Objects.oflag1[(int)ObjectIndices.candl - 1] &= ~ObjectFlags.ONBT;
            MessageHandler.Speak(47, game);
            // 						!TELL OF WINDS.
            game.Clock.Flags[(int)ClockIndices.cevcnd - 1] = false;
            // 						!HALT CANDLE COUNTDOWN.
            return ret_val;

            // R14--	BOOM ROOM.  BLOW HIM UP IF CARRYING FLAMING OBJECT.

            L14000:
            j = game.Objects.odesc2[(int)ObjectIndices.candl - 1];
            // 						!ASSUME CANDLE.
            if (game.Objects.oadv[(int)ObjectIndices.candl - 1] == game.Player.Winner && (
                game.Objects.oflag1[(int)ObjectIndices.candl - 1] & ObjectFlags.ONBT) != 0)
            {
                goto L14100;
            }
            j = game.Objects.odesc2[(int)ObjectIndices.torch - 1];
            // 						!ASSUME TORCH.
            if (game.Objects.oadv[(int)ObjectIndices.torch - 1] == game.Player.Winner && (
                game.Objects.oflag1[(int)ObjectIndices.torch - 1] & ObjectFlags.ONBT) != 0)
            {
                goto L14100;
            }
            j = game.Objects.odesc2[(int)ObjectIndices.match - 1];
            if (game.Objects.oadv[(int)ObjectIndices.match - 1] == game.Player.Winner && (
                game.Objects.oflag1[(int)ObjectIndices.match - 1] & ObjectFlags.ONBT) != 0)
            {
                goto L14100;
            }
            return ret_val;
            // 						!SAFE

            L14100:
            if (game.ParserVectors.prsa != (int)VIndices.trnonw)
            {
                goto L14200;
            }
            // 						!TURN ON?
            MessageHandler.rspsub_(294, j, game);
            // 						!BOOM
            // 						!
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            L14200:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw)
            {
                return ret_val;
            }
            // 						!WALKIN?
            MessageHandler.rspsub_(295, j, game);
            // 						!BOOM
            // 						!
            AdventurerHandler.jigsup_(game, 44);
            return ret_val;

            // R15--	NO-OBJS.  SEE IF EMPTY HANDED, SCORE LIGHT SHAFT.

            L15000:
            game.Flags.empthf = true;
            // 						!ASSUME TRUE.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // 						!SEE IF CARRYING.
                if (game.Objects.oadv[i - 1] == game.Player.Winner)
                {
                    game.Flags.empthf = false;
                }
                // L15100:
            }

            if (game.Player.Here != (int)RoomIndices.bshaf || !RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                return ret_val;
            }
            AdventurerHandler.scrupd_(game, game.State.ltshft);
            // 						!SCORE LIGHT SHAFT.
            game.State.ltshft = 0;
            // 						!NEVER AGAIN.
            return ret_val;
            // RAPPL1, PAGE 6

            // R16--	MACHINE ROOM.  DESCRIBE MACHINE.

            L16000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 46;
            // 						!ASSUME LID CLOSED.
            if ((game.Objects.oflag2[(int)ObjectIndices.machi - 1] & ObjectFlags2.OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, OPEN.
            MessageHandler.rspsub_(45, i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R17--	BAT ROOM.  UNLESS CARRYING GARLIC, FLY AWAY WITH ME...

            L17000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L17500;
            }
            // 						!LOOK?
            MessageHandler.Speak(48, game);
            // 						!DESCRIBE ROOM.
            if (game.Objects.oadv[(int)ObjectIndices.garli - 1] == game.Player.Winner)
            {
                MessageHandler.Speak(49, game);
            }
            // 						!BAT HOLDS NOSE.
            return ret_val;

            L17500:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw || game.Objects.oadv[(int)ObjectIndices.garli - 1]
                == game.Player.Winner)
            {
                return ret_val;
            }
            MessageHandler.Speak(50, game);
            // 						!TIME TO FLY, JACK.
            f = AdventurerHandler.moveto_(bats_1.batdrp[rnd_(9)], game.Player.Winner);
            // 						!SELECT RANDOM DEST.
            ret_val = false;
            // 						!INDICATE NEW DESC NEEDED.
            return ret_val;

            // R18--	DOME ROOM.  STATE DEPENDS ON WHETHER ROPE TIED TO RAILING.

            L18000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L18500;
            }
            // 						!LOOK?
            MessageHandler.Speak(51, game);
            // 						!DESCRIBE.
            if (game.Flags.domef)
            {
                MessageHandler.Speak(52, game);
            }
            // 						!IF ROPE, DESCRIBE.
            return ret_val;

            L18500:
            if (game.ParserVectors.prsa == (int)VIndices.leapw)
            {
                AdventurerHandler.jigsup_(game, 53);
            }
            // 						!DID HE JUMP???
            return ret_val;

            // R19--	TORCH ROOM.  ALSO DEPENDS ON WHETHER ROPE TIED TO RAILING.

            L19000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(54, game);
            // 						!DESCRIBE.
            if (game.Flags.domef)
            {
                MessageHandler.Speak(55, game);
            }
            // 						!IF ROPE, DESCRIBE.
            return ret_val;

            // R20--	CAROUSEL ROOM.  SPIN HIM OR KILL HIM.

            L20000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L20500;
            }
            // 						!LOOK?
            MessageHandler.Speak(56, game);
            // 						!DESCRIBE.
            if (!game.Flags.caroff)
            {
                MessageHandler.Speak(57, game);
            }
            // 						!IF NOT FLIPPED, SPIN.
            return ret_val;

            L20500:
            if (game.ParserVectors.prsa == (int)VIndices.walkiw && game.Flags.carozf)
            {
                AdventurerHandler.jigsup_(game, 58);
            }
            // 						!WALKED IN.
            return ret_val;
            // RAPPL1, PAGE 7

            // R21--	LLD ROOM.  HANDLE EXORCISE, DESCRIPTIONS.

            L21000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L21500;
            }
            // 						!LOOK?
            MessageHandler.Speak(59, game);
            // 						!DESCRIBE.
            if (!game.Flags.lldf)
            {
                MessageHandler.Speak(60, game);
            }
            // 						!IF NOT VANISHED, GHOSTS.
            return ret_val;

            L21500:
            if (game.ParserVectors.prsa != (int)VIndices.exorcw)
            {
                return ret_val;
            }
            // 						!EXORCISE?
            if (game.Objects.oadv[(int)ObjectIndices.bell - 1] == game.Player.Winner && game.Objects.oadv[
                (int)ObjectIndices.book - 1] == game.Player.Winner && game.Objects.oadv[
                    (int)ObjectIndices.candl - 1] == game.Player.Winner && (game.Objects.oflag1[
                        (int)ObjectIndices.candl - 1] & ObjectFlags.ONBT) != 0)
            {
                goto L21600;
            }
            MessageHandler.Speak(62, game);
            // 						!NOT EQUIPPED.
            return ret_val;

            L21600:
            if (ObjectHandler.qhere_((int)ObjectIndices.ghost, game.Player.Here, game))
            {
                goto L21700;
            }
            // 						!GHOST HERE?
            AdventurerHandler.jigsup_(game, 61);
            // 						!NOPE, EXORCISE YOU.
            return ret_val;

            L21700:
            ObjectHandler.newsta_((int)ObjectIndices.ghost, 63, 0, 0, 0, game);
            // 						!VANISH GHOST.
            game.Flags.lldf = true;
            // 						!OPEN GATE.
            return ret_val;

            // R22--	LLD2-ROOM.  IS HIS HEAD ON A POLE?

            L22000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(64, game);
            // 						!DESCRIBE.
            if (game.Flags.onpolf)
            {
                MessageHandler.Speak(65, game);
            }
            // 						!ON POLE?
            return ret_val;

            // R23--	DAM ROOM.  DESCRIBE RESERVOIR, PANEL.

            L23000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(66, game);
            // 						!DESCRIBE.
            i = 67;
            if (game.Flags.lwtidf)
            {
                i = 68;
            }
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE RESERVOIR.
            MessageHandler.Speak(69, game);
            // 						!DESCRIBE PANEL.
            if (game.Flags.gatef)
            {
                MessageHandler.Speak(70, game);
            }
            // 						!BUBBLE IS GLOWING.
            return ret_val;

            // R24--	TREE ROOM

            L24000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(660, game);
            // 						!DESCRIBE.
            i = 661;
            // 						!SET FLAG FOR BELOW.
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // 						!DESCRIBE OBJ IN FORE3.
                if (!ObjectHandler.qhere_(j, (int)RoomIndices.fore3, game) || j == (int)ObjectIndices.ftree)
                {
                    goto L24200;
                }

                MessageHandler.Speak(i, game);
                // 						!SET STAGE,
                i = 0;
                MessageHandler.rspsub_(502, game.Objects.odesc2[j - 1], game);
                // 						!DESCRIBE.
                L24200:
                ;
            }
            return ret_val;
            // RAPPL1, PAGE 8

            // R25--	CYCLOPS-ROOM.  DEPENDS ON CYCLOPS STATE, ASLEEP FLAG, MAGIC FLAG.


            L25000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(606, game);
            // 						!DESCRIBE.
            i = 607;
            // 						!ASSUME BASIC STATE.
            if (game.Switch.rvcyc > 0)
            {
                i = 608;
            }
            // 						!>0?  HUNGRY.
            if (game.Switch.rvcyc < 0)
            {
                i = 609;
            }
            // 						!<0?  THIRSTY.
            if (game.Flags.cyclof)
            {
                i = 610;
            }
            // 						!ASLEEP?
            if (game.Flags.magicf)
            {
                i = 611;
            }
            // 						!GONE?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            if (!game.Flags.cyclof && game.Switch.rvcyc != 0)
            {
                i__1 = Math.Abs(game.Switch.rvcyc) + 193;
                MessageHandler.Speak(i__1, game);
            }
            return ret_val;

            // R26--	BANK BOX ROOM.

            L26000:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw)
            {
                return ret_val;
            }
            // 						!SURPRISE HIM.
            for (i = 1; i <= 8; i += 2)
            {
                // 						!SCOLRM DEPENDS ON
                if (game.Screen.fromdr == game.Screen.scoldr[i - 1])
                {
                    game.Screen.scolrm = game.Screen.scoldr[i];
                }
                // L26100:
            }
            // 						!ENTRY DIRECTION.
            return ret_val;

            // R27--	TREASURE ROOM.

            L27000:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw || !game.Hack.thfact)
            {
                return ret_val;
            }
            if (game.Objects.oroom[(int)ObjectIndices.thief - 1] != game.Player.Here)
            {
                ObjectHandler.newsta_((int)ObjectIndices.thief, 82, game.Player.Here, 0, 0, game);
            }
            game.Hack.thfpos = game.Player.Here;
            // 						!RESET SEARCH PATTERN.
            game.Objects.oflag2[(int)ObjectIndices.thief - 1] |= ObjectFlags2.FITEBT;
            if (game.Objects.oroom[(int)ObjectIndices.chali - 1] == game.Player.Here)
            {
                game.Objects.oflag1[(int)ObjectIndices.chali - 1] &= ~ObjectFlags.TAKEBT;
            }

            // 	VANISH EVERYTHING IN ROOM

            j = 0;
            // 						!ASSUME NOTHING TO VANISH.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (i == (int)ObjectIndices.chali || i == (int)ObjectIndices.thief || !ObjectHandler.qhere_(i, game.Player.Here, game))
                {
                    goto L27200;
                }
                j = 83;
                // 						!FLAG BYEBYE.
                game.Objects.oflag1[i - 1] &= ~ObjectFlags.VISIBT;
                L27200:
                ;
            }
            MessageHandler.Speak(j, game);
            // 						!DESCRIBE.
            return ret_val;

            // R28--	CLIFF FUNCTION.  SEE IF CARRYING INFLATED BOAT.

            L28000:
            game.Flags.deflaf = game.Objects.oadv[(int)ObjectIndices.rboat - 1] != game.Player.Winner;
            // 						!TRUE IF NOT CARRYING.
            return ret_val;
            // RAPPL1, PAGE 9

            // R29--	RIVR4 ROOM.  PLAY WITH BUOY.

            L29000:
            if (!game.Flags.buoyf || game.Objects.oadv[(int)ObjectIndices.buoy - 1] != game.Player.Winner)
            {
                return ret_val;
            }
            MessageHandler.Speak(84, game);
            // 						!GIVE HINT,
            game.Flags.buoyf = false;
            // 						!THEN DISABLE.
            return ret_val;

            // R30--	OVERFALLS.  DOOM.

            L30000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                AdventurerHandler.jigsup_(game, 85);
            }
            // 						!OVER YOU GO.
            return ret_val;

            // R31--	BEACH ROOM.  DIG A HOLE.

            L31000:
            if (game.ParserVectors.prsa != (int)VIndices.digw || game.ParserVectors.prso != (int)ObjectIndices.shove)
            {
                return ret_val;
            }
            ++game.Switch.rvsnd;
            // 						!INCREMENT DIG STATE.
            switch (game.Switch.rvsnd)
            {
                case 1: goto L31100;
                case 2: goto L31100;
                case 3: goto L31100;
                case 4: goto L31400;
                case 5: goto L31500;
            }
            // 						!PROCESS STATE.
            //bug_(2, game.Switch.rvsnd);
            throw new InvalidOperationException("2");

            L31100:
            i__1 = game.Switch.rvsnd + 85;
            MessageHandler.Speak(i__1, game);
            // 						!1-3... DISCOURAGE HIM.
            return ret_val;

            L31400:
            i = 89;
            // 						!ASSUME DISCOVERY.
            if ((game.Objects.oflag1[(int)ObjectIndices.statu - 1] & ObjectFlags.VISIBT) != 0)
            {
                i = 88;
            }

            MessageHandler.Speak(i, game);
            game.Objects.oflag1[(int)ObjectIndices.statu - 1] |= ObjectFlags.VISIBT;
            return ret_val;

            L31500:
            game.Switch.rvsnd = 0;
            // 						!5... SAND COLLAPSES
            AdventurerHandler.jigsup_(game, 90);
            // 						!AND SO DOES HE.
            return ret_val;

            // R32--	TCAVE ROOM.  DIG A HOLE IN GUANO.

            L32000:
            if (game.ParserVectors.prsa != (int)VIndices.digw || game.ParserVectors.prso != (int)ObjectIndices.shove)
            {
                return ret_val;
            }
            i = 91;
            // 						!ASSUME NO GUANO.
            if (!ObjectHandler.qhere_((int)ObjectIndices.guano, game.Player.Here, game))
            {
                goto L32100;
            }
            // 						!IS IT HERE?
            // Computing MIN
            i__1 = 4;
            i__2 = game.Switch.rvgua + 1;

            game.Switch.rvgua = Math.Min(i__1, i__2);
            // 						!YES, SET NEW STATE.
            i = game.Switch.rvgua + 91;
            // 						!GET NASTY REMARK.
            L32100:
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R33--	FALLS ROOM

            L33000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(96, game);
            // 						!DESCRIBE.
            i = 97;
            // 						!ASSUME NO RAINBOW.
            if (game.Flags.rainbf)
            {
                i = 98;
            }
            // 						!GOT ONE?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;
            // RAPPL1, PAGE 10

            // R34--	LEDGE FUNCTION.  LEDGE CAN COLLAPSE.

            L34000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(100, game);
            // 						!DESCRIBE.
            i = 102;
            // 						!ASSUME SAFE ROOM OK.
            if ((game.Rooms.RoomFlags[(int)RoomIndices.msafe - 1] & RoomFlags.RMUNG) != 0)
            {
                i = 101;
            }
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R35--	SAFE ROOM.  STATE DEPENDS ON WHETHER SAFE BLOWN.

            L35000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            MessageHandler.Speak(104, game);
            // 						!DESCRIBE.
            i = 105;
            // 						!ASSUME OK.
            if (game.Flags.safef)
            {
                i = 106;
            }
            // 						!BLOWN?
            MessageHandler.Speak(i, game);
            // 						!DESCRIBE.
            return ret_val;

            // R36--	MAGNET ROOM.  DESCRIBE, CHECK FOR SPINDIZZY DOOM.

            L36000:
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L36500;
            }
            // 						!LOOK?
            MessageHandler.Speak(107, game);
            // 						!DESCRIBE.
            return ret_val;

            L36500:
            if (game.ParserVectors.prsa != (int)VIndices.walkiw || !game.Flags.caroff)
            {
                return ret_val;
            }
            // 						!WALKIN ON FLIPPED?
            if (game.Flags.carozf)
            {
                goto L36600;
            }
            // 						!ZOOM?
            MessageHandler.Speak(108, game);
            // 						!NO, SPIN HIS COMPASS.
            return ret_val;

            L36600:
            i = 58;
            // 						!SPIN HIS INSIDES.
            if (game.Player.Winner != (int)AIndices.player)
            {
                i = 99;
            }
            // 						!SPIN ROBOT.
            AdventurerHandler.jigsup_(game, i);
            // 						!DEAD.
            return ret_val;

            // R37--	CAGE ROOM.  IF SOLVED CAGE, MOVE TO OTHER CAGE ROOM.

            L37000:
            if (game.Flags.cagesf)
            {
                f = AdventurerHandler.moveto_(game, (int)RoomIndices.cager, game.Player.Winner);
            }
            // !IF SOLVED, MOVE.
            return ret_val;
        }

        public static bool rappl2_(int ri, Game game)
        {
            {
                //  Initialized data

                const int newrms = 38;

                //  System generated locals
                int i__1;
                bool ret_val;

                //  Local variables
                int i;
                int j;

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
                    case 20: goto L57000;
                    case 21: goto L58000;
                    case 22: goto L59000;
                    case 23: goto L60000;
                }

                //bug_(70, ri);
                throw new InvalidOperationException("70");

                return ret_val;

                //  R38--	MIRROR D ROOM

                L38000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    lookto_(game, (int)RoomIndices.fdoor, (int)RoomIndices.mrg, 0, 682, 681);
                }
                return ret_val;

                //  R39--	MIRROR G ROOM

                L39000:
                if (game.ParserVectors.prsa == (int)VIndices.walkiw)
                {
                    AdventurerHandler.jigsup_(game, 685);
                }
                return ret_val;

                //  R40--	MIRROR C ROOM

                L40000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    lookto_(game, (int)RoomIndices.mrg, (int)RoomIndices.mrb, 683, 0, 681);
                }
                return ret_val;

                //  R41--	MIRROR B ROOM

                L41000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    lookto_(game, (int)RoomIndices.mrc, (int)RoomIndices.mra, 0, 0, 681);
                }
                return ret_val;

                //  R42--	MIRROR A ROOM

                L42000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    lookto_(game, (int)RoomIndices.mrb, 0, 0, 684, 681);
                }
                return ret_val;
                //  RAPPL2, PAGE 3

                //  R43--	MIRROR C EAST/WEST

                L43000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    ewtell_(game.Player.Here, 683);
                }
                return ret_val;

                //  R44--	MIRROR B EAST/WEST

                L44000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    ewtell_(game.Player.Here, 686);
                }
                return ret_val;

                //  R45--	MIRROR A EAST/WEST

                L45000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    ewtell_(game.Player.Here, 687);
                }
                return ret_val;

                //  R46--	INSIDE MIRROR

                L46000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
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
                if (game.Switch.mdir == 270 && game.Switch.mloc == (int)RoomIndices.mrb)
                {
                    i = Math.Min(game.Switch.poleuf, 1) + 690;
                }
                if (game.Switch.mdir % 180 == 0)
                {
                    i = Math.Min(game.Switch.poleuf, 1) + 692;
                }
                MessageHandler.Speak(i, game);
                // !DESCRIBE POLE.
                i__1 = game.Switch.mdir / 45 + 695;
                MessageHandler.rspsub_(694, i__1, game);
                // !DESCRIBE ARROW.
                return ret_val;
                //  RAPPL2, PAGE 4

                //  R47--	MIRROR EYE ROOM

                L47000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 704;
                // !ASSUME BEAM STOP.
                i__1 = game.Objects.Count;
                for (j = 1; j <= i__1; ++j)
                {
                    if (ObjectHandler.qhere_(j, game.Player.Here, game) && j != (int)ObjectIndices.rbeam)
                    {
                        goto L47200;
                    }
                    //  L47100:
                }
                i = 703;
                L47200:
                MessageHandler.rspsub_(i, game.Objects.odesc2[j - 1], game);
                // !DESCRIBE BEAM.
                lookto_(game, (int)RoomIndices.mra, 0, 0, 0, 0);
                // !LOOK NORTH.
                return ret_val;

                //  R48--	INSIDE CRYPT

                L48000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !CRYPT IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.tomb - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(705, i, game);
                return ret_val;

                //  R49--	SOUTH CORRIDOR

                L49000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                MessageHandler.Speak(706, game);
                // !DESCRIBE.
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.odoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                if (game.Switch.lcell == 4)
                {
                    MessageHandler.rspsub_(707, i, game);
                }
                // !DESCRIBE ODOOR IF THERE.
                return ret_val;

                //  R50--	BEHIND DOOR

                L50000:
                if (game.ParserVectors.prsa != (int)VIndices.walkiw)
                {
                    goto L50100;
                }
                // !WALK IN?
                game.Clock.Flags[(int)ClockIndices.cevfol - 1] = true;
                // !MASTER FOLLOWS.
                game.Clock.Ticks[(int)ClockIndices.cevfol - 1] = -1;
                return ret_val;

                L50100:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !QDOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.qdoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(708, i, game);
                return ret_val;
                //  RAPPL2, PAGE 5

                //  R51--	FRONT DOOR

                L51000:
                if (game.ParserVectors.prsa == (int)VIndices.walkiw)
                {
                    game.Clock.Ticks[(int)ClockIndices.cevfol - 1] = 0;
                }
                // !IF EXITS, KILL FOLLOW.
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                lookto_(game, 0, (int)RoomIndices.mrd, 709, 0, 0);
                // !DESCRIBE SOUTH.
                i = 46;
                // !PANEL IS OPEN/CLOSED.
                if (game.Flags.inqstf)
                {
                    i = 12;
                }
                // !OPEN IF INQ STARTED.
                j = 46;
                // !QDOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.qdoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    j = 12;
                }

                MessageHandler.rspsb2_(710, i, j, game);

                return ret_val;

                //  R52--	NORTH CORRIDOR

                L52000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                if ((game.Objects.oflag2[(int)ObjectIndices.cdoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                // !CDOOR IS OPEN/CLOSED.
                MessageHandler.rspsub_(711, i, game);
                return ret_val;

                //  R53--	PARAPET

                L53000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    i__1 = game.Switch.pnumb + 712;
                    MessageHandler.rspsub_(712, i__1, game);
                }
                return ret_val;

                //  R54--	CELL

                L54000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 721;
                // !CDOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.cdoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 722;
                }

                MessageHandler.Speak(i, game);
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.odoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                if (game.Switch.lcell == 4)
                {
                    MessageHandler.rspsub_(723, i, game);
                }
                // 						!DESCRIBE.
                return ret_val;

                //  R55--	PRISON CELL

                L55000:
                if (game.ParserVectors.prsa == (int)VIndices.lookw)
                {
                    MessageHandler.Speak(724, game);
                }
                // !LOOK?
                return ret_val;

                //  R56--	NIRVANA CELL

                L56000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.odoor - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(725, i, game);
                return ret_val;
                //  RAPPL2, PAGE 6

                //  R57--	NIRVANA AND END OF GAME

                L57000:
                if (game.ParserVectors.prsa != (int)VIndices.walkiw)
                {
                    return ret_val;
                }
                // !WALKIN?
                MessageHandler.Speak(726, game);
                AdventurerHandler.score_(game, false);
                //  moved to exit routine	CLOSE(DBCH)
                //exit_();
                throw new ApplicationException("Exit");

                //  R58--	TOMB ROOM

                L58000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !TOMB IS OPEN/CLOSED.
                if ((game.Objects.oflag2[(int)ObjectIndices.tomb - 1] & ObjectFlags2.OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(792, i, game);
                return ret_val;

                //  R59--	PUZZLE SIDE ROOM

                L59000:
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
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
                if (game.ParserVectors.prsa != (int)VIndices.lookw)
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
                if ((game.Objects.oflag2[(int)ObjectIndices.warni - 1] & ObjectFlags2.TCHBT) != 0)
                {
                    MessageHandler.Speak(869, game);
                }

                return ret_val;

                L60100:
                cpinfo_(880, game.Switch.cphere);
                // !DESCRIBE ROOM.
                return ret_val;
            }
        }

        public static bool prob_(Game game, int g, int b)
        {
            /* System generated locals */
            bool ret_val;

            /* Local variables */
            int i;

            i = g;
            /* 						!ASSUME GOOD LUCK. */
            if (game.Flags.badlkf)
            {
                i = b;
            }
            /* 						!IF BAD, TOO BAD. */
            ret_val = rnd_(100) < i;
            /* 						!COMPUTE. */
            return ret_val;
        }

        /// <summary>
        /// opncls_ - Process open/close for doors
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="so"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static bool opncls_(int obj, int so, int sc, Game game)
        {
            int i__1;
            bool ret_val;

            ret_val = true;
            // !ASSUME WINS. */
            if (game.ParserVectors.prsa == (int)VIndices.closew)
            {
                goto L100;
            }

            // !CLOSE? */
            if (game.ParserVectors.prsa == (int)VIndices.openw)
            {
                goto L50;
            }
            // !OPEN? */
            ret_val = false;
            // !LOSE */
            return ret_val;

            L50:
            if ((game.Objects.oflag2[obj - 1] & ObjectFlags2.OPENBT) != 0)
            {
                goto L200;
            }

            // !OPEN... IS IT? */
            MessageHandler.Speak(so, game);
            game.Objects.oflag2[obj - 1] |= ObjectFlags2.OPENBT;
            return ret_val;

            L100:
            if (!((game.Objects.oflag2[obj - 1] & ObjectFlags2.OPENBT) != 0))
            {
                goto L200;
            }

            // !CLOSE... IS IT? */
            MessageHandler.Speak(sc, game);
            game.Objects.oflag2[obj - 1] &= ~ObjectFlags2.OPENBT;
            return ret_val;

            L200:
            i__1 = rnd_(3) + 125;
            MessageHandler.Speak(i__1, game);
            // !DUMMY. */
            return ret_val;
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

            /* Local variables */
            int i, m1, dir, mrbf;

            MessageHandler.Speak(game, ht);
            /* 						!DESCRIBE HALL. */
            MessageHandler.Speak(game, nt);
            /* 						!DESCRIBE NORTH VIEW. */
            MessageHandler.Speak(game, st);
            /* 						!DESCRIBE SOUTH VIEW. */
            dir = 0;
            /* 						!ASSUME NO DIRECTION. */
            if ((i__1 = game.Switch.mloc - game.Player.Here, Math.Abs(i__1)) != 1)
            {
                goto L200;
            }

            /* 						!MIRROR TO N OR S? */
            if (game.Switch.mloc == nrm)
            {
                dir = 695;
            }
            if (game.Switch.mloc == srm)
            {
                dir = 699;
            }
            /* 						!DIR=N/S. */
            if (game.Switch.mdir % 180 != 0)
            {
                goto L100;
            }
            /* 						!MIRROR N-S? */
            MessageHandler.rspsub_(game, 847, dir);
            /* 						!YES, HE SEES PANEL */
            MessageHandler.rspsb2_(game, 848, dir, dir);
            /* 						!AND NARROW ROOMS. */
            goto L200;

            L100:
            m1 = mrhere_(game, game.Player.Here);
            /* 						!WHICH MIRROR? */
            mrbf = 0;
            /* 						!ASSUME INTACT. */
            if (m1 == 1 && !game.Flags.mr1f || m1 == 2 && !game.Flags.mr2f)
            {
                mrbf = 1;
            }

            i__1 = mrbf + 849;
            MessageHandler.rspsub_(game, i__1, dir);
            /* 						!DESCRIBE. */
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
            /* 						!ASSUME NO MORE TO DO. */
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
            /* 						!DESCRIBE HALLS. */
        }

        /* MRHERE--	IS MIRROR HERE? */

        /* DECLARATIONS */

        public static int mrhere_(Game game, int rm)
        {
            /* System generated locals */
            int ret_val, i__1;

            if (rm < (int)RoomIndices.mrae || rm > (int)RoomIndices.mrdw)
            {
                goto L100;
            }

            /* RM IS AN E-W ROOM, MIRROR MUST BE N-S (MDIR= 0 OR 180) */

            ret_val = 1;
            /* 						!ASSUME MIRROR 1 HERE. */
            if ((rm - (int)RoomIndices.mrae) % 2 == game.Switch.mdir / 180)
            {
                ret_val = 2;
            }
            return ret_val;

            /* RM IS NORTH OR SOUTH OF MIRROR.  IF MIRROR IS N-S OR NOT */
            /* WITHIN ONE ROOM OF RM, LOSE. */

            L100:
            ret_val = 0;
            if ((i__1 = game.Switch.mloc - rm, Math.Abs(i__1)) != 1 || game.Switch.mdir % 180 == 0)
            {
                return ret_val;
            }

            /* RM IS WITHIN ONE OF MLOC, AND MDIR IS E-W */

            ret_val = 1;
            if (rm < game.Switch.mloc && game.Switch.mdir < 180 || rm > game.Switch.mloc && game.Switch.mdir > 180)
            {
                ret_val = 2;
            }
            return ret_val;
        }
    }
}