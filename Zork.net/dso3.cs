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
        public static bool FindExit(Game game, int dir, RoomIds rm)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            int i, xi;
            int xxxflg;

            ret_val = true;
            // !ASSUME WINS.
            // !FIND FIRST ENTRY.
            xi = game.Rooms[rm].Exit;

            // !NO EXITS?
            if (xi == 0)
            {
                goto L1000;
            }

            GETENTRY:
            // !GET ENTRY.
            i = game.Exits.Travel[xi - 1];

            game.curxt_.xroom1 = (RoomIds)(i & xpars_.xrmask);

            // mask to 16-bits to get rid of sign extension problems with 32-bit ints
            xxxflg = ~xpars_.xlflag & 65535;

            game.curxt_.xtype = ((i & xxxflg) / xpars_.xfshft & xpars_.xfmask) + 1;

            // !BRANCH ON ENTRY.
            switch (game.curxt_.xtype)
            {
                case 1: goto NEXTENTRY;
                case 2: goto L120;
                case 3: goto L130;
                case 4: goto L130;
            }

            throw new InvalidOperationException();
            //bug_(10, game.curxt_.xtype);

            L130:
            game.curxt_.xobj = (ObjectIds)(int)(game.Exits.Travel[xi + 1] & xpars_.xrmask);
            game.curxt_.xactio = game.Exits.Travel[xi + 1] / xpars_.xashft;

            L120:
            // !DOOR/CEXIT/NEXIT - STRING.
            game.curxt_.xstrng = game.Exits.Travel[xi];

            NEXTENTRY:
            // !ADVANCE TO NEXT ENTRY.
            xi += xpars_.xelnt[game.curxt_.xtype - 1];
            if ((i & xpars_.xdmask) == dir)
            {
                return ret_val;
            }

            if ((i & xpars_.xlflag) == 0)
            {
                goto GETENTRY;
            }

            L1000:
            // !YES, LOSE.
            ret_val = false;
            return ret_val;
        }

        /// <summary>
        /// fwim_ - Find what I mean.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="rm"></param>
        /// <param name="con"></param>
        /// <param name="adv"></param>
        /// <param name="nocare"></param>
        /// <returns></returns>
        public static int FindWhatIMean(Game game, int f1, int f2, RoomIds rm, ObjectIds con, ActorIds adv, bool nocare)
        {
            int ret_val, i__1, i__2;
            ObjectIds i, j;
            ret_val = 0;

            // !ASSUME NOTHING.
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !LOOP
                if ((rm == 0 || RoomHandler.GetRoomThatContainsObject(i, game).Id != rm)
                    && (adv == 0 || game.Objects[i].Adventurer != adv)
                    && (con == 0 || game.Objects[i].Container != con))
                {
                    goto L1000;
                }

                // OBJECT IS ON LIST... IS IT A MATCH?

                if ((game.Objects[i].Flag1 & ObjectFlags.IsVisible) == 0)
                {
                    goto L1000;
                }

                if (!(nocare) & (game.Objects[i].Flag1 & ObjectFlags.IsTakeable) == 0
                    || (game.Objects[i].Flag1 & (ObjectFlags)f1) == 0
                    && (game.Objects[i].Flag2 & (ObjectFlags2)f2) == 0)
                {
                    goto L500;
                }

                if (ret_val == 0)
                {
                    goto L400;
                }

                // !ALREADY GOT SOMETHING?
                // !YES, AMBIGUOUS.
                ret_val = -ret_val;
                return ret_val;

                L400:
                ret_val = (int)i;
                // !NOTE MATCH.

                // DOES OBJECT CONTAIN A MATCH?

                L500:
                if ((game.Objects[i].Flag2 & ObjectFlags2.IsOpen) == 0)
                {
                    goto L1000;
                }

                i__2 = game.Objects.Count;
                for (j = (ObjectIds)1; j <= (ObjectIds)i__2; ++j)
                {
                    // !NO, SEARCH CONTENTS.
                    if (game.Objects[j].Container != i
                        || (game.Objects[j].Flag1 & ObjectFlags.IsVisible) == 0
                        || (game.Objects[j].Flag1 & (ObjectFlags)f1) == 0
                        && (game.Objects[j].Flag2 & (ObjectFlags2)f2) == 0)
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
                    ret_val = (int)j;
                    L700:
                    ;
                }
                L1000:
                ;
            }
            return ret_val;
        }
    }
}
