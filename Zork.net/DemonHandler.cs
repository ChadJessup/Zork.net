using System;
using System.Linq;

namespace Zork.Core
{
    public static class DemonHandler
    {
        /// <summary>
        /// fightd_ - Intermove fight demon
        /// </summary>
        /// <param name="game"></param>
        public static void Fight(Game game)
        {
            int rout = 1;

            int i__1, i__2;

            // Local variables
            bool f;
            int i, ra;
            ObjectIds obj, j;
            int res;
            int output;

            foreach (var villian in game.Villians.Values)
            {
                // !CLEAR OPPONENT SLOT.
                villian.Opponent = 0;
                // !GET OBJECT NO.
                obj = villian.Id;
                // !GET HIS ACTION.
                ra = game.Objects[obj].Action;

                // !ADVENTURER STILL HERE?
                if (game.Player.Here != RoomHandler.GetRoomThatContainsObject(obj, game).Id)
                {
                    goto L2200;
                }

                if (obj == ObjectIds.Thief && game.Flags.IsThiefEngrossed)
                {
                    continue;
                }

                // !THIEF ENGROSSED?
                if (game.Objects[obj].Capacity >= 0)
                {
                    goto L2050;
                }

                // !YES, VILL AWAKE?
                if (villian.WakeupProbability == 0 || !RoomHandler.prob_(game, villian.WakeupProbability, villian.WakeupProbability))
                {
                    goto L2025;
                }

                i__2 = game.Objects[obj].Capacity;
                game.Objects[obj].Capacity = Math.Abs(i__2);

                villian.WakeupProbability = 0;

                if (ra == 0)
                {
                    continue;
                }

                // !ANYTHING TO DO?
                game.ParserVectors.prsa = VerbIds.inxw;
                // !YES, WAKE HIM UP.
                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
                // !NOTHING ELSE HAPPENS.
                continue;

                L2025:
                // !INCREASE WAKEUP PROB.
                villian.WakeupProbability += 10;
                // !NOTHING ELSE.
                continue;

                L2050:
                if ((game.Objects[obj].Flag2 & ObjectFlags2.IsFighting) == 0)
                {
                    goto L2100;
                }

                // !FIGHTING, SET UP OPP.
                villian.Opponent = obj;
                continue;

                L2100:
                if (ra == 0)
                {
                    continue;
                }

                // !NOT FIGHTING,
                game.ParserVectors.prsa = VerbIds.frstqw;
                // !SET UP PROBABILITY
                if (!ObjectHandler.DoObjectSpecialAction(ra, 0, game))
                {
                    continue;
                }

                // !OF FIGHTING.
                game.Objects[obj].Flag2 |= ObjectFlags2.IsFighting;
                // !SET UP OPP.
                villian.Opponent = obj;

                continue;

                L2200:
                if ((game.Objects[obj].Flag2 & ObjectFlags2.IsFighting) == 0 || ra == 0)
                {
                    goto L2300;
                }

                // !HAVE A FIGHT.
                game.ParserVectors.prsa = VerbIds.Fight;

                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);

                L2300:
                if (obj == ObjectIds.Thief)
                {
                    game.Flags.IsThiefEngrossed = false;
                }

                // !TURN OFF ENGROSSED.
                game.Adventurers[ActorIds.Player].Flag &= ~game.astag;
                game.Objects[obj].Flag2 &= ~((int)ObjectFlags2.IsStaggered + ObjectFlags2.IsFighting);

                if (game.Objects[obj].Capacity >= 0 || ra == 0)
                {
                    continue;
                }

                game.ParserVectors.prsa = VerbIds.inxw;
                // !WAKE HIM UP.
                f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
                i__2 = game.Objects[obj].Capacity;
                game.Objects[obj].Capacity = Math.Abs(i__2);
            }

            // FIGHTD, PAGE 3

            // NOW DO ACTUAL COUNTERBLOWS.

            output = 0;
            // !ASSUME HERO OK.
            L2600:
            foreach (var villian in game.Villians.Values)
            {
                j = villian.Opponent;

                // !SLOT EMPTY?
                if (j == 0)
                {
                    continue;
                }

                // !STOP CMD STREAM.
                game.ParserVectors.prscon = 1;
                // !VILLAIN ACTION?
                ra = game.Objects[j].Action;
                if (ra == 0)
                {
                    goto STRIKEBLOW;
                }

                // !SEE IF
                game.ParserVectors.prsa = VerbIds.Fight;

                // !SPECIAL ACTION.
                if (ObjectHandler.DoObjectSpecialAction(ra, 0, game))
                {
                    continue;
                }

                STRIKEBLOW:
                // !STRIKE BLOW.
                res = StrikeBlow(game, ActorIds.Player, j, villian.Melee, false, output);

                // !IF HERO DEAD, EXIT.
                if (res < 0)
                {
                    return;
                }

                // !IF HERO OUT, SET FLG.
                if (res == rout)
                {
                    output = game.rnd_(3) + 2;
                }
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
        public static int StrikeBlow(Game game, ActorIds adventurer, ObjectIds villian, int remark, bool isHeroAttacking, int output)
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
            ObjectIds dweap;
            int pblose;

            // !GET VILLAIN ACTION,
            ra = game.Objects[villian].Action;

            // !DESCRIPTION.
            dv = game.Objects[villian].Description2;
            // !ASSUME NO RESULT.
            ret_val = rmiss;

            // !HERO STRIKING BLOW?
            // HERO IS ATTACKER, VILLAIN IS DEFENDER.
            if (!(isHeroAttacking))
            {
                goto L1000;
            }

            pblose = 10;
            // !BAD LK PROB.
            game.Objects[villian].Flag2 |= ObjectFlags2.IsFighting;

            if ((game.Adventurers[adventurer].Flag & game.astag) == 0)
            {
                goto L100;
            }

            MessageHandler.Speak(game, 591);
            // !YES, CANT FIGHT.
            game.Adventurers[adventurer].Flag &= ~game.astag;
            return ret_val;

            L100:
            att = dso4.ComputeFightStrength(game, adventurer, true);
            // !GET HIS STRENGTH.
            oa = att;
            def = dso4.ComputeVillianStrength(game, villian);
            // !GET VILL STRENGTH.
            od = def;
            dweap = 0;
            // !ASSUME NO WEAPON.
            i__1 = game.Objects.Count;


            foreach (var weapon in game.Objects[villian].ContainedObjects.Where(o => o.Flag2.HasFlag(ObjectFlags2.IsWeapon)))
            {
                dweap = weapon.Id;
            }

            // !KILLING SELF?
            if (villian == game.Adventurers[ActorIds.Player].ObjectId)
            {
                goto L300;
            }

            // !DEFENDER ALIVE?
            if (def != 0)
            {
                goto L2000;
            }

            // !VILLAIN DEAD.
            MessageHandler.rspsub_(game, 592, dv);

            return ret_val;

            L300:
            // !KILLING SELF.
            AdventurerHandler.jigsup_(game, 593);
            return ret_val;

            // VILLAIN IS ATTACKER, HERO IS DEFENDER.

            L1000:
            pblose = 50;
            // !BAD LK PROB.
            game.Adventurers[adventurer].Flag &= ~game.astag;
            if ((game.Objects[villian].Flag2 & ObjectFlags2.IsStaggered) == 0)
            {
                goto L1200;
            }

            game.Objects[villian].Flag2 &= ~ObjectFlags2.IsStaggered;
            MessageHandler.rspsub_(game, 594, dv);
            // !DESCRIBE.
            return ret_val;

            L1200:
            att = dso4.ComputeVillianStrength(game, villian);
            // !SET UP ATT, DEF.
            oa = att;
            def = dso4.ComputeFightStrength(game, adventurer, true);
            if (def <= 0)
            {
                return ret_val;
            }

            // !DONT ALLOW DEAD DEF.
            od = dso4.ComputeFightStrength(game, adventurer, false);
            // !FIND A WEAPON.
            i__1 = Parser.FindWhatIMean(0, ObjectFlags2.IsWeapon, 0, 0, adventurer, true, game);

            dweap = (ObjectIds)Math.Abs(i__1);
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
            if (isHeroAttacking)
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

            mi = rstate[(remark - 1) * 9 + res];
            // !CHOOSE TABLE ENTRY.
            if (mi == 0)
            {
                goto L3000;
            }

            i__1 = mi / 1000;
            i = (ObjectIds)(mi % 1000 + game.rnd_(i__1) + game.Star.mbase + 1);
            j = dv;

            if (!(isHeroAttacking) && dweap != 0)
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
            if (isHeroAttacking)
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
            if (isHeroAttacking)
            {
                goto L3550;
            }
            // !STAGGERED.
            game.Adventurers[adventurer].Flag |= game.astag;
            goto L4000;

            L3550:
            game.Objects[villian].Flag2 |= ObjectFlags2.IsStaggered;
            goto L4000;

            L3600:
            // !LOSE WEAPON.
            ObjectHandler.SetNewObjectStatus((ObjectIds)dweap, 0, game.Player.Here, 0, 0, game);
            dweap = 0;

            // !IF HERO, DONE.
            if (isHeroAttacking)
            {
                goto L4000;
            }

            // !GET NEW.
            i__1 = Parser.FindWhatIMean(0, ObjectFlags2.IsWeapon, 0, 0, adventurer, true, game);
            dweap = (ObjectIds)Math.Abs(i__1);
            if (dweap != 0)
            {
                MessageHandler.rspsub_(605, game.Objects[(ObjectIds)dweap].Description2, game);
            }
            // BLOW, PAGE 6

            L4000:
            ret_val = res;
            // !RETURN RESULT.
            // !HERO?
            if (!(isHeroAttacking))
            {
                goto L4500;
            }
            // !STORE NEW CAPACITY.
            game.Objects[villian].Capacity = def;
            // !DEAD?
            if (def != 0)
            {
                goto L4100;
            }
            game.Objects[villian].Flag2 &= ~ObjectFlags2.IsFighting;
            // !HE DIES.
            MessageHandler.rspsub_(game, 572, dv);

            // !MAKE HIM DISAPPEAR.
            ObjectHandler.SetNewObjectStatus(villian, 0, 0, 0, 0, game);
            if (ra == 0)
            {
                return ret_val;
            }

            // !IF NX TO DO, EXIT.
            game.ParserVectors.prsa = VerbIds.Dead;
            // !LET HIM KNOW.
            f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
            return ret_val;

            L4100:
            if (res != rout || ra == 0)
            {
                return ret_val;
            }

            game.ParserVectors.prsa = VerbIds.OutCold;
            // !LET HIM BE OUT.
            f = ObjectHandler.DoObjectSpecialAction(ra, 0, game);
            return ret_val;

            L4500:
            // !ASSUME DEAD.
            game.Adventurers[adventurer].Strength = -10000;
            if (def != 0)
            {
                game.Adventurers[adventurer].Strength = def - od;
            }

            if (def >= od)
            {
                goto L4600;
            }

            game.Clock.Ticks[(int)ClockIndices.cevcur - 1] = 30;
            game.Clock.Flags[(int)ClockIndices.cevcur - 1] = true;

            L4600:
            if (dso4.ComputeFightStrength(game, adventurer, true) > 0)
            {
                return ret_val;
            }

            game.Adventurers[adventurer].Strength = 1 - dso4.ComputeFightStrength(game, adventurer, false);

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

            if (!game.Flags.IsEndGame)
            {
                ret_val = RoomHandler.GetRoomThatContainsObject(ObjectIds.Cyclops, game).Id == roomId || RoomHandler.GetRoomThatContainsObject(ObjectIds.Troll, game).Id == roomId || RoomHandler.GetRoomThatContainsObject(ObjectIds.Thief, game).Id == roomId && game.Hack.IsThiefActive;
            }
            else
            {
                ret_val = roomId == RoomIds.mrg || roomId == RoomIds.mrge || roomId == RoomIds.mrgw || roomId == RoomIds.inmir && game.Switches.mloc == RoomIds.mrg;
            }

            return ret_val;
        }
    }
}