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

        public static bool rappl1_(int roomInfo, Game game)
        {
            /*
             logical rappl1_(ri)
integer ri;
{
	// System generated locals
            integer i__1, i__2;
            logical ret_val;

            // Local variables
            logical f;
            integer i;
            integer j;

            ret_val = TRUE_;
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
            bug_(1, ri);

            // R1--	EAST OF HOUSE.  DESCRIPTION DEPENDS ON STATE OF WINDOW

            L1000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 13;
            // 						!ASSUME CLOSED.
            if ((game.Objects.oflag2[oindex_1.windo - 1] & OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, AJAR.
            MessageHandler.rspsub_(11, i);
            // 						!DESCRIBE.
            return ret_val;

            // R2--	KITCHEN.  SAME VIEW FROM INSIDE.

            L2000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 13;
            // 						!ASSUME CLOSED.
            if ((game.Objects.oflag2[oindex_1.windo - 1] & OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, AJAR.
            MessageHandler.rspsub_(14, i);
            // 						!DESCRIBE.
            return ret_val;

            // R3--	LIVING ROOM.  DESCRIPTION DEPENDS ON MAGICF (STATE OF
            // 	DOOR TO CYCLOPS ROOM), RUG (MOVED OR NOT), DOOR (OPEN OR CLOSED)

            L3000:
            if (prsvec_1.prsa != vindex_1.lookw)
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
            rspeak_(i);
            // 						!DESCRIBE.
            i = game.Flags.orrug + 17;
            // 						!ASSUME INITIAL STATE.
            if ((game.Objects.oflag2[oindex_1.door - 1] & OPENBT) != 0)
            {
                i += 2;
            }
            // 						!DOOR OPEN?
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // 	NOT A LOOK WORD.  REEVALUATE TROPHY CASE.

            L3500:
            if (prsvec_1.prsa != vindex_1.takew && (prsvec_1.prsa != vindex_1.putw ||
                prsvec_1.prsi != oindex_1.tcase))
            {
                return ret_val;
            }
            advs_1.ascore[game.Player.winner - 1] = state_1.rwscor;
            // 						!SCORE TROPHY CASE.
            i__1 = game.Objects.olnt;
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
                if (j != oindex_1.tcase)
                {
                    goto L3550;
                }
                // 						!DO ALL LEVELS.
                advs_1.ascore[game.Player.winner - 1] += game.Objects.otval[i - 1];
                L3600:
                ;
            }
            scrupd_(0);
            // 						!SEE IF ENDGAME TRIG.
            return ret_val;
            // RAPPL1, PAGE 3

            // R4--	CELLAR.  SHUT DOOR AND BAR IT IF HE JUST WALKED IN.

            L4000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L4500;
            }
            // 						!LOOK?
            rspeak_(21);
            // 						!DESCRIBE CELLAR.
            return ret_val;

            L4500:
            if (prsvec_1.prsa != vindex_1.walkiw)
            {
                return ret_val;
            }
            // 						!WALKIN?
            if ((game.Objects.oflag2[oindex_1.door - 1] & OPENBT +
                TCHBT) != OPENBT)
            {
                return ret_val;
            }
            game.Objects.oflag2[oindex_1.door - 1] = (game.Objects.oflag2[oindex_1.door - 1] |
                TCHBT) & ~OPENBT;
            rspeak_(22);
            // 						!SLAM AND BOLT DOOR.
            return ret_val;

            // R5--	MAZE11.  DESCRIBE STATE OF GRATING.

            L5000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(23);
            // 						!DESCRIBE.
            i = 24;
            // 						!ASSUME LOCKED.
            if (game.Flags.grunlf)
            {
                i = 26;
            }
            // 						!UNLOCKED?
            if ((game.Objects.oflag2[oindex_1.grate - 1] & OPENBT) != 0)
            {
                i = 25;
            }
            // 						!OPEN?
            rspeak_(i);
            // 						!DESCRIBE GRATE.
            return ret_val;

            // R6--	CLEARING.  DESCRIBE CLEARING, MOVE LEAVES.

            L6000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L6500;
            }
            // 						!LOOK?
            rspeak_(27);
            // 						!DESCRIBE.
            if (game.Flags.rvclr == 0)
            {
                return ret_val;
            }
            // 						!LEAVES MOVED?
            i = 28;
            // 						!YES, ASSUME GRATE CLOSED.
            if ((game.Objects.oflag2[oindex_1.grate - 1] & OPENBT) != 0)
            {
                i = 29;
            }
            // 						!OPEN?
            rspeak_(i);
            // 						!DESCRIBE GRATE.
            return ret_val;

            L6500:
            if (game.Flags.rvclr != 0 || qhere_(oindex_1.leave, rindex_1.clear) && (
                prsvec_1.prsa != vindex_1.movew || prsvec_1.prso !=
                oindex_1.leave))
            {
                return ret_val;
            }
            rspeak_(30);
            // 						!MOVE LEAVES, REVEAL GRATE.
            game.Flags.rvclr = 1;
            // 						!INDICATE LEAVES MOVED.
            return ret_val;
            // RAPPL1, PAGE 4

            // R7--	RESERVOIR SOUTH.  DESCRIPTION DEPENDS ON LOW TIDE FLAG.

            L7000:
            if (prsvec_1.prsa != vindex_1.lookw)
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
            rspeak_(i);
            // 						!DESCRIBE.
            rspeak_(33);
            // 						!DESCRIBE EXITS.
            return ret_val;

            // R8--	RESERVOIR.  STATE DEPENDS ON LOW TIDE FLAG.

            L8000:
            if (prsvec_1.prsa != vindex_1.lookw)
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
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // R9--	RESERVOIR NORTH.  ALSO DEPENDS ON LOW TIDE FLAG.

            L9000:
            if (prsvec_1.prsa != vindex_1.lookw)
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
            rspeak_(i);
            rspeak_(38);
            return ret_val;

            // R10--	GLACIER ROOM.  STATE DEPENDS ON MELTED, VANISHED FLAGS.

            L10000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(39);
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
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // R11--	FOREST ROOM

            L11000:
            if (prsvec_1.prsa == vindex_1.walkiw)
            {
                cevent_1.cflag[cindex_1.cevfor - 1] = TRUE_;
            }
            // 						!IF WALK IN, BIRDIE.
            return ret_val;

            // R12--	MIRROR ROOM.  STATE DEPENDS ON MIRROR INTACT.

            L12000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(42);
            // 						!DESCRIBE.
            if (game.Flags.mirrmf)
            {
                rspeak_(43);
            }
            // 						!IF BROKEN, NASTY REMARK.
            return ret_val;
            // RAPPL1, PAGE 5

            // R13--	CAVE2 ROOM.  BLOW OUT CANDLES WITH 50% PROBABILITY.

            L13000:
            if (prsvec_1.prsa != vindex_1.walkiw)
            {
                return ret_val;
            }
            // 						!WALKIN?
            if (prob_(50, 50) || game.Objects.oadv[oindex_1.candl - 1] !=
                game.Player.winner || !((game.Objects.oflag1[oindex_1.candl - 1] &
                    ONBT) != 0))
            {
                return ret_val;
            }
            game.Objects.oflag1[oindex_1.candl - 1] &= ~ONBT;
            rspeak_(47);
            // 						!TELL OF WINDS.
            cevent_1.cflag[cindex_1.cevcnd - 1] = FALSE_;
            // 						!HALT CANDLE COUNTDOWN.
            return ret_val;

            // R14--	BOOM ROOM.  BLOW HIM UP IF CARRYING FLAMING OBJECT.

            L14000:
            j = game.Objects.odesc2[oindex_1.candl - 1];
            // 						!ASSUME CANDLE.
            if (game.Objects.oadv[oindex_1.candl - 1] == game.Player.winner && (
                game.Objects.oflag1[oindex_1.candl - 1] & ONBT) != 0)
            {
                goto L14100;
            }
            j = game.Objects.odesc2[oindex_1.torch - 1];
            // 						!ASSUME TORCH.
            if (game.Objects.oadv[oindex_1.torch - 1] == game.Player.winner && (
                game.Objects.oflag1[oindex_1.torch - 1] & ONBT) != 0)
            {
                goto L14100;
            }
            j = game.Objects.odesc2[oindex_1.match - 1];
            if (game.Objects.oadv[oindex_1.match - 1] == game.Player.winner && (
                game.Objects.oflag1[oindex_1.match - 1] & ONBT) != 0)
            {
                goto L14100;
            }
            return ret_val;
            // 						!SAFE

            L14100:
            if (prsvec_1.prsa != vindex_1.trnonw)
            {
                goto L14200;
            }
            // 						!TURN ON?
            MessageHandler.rspsub_(294, j);
            // 						!BOOM
            // 						!
            jigsup_(44);
            return ret_val;

            L14200:
            if (prsvec_1.prsa != vindex_1.walkiw)
            {
                return ret_val;
            }
            // 						!WALKIN?
            MessageHandler.rspsub_(295, j);
            // 						!BOOM
            // 						!
            jigsup_(44);
            return ret_val;

            // R15--	NO-OBJS.  SEE IF EMPTY HANDED, SCORE LIGHT SHAFT.

            L15000:
            game.Flags.empthf = TRUE_;
            // 						!ASSUME TRUE.
            i__1 = game.Objects.olnt;
            for (i = 1; i <= i__1; ++i)
            {
                // 						!SEE IF CARRYING.
                if (game.Objects.oadv[i - 1] == game.Player.winner)
                {
                    game.Flags.empthf = FALSE_;
                }
                // L15100:
            }

            if (game.Player.Here != rindex_1.bshaf || !lit_(game.Player.Here))
            {
                return ret_val;
            }
            scrupd_(state_1.ltshft);
            // 						!SCORE LIGHT SHAFT.
            state_1.ltshft = 0;
            // 						!NEVER AGAIN.
            return ret_val;
            // RAPPL1, PAGE 6

            // R16--	MACHINE ROOM.  DESCRIBE MACHINE.

            L16000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            i = 46;
            // 						!ASSUME LID CLOSED.
            if ((game.Objects.oflag2[oindex_1.machi - 1] & OPENBT) != 0)
            {
                i = 12;
            }
            // 						!IF OPEN, OPEN.
            MessageHandler.rspsub_(45, i);
            // 						!DESCRIBE.
            return ret_val;

            // R17--	BAT ROOM.  UNLESS CARRYING GARLIC, FLY AWAY WITH ME...

            L17000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L17500;
            }
            // 						!LOOK?
            rspeak_(48);
            // 						!DESCRIBE ROOM.
            if (game.Objects.oadv[oindex_1.garli - 1] == game.Player.winner)
            {
                rspeak_(49);
            }
            // 						!BAT HOLDS NOSE.
            return ret_val;

            L17500:
            if (prsvec_1.prsa != vindex_1.walkiw || game.Objects.oadv[oindex_1.garli - 1]
                == game.Player.winner)
            {
                return ret_val;
            }
            rspeak_(50);
            // 						!TIME TO FLY, JACK.
            f = moveto_(bats_1.batdrp[rnd_(9)], game.Player.winner);
            // 						!SELECT RANDOM DEST.
            ret_val = FALSE_;
            // 						!INDICATE NEW DESC NEEDED.
            return ret_val;

            // R18--	DOME ROOM.  STATE DEPENDS ON WHETHER ROPE TIED TO RAILING.

            L18000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L18500;
            }
            // 						!LOOK?
            rspeak_(51);
            // 						!DESCRIBE.
            if (game.Flags.domef)
            {
                rspeak_(52);
            }
            // 						!IF ROPE, DESCRIBE.
            return ret_val;

            L18500:
            if (prsvec_1.prsa == vindex_1.leapw)
            {
                jigsup_(53);
            }
            // 						!DID HE JUMP???
            return ret_val;

            // R19--	TORCH ROOM.  ALSO DEPENDS ON WHETHER ROPE TIED TO RAILING.

            L19000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(54);
            // 						!DESCRIBE.
            if (game.Flags.domef)
            {
                rspeak_(55);
            }
            // 						!IF ROPE, DESCRIBE.
            return ret_val;

            // R20--	CAROUSEL ROOM.  SPIN HIM OR KILL HIM.

            L20000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L20500;
            }
            // 						!LOOK?
            rspeak_(56);
            // 						!DESCRIBE.
            if (!game.Flags.caroff)
            {
                rspeak_(57);
            }
            // 						!IF NOT FLIPPED, SPIN.
            return ret_val;

            L20500:
            if (prsvec_1.prsa == vindex_1.walkiw && game.Flags.carozf)
            {
                jigsup_(58);
            }
            // 						!WALKED IN.
            return ret_val;
            // RAPPL1, PAGE 7

            // R21--	LLD ROOM.  HANDLE EXORCISE, DESCRIPTIONS.

            L21000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L21500;
            }
            // 						!LOOK?
            rspeak_(59);
            // 						!DESCRIBE.
            if (!game.Flags.lldf)
            {
                rspeak_(60);
            }
            // 						!IF NOT VANISHED, GHOSTS.
            return ret_val;

            L21500:
            if (prsvec_1.prsa != vindex_1.exorcw)
            {
                return ret_val;
            }
            // 						!EXORCISE?
            if (game.Objects.oadv[oindex_1.bell - 1] == game.Player.winner && game.Objects.oadv[
                oindex_1.book - 1] == game.Player.winner && game.Objects.oadv[
                    oindex_1.candl - 1] == game.Player.winner && (game.Objects.oflag1[
                        oindex_1.candl - 1] & ONBT) != 0)
            {
                goto L21600;
            }
            rspeak_(62);
            // 						!NOT EQUIPPED.
            return ret_val;

            L21600:
            if (qhere_(oindex_1.ghost, game.Player.Here))
            {
                goto L21700;
            }
            // 						!GHOST HERE?
            jigsup_(61);
            // 						!NOPE, EXORCISE YOU.
            return ret_val;

            L21700:
            newsta_(oindex_1.ghost, 63, 0, 0, 0);
            // 						!VANISH GHOST.
            game.Flags.lldf = TRUE_;
            // 						!OPEN GATE.
            return ret_val;

            // R22--	LLD2-ROOM.  IS HIS HEAD ON A POLE?

            L22000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(64);
            // 						!DESCRIBE.
            if (game.Flags.onpolf)
            {
                rspeak_(65);
            }
            // 						!ON POLE?
            return ret_val;

            // R23--	DAM ROOM.  DESCRIBE RESERVOIR, PANEL.

            L23000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(66);
            // 						!DESCRIBE.
            i = 67;
            if (game.Flags.lwtidf)
            {
                i = 68;
            }
            rspeak_(i);
            // 						!DESCRIBE RESERVOIR.
            rspeak_(69);
            // 						!DESCRIBE PANEL.
            if (game.Flags.gatef)
            {
                rspeak_(70);
            }
            // 						!BUBBLE IS GLOWING.
            return ret_val;

            // R24--	TREE ROOM

            L24000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(660);
            // 						!DESCRIBE.
            i = 661;
            // 						!SET FLAG FOR BELOW.
            i__1 = game.Objects.olnt;
            for (j = 1; j <= i__1; ++j)
            {
                // 						!DESCRIBE OBJ IN FORE3.
                if (!qhere_(j, rindex_1.fore3) || j == oindex_1.ftree)
                {
                    goto L24200;
                }
                rspeak_(i);
                // 						!SET STAGE,
                i = 0;
                MessageHandler.rspsub_(502, game.Objects.odesc2[j - 1]);
                // 						!DESCRIBE.
                L24200:
                ;
            }
            return ret_val;
            // RAPPL1, PAGE 8

            // R25--	CYCLOPS-ROOM.  DEPENDS ON CYCLOPS STATE, ASLEEP FLAG, MAGIC FLAG.


            L25000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(606);
            // 						!DESCRIBE.
            i = 607;
            // 						!ASSUME BASIC STATE.
            if (game.Flags.rvcyc > 0)
            {
                i = 608;
            }
            // 						!>0?  HUNGRY.
            if (game.Flags.rvcyc < 0)
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
            rspeak_(i);
            // 						!DESCRIBE.
            if (!game.Flags.cyclof && game.Flags.rvcyc != 0)
            {
                i__1 = abs(game.Flags.rvcyc) + 193;
                rspeak_(i__1);
            }
            return ret_val;

            // R26--	BANK BOX ROOM.

            L26000:
            if (prsvec_1.prsa != vindex_1.walkiw)
            {
                return ret_val;
            }
            // 						!SURPRISE HIM.
            for (i = 1; i <= 8; i += 2)
            {
                // 						!SCOLRM DEPENDS ON
                if (screen_1.fromdr == screen_1.scoldr[i - 1])
                {
                    screen_1.scolrm = screen_1.scoldr[i];
                }
                // L26100:
            }
            // 						!ENTRY DIRECTION.
            return ret_val;

            // R27--	TREASURE ROOM.

            L27000:
            if (prsvec_1.prsa != vindex_1.walkiw || !hack_1.thfact)
            {
                return ret_val;
            }
            if (game.Objects.oroom[oindex_1.thief - 1] != game.Player.Here)
            {
                newsta_(oindex_1.thief, 82, game.Player.Here, 0, 0);
            }
            hack_1.thfpos = game.Player.Here;
            // 						!RESET SEARCH PATTERN.
            game.Objects.oflag2[oindex_1.thief - 1] |= FITEBT;
            if (game.Objects.oroom[oindex_1.chali - 1] == game.Player.Here)
            {
                game.Objects.oflag1[oindex_1.chali - 1] &= ~TAKEBT;
            }

            // 	VANISH EVERYTHING IN ROOM

            j = 0;
            // 						!ASSUME NOTHING TO VANISH.
            i__1 = game.Objects.olnt;
            for (i = 1; i <= i__1; ++i)
            {
                if (i == oindex_1.chali || i == oindex_1.thief || !qhere_(i,
                    game.Player.Here))
                {
                    goto L27200;
                }
                j = 83;
                // 						!FLAG BYEBYE.
                game.Objects.oflag1[i - 1] &= ~VISIBT;
                L27200:
                ;
            }
            rspeak_(j);
            // 						!DESCRIBE.
            return ret_val;

            // R28--	CLIFF FUNCTION.  SEE IF CARRYING INFLATED BOAT.

            L28000:
            game.Flags.deflaf = game.Objects.oadv[oindex_1.rboat - 1] != game.Player.winner;
            // 						!TRUE IF NOT CARRYING.
            return ret_val;
            // RAPPL1, PAGE 9

            // R29--	RIVR4 ROOM.  PLAY WITH BUOY.

            L29000:
            if (!game.Flags.buoyf || game.Objects.oadv[oindex_1.buoy - 1] != game.Player.winner)
            {
                return ret_val;
            }
            rspeak_(84);
            // 						!GIVE HINT,
            game.Flags.buoyf = FALSE_;
            // 						!THEN DISABLE.
            return ret_val;

            // R30--	OVERFALLS.  DOOM.

            L30000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                jigsup_(85);
            }
            // 						!OVER YOU GO.
            return ret_val;

            // R31--	BEACH ROOM.  DIG A HOLE.

            L31000:
            if (prsvec_1.prsa != vindex_1.digw || prsvec_1.prso != oindex_1.shove)
            {
                return ret_val;
            }
            ++game.Flags.rvsnd;
            // 						!INCREMENT DIG STATE.
            switch (game.Flags.rvsnd)
            {
                case 1: goto L31100;
                case 2: goto L31100;
                case 3: goto L31100;
                case 4: goto L31400;
                case 5: goto L31500;
            }
            // 						!PROCESS STATE.
            bug_(2, game.Flags.rvsnd);

            L31100:
            i__1 = game.Flags.rvsnd + 85;
            rspeak_(i__1);
            // 						!1-3... DISCOURAGE HIM.
            return ret_val;

            L31400:
            i = 89;
            // 						!ASSUME DISCOVERY.
            if ((game.Objects.oflag1[oindex_1.statu - 1] & VISIBT) != 0)
            {
                i = 88;
            }
            rspeak_(i);
            game.Objects.oflag1[oindex_1.statu - 1] |= VISIBT;
            return ret_val;

            L31500:
            game.Flags.rvsnd = 0;
            // 						!5... SAND COLLAPSES
            jigsup_(90);
            // 						!AND SO DOES HE.
            return ret_val;

            // R32--	TCAVE ROOM.  DIG A HOLE IN GUANO.

            L32000:
            if (prsvec_1.prsa != vindex_1.digw || prsvec_1.prso != oindex_1.shove)
            {
                return ret_val;
            }
            i = 91;
            // 						!ASSUME NO GUANO.
            if (!qhere_(oindex_1.guano, game.Player.Here))
            {
                goto L32100;
            }
            // 						!IS IT HERE?
            // Computing MIN
            i__1 = 4, i__2 = game.Flags.rvgua + 1;
            game.Flags.rvgua = min(i__1, i__2);
            // 						!YES, SET NEW STATE.
            i = game.Flags.rvgua + 91;
            // 						!GET NASTY REMARK.
            L32100:
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // R33--	FALLS ROOM

            L33000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(96);
            // 						!DESCRIBE.
            i = 97;
            // 						!ASSUME NO RAINBOW.
            if (game.Flags.rainbf)
            {
                i = 98;
            }
            // 						!GOT ONE?
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;
            // RAPPL1, PAGE 10

            // R34--	LEDGE FUNCTION.  LEDGE CAN COLLAPSE.

            L34000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(100);
            // 						!DESCRIBE.
            i = 102;
            // 						!ASSUME SAFE ROOM OK.
            if ((rooms_1.rflag[rindex_1.msafe - 1] & RMUNG) != 0)
            {
                i = 101;
            }
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // R35--	SAFE ROOM.  STATE DEPENDS ON WHETHER SAFE BLOWN.

            L35000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                return ret_val;
            }
            // 						!LOOK?
            rspeak_(104);
            // 						!DESCRIBE.
            i = 105;
            // 						!ASSUME OK.
            if (game.Flags.safef)
            {
                i = 106;
            }
            // 						!BLOWN?
            rspeak_(i);
            // 						!DESCRIBE.
            return ret_val;

            // R36--	MAGNET ROOM.  DESCRIBE, CHECK FOR SPINDIZZY DOOM.

            L36000:
            if (prsvec_1.prsa != vindex_1.lookw)
            {
                goto L36500;
            }
            // 						!LOOK?
            rspeak_(107);
            // 						!DESCRIBE.
            return ret_val;

            L36500:
            if (prsvec_1.prsa != vindex_1.walkiw || !game.Flags.caroff)
            {
                return ret_val;
            }
            // 						!WALKIN ON FLIPPED?
            if (game.Flags.carozf)
            {
                goto L36600;
            }
            // 						!ZOOM?
            rspeak_(108);
            // 						!NO, SPIN HIS COMPASS.
            return ret_val;

            L36600:
            i = 58;
            // 						!SPIN HIS INSIDES.
            if (game.Player.winner != aindex_1.player)
            {
                i = 99;
            }
            // 						!SPIN ROBOT.
            jigsup_(i);
            // 						!DEAD.
            return ret_val;

            // R37--	CAGE ROOM.  IF SOLVED CAGE, MOVE TO OTHER CAGE ROOM.

            L37000:
            if (game.Flags.cagesf)
            {
                f = moveto_(rindex_1.cager, game.Player.winner);
            }
            // !IF SOLVED, MOVE.
            return ret_val;
            */
            return true;
        }

        public static bool rappl2_(int roomInfo, Game game)
        {
            /*
            logical rappl2_(ri)
integer ri;
            {
                //  Initialized data

                const integer newrms = 38;

                //  System generated locals
                integer i__1;
                logical ret_val;

                //  Local variables
                integer i;
                integer j;

                ret_val = TRUE_;
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
                bug_(70, ri);
                return ret_val;

                //  R38--	MIRROR D ROOM

                L38000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    lookto_(rindex_1.fdoor, rindex_1.mrg, 0, 682, 681);
                }
                return ret_val;

                //  R39--	MIRROR G ROOM

                L39000:
                if (prsvec_1.prsa == vindex_1.walkiw)
                {
                    jigsup_(685);
                }
                return ret_val;

                //  R40--	MIRROR C ROOM

                L40000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    lookto_(rindex_1.mrg, rindex_1.mrb, 683, 0, 681);
                }
                return ret_val;

                //  R41--	MIRROR B ROOM

                L41000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    lookto_(rindex_1.mrc, rindex_1.mra, 0, 0, 681);
                }
                return ret_val;

                //  R42--	MIRROR A ROOM

                L42000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    lookto_(rindex_1.mrb, 0, 0, 684, 681);
                }
                return ret_val;
                //  RAPPL2, PAGE 3

                //  R43--	MIRROR C EAST/WEST

                L43000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    ewtell_(game.Player.Here, 683);
                }
                return ret_val;

                //  R44--	MIRROR B EAST/WEST

                L44000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    ewtell_(game.Player.Here, 686);
                }
                return ret_val;

                //  R45--	MIRROR A EAST/WEST

                L45000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    ewtell_(game.Player.Here, 687);
                }
                return ret_val;

                //  R46--	INSIDE MIRROR

                L46000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                rspeak_(688);
                // !DESCRIBE

                //  NOW DESCRIBE POLE STATE.

                //  CASES 1,2--	MDIR=270 & MLOC=MRB, POLE IS UP OR IN HOLE
                //  CASES 3,4--	MDIR=0 V MDIR=180, POLE IS UP OR IN CHANNEL
                //  CASE 5--	POLE IS UP

                i = 689;
                // !ASSUME CASE 5.
                if (game.Flags.mdir == 270 && game.Flags.mloc == rindex_1.mrb)
                {
                    i = min(game.Flags.poleuf, 1) + 690;
                }
                if (game.Flags.mdir % 180 == 0)
                {
                    i = min(game.Flags.poleuf, 1) + 692;
                }
                rspeak_(i);
                // !DESCRIBE POLE.
                i__1 = game.Flags.mdir / 45 + 695;
                MessageHandler.rspsub_(694, i__1);
                // !DESCRIBE ARROW.
                return ret_val;
                //  RAPPL2, PAGE 4

                //  R47--	MIRROR EYE ROOM

                L47000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 704;
                // !ASSUME BEAM STOP.
                i__1 = game.Objects.olnt;
                for (j = 1; j <= i__1; ++j)
                {
                    if (qhere_(j, game.Player.Here) && j != oindex_1.rbeam)
                    {
                        goto L47200;
                    }
                    //  L47100:
                }
                i = 703;
                L47200:
                MessageHandler.rspsub_(i, game.Objects.odesc2[j - 1]);
                // !DESCRIBE BEAM.
                lookto_(rindex_1.mra, 0, 0, 0, 0);
                // !LOOK NORTH.
                return ret_val;

                //  R48--	INSIDE CRYPT

                L48000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !CRYPT IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.tomb - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(705, i);
                return ret_val;

                //  R49--	SOUTH CORRIDOR

                L49000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                rspeak_(706);
                // !DESCRIBE.
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.odoor - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                if (game.Flags.lcell == 4)
                {
                    MessageHandler.rspsub_(707, i);
                }
                // !DESCRIBE ODOOR IF THERE.
                return ret_val;

                //  R50--	BEHIND DOOR

                L50000:
                if (prsvec_1.prsa != vindex_1.walkiw)
                {
                    goto L50100;
                }
                // !WALK IN?
                cevent_1.cflag[cindex_1.cevfol - 1] = TRUE_;
                // !MASTER FOLLOWS.
                cevent_1.ctick[cindex_1.cevfol - 1] = -1;
                return ret_val;

                L50100:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !QDOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.qdoor - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(708, i);
                return ret_val;
                //  RAPPL2, PAGE 5

                //  R51--	FRONT DOOR

                L51000:
                if (prsvec_1.prsa == vindex_1.walkiw)
                {
                    cevent_1.ctick[cindex_1.cevfol - 1] = 0;
                }
                // !IF EXITS, KILL FOLLOW.
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                lookto_(0, rindex_1.mrd, 709, 0, 0);
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
                if ((game.Objects.oflag2[oindex_1.qdoor - 1] & OPENBT) != 0)
                {
                    j = 12;
                }
                rspsb2_(710, i, j);
                return ret_val;

                //  R52--	NORTH CORRIDOR

                L52000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                if ((game.Objects.oflag2[oindex_1.cdoor - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                // !CDOOR IS OPEN/CLOSED.
                MessageHandler.rspsub_(711, i);
                return ret_val;

                //  R53--	PARAPET

                L53000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    i__1 = game.Flags.pnumb + 712;
                    MessageHandler.rspsub_(712, i__1);
                }
                return ret_val;

                //  R54--	CELL

                L54000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 721;
                // !CDOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.cdoor - 1] & OPENBT) != 0)
                {
                    i = 722;
                }
                rspeak_(i);
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.odoor - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                if (game.Flags.lcell == 4)
                {
                    MessageHandler.rspsub_(723, i);
                }
                // 						!DESCRIBE.
                return ret_val;

                //  R55--	PRISON CELL

                L55000:
                if (prsvec_1.prsa == vindex_1.lookw)
                {
                    rspeak_(724);
                }
                // !LOOK?
                return ret_val;

                //  R56--	NIRVANA CELL

                L56000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !ODOOR IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.odoor - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(725, i);
                return ret_val;
                //  RAPPL2, PAGE 6

                //  R57--	NIRVANA AND END OF GAME

                L57000:
                if (prsvec_1.prsa != vindex_1.walkiw)
                {
                    return ret_val;
                }
                // !WALKIN?
                rspeak_(726);
                score_(0);
                //  moved to exit routine	CLOSE(DBCH)
                exit_();

                //  R58--	TOMB ROOM

                L58000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                i = 46;
                // !TOMB IS OPEN/CLOSED.
                if ((game.Objects.oflag2[oindex_1.tomb - 1] & OPENBT) != 0)
                {
                    i = 12;
                }
                MessageHandler.rspsub_(792, i);
                return ret_val;

                //  R59--	PUZZLE SIDE ROOM

                L59000:
                if (prsvec_1.prsa != vindex_1.lookw)
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
                rspeak_(i);
                // !DESCRIBE.
                return ret_val;

                //  R60--	PUZZLE ROOM

                L60000:
                if (prsvec_1.prsa != vindex_1.lookw)
                {
                    return ret_val;
                }
                // !LOOK?
                if (game.Flags.cpushf)
                {
                    goto L60100;
                }
                // !STARTED PUZZLE?
                rspeak_(868);
                // !NO, DESCRIBE.
                if ((game.Objects.oflag2[oindex_1.warni - 1] & TCHBT) != 0)
                {
                    rspeak_(869);
                }
                return ret_val;

                L60100:
                cpinfo_(880, game.Flags.cphere);
                // !DESCRIBE ROOM.
                return ret_val;
                */
            return true;
        }
    }
}