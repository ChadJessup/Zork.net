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
            if (game.ParserVectors.prsa != VerbIds.Raise
             || game.ParserVectors.DirectObject != ObjectIds.rcage)
            {
                goto L1200;
            }

            // !ROBOT RAISED CAGE.
            game.Clock.Flags[(int)ClockIndices.cevsph - 1] = false;

            // !RESET FOR PLAYER.
            game.Player.Winner = ActorIds.Player;

            // !MOVE TO NEW ROOM.
            f = AdventurerHandler.moveto_(game, RoomIds.cager, game.Player.Winner);

            // !INSTALL CAGE IN ROOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.cage, 567, game.Rooms[RoomIds.cager], 0, 0, game);

            // !INSTALL ROBOT IN ROOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Robot, 0, RoomIds.cager, 0, 0, game);

            // !ALSO MOVE ROBOT/ADV.
            game.Adventurers[ActorIds.Robot].CurrentRoom = game.Rooms[RoomIds.cager];

            // !CAGE SOLVED.
            game.Flags.cagesf = true;
            game.Objects[ObjectIds.Robot].Flag1 &= ~ObjectFlags.HasNoDescription;
            game.Objects[ObjectIds.Sphere].Flag1 |=  ObjectFlags.IsTakeable;

            return ret_val;

            L1200:
            if (game.ParserVectors.prsa != VerbIds.Drink
                && game.ParserVectors.prsa != VerbIds.Eat)
            {
                goto L1300;
            }

            // !EAT OR DRINK, JOKE.
            MessageHandler.rspeak_(game, 568);
            return ret_val;

            L1300:
            // !READ,
            if (game.ParserVectors.prsa != VerbIds.Read)
            {
                goto L1400;
            }

            // !JOKE.
            MessageHandler.rspsub_(game, 569);
            return ret_val;

            L1400:
            if (game.ParserVectors.prsa == VerbIds.Walk
             || game.ParserVectors.prsa == VerbIds.Take
             || game.ParserVectors.prsa == VerbIds.Drop
             || game.ParserVectors.prsa == VerbIds.Put
             || game.ParserVectors.prsa == VerbIds.Push
             || game.ParserVectors.prsa == VerbIds.Throw
             || game.ParserVectors.prsa == VerbIds.Turn
             || game.ParserVectors.prsa == VerbIds.Leap)
            {
                goto L10;
            }

            // !JOKE.
            MessageHandler.rspsub_(game, 570);
            return ret_val;
            // AAPPLI, PAGE 3

            // A2--	MASTER.  PROCESS MOST COMMANDS GIVEN TO MASTER.

            L2000:
            if ((game.Objects[ObjectIds.qdoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L2100;
            }

            // !NO MASTER YET.
            MessageHandler.rspsub_(game, 783);
            return ret_val;

            L2100:
            // !WALK?
            if (game.ParserVectors.prsa != VerbIds.Walk)
            {
                goto L2200;
            }

            i = 784;
            // !ASSUME WONT.
            if (game.Player.Here == RoomIds.scorr
                && (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xnorth || game.ParserVectors.DirectObject == (ObjectIds)XSearch.xenter)
                || game.Player.Here == RoomIds.ncorr
                && (game.ParserVectors.DirectObject == (ObjectIds)XSearch.xsouth || game.ParserVectors.DirectObject == (ObjectIds)XSearch.xenter))
            {
                i = 785;
            }

            MessageHandler.rspsub_(game, i);
            return ret_val;

            L2200:
            if (game.ParserVectors.prsa == VerbIds.Take
                || game.ParserVectors.prsa == VerbIds.Drop
                || game.ParserVectors.prsa == VerbIds.Put
                || game.ParserVectors.prsa == VerbIds.Throw
                || game.ParserVectors.prsa == VerbIds.Push
                || game.ParserVectors.prsa == VerbIds.Turn
                || game.ParserVectors.prsa == VerbIds.Spin
                || game.ParserVectors.prsa == VerbIds.trntow
                || game.ParserVectors.prsa == VerbIds.follow
                || game.ParserVectors.prsa == VerbIds.stayw
                || game.ParserVectors.prsa == VerbIds.Open
                || game.ParserVectors.prsa == VerbIds.Close
                || game.ParserVectors.prsa == VerbIds.Kill)
            {
                goto L10;
            }

            // !MASTER CANT DO IT.
            MessageHandler.rspsub_(game, 786);
            return ret_val;
        }

        /// <summary>
        /// thiefd_ - Intermove thief demon
        /// </summary>
        /// <param name="game"></param>
        public static void thiefd_(Game game)
        {
            int i__1, i__2;
            int j, nr;
            ObjectIds i;
            bool once;
            RoomIds rhere = RoomIds.cpuzz;

            // !SET UP DETAIL FLAG.
            once = false;
            // !INIT FLAG.
            L1025:
            // !VISIBLE POS.
            //rhere = RoomHandler.GetRoomThatContainsObject(ObjectIds.thief, game).Id;
            if (rhere != 0)
            {
                game.Hack.ThiefPosition = rhere;
            }

            if (game.Hack.ThiefPosition == game.Player.Here)
            {
                goto L1100;
            }

            // !THIEF IN WIN RM?
            if (game.Hack.ThiefPosition != RoomIds.Treasure)
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
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 0, null, 0, 0, game);
            // !YES, VANISH.
            rhere = 0;

            if (ObjectHandler.IsObjectInRoom(game, ObjectIds.Stilletto, RoomIds.Treasure)
                || game.Objects[ObjectIds.Stilletto].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, null, ObjectIds.Thief, 0, game);
            }

            L1050:
            i__1 = -(int)ObjectIds.Thief;
            i = (ObjectIds)dso4.RobAdventurer(game, (ActorIds)i__1, game.Hack.ThiefPosition, 0, 0);

            // !DROP VALUABLES.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Egg, game.Hack.ThiefPosition, game))
            {
                game.Objects[ObjectIds.Egg].Flag2 |= ObjectFlags2.IsOpen;
            }

            goto L1700;
            // THIEF AND WINNER IN SAME ROOM.

            L1100:
            // !IF TREAS ROOM, NOTHING.
            if (game.Hack.ThiefPosition == RoomIds.Treasure)
            {
                goto L1700;
            }

            if ((game.Rooms[game.Hack.ThiefPosition].Flags & RoomFlags.LIGHT) != 0)
            {
                goto L1400;
            }

            // !THIEF ANNOUNCED?
            if (game.Hack.WasThiefIntroduced)
            {
                goto L1300;
            }

            // !IF INVIS AND 30%.
            if (rhere != 0 || RoomHandler.prob_(game, 70, 70))
            {
                goto L1150;
            }

            // !ABORT IF NO STILLETTO.
            if (game.Objects[ObjectIds.Stilletto].Container != ObjectIds.Thief)
            {
                goto L1700;
            }

            // !INSERT THIEF INTO ROOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 583, game.Rooms[game.Hack.ThiefPosition], 0, 0, game);

            // !THIEF IS ANNOUNCED.
            game.Hack.WasThiefIntroduced = true;
            return;

            L1150:
            if (rhere == 0 || (game.Objects[ObjectIds.Thief].Flag2 & ObjectFlags2.IsFighting) == 0)
            {
                goto L1200;
            }

            if (dso4.IsVillianWinning(game, ObjectIds.Thief, game.Player.Winner))
            {
                goto L1175;
            }

            // !WINNING?
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 584, null, 0, 0, game);
            // !NO, VANISH THIEF.
            game.Objects[ObjectIds.Thief].Flag2 &= ~ObjectFlags2.IsFighting;
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Stilletto, game.Hack.ThiefPosition, game)
                || game.Objects[ObjectIds.Stilletto].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, null, ObjectIds.Thief, 0, game);
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
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 585, null, 0, 0, game);
            // !VANISH THIEF.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Stilletto, game.Hack.ThiefPosition, game) || game.Objects[ObjectIds.Stilletto].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, null, ObjectIds.Thief, 0, game);
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
            i__1 = -(int)ObjectIds.Thief;
            i__2 = -(int)ObjectIds.Thief;
            nr = dso4.RobRoom(game, game.Hack.ThiefPosition, 100, 0, 0, i__1) + dso4.RobAdventurer(game, game.Player.Winner, 0, 0, (ActorIds)i__2);
            i = (ObjectIds)586;
            // !ROBBED EM.
            if (rhere != 0)
            {
                i = (ObjectIds)588;
            }

            // !WAS HE VISIBLE?
            if (nr != 0)
            {
                ++i;
            }

            // !DID HE GET ANYTHING?
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, (int)i, null, 0, 0, game);
            // !VANISH THIEF.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Stilletto, game.Hack.ThiefPosition, game) || game.Objects[ObjectIds.Stilletto].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, null, ObjectIds.Thief, 0, game);
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
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 0, null, 0, 0, game);
            // !VANISH.
            rhere = 0;
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Stilletto, game.Hack.ThiefPosition, game) || game.Objects[ObjectIds.Stilletto].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, null, ObjectIds.Thief, 0, game);
            }

            if ((game.Rooms[game.Hack.ThiefPosition].Flags & RoomFlags.SEEN) == 0)
            {
                goto L1700;
            }

            i__1 = -(int)ObjectIds.Thief;
            i = (ObjectIds)dso4.RobRoom(game, game.Hack.ThiefPosition, 75, 0, 0, i__1);
            // !ROB ROOM 75%.
            if (game.Hack.ThiefPosition < RoomIds.maze1
                || game.Hack.ThiefPosition > RoomIds.maz15
                || game.Player.Here < RoomIds.maze1
                || game.Player.Here > RoomIds.maz15)
            {
                goto L1500;
            }

            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !BOTH IN MAZE.
                if (!ObjectHandler.IsObjectInRoom(i, game.Hack.ThiefPosition, game)
                    || RoomHandler.prob_(game, 60, 60)
                    || (game.Objects[i].Flag1 & (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable) != (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable)
                {
                    goto L1450;
                }

                MessageHandler.rspsub_(game, 590, game.Objects[i].Description2Id);
                // !TAKE OBJECT.
                if (RoomHandler.prob_(game, 40, 20))
                {
                    goto L1700;
                }

                i__2 = -(int)ObjectIds.Thief;
                ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, null, 0, (ActorIds)i__2, game);
                // !MOST OF THE TIME.
                game.Objects[i].Flag2 |= ObjectFlags2.WasTouched;
                goto L1700;
                L1450:
                ;
            }
            goto L1700;

            L1500:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !NOT IN MAZE.
                if (!ObjectHandler.IsObjectInRoom(i, game.Hack.ThiefPosition, game)
                    || game.Objects[i].otval != 0
                    || RoomHandler.prob_(game, 80, 60)
                    || (game.Objects[i].Flag1 & (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable) != (int)ObjectFlags.IsVisible + ObjectFlags.IsTakeable)
                {
                    goto L1550;
                }
                i__2 = -(int)ObjectIds.Thief;
                ObjectHandler.SetNewObjectStatus(i, 0, null, 0, (ActorIds)i__2, game);
                game.Objects[i].Flag2 |= ObjectFlags2.WasTouched;
                goto L1700;
                L1550:
                ;
            }

            // NOW MOVE TO NEW ROOM.

            L1700:
            if (game.Objects[ObjectIds.Rope].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
            {
                game.Flags.IsRopeTiedToRailingInDomeRoom = false;
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
                game.Hack.ThiefPosition = (RoomIds)game.Rooms.Count - 1;
            }

            if ((game.Rooms[game.Hack.ThiefPosition].Flags & (int)RoomFlags.LAND + (int)RoomFlags.SACRED + RoomFlags.REND) != RoomFlags.LAND)
            {
                goto L1750;
            }

            game.Hack.WasThiefIntroduced = false;
            // !NOT ANNOUNCED.
            goto L1025;
            // !ONCE MORE.

            // ALL DONE.

            L1800:
            if (game.Hack.ThiefPosition == RoomIds.Treasure)
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
            for (i = (ObjectIds)0; i < (ObjectIds)i__1; ++i)
            {
                if (game.Objects[i].Adventurer != (ActorIds)(-(int)ObjectIds.Thief)
                    || RoomHandler.prob_(game, 70, 70)
                    || game.Objects[i].otval > 0)
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