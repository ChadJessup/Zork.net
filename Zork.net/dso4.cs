using System;
using System.Collections.Generic;
using System.Text;
using Zork.Core.Object;

namespace Zork.Core
{
    public static class dso4
    {
        /* ROBADV-- STEAL WINNER'S VALUABLES */

        public static int robadv_(Game game, int adv, int nr, ObjectIndices nc, int na)
        {
            int ret_val, i__1;
            int i;

            ret_val = 0;
            /* 						!COUNT OBJECTS */
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (game.Objects.oadv[i - 1] != adv || game.Objects.otval[i - 1] <= 0
                    || (game.Objects.oflag2[i - 1] & ObjectFlags2.SCRDBT) != 0)
                {
                    goto L100;
                }

                ObjectHandler.newsta_(i, 0, nr, (int)nc, na, game);
                /* 						!STEAL OBJECT */
                ++ret_val;
                L100:
                ;
            }
            return ret_val;
        } /* robadv_ */

        /* ROBRM-- STEAL ROOM VALUABLES */

        /* DECLARATIONS */

        public static int robrm_(Game game, int rm, int pr, int nr, int nc, int na)
        {
            int ret_val, i__1, i__2;
            int i;

            /* OBJECTS */
            ret_val = 0;
            /* 						!COUNT OBJECTS */
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i) {
                /* 						!LOOP ON OBJECTS. */
                if (!ObjectHandler.qhere_(i, rm, game))
                {
                    goto L100;
                }

                if (game.Objects.otval[i - 1] <= 0 || (game.Objects.oflag2[i - 1] & ObjectFlags2.SCRDBT) != 0 || (game.Objects.oflag1[i - 1] & ObjectFlags.VISIBT) == 0 || !RoomHandler.prob_(game, pr, pr))
                {
                    goto L50;
                }

                ObjectHandler.newsta_(i, 0, nr, nc, na, game);
                ++ret_val;
                game.Objects.oflag2[i - 1] |= ObjectFlags2.TCHBT;
                goto L100;
                L50:
                if ((game.Objects.oflag2[i - 1] & ObjectFlags2.ACTRBT) != 0)
                {
                    i__2 = ObjectHandler.GetActor(i, game);
                    ret_val += robadv_(game, i__2, nr, (ObjectIndices)nc, na);
                }
                L100:
                ;
            }
            return ret_val;
        } /* robrm_ */

        /* WINNIN-- SEE IF VILLAIN IS WINNING */

        /* DECLARATIONS */

        public static bool winnin_(Game game, int vl, int hr)
        {
            /* System generated locals */
            bool ret_val;

            /* Local variables */
            int ps, vs;


            /* OBJECTS */
            vs = game.Objects.ocapac[vl - 1];
            /* 						!VILLAIN STRENGTH */
            ps = vs - fights_(game, hr, true);
            /* 						!HIS MARGIN OVER HERO */
            ret_val = RoomHandler.prob_(game, 90, 100);
            if (ps > 3) {
                return ret_val;
            }
            /* 						!+3... 90% WINNING */
            ret_val = RoomHandler.prob_(game, 75, 85);
            if (ps > 0) {
                return ret_val;
            }
            /* 						!>0... 75% WINNING */
            ret_val = RoomHandler.prob_(game, 50, 30);
            if (ps == 0) {
                return ret_val;
            }
            /* 						!=0... 50% WINNING */
            ret_val = RoomHandler.prob_(game, 25, 25);
            if (vs > 1) {
                return ret_val;
            }
            /* 						!ANY VILLAIN STRENGTH. */
            ret_val = RoomHandler.prob_(game, 10, 0);
            return ret_val;
        } /* winnin_ */

        /* FIGHTS-- COMPUTE FIGHT STRENGTH */
        public static int fights_(Game game, int h, bool flg)
        {
            const int smin = 2;
            const int smax = 7;

            /* System generated locals */
            int ret_val;

            ret_val = smin + ((smax - smin) * game.Adventurers.Scores[h - 1] + game.State.MaxScore / 2) / game.State.MaxScore;
            if (flg) {
                ret_val += game.Adventurers.astren[h - 1];
            }
            return ret_val;
        } /* fights_ */

        /* VILSTR-	COMPUTE VILLAIN STRENGTH */

        public static int vilstr_(Game game, int v)
        {
            /* System generated locals */
            int ret_val, i__1, i__2, i__3;

            /* Local variables */
            int i;

            ret_val = game.Objects.ocapac[v - 1];
            if (ret_val <= 0)
            {
                return ret_val;
            }

            if (v != (int)ObjectIndices.thief || !game.Flags.thfenf)
            {
                goto L100;
            }
            game.Flags.thfenf = false;
            /* 						!THIEF UNENGROSSED. */
            ret_val = Math.Min(ret_val, 2);
            /* 						!NO BETTER THAN 2. */

            L100:
            i__1 = game.Villians.Count;
            for (i = 1; i <= i__1; ++i) {
                /* 						!SEE IF  BEST WEAPON. */
                if (game.Villians.villns[i - 1] == v && game.ParserVectors.prsi == game.Villians.vbest[i - 1])
                {
                    /* Computing MAX */
                    i__2 = 1;
                    i__3 = ret_val - 1;
                    ret_val = Math.Max(i__2, i__3);
                }
                /* L200: */
            }

            return ret_val;
        } /* vilstr_ */
    }
}