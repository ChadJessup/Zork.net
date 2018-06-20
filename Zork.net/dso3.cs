using System;

namespace Zork.Core
{
    public static class dso3
    {
        /// <summary>
        /// findxt_ - Find Exit from room.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="dir"></param>
        /// <param name="rm"></param>
        /// <returns></returns>
        public static bool FindExit(Game game, int dir, int rm)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            int i, xi;
            int xxxflg;

            ret_val = true;
            // !ASSUME WINS.
            xi = game.Rooms[rm - 1].Exit;
            // !FIND FIRST ENTRY.
            if (xi == 0)
            {
                goto L1000;
            }
            // !NO EXITS?

            L100:
            i = game.Exits.Travel[xi - 1];
            // !GET ENTRY.
            game.curxt_.xroom1 = i & xpars_.xrmask;
            // mask to 16-bits to get rid of sign extension problems with 32-bit ints

            xxxflg = ~xpars_.xlflag & 65535;
            game.curxt_.xtype = ((i & xxxflg) / xpars_.xfshft & xpars_.xfmask) + 1;
            switch (game.curxt_.xtype)
            {
                case 1: goto L110;
                case 2: goto L120;
                case 3: goto L130;
                case 4: goto L130;
            }
            // !BRANCH ON ENTRY.
            throw new InvalidOperationException();
            //bug_(10, game.curxt_.xtype);

            L130:
            game.curxt_.xobj = game.Exits.Travel[xi + 1] & xpars_.xrmask;
            game.curxt_.xactio = game.Exits.Travel[xi + 1] / xpars_.xashft;
            L120:
            game.curxt_.xstrng = game.Exits.Travel[xi];
            // !DOOR/CEXIT/NEXIT - STRING.
            L110:
            xi += xpars_.xelnt[game.curxt_.xtype - 1];
            // !ADVANCE TO NEXT ENTRY.
            if ((i & xpars_.xdmask) == dir)
            {
                return ret_val;
            }

            if ((i & xpars_.xlflag) == 0)
            {
                goto L100;
            }

            L1000:
            ret_val = false;
            // !YES, LOSE.
            return ret_val;
        } // findxt_

        // FWIM- FIND WHAT I MEAN
        public static int fwim_(Game game, int f1, int f2, int rm, int con, int adv, bool nocare)
        {
            int ret_val, i__1, i__2;
            int i, j;
            ret_val = 0;

            // !ASSUME NOTHING.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP
                if ((rm == 0 || game.Objects.oroom[i - 1] != rm) && (adv == 0 ||
                    game.Objects.oadv[i - 1] != adv) && (con == 0 || game.Objects.ocan[
                    i - 1] != con))
                {
                    goto L1000;
                }

                // OBJECT IS ON LIST... IS IT A MATCH?

                if ((game.Objects.oflag1[i - 1] & ObjectFlags.IsVisible) == 0)
                {
                    goto L1000;
                }

                if (!(nocare) & (game.Objects.oflag1[i - 1] & ObjectFlags.TAKEBT) == 0
                    || (game.Objects.oflag1[i - 1] & (ObjectFlags)f1) == 0
                    && (game.Objects.oflag2[i - 1] & (ObjectFlags2)f2) == 0)
                {
                    goto L500;
                }

                if (ret_val == 0)
                {
                    goto L400;
                }

                // !ALREADY GOT SOMETHING?
                ret_val = -ret_val;
                // !YES, AMBIGUOUS.
                return ret_val;

                L400:
                ret_val = i;
                // !NOTE MATCH.

                // DOES OBJECT CONTAIN A MATCH?

                L500:
                if ((game.Objects.oflag2[i - 1] & ObjectFlags2.IsOpen) == 0)
                {
                    goto L1000;
                }
                i__2 = game.Objects.Count;
                for (j = 1; j <= i__2; ++j)
                {
                    // !NO, SEARCH CONTENTS.
                    if (game.Objects.ocan[j - 1] != i
                        || (game.Objects.oflag1[j - 1] & ObjectFlags.IsVisible) == 0
                        || (game.Objects.oflag1[j - 1] & (ObjectFlags)f1) == 0
                        && (game.Objects.oflag2[j - 1] & (ObjectFlags2)f2) == 0)
                    {
                        goto L700;
                    }

                    if (ret_val == 0)
                    {
                        goto L600;
                    }
                    ret_val = -ret_val;
                    return ret_val;

                    L600:
                    ret_val = j;
                    L700:
                    ;
                }
                L1000:
                ;
            }
            return ret_val;
        }

        // YESNO- OBTAIN YES/NO ANSWER
        // 	YES-IS-TRUE=YESNO(QUESTION,YES-STRING,NO-STRING)
        public static bool yesno_(Game game, int q, int y, int n)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            char[] ans = new char[100];

            L100:
            MessageHandler.rspeak_(game, q);
            // !ASK
            //(void) fflush(stdout);
            //(void) fgets(ans, sizeof ans, stdin);
            MessageHandler.more_input();
            // !GET ANSWER
            if (ans[0] == 'Y' || ans[0] == 'y')
            {
                goto L200;
            }
            if (ans[0] == 'N' || ans[0] == 'n')
            {
                goto L300;
            }
            MessageHandler.rspeak_(game, 6);
            // !SCOLD.
            goto L100;

            L200:
            ret_val = true;
            // !YES,
            MessageHandler.rspeak_(game, y);
            // !OUT WITH IT.
            return ret_val;

            L300:
            ret_val = false;
            // !NO,
            MessageHandler.rspeak_(game, n);
            // !LIKEWISE.
            return ret_val;
        }
    }
}
