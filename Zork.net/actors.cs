using System;
using Zork.Core.Room;

namespace Zork.Core
{
    public static class actors
    {
        /* AAPPLI- APPLICABLES FOR ADVENTURERS */
        public static bool aappli_(Game game, int ri)
        {
            /* System generated locals */
            bool ret_val;

            /* Local variables */
            bool f;
            int i;

            if (ri == 0)
            {
                goto L10;
            }
            /* 						!IF ZERO, NO APP. */
            ret_val = true;
            /* 						!ASSUME WINS. */
            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L2000;
            }
            /* 						!BRANCH ON ADV. */
            throw new InvalidOperationException();
            //bug_(11, ri);

            /* COMMON FALSE RETURN. */

            L10:
            ret_val = false;
            return ret_val;

            /* A1--	ROBOT.  PROCESS MOST COMMANDS GIVEN TO ROBOT. */

            L1000:
            if (game.ParserVectors.prsa != (int)VIndices.raisew || game.ParserVectors.prso != (int)ObjectIndices.rcage)
            {

                goto L1200;
            }
            game.Clock.Flags[(int)ClockIndices.cevsph - 1] = false;
            /* 						!ROBOT RAISED CAGE. */
            game.Player.Winner= (int)AIndices.player;
            /* 						!RESET FOR PLAYER. */
            f = AdventurerHandler.moveto_(game, (int)RoomIndices.cager, game.Player.Winner);
            /* 						!MOVE TO NEW ROOM. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.cage, 567, (int)RoomIndices.cager, 0, 0);
            /* 						!INSTALL CAGE IN ROOM. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.robot, 0, (int)RoomIndices.cager, 0, 0);
            /* 						!INSTALL ROBOT IN ROOM. */
            game.Adventurers.Rooms[(int)AIndices.arobot - 1] = (int)RoomIndices.cager;
            /* 						!ALSO MOVE ROBOT/ADV. */
            game.Flags.cagesf = true;
            /* 						!CAGE SOLVED. */
            game.Objects.oflag1[(int)ObjectIndices.robot - 1] &= ~ObjectFlags.NDSCBT;
            game.Objects.oflag1[(int)ObjectIndices.spher - 1] |=  ObjectFlags.TAKEBT;
            return ret_val;

            L1200:
            if (game.ParserVectors.prsa != (int)VIndices.drinkw && game.ParserVectors.prsa != (int)VIndices.eatw)
            {
                goto L1300;
            }

            MessageHandler.rspeak_(game, 568);
            /* 						!EAT OR DRINK, JOKE. */
            return ret_val;

            L1300:
            if (game.ParserVectors.prsa != (int)VIndices.readw)
            {
                goto L1400;
            }
            /* 						!READ, */
            MessageHandler.rspsub_(game, 569);
            /* 						!JOKE. */
            return ret_val;

            L1400:
            if (game.ParserVectors.prsa == (int)VIndices.walkw || game.ParserVectors.prsa == (int)VIndices.takew ||
                game.ParserVectors.prsa == (int)VIndices.dropw || game.ParserVectors.prsa == (int)VIndices.putw
                || game.ParserVectors.prsa == (int)VIndices.pushw || game.ParserVectors.prsa ==
                (int)VIndices.throww || game.ParserVectors.prsa == (int)VIndices.turnw ||
                game.ParserVectors.prsa == (int)VIndices.leapw)
            {
                goto L10;
            }
            MessageHandler.rspsub_(game, 570);
            /* 						!JOKE. */
            return ret_val;
            /* AAPPLI, PAGE 3 */

            /* A2--	MASTER.  PROCESS MOST COMMANDS GIVEN TO MASTER. */

            L2000:
            if ((game.Objects.oflag2[(int)ObjectIndices.qdoor - 1] & ObjectFlags2.OPENBT) != 0)
            {
                goto L2100;
            }
            MessageHandler.rspsub_(game, 783);
            /* 						!NO MASTER YET. */
            return ret_val;

            L2100:
            if (game.ParserVectors.prsa != (int)VIndices.walkw)
            {
                goto L2200;
            }
            /* 						!WALK? */
            i = 784;
            /* 						!ASSUME WONT. */
            if (game.Player.Here == (int)RoomIndices.scorr && (game.ParserVectors.prso == (int)XSearch.xnorth ||
                game.ParserVectors.prso == (int)XSearch.xenter) || game.Player.Here == (int)RoomIndices.ncorr
                && (game.ParserVectors.prso == (int)XSearch.xsouth || game.ParserVectors.prso ==
                (int)XSearch.xenter))
            {
                i = 785;
            }
            MessageHandler.rspsub_(game, i);
            return ret_val;

            L2200:
            if (game.ParserVectors.prsa == (int)VIndices.takew || game.ParserVectors.prsa == (int)VIndices.dropw ||
                game.ParserVectors.prsa == (int)VIndices.putw || game.ParserVectors.prsa ==
                (int)VIndices.throww || game.ParserVectors.prsa == (int)VIndices.pushw ||
                game.ParserVectors.prsa == (int)VIndices.turnw || game.ParserVectors.prsa ==
                (int)VIndices.spinw || game.ParserVectors.prsa == (int)VIndices.trntow ||
                game.ParserVectors.prsa == (int)VIndices.follow || game.ParserVectors.prsa ==
                (int)VIndices.stayw || game.ParserVectors.prsa == (int)VIndices.openw ||
                game.ParserVectors.prsa == (int)VIndices.closew || game.ParserVectors.prsa ==
                (int)VIndices.killw)
            {
                goto L10;
            }
            MessageHandler.rspsub_(game, 786);
            /* 						!MASTER CANT DO IT. */
            return ret_val;

        } /* aappli_ */

        /* THIEFD-	INTERMOVE THIEF DEMON */
        public static void thiefd_(Game game)
        {
            /* System generated locals */
            int i__1, i__2;

            /* Local variables */
            int i, j, nr;
            bool once;
            int rhere;

            /* 						!SET UP DETAIL FLAG. */
            once = false;
            /* 						!INIT FLAG. */
            L1025:
            rhere = game.Objects.oroom[(int)ObjectIndices.thief - 1];
            /* 						!VISIBLE POS. */
            if (rhere != 0)
            {
                game.Hack.thfpos = rhere;
            }

            if (game.Hack.thfpos == game.Player.Here)
            {
                goto L1100;
            }
            /* 						!THIEF IN WIN RM? */
            if (game.Hack.thfpos != (int)RoomIndices.treas)
            {
                goto L1400;
            }
            /* 						!THIEF NOT IN TREAS? */

            /* THIEF IS IN TREASURE ROOM, AND WINNER IS NOT. */

            if (rhere == 0)
            {
                goto L1050;
            }
            /* 						!VISIBLE? */
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, 0, 0, 0, 0);
            /* 						!YES, VANISH. */
            rhere = 0;
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, (int)RoomIndices.treas) || game.Objects.oadv[
                (int)ObjectIndices.still - 1] == -(int)ObjectIndices.thief)
            {
                ObjectHandler.newsta_(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            }
            L1050:
            i__1 = -(int)ObjectIndices.thief;
            i = dso4.robadv_(game, i__1, game.Hack.thfpos, 0, 0);
            /* 						!DROP VALUABLES. */
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.egg, game.Hack.thfpos))
            {
                game.Objects.oflag2[(int)ObjectIndices.egg - 1] |= ObjectFlags2.OPENBT;
            }
            goto L1700;

            /* THIEF AND WINNER IN SAME ROOM. */

            L1100:
            if (game.Hack.thfpos == (int)RoomIndices.treas)
            {
                goto L1700;
            }
            /* 						!IF TREAS ROOM, NOTHING. */
            if ((game.Rooms.RoomFlags[game.Hack.thfpos - 1] & RoomFlags.RLIGHT) != 0)
            {
                goto L1400;
            }
            if (game.Hack.thfflg)
            {
                goto L1300;
            }
            /* 						!THIEF ANNOUNCED? */
            if (rhere != 0 || RoomHandler.prob_(game, 70, 70))
            {
                goto L1150;
            }
            /* 						!IF INVIS AND 30%. */
            if (game.Objects.ocan[(int)ObjectIndices.still - 1] != (int)ObjectIndices.thief)
            {
                goto L1700;
            }
            /* 						!ABORT IF NO STILLETTO. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, 583, game.Hack.thfpos, 0, 0);
            /* 						!INSERT THIEF INTO ROOM. */
            game.Hack.thfflg = true;
            /* 						!THIEF IS ANNOUNCED. */
            return;

            L1150:
            if (rhere == 0 || (game.Objects.oflag2[(int)ObjectIndices.thief - 1] & ObjectFlags2.FITEBT)
                == 0)
            {
                goto L1200;
            }
            if (dso4.winnin_(game, (int)ObjectIndices.thief, game.Player.Winner))
            {
                goto L1175;
            }
            /* 						!WINNING? */
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, 584, 0, 0, 0);
            /* 						!NO, VANISH THIEF. */
            game.Objects.oflag2[(int)ObjectIndices.thief - 1] &= ~ObjectFlags2.FITEBT;
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, game.Hack.thfpos)
                || game.Objects.oadv[(int)ObjectIndices.still - 1] == -(int)ObjectIndices.thief)
            {
                ObjectHandler.newsta_(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            }
            return;

            L1175:
            if (RoomHandler.prob_(game, 90, 90))
            {
                goto L1700;
            }
            /* 						!90% CHANCE TO STAY. */

            L1200:
            if (rhere == 0 || RoomHandler.prob_(game, 70, 70))
            {
                goto L1250;
            }
            /* 						!IF VISIBLE AND 30% */
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, 585, 0, 0, 0);
            /* 						!VANISH THIEF. */
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, game.Hack.thfpos) || game.Objects.oadv[
                (int)ObjectIndices.still - 1] == -(int)ObjectIndices.thief)
            {
                ObjectHandler.newsta_(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            }
            return;

            L1300:
            if (rhere == 0)
            {
                goto L1700;
            }
            /* 						!ANNOUNCED.  VISIBLE? */
            L1250:
            if (RoomHandler.prob_(game, 70, 70))
            {
                return;
            }
            /* 						!70% CHANCE TO DO NOTHING. */
            game.Hack.thfflg = true;
            i__1 = -(int)ObjectIndices.thief;
            i__2 = -(int)ObjectIndices.thief;
            nr = dso4.robrm_(game, game.Hack.thfpos, 100, 0, 0, i__1) + dso4.robadv_(game, game.Player.Winner, 0, 0, i__2);
            i = 586;
            /* 						!ROBBED EM. */
            if (rhere != 0)
            {
                i = 588;
            }
            /* 						!WAS HE VISIBLE? */
            if (nr != 0)
            {
                ++i;
            }
            /* 						!DID HE GET ANYTHING? */
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, i, 0, 0, 0);
            /* 						!VANISH THIEF. */
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, game.Hack.thfpos) || game.Objects.oadv[
                (int)ObjectIndices.still - 1] == -(int)ObjectIndices.thief)
            {
                ObjectHandler.newsta_(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            }
            if (nr != 0 && !RoomHandler.IsRoomLit(game.Hack.thfpos, game))
            {
                MessageHandler.rspsub_(game, 406);
            }
            rhere = 0;
            goto L1700;
            /* 						!ONWARD. */

            /* NOT IN ADVENTURERS ROOM. */

            L1400:
            ObjectHandler.newsta_(game, (int)ObjectIndices.thief, 0, 0, 0, 0);
            /* 						!VANISH. */
            rhere = 0;
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, game.Hack.thfpos) || game.Objects.oadv[
                (int)ObjectIndices.still - 1] == -(int)ObjectIndices.thief)
            {
                ObjectHandler.newsta_(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            }
            if ((game.Rooms.RoomFlags[game.Hack.thfpos - 1] & RoomFlags.RSEEN) == 0)
            {
                goto L1700;
            }

            i__1 = -(int)ObjectIndices.thief;
            i = dso4.robrm_(game, game.Hack.thfpos, 75, 0, 0, i__1);
            /* 						!ROB ROOM 75%. */
            if (game.Hack.thfpos < (int)RoomIndices.maze1 || game.Hack.thfpos > (int)RoomIndices.maz15 ||
                game.Player.Here < (int)RoomIndices.maze1 || game.Player.Here > (int)RoomIndices.maz15)
            {
                goto L1500;
            }
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!BOTH IN MAZE. */
                if (!ObjectHandler.qhere_(game, i, game.Hack.thfpos)
                    || RoomHandler.prob_(game, 60, 60)
                    || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.VISIBT + ObjectFlags.TAKEBT) != (int)ObjectFlags.VISIBT + ObjectFlags.TAKEBT)
                {
                    goto L1450;
                }

                MessageHandler.rspsub_(game, 590, game.Objects.odesc2[i - 1]);
                /* 						!TAKE OBJECT. */
                if (RoomHandler.prob_(game, 40, 20))
                {
                    goto L1700;
                }
                i__2 = -(int)ObjectIndices.thief;
                ObjectHandler.newsta_(game, i, 0, 0, 0, i__2);
                /* 						!MOST OF THE TIME. */
                game.Objects.oflag2[i - 1] |= ObjectFlags2.TCHBT;
                goto L1700;
                L1450:
                ;
            }
            goto L1700;

            L1500:
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!NOT IN MAZE. */
                if (!ObjectHandler.qhere_(game, i, game.Hack.thfpos)
                    || game.Objects.otval[i - 1] != 0
                    || RoomHandler.prob_(game, 80, 60)
                    || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.VISIBT + ObjectFlags.TAKEBT) != (int)ObjectFlags.VISIBT + ObjectFlags.TAKEBT)
                {
                    goto L1550;
                }
                i__2 = -(int)ObjectIndices.thief;
                ObjectHandler.newsta_(game, i, 0, 0, 0, i__2);
                game.Objects.oflag2[i - 1] |= ObjectFlags2.TCHBT;
                goto L1700;
                L1550:
                ;
            }

            /* NOW MOVE TO NEW ROOM. */

            L1700:
            if (game.Objects.oadv[(int)ObjectIndices.rope - 1] == -(int)ObjectIndices.thief)
            {
                game.Flags.domef = false;
            }
            if (once)
            {
                goto L1800;
            }
            once = !once;
            L1750:
            --game.Hack.thfpos;
            /* 						!NEXT ROOM. */
            if (game.Hack.thfpos <= 0)
            {
                game.Hack.thfpos = game.Rooms.Count;
            }
            if ((game.Rooms.RoomFlags[game.Hack.thfpos - 1] & (int)RoomFlags.RLAND + (int)RoomFlags.RSACRD + RoomFlags.REND) != RoomFlags.RLAND)
            {
                goto L1750;
            }
            game.Hack.thfflg = false;
            /* 						!NOT ANNOUNCED. */
            goto L1025;
            /* 						!ONCE MORE. */

            /* ALL DONE. */

            L1800:
            if (game.Hack.thfpos == (int)RoomIndices.treas)
            {
                return;
            }
            /* 						!IN TREASURE ROOM? */
            j = 591;
            /* 						!NO, DROP STUFF. */
            if (game.Hack.thfpos != game.Player.Here)
            {
                j = 0;
            }
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (game.Objects.oadv[i - 1] != -(int)ObjectIndices.thief
                    || RoomHandler.prob_(game, 70, 70)
                    || game.Objects.otval[i - 1] > 0)
                {
                    goto L1850;
                }

                ObjectHandler.newsta_(game, i, j, game.Hack.thfpos, 0, 0);
                j = 0;
                L1850:
                ;
            }
            return;

        }
    }
}