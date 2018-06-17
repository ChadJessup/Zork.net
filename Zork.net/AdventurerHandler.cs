using Zork.Core;
using Zork.Core.Object;
using Zork.Core.Room;

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
            if (adventurer != (int)AIndices.player)
            {
                i = 576;
            }
            // !IF NOT ME.
            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !LOOP
                if (game.Objects.oadv[j - 1] != adventurer || (game.Objects.oflag1[j - 1] & ObjectFlags.VISIBT) == 0)
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
            if (adventurer == (int)AIndices.player)
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
                if (game.Objects.oadv[j - 1] != adventurer || (game.Objects.oflag1[j - 1] &
                    ObjectFlags.VISIBT) == 0 || (game.Objects.oflag1[j - 1] &
                    ObjectFlags.TRANBT) == 0 && (game.Objects.oflag2[j - 1] &
                    ObjectFlags2.OPENBT) == 0)
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

            MessageHandler.Speak(desc, game);
            // !DESCRIBE SAD STATE.
            game.ParserVectors.prscon = 1;
            // !STOP PARSER.
            //            if (debug_1.dbgflg != 0)
            //            {
            //                return;
            //            }

            // !IF DBG, EXIT.
            game.Adventurers.Vehicles[game.Player.Winner - 1] = 0;

            // !GET RID OF VEHICLE.
            if (game.Player.Winner == (int)AIndices.player)
            {
                goto L100;
            }

            // !HIMSELF?
            MessageHandler.rspsub_(432, game.Objects.odesc2[game.Adventurers.Objects[game.Player.Winner - 1] - 1], game);
            // !NO, SAY WHO DIED.
            ObjectHandler.newsta_(game.Adventurers.Objects[game.Player.Winner - 1], 0, 0, 0, 0, game);
            // !SEND TO HYPER SPACE.
            return;

            L100:
            if (game.Flags.endgmf)
            {
                goto L900;
            }
            // !NO RECOVERY IN END GAME.

            // always exit for plopbot's purposes
            goto L1000;
            //    if (game.State.deaths >= 2) {
            //	goto L1000;
            //    }

            // !DEAD TWICE? KICK HIM OFF.
            if (!yesno_(10, 9, 8))
            {
                goto L1100;
            }
            // !CONTINUE?

            i__1 = game.Objects.Count;
            for (j = 1; j <= i__1; ++j)
            {
                // !TURN OFF FIGHTING.
                if (ObjectHandler.qhere_(j, game.Player.Here, game))
                {
                    game.Objects.oflag2[j - 1] &= ~ObjectFlags2.FITEBT;
                }
                // L50:
            }

            ++game.State.Deaths;
            AdventurerHandler.scrupd_(game, -10);
            // !CHARGE TEN POINTS.
            f = AdventurerHandler.moveto_(game, RoomIndices.fore1, game.Player.Winner);
            // !REPOSITION HIM.
            game.Flags.egyptf = true;
            // !RESTORE COFFIN.
            if (game.Objects.oadv[(int)ObjectIndices.coffi - 1] == game.Player.Winner)
            {
                ObjectHandler.newsta_(ObjectIndices.coffi, 0, RoomIndices.egypt, 0, 0, game);
            }

            game.Objects.oflag2[(int)ObjectIndices.door - 1] &= ~ObjectFlags2.TCHBT;
            game.Objects.oflag1[(int)ObjectIndices.robot - 1] = (game.Objects.oflag1[(int)ObjectIndices.robot - 1] | ObjectFlags.VISIBT) & ~ObjectFlags.NDSCBT;

            if (game.Objects.oroom[(int)ObjectIndices.lamp - 1] != 0 || game.Objects.oadv[(int)ObjectIndices.lamp - 1] == game.Player.Winner)
            {
                ObjectHandler.newsta_(ObjectIndices.lamp, 0, RoomIndices.lroom, 0, 0, game);
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

                ObjectHandler.newsta_(j, 0, rlist[i - 1], 0, 0, game);
                L200:
                ;
            }

            L400:
            i = game.Rooms.Count + 1;

            // !NOW MOVE VALUABLES.
            nonofl = (int)(RoomFlags.RAIR + (int)RoomFlags.RWATER + (int)RoomFlags.RSACRD + (int)RoomFlags.REND);
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
                if ((game.Rooms.RoomFlags[i - 1] & (RoomFlags)nonofl) != 0)
                {
                    goto L250;
                }

                ObjectHandler.newsta_(j, 0, i, 0, 0, game);
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
                if ((game.Rooms.RoomFlags[i - 1] & (RoomFlags)nonofl) != 0)
                {
                    goto L450;
                }

                ObjectHandler.newsta_(j, 0, i, 0, 0, game);
                L500:
                ;
            }
            return;

            // CAN'T OR WON'T CONTINUE, CLEAN UP AND EXIT.

            L900:
            MessageHandler.Speak(625, game);
            // !IN ENDGAME, LOSE.
            goto L1100;

            L1000:
            MessageHandler.Speak(7, game);
            // !INVOLUNTARY EXIT.
            L1100:
            AdventurerHandler.score_(game, false);
            // !TELL SCORE.
            //(void)fclose(dbfile);
            //exit_();

        }

        public static bool moveto_(Game game, RoomIndices nr, int who) => moveto_(game, (int)nr, who);
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
            lhr = (game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RLAND) != 0;
            lnr = (game.Rooms.RoomFlags[nr - 1] & RoomFlags.RLAND) != 0;
            j = game.Adventurers.Vehicles[who - 1];
            // !HIS VEHICLE

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
            if (j == (int)ObjectIndices.rboat)
            {
                bits = (int)RoomFlags.RWATER;
            }

            // !IN BOAT?
            if (j == (int)ObjectIndices.ballo)
            {
                bits = (int)RoomFlags.RAIR;
            }

            // !IN BALLOON?
            if (j == (int)ObjectIndices.bucke)
            {
                bits = (int)RoomFlags.RBUCK;
            }

            // !IN BUCKET?
            nlv = (game.Rooms.RoomFlags[nr - 1] & (RoomFlags)bits) == 0;
            if (!lnr && nlv || lnr && lhr && nlv && bits != (int)RoomFlags.RLAND)
            {
                goto L800;
            }

            L500:
            ret_val = true;
            // !MOVE SHOULD SUCCEED.
            if ((game.Rooms.RoomFlags[nr - 1] & RoomFlags.RMUNG) == 0)
            {
                goto L600;
            }

            MessageHandler.Speak(rrand[nr - 1], game);
            // !YES, TELL HOW.
            return ret_val;

            L600:
            if (who != (int)AIndices.player)
            {
                ObjectHandler.newsta_(game.Adventurers.Objects[who - 1], 0, nr, 0, 0, game);
            }

            if (j != 0)
            {
                ObjectHandler.newsta_(j, 0, nr, 0, 0, game);
            }

            game.Player.Here = nr;
            game.Adventurers.Rooms[who - 1] = game.Player.Here;
            AdventurerHandler.scrupd_(game, game.Rooms.RoomValues[nr - 1]);
            // !SCORE ROOM
            game.Rooms.RoomValues[nr - 1] = 0;
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
        /// <param name="flg"></param>
        public static void score_(Game game, bool flg)
        {
            int[] rank = { 20, 19, 18, 16, 12, 8, 4, 2, 1, 0 };
            int[] erank = { 20, 15, 10, 5, 0 };

            int i__1;

            int i, intAs;

            intAs = game.Adventurers.Scores[game.Player.Winner - 1];

            if (game.Flags.endgmf)
            {
                goto L60;
            }

            // !ENDGAME?
            MessageHandler.more_output(string.Empty);

            MessageHandler.more_output("Your score ");
            if (flg)
            {
                MessageHandler.more_output("would be");
            }
            else
            {
                MessageHandler.more_output("is");
            }

            MessageHandler.more_output($" {intAs} [total of {game.State.MaxScore} points], in {game.State.Moves} move");

            if (game.State.Moves != 1)
            {
                MessageHandler.more_output("s");
            }

            MessageHandler.more_output(".\n");

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
            MessageHandler.more_output(string.Empty);
            MessageHandler.more_output("Your score in the endgame ");

            if (flg)
            {
                MessageHandler.more_output("would be");
            }
            else
            {
                MessageHandler.more_output("is");
            }

            MessageHandler.more_output($" {game.State.egscor} [total of {game.State.egmxsc} points], in {game.State.Moves} moves.\n");

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
        /// scrupd_ - Update winner's score.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="n"></param>
        public static void scrupd_(Game game, int n)
        {
            if (game.Flags.endgmf)
            {
                goto L100;
            }

            // !ENDGAME?
            game.Adventurers.Scores[game.Player.Winner - 1] += n;
            // !UPDATE SCORE
            game.State.rwscor += n;
            // !UPDATE RAW SCORE
            if (game.Adventurers.Scores[game.Player.Winner - 1] < game.State.MaxScore - game.State.Deaths * 10)
            {
                return;
            }

            game.Clock.Flags[(int)ClockIndices.cevegh - 1] = true;

            // !TURN ON END GAME
            game.Clock.Ticks[(int)ClockIndices.cevegh - 1] = 15;
            return;

            L100:
            game.State.egscor += n;
            // !UPDATE EG SCORE.
        }
    }
}
