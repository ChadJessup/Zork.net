using System;

namespace Zork.Core
{
    public static class DemonHandler
    {
        /// <summary>
        /// fightd_ - Intermove fight demon
        /// </summary>
        /// <param name="game"></param>
        public static void fightd_(Game game)
        {
            int rout = 1;

            int i__1, i__2;

            // Local variables
            bool f;
            int i, j, ra;
            int obj;
            int res;
            int output;

            i__1 = game.Villians.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP THRU VILLAINS.
                game.Villians.vopps[i - 1] = 0;
                // !CLEAR OPPONENT SLOT.
                obj = game.Villians.villns[i - 1];
                // !GET OBJECT NO.
                ra = game.Objects.oactio[obj - 1];
                // !GET HIS ACTION.
                if (game.Player.Here != game.Objects.oroom[obj - 1])
                {
                    goto L2200;
                }
                // !ADVENTURER STILL HERE?
                if (obj == (int)ObjectIndices.thief && game.Flags.thfenf)
                {
                    goto L2400;
                }
                // !THIEF ENGROSSED?
                if (game.Objects.ocapac[obj - 1] >= 0)
                {
                    goto L2050;
                }

                // !YES, VILL AWAKE?
                if (game.Villians.vprob[i - 1] == 0 || !RoomHandler.prob_(game, game.Villians.vprob[i - 1], game.Villians.vprob[i - 1]))
                {
                    goto L2025;
                }

                i__2 = game.Objects.ocapac[obj - 1];
                game.Objects.ocapac[obj - 1] = Math.Abs(i__2);
                game.Villians.vprob[i - 1] = 0;
                if (ra == 0)
                {
                    goto L2400;
                }
                // !ANYTHING TO DO?
                game.ParserVectors.prsa = (int)VIndices.inxw;
                // !YES, WAKE HIM UP.
                f = ObjectHandler.oappli_(ra, 0, game);
                goto L2400;
                // !NOTHING ELSE HAPPENS.

                L2025:
                game.Villians.vprob[i - 1] += 10;
                // !INCREASE WAKEUP PROB.
                goto L2400;
                // !NOTHING ELSE.

                L2050:
                if ((game.Objects.oflag2[obj - 1] & ObjectFlags2.FITEBT) == 0)
                {
                    goto L2100;
                }
                game.Villians.vopps[i - 1] = obj;
                // !FIGHTING, SET UP OPP.
                goto L2400;

                L2100:
                if (ra == 0)
                {
                    goto L2400;
                }
                // !NOT FIGHTING,
                game.ParserVectors.prsa = (int)VIndices.frstqw;
                // !SET UP PROBABILITY
                if (!ObjectHandler.oappli_(ra, 0, game))
                {
                    goto L2400;
                }
                // !OF FIGHTING.
                game.Objects.oflag2[obj - 1] |= ObjectFlags2.FITEBT;
                game.Villians.vopps[i - 1] = obj;
                // !SET UP OPP.
                goto L2400;

                L2200:
                if ((game.Objects.oflag2[obj - 1] & ObjectFlags2.FITEBT) == 0 || ra == 0)
                {
                    goto L2300;
                }
                game.ParserVectors.prsa = (int)VIndices.fightw;
                // !HAVE A FIGHT.
                f = ObjectHandler.oappli_(ra, 0, game);
                L2300:
                if (obj == (int)ObjectIndices.thief)
                {
                    game.Flags.thfenf = false;
                }
                // !TURN OFF ENGROSSED.
                game.Adventurers.Flags[(int)AIndices.player - 1] &= ~game.astag;
                game.Objects.oflag2[obj - 1] &= ~((int)ObjectFlags2.STAGBT + ObjectFlags2.FITEBT);
                if (game.Objects.ocapac[obj - 1] >= 0 || ra == 0)
                {
                    goto L2400;
                }
                game.ParserVectors.prsa = (int)VIndices.inxw;
                // !WAKE HIM UP.
                f = ObjectHandler.oappli_(ra, 0, game);
                i__2 = game.Objects.ocapac[obj - 1];
                game.Objects.ocapac[obj - 1] = Math.Abs(i__2);
                L2400:
                ;
            }
            // FIGHTD, PAGE 3

            // NOW DO ACTUAL COUNTERBLOWS.

            output = 0;
            // !ASSUME HERO OK.
            L2600:
            i__1 = game.Villians.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP THRU OPPS.
                j = game.Villians.vopps[i - 1];
                if (j == 0)
                {
                    goto L2700;
                }
                // !SLOT EMPTY?
                game.ParserVectors.prscon = 1;
                // !STOP CMD STREAM.
                ra = game.Objects.oactio[j - 1];
                if (ra == 0)
                {
                    goto L2650;
                }
                // !VILLAIN ACTION?
                game.ParserVectors.prsa = (int)VIndices.fightw;
                // !SEE IF
                if (ObjectHandler.oappli_(ra, 0, game))
                {
                    goto L2700;
                }
                // !SPECIAL ACTION.
                L2650:
                res = blow_(game, (int)AIndices.player, j, game.Villians.vmelee[i - 1], false, output);

                // !STRIKE BLOW.
                if (res < 0)
                {
                    return;
                }
                // !IF HERO DEAD, EXIT.
                if (res == rout)
                {
                    output = game.rnd_(3) + 2;
                }
                // !IF HERO OUT, SET FLG.
                L2700:
                ;
            }
            --output;
            // !DECREMENT OUT COUNT.
            if (output > 0) {
                goto L2600;
            }
            // !IF STILL OUT, GO AGAIN.
            return;

        } // fightd_

        // BLOW- STRIKE BLOW
        public static int blow_(Game game, int h, int v, int rmk, bool hflg, int output)
        {
            int rmiss = 0;
            int rout = 1;
            int rkill = 2;
            int rstag = 5;
            int rlose = 6;
            int rhes = 7;
            int rsit = 8;
            int [] def1r = { 1, 2, 3 };
            int [] def2r = { 13, 23, 24, 25 };
            int [] def3r = { 35, 36, 46, 47, 57 };
            int [] rvectr = { 0,0,0,0,5,5,1,1,2,2,2,2,0,0,0,0,0,5,
        5,3,3,1,0,0,0,5,5,3,3,3,1,2,2,2,0,0,0,0,0,5,5,3,3,4,4,0,0,0,5,5,
        3,3,3,4,4,4,0,5,5,3,3,3,3,4,4,4 };
            int [] rstate = { 5000,3005,3008,4011,3015,3018,1021,
        0,0,5022,3027,3030,4033,3037,3040,1043,0,0,4044,2048,4050,4054,
        5058,4063,4067,3071,1074,4075,1079,4080,4084,4088,4092,4096,4100,
        1104,4105,2109,4111,4115,4119,4123,4127,3131,3134 };

            int ret_val, i__1, i__2;

            // Local variables
            bool f;
            int i, j, oa, ra, od, mi, dv, def;
            int tbl;
            int att, res;
            int dweap;
            int pblose;

            ra = game.Objects.oactio[v - 1];
            // !GET VILLAIN ACTION,
            dv = game.Objects.odesc2[v - 1];
            // !DESCRIPTION.
            ret_val = rmiss;
            // !ASSUME NO RESULT.
            if (!(hflg)) {
                goto L1000;
            }
            // !HERO STRIKING BLOW?

            // HERO IS ATTACKER, VILLAIN IS DEFENDER.

            pblose = 10;
            // !BAD LK PROB.
            game.Objects.oflag2[v - 1] |= ObjectFlags2.FITEBT;
            if ((game.Adventurers.Flags[h - 1] & game.astag) == 0)
            {
                goto L100;
            }

            MessageHandler.Speak(game, 591);
            // !YES, CANT FIGHT.
            game.Adventurers.Flags[h - 1] &= ~game.astag;
            return ret_val;

            L100:
            att = dso4.fights_(game, h, true);
            // !GET HIS STRENGTH.
            oa = att;
            def = dso4.vilstr_(game, v);
            // !GET VILL STRENGTH.
            od = def;
            dweap = 0;
            // !ASSUME NO WEAPON.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !SEARCH VILLAIN.
                if (game.Objects.ocan[i - 1] == v && (game.Objects.oflag2[i - 1] & ObjectFlags2.WEAPBT) != 0)
                {
                    dweap = i;
                }
                // L200:
            }

            if (v == game.Adventurers.Objects[(int)AIndices.player - 1])
            {
                goto L300;
            }

            // !KILLING SELF?
            if (def != 0)
            {
                goto L2000;
            }

            // !DEFENDER ALIVE?
            MessageHandler.rspsub_(game, 592, dv);
            // !VILLAIN DEAD.
            return ret_val;

            L300:
            AdventurerHandler.jigsup_(game, 593);
            // !KILLING SELF.
            return ret_val;

            // VILLAIN IS ATTACKER, HERO IS DEFENDER.

            L1000:
            pblose = 50;
            // !BAD LK PROB.
            game.Adventurers.Flags[h - 1] &= ~game.astag;
            if ((game.Objects.oflag2[v - 1] & ObjectFlags2.STAGBT) == 0)
            {
                goto L1200;
            }
            game.Objects.oflag2[v - 1] &= ~ObjectFlags2.STAGBT;
            MessageHandler.rspsub_(game, 594, dv);
            // !DESCRIBE.
            return ret_val;

            L1200:
            att = dso4.vilstr_(game, v);
            // !SET UP ATT, DEF.
            oa = att;
            def = dso4.fights_(game, h, true);
            if (def <= 0) {
                return ret_val;
            }
            // !DONT ALLOW DEAD DEF.
            od = dso4.fights_(game, h, false);
            i__1 = Parser.fwim_(0, (int)ObjectFlags2.WEAPBT, 0, 0, h, true, game);
            dweap = Math.Abs(i__1);
            // !FIND A WEAPON.
            // BLOW, PAGE 4

            // PARTIES ARE NOW EQUIPPED.  DEF CANNOT BE ZERO.
            // ATT MUST BE > 0.

            L2000:
            if (def > 0) {
                goto L2100;
            }
            // !DEF ALIVE?
            res = rkill;
            if (hflg)
            {
                MessageHandler.rspsub_(595, dv, game);
            }

            // !DEADER.
            goto L3000;

            L2100:
            if ((i__1 = def - 2) < 0) {
                goto L2200;
            } else if (i__1 == 0) {
                goto L2300;
            } else {
                goto L2400;
            }
            // !DEF <2,=2,>2
            L2200:
            att = Math.Min(att, 3);
            // !SCALE ATT.
            tbl = def1r[att - 1];
            // !CHOOSE TABLE.
            goto L2500;

            L2300:
            att = Math.Min(att, 4);
            // !SCALE ATT.
            tbl = def2r[att - 1];
            // !CHOOSE TABLE.
            goto L2500;

            L2400:
            att -= def;
            // !SCALE ATT.
            // Computing MIN
            i__1 = 2;
            i__2 = Math.Max(-2, att);
            att = Math.Min(i__1, i__2) + 3;
            tbl = def3r[att - 1];

            L2500:
            res = rvectr[tbl + game.rnd_(10) - 1];
            // !GET RESULT.
            if (output == 0)
            {
                goto L2600;
            }

            // !WAS HE OUT?
            if (res == rstag)
            {
                goto L2550;
            }
            // !YES, STAG--> HES.
            res = rsit;
            // !OTHERWISE, SITTING.
            goto L2600;
            L2550:
            res = rhes;
            L2600:
            if (res == rstag && dweap != 0 && RoomHandler.prob_(game, 25, pblose))
            {
                res = rlose;
            }

            mi = rstate[(rmk - 1) * 9 + res];
            // !CHOOSE TABLE ENTRY.
            if (mi == 0)
            {
                goto L3000;
            }

            i__1 = mi / 1000;
            i = mi % 1000 + game.rnd_(i__1) + game.Star.mbase + 1;
            j = dv;
            if (!(hflg) && dweap != 0)
            {
                j = game.Objects.odesc2[dweap - 1];
            }

            MessageHandler.rspsub_(i, j, game);
            // !PRESENT RESULT.
            // BLOW, PAGE 5

            // NOW APPLY RESULT

            L3000:
            switch (res + 1)
            {
                case 1: goto L4000;
                case 2: goto L3100;
                case 3: goto L3200;
                case 4: goto L3300;
                case 5: goto L3400;
                case 6: goto L3500;
                case 7: goto L3600;
                case 8: goto L4000;
                case 9: goto L3200;
            }

            L3100:
            if (hflg)
            {
                def = -def;
            }
            // !UNCONSCIOUS.
            goto L4000;

            L3200:
            def = 0;
            // !KILLED OR SITTING DUCK.
            goto L4000;

            L3300:
            // Computing MAX
            i__1 = 0;
            i__2 = def - 1;
            def = Math.Max(i__1, i__2);
            // !LIGHT WOUND.
            goto L4000;

            L3400:
            // Computing MAX
            i__1 = 0;
            i__2 = def - 2;
            def = Math.Max(i__1, i__2);
            // !SERIOUS WOUND.
            goto L4000;

            L3500:
            if (hflg) {
                goto L3550;
            }
            // !STAGGERED.
            game.Adventurers.Flags[h - 1] |= game.astag;
            goto L4000;

            L3550:
            game.Objects.oflag2[v - 1] |= ObjectFlags2.STAGBT;
            goto L4000;

            L3600:
            ObjectHandler.SetNewObjectStatus(dweap, 0, game.Player.Here, 0, 0, game);
            // !LOSE WEAPON.
            dweap = 0;
            if (hflg) {
                goto L4000;
            }

            // !IF HERO, DONE.
            i__1 = Parser.fwim_(0, (int)ObjectFlags2.WEAPBT, 0, 0, h, true, game);
            dweap = Math.Abs(i__1);
            // !GET NEW.
            if (dweap != 0) {
                MessageHandler.rspsub_(605, game.Objects.odesc2[dweap - 1], game);
            }
            // BLOW, PAGE 6

            L4000:
            ret_val = res;
            // !RETURN RESULT.
            if (!(hflg)) {
                goto L4500;
            }
            // !HERO?
            game.Objects.ocapac[v - 1] = def;
            // !STORE NEW CAPACITY.
            if (def != 0) {
                goto L4100;
            }
            // !DEAD?
            game.Objects.oflag2[v - 1] &= ~ObjectFlags2.FITEBT;
            MessageHandler.rspsub_(game, 572, dv);
            // !HE DIES.
            ObjectHandler.SetNewObjectStatus(v, 0, 0, 0, 0, game);
            // !MAKE HIM DISAPPEAR.
            if (ra == 0) {
                return ret_val;
            }
            // !IF NX TO DO, EXIT.
            game.ParserVectors.prsa = (int)VIndices.deadxw;
            // !LET HIM KNOW.
            f = ObjectHandler.oappli_(ra, 0, game);
            return ret_val;

            L4100:
            if (res != rout || ra == 0) {
                return ret_val;
            }
            game.ParserVectors.prsa = (int)VIndices.outxw;
            // !LET HIM BE OUT.
            f = ObjectHandler.oappli_(ra, 0, game);
            return ret_val;

            L4500:
            game.Adventurers.astren[h - 1] = -10000;
            // !ASSUME DEAD.
            if (def != 0) {
                game.Adventurers.astren[h - 1] = def - od;
            }
            if (def >= od) {
                goto L4600;
            }
            game.Clock.Ticks[(int)ClockIndices.cevcur - 1] = 30;
            game.Clock.Flags[(int)ClockIndices.cevcur - 1] = true;

            L4600:
            if (dso4.fights_(game, h, true) > 0)
            {
                return ret_val;
            }

            game.Adventurers.astren[h - 1] = 1 - dso4.fights_(game, h, false);
            // !HE'S DEAD.
            AdventurerHandler.jigsup_(game, 596);
            ret_val = -1;
            return ret_val;

        } // blow_

        // SWORDD- SWORD INTERMOVE DEMON
        public static void swordd_(Game game)
        {
            // System generated locals
            int i__1, i__2;

            // Local variables
            int i, ng;

            if (game.Objects.oadv[(int)ObjectIndices.sword - 1] != (int)AIndices.player)
            {
                goto L500;
            }
            // !HOLDING SWORD?
            ng = 2;
            // !ASSUME VILL CLOSE.
            if (infest_(game, game.Player.Here))
            {
                goto L300;
            }
            // !VILL HERE?
            ng = 1;
            i__1 = (int)XSearch.xmax;
            i__2 = (int)XSearch.xmin;
            for (i = (int)XSearch.xmin; i__2 < 0 ? i >= i__1 : i <= i__1; i += i__2)
            {
                // !NO, SEARCH ROOMS.
                if (!dso3.findxt_(game, i, game.Player.Here))
                {
                    goto L200;
                }
                // !ROOM THAT WAY?
                switch (game.curxt_.xtype)
                {
                    case 1: goto L50;
                    case 2: goto L200;
                    case 3: goto L50;
                    case 4: goto L50;
                }
                // !SEE IF ROOM AT ALL.
                L50:
                if (infest_(game, game.curxt_.xroom1))
                {
                    goto L300;
                }
                // !CHECK ROOM.
                L200:
                ;
            }
            ng = 0;
            // !NO GLOW.

            L300:
            if (ng == game.Hack.swdsta)
            {
                return;
            }
            // !ANY STATE CHANGE?
            i__2 = ng + 495;
            MessageHandler.Speak(game, i__2);
            // !YES, TELL NEW STATE.
            game.Hack.swdsta = ng;
            return;

            L500:
            game.Hack.swdact = false;
            // !DROPPED SWORD,
            return;
            // !DISABLE DEMON.
        } // swordd_

        // INFEST-	SUBROUTINE TO TEST FOR INFESTED ROOM

        public static bool infest_(Game game, int r)
        {
            // System generated locals
            bool ret_val;

            if (!game.Flags.endgmf)
            {
                ret_val = game.Objects.oroom[(int)ObjectIndices.cyclo - 1] == r || game.Objects.oroom[(int)ObjectIndices.troll - 1] == r || game.Objects.oroom[(int)ObjectIndices.thief - 1] == r && game.Hack.thfact;
            }
            else
            {
                ret_val = r == (int)RoomIndices.mrg || r == (int)RoomIndices.mrge || r == (int)RoomIndices.mrgw || r == (int)RoomIndices.inmir && game.Switches.mloc == (int)RoomIndices.mrg;
            }

            return ret_val;
        }
    }
}