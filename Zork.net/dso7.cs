using System;

namespace Zork.Core
{
    public static class dso7
    {
        // ENCRYP--	ENCRYPT PASSWORD
        public static void encryp_(Game game, char[] inw, char[] outw)
        {
            string keyw = "ECORMS";

            int i;
            int j;
            int[] uinw = new int[10];
            int usum;
            char[] ukeyw = new char[1 * 6];
            int uinws, ukeyws;

            // Parameter adjustments
            //--outw;
            //--inw;

            // Function Body

            uinws = 0;
            // !UNBIASED INW SUM.
            ukeyws = 0;
            // !UNBIASED KEYW SUM.
            j = 1;
            // !POINTER IN KEYWORD.
            for (i = 1; i <= 6; ++i)
            {
                // !UNBIAS, COMPUTE SUMS.
                ukeyw[i - 1] = (char)(keyw[i - 1] - 64);
                if (inw[j] <= '@')
                {
                    j = 1;
                }
                uinw[i - 1] = inw[j] - 64;
                ukeyws += ukeyw[i - 1];
                uinws += uinw[i - 1];
                ++j;
                // L100:
            }

            usum = uinws % 8 + (ukeyws % 8 << 3);
            // !COMPUTE MASK.
            for (i = 1; i <= 6; ++i)
            {
                j = (uinw[i - 1] ^ ukeyw[i - 1] ^ usum) & 31;
                usum = (usum + 1) % 32;
                if (j > 26)
                {
                    j %= 26;
                }

                //outw[i] = (Math.Max(1, j) + 64);
                // L200:
            }
        }

        // CPGOTO--	MOVE TO NEXT STATE IN PUZZLE ROOM
        public static void cpgoto_(Game game, int st)
        {
            int i__1, i__2;
            int i;

            game.Rooms[(int)RoomIds.cpuzz - 1].Flags &= ~RoomFlags.SEEN;
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !RELOCATE OBJECTS.
                if (game.Objects.oroom[i - 1] == (int)RoomIds.cpuzz && (game.Objects.oflag2[i - 1] & (int)ObjectFlags2.ACTRBT + ObjectFlags2.VILLBT) == 0)
                {
                    i__2 = game.Switches.cphere * game.hyper_.hfactr;
                    ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, (RoomIds)i__2, 0, 0, game);
                }

                if (game.Objects.oroom[i - 1] == st * game.hyper_.hfactr)
                {
                    ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, RoomIds.cpuzz, 0, 0, game);
                }
                // L100:
            }
            game.Switches.cphere = st;
        }

        // CPINFO--	DESCRIBE PUZZLE ROOM
        public static void cpinfo_(Game game, int rmk, int st)
        {
            int[] dgmoft = { -9, -8, -7, -1, 1, 7, 8, 9 };
            string pict = "SSS M";

            int i, j, k, l;
            char[] dgm = new char[1 * 8];

            MessageHandler.rspeak_(game, rmk);
            for (i = 1; i <= 8; ++i)
            {
                j = dgmoft[i - 1];
                dgm[i - 1] = pict[PuzzleHandler.cpvec[st + j - 1] + 3];
                // !GET PICTURE ELEMENT.
                if (Math.Abs(j) == 1 || Math.Abs(j) == 8)
                {
                    goto L100;
                }
                k = 8;
                if (j < 0)
                {
                    k = -8;
                }
                // !GET ORTHO DIR.
                l = j - k;
                if (PuzzleHandler.cpvec[st + k - 1] != 0 && PuzzleHandler.cpvec[st + l - 1] !=
                    0)
                {
                    dgm[i - 1] = '?';
                }
                L100:
                ;
            }

            // more_output(NULL);
            // game.WriteOutput("       |%c%c %c%c %c%c|\n", dgm[0], dgm[0], dgm[1], dgm[1], dgm[2], dgm[2]);
            // more_output(NULL);
            // game.WriteOutput(" West  |%c%c .. %c%c| East\n", dgm[3], dgm[3], dgm[4], dgm[4]);
            // more_output(NULL);
            // game.WriteOutput("       |%c%c %c%c %c%c|\n", dgm[5], dgm[5], dgm[6], dgm[6], dgm[7], dgm[7]);

            if (st == 10)
            {
                MessageHandler.rspeak_(game, 870);
            }
            // !AT HOLE?
            if (st == 37)
            {
                MessageHandler.rspeak_(game, 871);
            }
            // !AT NICHE?
            i = 872;
            // !DOOR OPEN?
            if (game.Flags.cpoutf)
            {
                i = 873;
            }

            if (st == 52)
            {
                MessageHandler.rspeak_(game, i);
            }

            // !AT DOOR?
            if (PuzzleHandler.cpvec[st] == -2)
            {
                MessageHandler.rspeak_(game, 874);
            }

            // !EAST LADDER?
            if (PuzzleHandler.cpvec[st - 2] == -3)
            {
                MessageHandler.rspeak_(game, 875);
            }
            // !WEST LADDER?
        }
    }
}