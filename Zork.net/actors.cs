using System;

namespace Zork.Core
{
    public static class Actors
    {
        // AAPPLI- APPLICABLES FOR ADVENTURERS
        public static bool aappli_(Game game, int ri)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            bool f;
            int i;

            // !IF ZERO, NO APP.
            if (ri == 0)
            {
                goto L10;
            }

            // !ASSUME WINS.
            ret_val = true;

            // !BRANCH ON ADV.
            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L2000;
            }

            throw new InvalidOperationException();
            //bug_(11, ri);


            // COMMON FALSE RETURN.
            L10:
            ret_val = false;
            return ret_val;

            // A1--	ROBOT.  PROCESS MOST COMMANDS GIVEN TO ROBOT.

            L1000:
            if (game.ParserVectors.prsa != (int)VerbIds.raisew
             || game.ParserVectors.prso != (int)ObjectIds.rcage)
            {
                goto L1200;
            }

            // !ROBOT RAISED CAGE.
            game.Clock.Flags[(int)ClockIndices.cevsph - 1] = false;

            // !RESET FOR PLAYER.
            game.Player.Winner= (int)ActorIds.Player;

            // !MOVE TO NEW ROOM.
            f = AdventurerHandler.moveto_(game, (int)RoomIds.cager, game.Player.Winner);

            // !INSTALL CAGE IN ROOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.cage, 567, (int)RoomIds.cager, 0, 0, game);

            // !INSTALL ROBOT IN ROOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.robot, 0, (int)RoomIds.cager, 0, 0, game);

            // !ALSO MOVE ROBOT/ADV.
            game.Adventurers.Rooms[(int)ActorIds.Robot - 1] = (int)RoomIds.cager;

            // !CAGE SOLVED.
            game.Flags.cagesf = true;
            game.Objects.oflag1[(int)ObjectIds.robot - 1] &= ~ObjectFlags.NDSCBT;
            game.Objects.oflag1[(int)ObjectIds.spher - 1] |=  ObjectFlags.IsTakeable;

            return ret_val;

            L1200:
            if (game.ParserVectors.prsa != (int)VerbIds.drinkw
                && game.ParserVectors.prsa != (int)VerbIds.eatw)
            {
                goto L1300;
            }

            // !EAT OR DRINK, JOKE.
            MessageHandler.rspeak_(game, 568);
            return ret_val;

            L1300:
            // !READ,
            if (game.ParserVectors.prsa != (int)VerbIds.Read)
            {
                goto L1400;
            }

            // !JOKE.
            MessageHandler.rspsub_(game, 569);
            return ret_val;

            L1400:
            if (game.ParserVectors.prsa == (int)VerbIds.Walk
             || game.ParserVectors.prsa == (int)VerbIds.takew
             || game.ParserVectors.prsa == (int)VerbIds.dropw
             || game.ParserVectors.prsa == (int)VerbIds.putw
             || game.ParserVectors.prsa == (int)VerbIds.pushw
             || game.ParserVectors.prsa == (int)VerbIds.throww
             || game.ParserVectors.prsa == (int)VerbIds.turnw
             || game.ParserVectors.prsa == (int)VerbIds.leapw)
            {
                goto L10;
            }

            // !JOKE.
            MessageHandler.rspsub_(game, 570);
            return ret_val;
            // AAPPLI, PAGE 3

            // A2--	MASTER.  PROCESS MOST COMMANDS GIVEN TO MASTER.

            L2000:
            if ((game.Objects.oflag2[(int)ObjectIds.qdoor - 1] & ObjectFlags2.IsOpen) != 0)
            {
                goto L2100;
            }

            // !NO MASTER YET.
            MessageHandler.rspsub_(game, 783);
            return ret_val;

            L2100:
            // !WALK?
            if (game.ParserVectors.prsa != (int)VerbIds.Walk)
            {
                goto L2200;
            }

            i = 784;
            // !ASSUME WONT.
            if (game.Player.Here == (int)RoomIds.scorr
                && (game.ParserVectors.prso == (int)XSearch.xnorth || game.ParserVectors.prso == (int)XSearch.xenter)
                || game.Player.Here == (int)RoomIds.ncorr
                && (game.ParserVectors.prso == (int)XSearch.xsouth || game.ParserVectors.prso == (int)XSearch.xenter))
            {
                i = 785;
            }

            MessageHandler.rspsub_(game, i);
            return ret_val;

            L2200:
            if (game.ParserVectors.prsa == (int)VerbIds.takew
                || game.ParserVectors.prsa == (int)VerbIds.dropw
                || game.ParserVectors.prsa == (int)VerbIds.putw
                || game.ParserVectors.prsa == (int)VerbIds.throww
                || game.ParserVectors.prsa == (int)VerbIds.pushw
                || game.ParserVectors.prsa == (int)VerbIds.turnw
                || game.ParserVectors.prsa == (int)VerbIds.spinw
                || game.ParserVectors.prsa == (int)VerbIds.trntow
                || game.ParserVectors.prsa == (int)VerbIds.follow
                || game.ParserVectors.prsa == (int)VerbIds.stayw
                || game.ParserVectors.prsa == (int)VerbIds.openw
                || game.ParserVectors.prsa == (int)VerbIds.closew
                || game.ParserVectors.prsa == (int)VerbIds.killw)
            {
                goto L10;
            }

            // !MASTER CANT DO IT.
            MessageHandler.rspsub_(game, 786);
            return ret_val;
        }

        // THIEFD-	INTERMOVE THIEF DEMON
        public static void thiefd_(Game game)
        {
            // System generated locals
            int i__1, i__2;

            // Local variables
            int i, j, nr;
            bool once;
            int rhere;

            // !SET UP DETAIL FLAG.
            once = false;
            // !INIT FLAG.
            L1025:
            // !VISIBLE POS.
            rhere = game.Objects.oroom[(int)ObjectIds.thief - 1];
            if (rhere != 0)
            {
                game.Hack.ThiefPosition = rhere;
            }

            if (game.Hack.ThiefPosition == game.Player.Here)
            {
                goto L1100;
            }

            // !THIEF IN WIN RM?
            if (game.Hack.ThiefPosition != (int)RoomIds.Treasure)
            {
                goto L1400;
            }

            // !THIEF NOT IN TREAS?

            // THIEF IS IN TREASURE ROOM, AND WINNER IS NOT.
            if (rhere == 0)
            {
                goto L1050;
            }

            // !VISIBLE?
            ObjectHandler.SetNewObjectStatus(ObjectIds.thief, 0, 0, 0, 0, game);
            // !YES, VANISH.
            rhere = 0;

            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.still, (int)RoomIds.Treasure)
                || game.Objects.oadv[(int)ObjectIds.still - 1] == -(int)ObjectIds.thief)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.still, 0, 0, (int)ObjectIds.thief, 0, game);
            }

            L1050:
            i__1 = -(int)ObjectIds.thief;
            i = dso4.RobAdventurer(game, i__1, game.Hack.ThiefPosition, 0, 0);

            // !DROP VALUABLES.
            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.Egg, game.Hack.ThiefPosition))
            {
                game.Objects.oflag2[(int)ObjectIds.Egg - 1] |= ObjectFlags2.IsOpen;
            }

            goto L1700;

            // THIEF AND WINNER IN SAME ROOM.

            L1100:
            if (game.Hack.ThiefPosition == (int)RoomIds.Treasure)
            {
                goto L1700;
            }

            // !IF TREAS ROOM, NOTHING.
            if ((game.Rooms[game.Hack.ThiefPosition - 1].Flags & RoomFlags.LIGHT) != 0)
            {
                goto L1400;
            }

            if (game.Hack.WasThiefIntroduced)
            {
                goto L1300;
            }

            // !THIEF ANNOUNCED?
            if (rhere != 0 || RoomHandler.prob_(game, 70, 70))
            {
                goto L1150;
            }

            // !IF INVIS AND 30%.
            if (game.Objects.ocan[(int)ObjectIds.still - 1] != (int)ObjectIds.thief)
            {
                goto L1700;
            }

            // !ABORT IF NO STILLETTO.
            ObjectHandler.SetNewObjectStatus(ObjectIds.thief, 583, game.Hack.ThiefPosition, 0, 0, game);
            // !INSERT THIEF INTO ROOM.
            // !THIEF IS ANNOUNCED.
            game.Hack.WasThiefIntroduced = true;
            return;

            L1150:
            if (rhere == 0 || (game.Objects.oflag2[(int)ObjectIds.thief - 1] & ObjectFlags2.FITEBT) == 0)
            {
                goto L1200;
            }

            if (dso4.IsVillianWinning(game, (int)ObjectIds.thief, game.Player.Winner))
            {
                goto L1175;
            }
            // !WINNING?
            ObjectHandler.SetNewObjectStatus(ObjectIds.thief, 584, 0, 0, 0, game);
            // !NO, VANISH THIEF.
            game.Objects.oflag2[(int)ObjectIds.thief - 1] &= ~ObjectFlags2.FITEBT;
            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.still, game.Hack.ThiefPosition)
                || game.Objects.oadv[(int)ObjectIds.still - 1] == -(int)ObjectIds.thief)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.still, 0, 0, (int)ObjectIds.thief, 0, game);
            }
            return;

            L1175:
            if (RoomHandler.prob_(game, 90, 90))
            {
                goto L1700;
            }
            // !90% CHANCE TO STAY.

            L1200:
            if (rhere == 0 || RoomHandler.prob_(game, 70, 70))
            {
                goto L1250;
            }
            // !IF VISIBLE AND 30%
            ObjectHandler.SetNewObjectStatus(ObjectIds.thief, 585, 0, 0, 0, game);
            // !VANISH THIEF.
            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.still, game.Hack.ThiefPosition) || game.Objects.oadv[(int)ObjectIds.still - 1] == -(int)ObjectIds.thief)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.still, 0, 0, (int)ObjectIds.thief, 0, game);
            }
            return;

            L1300:
            if (rhere == 0)
            {
                goto L1700;
            }
            // !ANNOUNCED.  VISIBLE?
            L1250:
            if (RoomHandler.prob_(game, 70, 70))
            {
                return;
            }
            // !70% CHANCE TO DO NOTHING.
            game.Hack.WasThiefIntroduced = true;
            i__1 = -(int)ObjectIds.thief;
            i__2 = -(int)ObjectIds.thief;
            nr = dso4.RobRoom(game, game.Hack.ThiefPosition, 100, 0, 0, i__1) + dso4.RobAdventurer(game, game.Player.Winner, 0, 0, i__2);
            i = 586;
            // !ROBBED EM.
            if (rhere != 0)
            {
                i = 588;
            }
            // !WAS HE VISIBLE?
            if (nr != 0)
            {
                ++i;
            }
            // !DID HE GET ANYTHING?
            ObjectHandler.SetNewObjectStatus(ObjectIds.thief, i, 0, 0, 0, game);
            // !VANISH THIEF.
            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.still, game.Hack.ThiefPosition) || game.Objects.oadv[
                (int)ObjectIds.still - 1] == -(int)ObjectIds.thief)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.still, 0, 0, (int)ObjectIds.thief, 0, game);
            }
            if (nr != 0 && !RoomHandler.IsRoomLit(game.Hack.ThiefPosition, game))
            {
                MessageHandler.rspsub_(game, 406);
            }
            rhere = 0;
            goto L1700;
            // !ONWARD.

            // NOT IN ADVENTURERS ROOM.

            L1400:
            ObjectHandler.SetNewObjectStatus(game, ObjectIds.thief, 0, 0, 0, 0);
            // !VANISH.
            rhere = 0;
            if (ObjectHandler.IsObjectInRoom(game, (int)ObjectIds.still, game.Hack.ThiefPosition)
                || game.Objects.oadv[(int)ObjectIds.still - 1] == -(int)ObjectIds.thief)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.still, 0, 0, (int)ObjectIds.thief, 0, game);
            }

            if ((game.Rooms[game.Hack.ThiefPosition - 1].Flags & RoomFlags.SEEN) == 0)
            {
                goto L1700;
            }

            i__1 = -(int)ObjectIds.thief;
            i = dso4.RobRoom(game, game.Hack.ThiefPosition, 75, 0, 0, i__1);
            // !ROB ROOM 75%.
            if (game.Hack.ThiefPosition < (int)RoomIds.maze1
                || game.Hack.ThiefPosition > (int)RoomIds.maz15
                || game.Player.Here < (int)RoomIds.maze1
                || game.Player.Here > (int)RoomIds.maz15)
            {
                goto L1500;
            }

            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !BOTH IN MAZE.
                if (!ObjectHandler.IsObjectInRoom(game, i, game.Hack.ThiefPosition)
                    || RoomHandler.prob_(game, 60, 60)
                    || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable) != (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable)
                {
                    goto L1450;
                }

                MessageHandler.rspsub_(game, 590, game.Objects.odesc2[i - 1]);
                // !TAKE OBJECT.
                if (RoomHandler.prob_(game, 40, 20))
                {
                    goto L1700;
                }

                i__2 = -(int)ObjectIds.thief;
                ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, 0, 0, i__2, game);
                // !MOST OF THE TIME.
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
                // !NOT IN MAZE.
                if (!ObjectHandler.IsObjectInRoom(game, i, game.Hack.ThiefPosition)
                    || game.Objects.otval[i - 1] != 0
                    || RoomHandler.prob_(game, 80, 60)
                    || (game.Objects.oflag1[i - 1] & (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable) != (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable)
                {
                    goto L1550;
                }
                i__2 = -(int)ObjectIds.thief;
                ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, 0, 0, i__2, game);
                game.Objects.oflag2[i - 1] |= ObjectFlags2.TCHBT;
                goto L1700;
                L1550:
                ;
            }

            // NOW MOVE TO NEW ROOM.

            L1700:
            if (game.Objects.oadv[(int)ObjectIds.Rope - 1] == -(int)ObjectIds.thief)
            {
                game.Flags.domef = false;
            }
            if (once)
            {
                goto L1800;
            }
            once = !once;
            L1750:
            --game.Hack.ThiefPosition;
            // !NEXT ROOM.
            if (game.Hack.ThiefPosition <= 0)
            {
                game.Hack.ThiefPosition = game.Rooms.Count;
            }
            if ((game.Rooms[game.Hack.ThiefPosition - 1].Flags & (int)RoomFlags.LAND + (int)RoomFlags.SACRED + RoomFlags.REND) != RoomFlags.LAND)
            {
                goto L1750;
            }
            game.Hack.WasThiefIntroduced = false;
            // !NOT ANNOUNCED.
            goto L1025;
            // !ONCE MORE.

            // ALL DONE.

            L1800:
            if (game.Hack.ThiefPosition == (int)RoomIds.Treasure)
            {
                return;
            }
            // !IN TREASURE ROOM?
            j = 591;
            // !NO, DROP STUFF.
            if (game.Hack.ThiefPosition != game.Player.Here)
            {
                j = 0;
            }
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (game.Objects.oadv[i - 1] != -(int)ObjectIds.thief
                    || RoomHandler.prob_(game, 70, 70)
                    || game.Objects.otval[i - 1] > 0)
                {
                    goto L1850;
                }

                ObjectHandler.SetNewObjectStatus((ObjectIds)i, j, game.Hack.ThiefPosition, 0, 0, game);
                j = 0;
                L1850:
                ;
            }
            return;

        }
    }
}