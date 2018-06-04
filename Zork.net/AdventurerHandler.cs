using Zork.Core.Object;

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

            /* Local variables */
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
    }
}