using System;

namespace Zork.Core
{
    public static class AdventurerHandler
    {
        /// <summary>
        /// invent_ - PRINT CONTENTS OF ADVENTURER
        /// </summary>
        /// <param name="game"></param>
        public static void PrintContents(ActorIds adventurerId, Game game)
        {
            ObjectIds i__1;

            // Local variables
            int i;
            ObjectIds j;

            i = 575;
            // !FIRST LINE.
            if (adventurerId != ActorIds.Player)
            {
                i = 576;
            }

            // !IF NOT ME.
            i__1 = (ObjectIds)game.Objects.Count;
            for (j = (ObjectIds)1; j <= i__1; ++j)
            {
                // !LOOP
                if (game.Objects[j].Adventurer != adventurerId || (game.Objects[j].Flag1 & ObjectFlags.IsVisible) == 0)
                {
                    goto L10;
                }

                MessageHandler.rspsub_(i, game.Objects[game.Adventurers[adventurerId].Object].Description2, game);
                i = 0;

                MessageHandler.rspsub_(502, game.Objects[j].Description2, game);
                L10:
                ;
            }

            if (i == 0)
            {
                goto L25;
            }

            // !ANY OBJECTS?
            if (adventurerId == ActorIds.Player)
            {
                MessageHandler.Speak(578, game);
            }

            // !NO, TELL HIM.
            return;

            L25:
            i__1 = (ObjectIds)game.Objects.Count;
            for (j = (ObjectIds)1; j <= i__1; ++j)
            {
                // !LOOP.
                if (game.Objects[j].Adventurer != adventurerId
                    || (game.Objects[j].Flag1 & ObjectFlags.IsVisible) == 0
                    || (game.Objects[j].Flag1 & ObjectFlags.IsTransparent) == 0
                    && (game.Objects[j].Flag2 & ObjectFlags2.IsOpen) == 0)
                {
                    goto L100;
                }

                if (!ObjectHandler.IsObjectEmpty((ObjectIds)j, game))
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
            RoomIds[] rlist = new RoomIds[]
            {
                (RoomIds)8,
                (RoomIds)6,
                (RoomIds)36,
                (RoomIds)35,
                (RoomIds)34,
                (RoomIds)4,
                (RoomIds)34,
                (RoomIds)6,
                (RoomIds)5
            };

            int i__1;
            int nonofl;
            bool f;
            ObjectIds j;
            RoomIds i;

            // !DESCRIBE SAD STATE.
            MessageHandler.Speak(desc, game);

            // !STOP PARSER.
            game.ParserVectors.prscon = 1;

            // !IF DBG, EXIT.
            //if (debug_1.dbgflg != 0)
            //{
            //    return;
            //}

            game.Adventurers[game.Player.Winner].Vehicle = 0;

            // !GET RID OF VEHICLE.
            if (game.Player.Winner == ActorIds.Player)
            {
                goto L100;
            }

            // !HIMSELF?
            // !NO, SAY WHO DIED.
            MessageHandler.rspsub_(432, game.Objects[game.Adventurers[game.Player.Winner].Object].Description2, game);

            // !SEND TO HYPER SPACE.
            ObjectHandler.SetNewObjectStatus(game.Adventurers[game.Player.Winner].Object, 0, 0, 0, 0, game);

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
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                // !TURN OFF FIGHTING.
                if (ObjectHandler.IsObjectInRoom((ObjectIds)j, game.Player.Here, game))
                {
                    game.Objects[j].Flag2 &= ~ObjectFlags2.FITEBT;
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
            if (game.Objects[ObjectIds.Coffin].Adventurer == game.Player.Winner)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Coffin, 0, RoomIds.Egypt, 0, 0, game);
            }

            game.Objects[ObjectIds.TrapDoor].Flag2 &= ~ObjectFlags2.WasTouched;
            game.Objects[ObjectIds.RobotObject].Flag1 = (game.Objects[ObjectIds.RobotObject].Flag1 | ObjectFlags.IsVisible) & ~ObjectFlags.HasNoDescription;

            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.Lamp, game).Id != 0 || game.Objects[ObjectIds.Lamp].Adventurer == game.Player.Winner)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Lamp, 0, RoomIds.LivingRoom, 0, 0, game);
            }

            // NOW REDISTRIBUTE HIS VALUABLES AND OTHER BELONGINGS.

            // THE LAMP HAS BEEN PLACED IN THE LIVING ROOM.
            // THE FIRST 8 NON-VALUABLES ARE PLACED IN LOCATIONS AROUND THE HOUSE.
            // HIS VALUABLES ARE PLACED AT THE END OF THE MAZE.
            // REMAINING NON-VALUABLES ARE PLACED AT THE END OF THE MAZE.

            i = (RoomIds)1;
            i__1 = game.Objects.Count;
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                // !LOOP THRU OBJECTS.
                if (game.Objects[j].Adventurer != game.Player.Winner || game.Objects[j].otval != 0)
                {
                    goto L200;
                }

                ++i;
                if ((int)i > 9)
                {
                    goto L400;
                }
                // !MOVE TO RANDOM LOCATIONS.

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, rlist[(int)i], 0, 0, game);
                L200:
                ;
            }

            L400:
            i = (RoomIds)game.Rooms.Count + 1;

            // !NOW MOVE VALUABLES.
            nonofl = (int)(RoomFlags.AIR + (int)RoomFlags.WATER + (int)RoomFlags.SACRED + (int)RoomFlags.REND);
            // !DONT MOVE HERE.
            i__1 = game.Objects.Count;
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                if (game.Objects[j].Adventurer != game.Player.Winner || game.Objects[j].otval == 0)
                {
                    goto L300;
                }
                L250:
                --i;
                // !FIND NEXT ROOM.
                if ((game.Rooms[i].Flags & (RoomFlags)nonofl) != 0)
                {
                    goto L250;
                }

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, (RoomIds)i, 0, 0, game);
                // !YES, MOVE.
                L300:
                ;
            }

            i__1 = game.Objects.Count;
            for (j = (ObjectIds)1; j <= (ObjectIds)i__1; ++j)
            {
                // !NOW GET RID OF REMAINDER.
                if (game.Objects[j].Adventurer != game.Player.Winner)
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

                ObjectHandler.SetNewObjectStatus((ObjectIds)j, 0, (RoomIds)i, 0, 0, game);
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

        public static bool moveto_(Game game, RoomIds nr, ActorIds who)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            ObjectIds j;
            bool lhr;
            bool lnr, nlv;
            int bits;

            ret_val = false;
            // !ASSUME FAILS.
            lhr = (game.Rooms[game.Player.Here].Flags & RoomFlags.LAND) != 0;
            lnr = (game.Rooms[nr].Flags & RoomFlags.LAND) != 0;

            // !HIS VEHICLE
            j = (ObjectIds)game.Adventurers[who].Vehicle;

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
            if (j == ObjectIds.rboat)
            {
                bits = (int)RoomFlags.WATER;
            }

            // !IN BOAT?
            if (j == ObjectIds.Balloon)
            {
                bits = (int)RoomFlags.AIR;
            }

            // !IN BALLOON?
            if (j == ObjectIds.Bucket)
            {
                bits = (int)RoomFlags.RBUCK;
            }

            // !IN BUCKET?
            nlv = (game.Rooms[nr].Flags & (RoomFlags)bits) == 0;
            if (!lnr && nlv || lnr && lhr && nlv && bits != (int)RoomFlags.LAND)
            {
                goto L800;
            }

            L500:
            ret_val = true;
            // !MOVE SHOULD SUCCEED.
            if ((game.Rooms[nr].Flags & RoomFlags.RMUNG) == 0)
            {
                goto L600;
            }

            MessageHandler.Speak(game.Rooms[nr].Action, game);
            // !YES, TELL HOW.
            return ret_val;

            L600:
            if (who != ActorIds.Player)
            {
                // TODO: chadj - fix this when needed.
                //ObjectHandler.SetNewObjectStatus((ObjectIds)game.OldAdventurers.Objects[(int)who - 1], 0, nr, 0, 0, game);
            }

            if (j != 0)
            {
                ObjectHandler.SetNewObjectStatus(j, 0, nr, 0, 0, game);
            }

            game.Player.Here = nr;
            game.Adventurers[who].CurrentRoom = game.Rooms[game.Player.Here];
            AdventurerHandler.ScoreUpdate(game, game.Rooms[nr].Score);
            // !SCORE ROOM
            game.Rooms[nr].Score = 0;
            return ret_val;

            L800:
            MessageHandler.rspsub_(428, game.Objects[j].Description2, game);
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

            intAs = game.Adventurers[game.Player.Winner].Score;

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
            game.Adventurers[game.Player.Winner].Score += incrementAmount;

            // !UPDATE RAW SCORE
            game.State.RawScore += incrementAmount;

            if (game.Adventurers[game.Player.Winner].Score < game.State.MaxScore - game.State.Deaths * 10)
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
