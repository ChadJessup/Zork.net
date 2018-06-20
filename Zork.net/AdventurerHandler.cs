using System;

namespace Zork.Core
{
    public static class AdventurerHandler
    {
        /// <summary>
        /// invent_ - PRINT CONTENTS OF ADVENTURER
        /// </summary>
        /// <param name="game"></param>
        public static void PrintContents(int adventurer, Game game)
        {
            int i__1;

            // Local variables
            int i, j;

            i = 575;
            // !FIRST LINE.
            if (adventurer != (int)ActorIds.Player)
            {
                i = 576;
            }

            // !IF NOT ME.
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !LOOP
                if (game.Objects.oadv[j - 1] != adventurer || (game.Objects.oflag1[j - 1] & ObjectFlags.IsVisible) == 0)
                {
                    goto L10;
                }

                MessageHandler.rspsub_(i, game.Objects.odesc2[game.Adventurers.Objects[adventurer - 1] - 1], game);
                i = 0;

                MessageHandler.rspsub_(502, game.Objects.odesc2[j - 1], game);
                L10:
                ;
            }

            if (i == 0)
            {
                goto L25;
            }

            // !ANY OBJECTS?
            if (adventurer == (int)ActorIds.Player)
            {
                MessageHandler.Speak(578, game);
            }

            // !NO, TELL HIM.
            return;

            L25:
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !LOOP.
                if (game.Objects.oadv[j - 1] != adventurer
                    || (game.Objects.oflag1[j - 1] & ObjectFlags.IsVisible) == 0
                    || (game.Objects.oflag1[j - 1] & ObjectFlags.IsTransparent) == 0
                    && (game.Objects.oflag2[j - 1] & ObjectFlags2.IsOpen) == 0)
                {
                    goto L100;
                }

                if (!ObjectHandler.IsObjectEmpty(j, game))
                {
                    ObjectHandler.PrintDescription(j, 573, game);
                }

                // !IF NOT EMPTY, LIST.
                L100:
                ;
            }
        }

        /// <summary>
        /// jigsup_ - You are dead
        /// </summary>
        /// <param name="desc"></param>
        public static void jigsup_(Game game, int desc)
        {
            int[] rlist = new int[] { 8, 6, 36, 35, 34, 4, 34, 6, 5 };

            int i__1;
            int nonofl;
            bool f;
            int i, j;

            // !DESCRIBE SAD STATE.
            MessageHandler.Speak(desc, game);

            // !STOP PARSER.
            game.ParserVectors.prscon = 1;

            // !IF DBG, EXIT.
            //if (debug_1.dbgflg != 0)
            //{
            //    return;
            //}

            game.Adventurers.Vehicles[game.Player.Winner - 1] = 0;

            // !GET RID OF VEHICLE.
            if (game.Player.Winner == (int)ActorIds.Player)
            {
                goto L100;
            }

            // !HIMSELF?
            // !NO, SAY WHO DIED.
            MessageHandler.rspsub_(432, game.Objects.odesc2[game.Adventurers.Objects[game.Player.Winner - 1] - 1], game);

            // !SEND TO HYPER SPACE.
            ObjectHandler.SetNewObjectStatus((ObjectIds)game.Adventurers.Objects[game.Player.Winner - 1], 0, 0, 0, 0, game);

            return;

            L100:
            // !NO RECOVERY IN END GAME.
            if (game.Flags.EndGame)
            {
                goto L900;
            }

            // always exit for plopbot's purposes
            goto L1000;
            //    if (game.State.Deaths >= 2) {
            //	goto L1000;
            //    }

            // !DEAD TWICE? KICK HIM OFF.
            if (!MessageHandler.AskYesNoQuestion(game, 10, 9, 8))
            {
                goto L1100;
            }
            // !CONTINUE?

            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !TURN OFF FIGHTING.
                if (ObjectHandler.IsObjectInRoom(j, game.Player.Here, game))
                {
                    game.Objects.oflag2[j - 1] &= ~ObjectFlags2.FITEBT;
                }
                // L50:
            }

            ++game.State.Deaths;
            // !CHARGE TEN POINTS.
            AdventurerHandler.ScoreUpdate(game, -10);
            // !REPOSITION HIM.
            f = AdventurerHandler.moveto_(game, RoomIds.Forest1, game.Player.Winner);
            // !RESTORE COFFIN.
            game.Flags.egyptf = true;
            if (game.Objects.oadv[(int)ObjectIds.Coffin - 1] == game.Player.Winner)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Coffin, 0, RoomIds.Egypt, 0, 0, game);
            }

            game.Objects.oflag2[(int)ObjectIds.door - 1] &= ~ObjectFlags2.TCHBT;
            game.Objects.oflag1[(int)ObjectIds.robot - 1] = (game.Objects.oflag1[(int)ObjectIds.robot - 1] | ObjectFlags.IsVisible) & ~ObjectFlags.NDSCBT;

            if (game.Objects.oroom[(int)ObjectIds.Lamp - 1] != 0 || game.Objects.oadv[(int)ObjectIds.Lamp - 1] == game.Player.Winner)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Lamp, 0, RoomIds.LivingRoom, 0, 0, game);
            }

            // NOW REDISTRIBUTE HIS VALUABLES AND OTHER BELONGINGS.

            // THE LAMP HAS BEEN PLACED IN THE LIVING ROOM.
            // THE FIRST 8 NON-VALUABLES ARE PLACED IN LOCATIONS AROUND THE HOUSE.
            // HIS VALUABLES ARE PLACED AT THE END OF THE MAZE.
            // REMAINING NON-VALUABLES ARE PLACED AT THE END OF THE MAZE.

            i = 1;
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !LOOP THRU OBJECTS.
                if (game.Objects.oadv[j - 1] != game.Player.Winner || game.Objects.otval[j - 1] != 0)
                {
                    goto L200;
                }

                ++i;
                if (i > 9)
                {
                    goto L400;
                }
                // !MOVE TO RANDOM LOCATIONS.

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, rlist[i - 1], 0, 0, game);
                L200:
                ;
            }

            L400:
            i = game.Rooms.Count + 1;

            // !NOW MOVE VALUABLES.
            nonofl = (int)(RoomFlags.AIR + (int)RoomFlags.WATER + (int)RoomFlags.SACRED + (int)RoomFlags.REND);
            // !DONT MOVE HERE.
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                if (game.Objects.oadv[j - 1] != game.Player.Winner || game.Objects.otval[j - 1] == 0)
                {
                    goto L300;
                }
                L250:
                --i;
                // !FIND NEXT ROOM.
                if ((game.Rooms[i - 1].Flags & (RoomFlags)nonofl) != 0)
                {
                    goto L250;
                }

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, i, 0, 0, game);
                // !YES, MOVE.
                L300:
                ;
            }

            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !NOW GET RID OF REMAINDER.
                if (game.Objects.oadv[j - 1] != game.Player.Winner)
                {
                    goto L500;
                }

                L450:
                --i;
                // !FIND NEXT ROOM.
                if ((game.Rooms[i - 1].Flags & (RoomFlags)nonofl) != 0)
                {
                    goto L450;
                }

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, i, 0, 0, game);
                L500:
                ;
            }
            return;

            // CAN'T OR WON'T CONTINUE, CLEAN UP AND EXIT.

            L900:
            // !IN ENDGAME, LOSE.
            MessageHandler.Speak(625, game);
            goto L1100;

            L1000:
            // !INVOLUNTARY EXIT.
            MessageHandler.Speak(7, game);

            L1100:
            // !TELL SCORE.
            AdventurerHandler.PrintScore(game, false);

            game.Exit();
        }

        public static bool moveto_(Game game, RoomIds nr, int who) => moveto_(game, (int)nr, who);
        public static bool moveto_(Game game, int nr, int who)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            int j;
            bool lhr;
            bool lnr, nlv;
            int bits;

            ret_val = false;
            // !ASSUME FAILS.
            lhr = (game.Rooms[game.Player.Here - 1].Flags & RoomFlags.LAND) != 0;
            lnr = (game.Rooms[nr - 1].Flags & RoomFlags.LAND) != 0;

            // !HIS VEHICLE
            j = game.Adventurers.Vehicles[who - 1];

            if (j != 0)
            {
                goto L100;
            }

            // !IN VEHICLE?
            if (lnr)
            {
                goto L500;
            }

            // !NO, GOING TO LAND?
            MessageHandler.Speak(427, game);
            // !CAN'T GO WITHOUT VEHICLE.
            return ret_val;

            L100:
            bits = 0;
            // !ASSUME NOWHERE.
            if (j == (int)ObjectIds.rboat)
            {
                bits = (int)RoomFlags.WATER;
            }

            // !IN BOAT?
            if (j == (int)ObjectIds.Balloon)
            {
                bits = (int)RoomFlags.AIR;
            }

            // !IN BALLOON?
            if (j == (int)ObjectIds.bucke)
            {
                bits = (int)RoomFlags.RBUCK;
            }

            // !IN BUCKET?
            nlv = (game.Rooms[nr - 1].Flags & (RoomFlags)bits) == 0;
            if (!lnr && nlv || lnr && lhr && nlv && bits != (int)RoomFlags.LAND)
            {
                goto L800;
            }

            L500:
            ret_val = true;
            // !MOVE SHOULD SUCCEED.
            if ((game.Rooms[nr - 1].Flags & RoomFlags.RMUNG) == 0)
            {
                goto L600;
            }

            MessageHandler.Speak(game.Rooms[nr - 1].Action, game);
            // !YES, TELL HOW.
            return ret_val;

            L600:
            if (who != (int)ActorIds.Player)
            {
                ObjectHandler.SetNewObjectStatus((ObjectIds)game.Adventurers.Objects[who - 1], 0, nr, 0, 0, game);
            }

            if (j != 0)
            {
                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, nr, 0, 0, game);
            }

            game.Player.Here = nr;
            game.Adventurers.Rooms[who - 1] = game.Player.Here;
            AdventurerHandler.ScoreUpdate(game, game.Rooms[nr - 1].Score);
            // !SCORE ROOM
            game.Rooms[nr - 1].Score = 0;
            return ret_val;

            L800:
            MessageHandler.rspsub_(428, game.Objects.odesc2[j - 1], game);
            // !WRONG VEHICLE.
            return ret_val;
        }

        /// <summary>
        /// score_ - Print out current score
        /// </summary>
        /// <param name="game"></param>
        /// <param name="isFutureTense"></param>
        public static void PrintScore(Game game, bool isFutureTense)
        {
            int[] rank = { 20, 19, 18, 16, 12, 8, 4, 2, 1, 0 };
            int[] erank = { 20, 15, 10, 5, 0 };

            int i__1;

            int i, intAs;

            intAs = game.Adventurers.Scores[game.Player.Winner - 1];

            if (game.Flags.EndGame)
            {
                goto L60;
            }

            // !ENDGAME?
            MessageHandler.more_output(game, string.Empty);

            MessageHandler.more_output(game, "Your score ");
            if (isFutureTense)
            {
                MessageHandler.more_output(game, "would be");
            }
            else
            {
                MessageHandler.more_output(game, "is");
            }

            MessageHandler.more_output(game, $" {intAs} [total of {game.State.MaxScore} points], in {game.State.Moves} move");

            if (game.State.Moves != 1)
            {
                MessageHandler.more_output(game, "s");
            }

            MessageHandler.more_output(game, $".{Environment.NewLine}");

            for (i = 1; i <= 10; ++i)
            {
                if (intAs * 20 / game.State.MaxScore >= rank[i - 1])
                {
                    goto L50;
                }
                // L10:
            }

            L50:
            i__1 = i + 484;
            MessageHandler.Speak(i__1, game);
            return;

            L60:
            MessageHandler.more_output(game, string.Empty);
            MessageHandler.more_output(game, "Your score in the endgame ");

            if (isFutureTense)
            {
                MessageHandler.more_output(game, "would be");
            }
            else
            {
                MessageHandler.more_output(game, "is");
            }

            MessageHandler.more_output(game, $" {game.State.egscor} [total of {game.State.egmxsc} points], in {game.State.Moves} moves.\n");

            for (i = 1; i <= 5; ++i)
            {
                if (game.State.egscor * 20 / game.State.egmxsc >= erank[i - 1])
                {
                    goto L80;
                }
                // L70:
            }

            L80:
            i__1 = i + 786;
            MessageHandler.Speak(i__1, game);
        }

        /// <summary>
        /// scrupd_ - Update Winner's score.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="incrementAmount"></param>
        public static void ScoreUpdate(Game game, int incrementAmount)
        {
            // !ENDGAME?
            if (game.Flags.EndGame)
            {
                goto L100;
            }

            // !UPDATE SCORE
            game.Adventurers.Scores[game.Player.Winner - 1] += incrementAmount;

            // !UPDATE RAW SCORE
            game.State.RawScore += incrementAmount;

            if (game.Adventurers.Scores[game.Player.Winner - 1] < game.State.MaxScore - game.State.Deaths * 10)
            {
                return;
            }

            // !TURN ON END GAME
            game.Clock.Flags[(int)ClockIndices.cevegh - 1] = true;

            game.Clock.Ticks[(int)ClockIndices.cevegh - 1] = 15;
            return;

            L100:
            // !UPDATE EG SCORE.
            game.State.egscor += incrementAmount;
        }
    }
}
