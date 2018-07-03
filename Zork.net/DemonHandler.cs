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
            int i, ra;
            ObjectIds obj, j;
            int res;
            int output;

            i__1 = game.Villians.Count;
                // !LOOP THRU VILLAINS.
            for (i = 1; i <= i__1; ++i)
            {
                // !CLEAR OPPONENT SLOT.
                game.Villians.vopps[i - 1] = 0;
                // !GET OBJECT NO.
                obj = (ObjectIds)game.Villians.villns[i - 1];
                // !GET HIS ACTION.
                ra = game.Objects[obj].Action;

                // !ADVENTURER STILL HERE?
                if (game.Player.Here != RoomHandler.GetRoomThatContainsObject(obj, game).Id)
                {
                    goto L2200;
                }

                if (obj == ObjectIds.thief && game.Flags.thfenf)
                {
                    goto L2400;
                }

                // !THIEF ENGROSSED?
                if (game.Objects[obj].Capacity >= 0)
                {
                    goto L2050;
                }

                // !YES, VILL AWAKE?
                if (game.Villians.vprob[i - 1] == 0 || !RoomHandler.prob_(game, game.Villians.vprob[i - 1], game.Villians.vprob[i - 1]))
                {
                    goto L2025;
                }

                i__2 = game.Objects[obj].Capacity;
                game.Objects[obj].Capacity = Math.Abs(i__2);
                game.Villians.vprob[i - 1] = 0;
                if (ra == 0)
                {
                    goto L2400;
                }
                // !ANYTHING TO DO?
                game.ParserVectors.prsa = VerbIds.inxw;
                // !YES, WAKE HIM UP.
                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
                goto L2400;
                // !NOTHING ELSE HAPPENS.

                L2025:
                game.Villians.vprob[i - 1] += 10;
                // !INCREASE WAKEUP PROB.
                goto L2400;
                // !NOTHING ELSE.

                L2050:
                if ((game.Objects[obj].Flag2 & ObjectFlags2.FITEBT) == 0)
                {
                    goto L2100;
                }

                game.Villians.vopps[i - 1] = (int)obj;
                // !FIGHTING, SET UP OPP.
                goto L2400;

                L2100:
                if (ra == 0)
                {
                    goto L2400;
                }

                // !NOT FIGHTING,
                game.ParserVectors.prsa = VerbIds.frstqw;
                // !SET UP PROBABILITY
                if (!ObjectHandler.DoObjectSpecialAction(ra, 0, game))
                {
                    goto L2400;
                }

                // !OF FIGHTING.
                game.Objects[obj].Flag2 |= ObjectFlags2.FITEBT;
                game.Villians.vopps[i - 1] = (int)obj;
                // !SET UP OPP.
                goto L2400;

                L2200:
                if ((game.Objects[obj].Flag2 & ObjectFlags2.FITEBT) == 0 || ra == 0)
                {
                    goto L2300;
                }
                game.ParserVectors.prsa = VerbIds.Fight;
                // !HAVE A FIGHT.
                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
                L2300:
                if (obj == ObjectIds.thief)
                {
                    game.Flags.thfenf = false;
                }
                // !TURN OFF ENGROSSED.
                game.Adventurers[ActorIds.Player].Flag &= ~game.astag;
                game.Objects[obj].Flag2 &= ~((int)ObjectFlags2.STAGBT + ObjectFlags2.FITEBT);
                if (game.Objects[obj].Capacity >= 0 || ra == 0)
                {
                    goto L2400;
                }
                game.ParserVectors.prsa = VerbIds.inxw;
                // !WAKE HIM UP.
                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
                i__2 = game.Objects[obj].Capacity;
                game.Objects[obj].Capacity = Math.Abs(i__2);
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
                j = (ObjectIds)game.Villians.vopps[i - 1];
                if (j == 0)
                {
                    goto L2700;
                }
                // !SLOT EMPTY?
                game.ParserVectors.prscon = 1;
                // !STOP CMD STREAM.
                ra = game.Objects[j].Action;
                if (ra == 0)
                {
                    goto L2650;
                }
                // !VILLAIN ACTION?
                game.ParserVectors.prsa = VerbIds.Fight;
                // !SEE IF
                if (ObjectHandler.DoObjectSpecialAction(ra, 0, game))
                {
                    goto L2700;
                }
                // !SPECIAL ACTION.
                L2650:
                res = StrikeBlow(game, ActorIds.Player, j, game.Villians.vmelee[i - 1], false, output);

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
            if (output > 0)
            {
                goto L2600;
            }
            // !IF STILL OUT, GO AGAIN.
            return;

        } // fightd_

        // BLOW- STRIKE BLOW
        public static int StrikeBlow(Game game, ActorIds h, ObjectIds v, int rmk, bool hflg, int output)
        {
            int rmiss = 0;
            int rout = 1;
            int rkill = 2;
            int rstag = 5;
            int rlose = 6;
            int rhes = 7;
            int rsit = 8;
            int[] def1r = { 1, 2, 3 };
            int[] def2r = { 13, 23, 24, 25 };
            int[] def3r = { 35, 36, 46, 47, 57 };
            int[] rvectr = { 0,0,0,0,5,5,1,1,2,2,2,2,0,0,0,0,0,5,
        5,3,3,1,0,0,0,5,5,3,3,3,1,2,2,2,0,0,0,0,0,5,5,3,3,4,4,0,0,0,5,5,
        3,3,3,4,4,4,0,5,5,3,3,3,3,4,4,4 };
            int[] rstate = { 5000,3005,3008,4011,3015,3018,1021,
        0,0,5022,3027,3030,4033,3037,3040,1043,0,0,4044,2048,4050,4054,
        5058,4063,4067,3071,1074,4075,1079,4080,4084,4088,4092,4096,4100,
        1104,4105,2109,4111,4115,4119,4123,4127,3131,3134 };

            int ret_val, i__1, i__2;

            // Local variables
            bool f;
            int j, oa, ra, od, mi, dv, def;
            ObjectIds i;
            int tbl;
            int att, res;
            int dweap;
            int pblose;

            ra = game.Objects[v].Action;
            // !GET VILLAIN ACTION,
            dv = game.Objects[v].Description2;
            // !DESCRIPTION.
            ret_val = rmiss;
            // !ASSUME NO RESULT.
            if (!(hflg))
            {
                goto L1000;
            }

            // !HERO STRIKING BLOW?

            // HERO IS ATTACKER, VILLAIN IS DEFENDER.

            pblose = 10;
            // !BAD LK PROB.
            game.Objects[v].Flag2 |= ObjectFlags2.FITEBT;

            if ((game.Adventurers[h].Flag & game.astag) == 0)
            {
                goto L100;
            }

            MessageHandler.Speak(game, 591);
            // !YES, CANT FIGHT.
            game.Adventurers[h].Flag &= ~game.astag;
            return ret_val;

            L100:
            att = dso4.ComputeFightStrength(game, (ActorIds)h, true);
            // !GET HIS STRENGTH.
            oa = att;
            def = dso4.ComputeVillianStrength(game, (int)v);
            // !GET VILL STRENGTH.
            od = def;
            dweap = 0;
            // !ASSUME NO WEAPON.
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !SEARCH VILLAIN.
                if (game.Objects[i].Container == v && (game.Objects[i].Flag2 & ObjectFlags2.IsWeapon) != 0)
                {
                    dweap = (int)i;
                }
                // L200:
            }

            if (v == game.Adventurers[ActorIds.Player].ObjectId)
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
            game.Adventurers[h].Flag &= ~game.astag;
            if ((game.Objects[v].Flag2 & ObjectFlags2.STAGBT) == 0)
            {
                goto L1200;
            }

            game.Objects[v].Flag2 &= ~ObjectFlags2.STAGBT;
            MessageHandler.rspsub_(game, 594, dv);
            // !DESCRIBE.
            return ret_val;

            L1200:
            att = dso4.ComputeVillianStrength(game, (int)v);
            // !SET UP ATT, DEF.
            oa = att;
            def = dso4.ComputeFightStrength(game, (ActorIds)h, true);
            if (def <= 0)
            {
                return ret_val;
            }

            // !DONT ALLOW DEAD DEF.
            od = dso4.ComputeFightStrength(game, h, false);
            i__1 = Parser.FindWhatIMean(0, ObjectFlags2.IsWeapon, 0, 0, h, true, game);
            dweap = Math.Abs(i__1);
            // !FIND A WEAPON.
            // BLOW, PAGE 4

            // PARTIES ARE NOW EQUIPPED.  DEF CANNOT BE ZERO.
            // ATT MUST BE > 0.

            L2000:
            if (def > 0)
            {
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
            if ((i__1 = def - 2) < 0)
            {
                goto L2200;
            }
            else if (i__1 == 0)
            {
                goto L2300;
            }
            else
            {
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
            i = (ObjectIds)(mi % 1000 + game.rnd_(i__1) + game.Star.mbase + 1);
            j = dv;

            if (!(hflg) && dweap != 0)
            {
                j = game.Objects[(ObjectIds)dweap].Description2;
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
            if (hflg)
            {
                goto L3550;
            }
            // !STAGGERED.
            game.Adventurers[h].Flag |= game.astag;
            goto L4000;

            L3550:
            game.Objects[v].Flag2 |= ObjectFlags2.STAGBT;
            goto L4000;

            L3600:
            ObjectHandler.SetNewObjectStatus((ObjectIds)dweap, 0, game.Player.Here, 0, 0, game);
            // !LOSE WEAPON.
            dweap = 0;
            if (hflg)
            {
                goto L4000;
            }

            // !IF HERO, DONE.
            i__1 = Parser.FindWhatIMean(0, ObjectFlags2.IsWeapon, 0, 0, h, true, game);
            dweap = Math.Abs(i__1);
            // !GET NEW.
            if (dweap != 0)
            {
                MessageHandler.rspsub_(605, game.Objects[(ObjectIds)dweap].Description2, game);
            }
            // BLOW, PAGE 6

            L4000:
            ret_val = res;
            // !RETURN RESULT.
            if (!(hflg))
            {
                goto L4500;
            }
            // !HERO?
            game.Objects[v].Capacity = def;
            // !STORE NEW CAPACITY.
            if (def != 0)
            {
                goto L4100;
            }
            // !DEAD?
            game.Objects[v].Flag2 &= ~ObjectFlags2.FITEBT;
            MessageHandler.rspsub_(game, 572, dv);
            // !HE DIES.
            ObjectHandler.SetNewObjectStatus((ObjectIds)v, 0, 0, 0, 0, game);
            // !MAKE HIM DISAPPEAR.
            if (ra == 0)
            {
                return ret_val;
            }

            // !IF NX TO DO, EXIT.
            game.ParserVectors.prsa = VerbIds.deadxw;
            // !LET HIM KNOW.
            f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
            return ret_val;

            L4100:
            if (res != rout || ra == 0)
            {
                return ret_val;
            }

            game.ParserVectors.prsa = VerbIds.outxw;
            // !LET HIM BE OUT.
            f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
            return ret_val;

            L4500:
            // !ASSUME DEAD.
            game.Adventurers[h].Strength = -10000;
            if (def != 0)
            {
                game.Adventurers[h].Strength = def - od;
            }

            if (def >= od)
            {
                goto L4600;
            }

            game.Clock.Ticks[(int)ClockIndices.cevcur - 1] = 30;
            game.Clock.Flags[(int)ClockIndices.cevcur - 1] = true;

            L4600:
            if (dso4.ComputeFightStrength(game, h, true) > 0)
            {
                return ret_val;
            }

            game.Adventurers[h].Strength = 1 - dso4.ComputeFightStrength(game, h, false);

            // !HE'S DEAD.
            AdventurerHandler.jigsup_(game, 596);

            ret_val = -1;
            return ret_val;
        }

        // SWORDD- SWORD INTERMOVE DEMON
        public static void swordd_(Game game)
        {
            // System generated locals
            int i__1, i__2;

            // Local variables
            int i, ng;

            // !HOLDING SWORD?
            if (game.Objects[ObjectIds.Sword].Adventurer != ActorIds.Player)
            {
                goto L500;
            }

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
                if (!dso3.FindExit(game, i, game.Player.Here))
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
            // !ANY STATE CHANGE?
            if (ng == game.Hack.SwordStatus)
            {
                return;
            }

            i__2 = ng + 495;
            // !YES, TELL NEW STATE.
            MessageHandler.Speak(game, i__2);

            game.Hack.SwordStatus = ng;
            return;

            L500:
            // !DROPPED SWORD,
            // !DISABLE DEMON.
            game.Hack.IsSwordActive = false;

            return;
        }

        /// <summary>
        /// infest_ - Test for infested room
        /// </summary>
        /// <param name="game"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static bool infest_(Game game, RoomIds roomId)
        {
            // System generated locals
            bool ret_val;

            if (!game.Flags.EndGame)
            {
                ret_val = RoomHandler.GetRoomThatContainsObject(ObjectIds.Cyclops, game).Id == roomId || RoomHandler.GetRoomThatContainsObject(ObjectIds.Troll, game).Id == roomId || RoomHandler.GetRoomThatContainsObject(ObjectIds.thief, game).Id == roomId && game.Hack.IsThiefActive;
            }
            else
            {
                ret_val = roomId == RoomIds.mrg || roomId == RoomIds.mrge || roomId == RoomIds.mrgw || roomId == RoomIds.inmir && game.Switches.mloc == RoomIds.mrg;
            }

            return ret_val;
        }
    }
}