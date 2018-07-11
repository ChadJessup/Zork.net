using System;

namespace Zork.Core
{
    public static class villns
    {
        // TROLLP-	TROLL FUNCTION
        public static bool trollp_(Game game, int arg)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            int i;

            ret_val = true;
            // !ASSUME WINS.
            if (game.ParserVectors.prsa != VerbIds.Fight)
            {
                goto L1100;
            }

            // !FIGHT?
            if (game.Objects[ObjectIds.Axe].Container == ObjectIds.Troll)
            {
                goto L10;
            }

            // !GOT AXE?  NOTHING.
            i = 433;

            // !ASSUME CANT GET.
            if (!ObjectHandler.IsObjectInRoom(ObjectIds.Axe, game.Player.Here, game))
            {
                goto L1050;
            }

            // !HERE?
            i = 434;
            // !YES, RECOVER.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Axe, 0, 0, ObjectIds.Troll, 0, game);
            L1050:
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Troll, game.Player.Here, game))
            {
                MessageHandler.rspsub_(game, i);
            }
            // !IF PLAYER HERE.
            return ret_val;

            L1100:
            if (game.ParserVectors.prsa != VerbIds.Dead)
            {
                goto L1200;
            }

            // !DEAD?
            game.Flags.trollf = true;
            // !PERMIT EXITS.
            return ret_val;

            L1200:
            if (game.ParserVectors.prsa != VerbIds.OutCold)
            {
                goto L1300;
            }

            // !OUT?
            game.Flags.trollf = true;
            // !PERMIT EXITS.
            game.Objects[ObjectIds.Axe].Flag1 &= ~ObjectFlags.IsVisible;
            game.Objects[ObjectIds.Troll].Description1Id = 435;
            // !TROLL OUT.
            return ret_val;

            L1300:
            if (game.ParserVectors.prsa != VerbIds.inxw)
            {
                goto L1400;
            }
            // !WAKE UP?
            game.Flags.trollf = false;
            // !FORBID EXITS.
            game.Objects[ObjectIds.Axe].Flag1 |= ObjectFlags.IsVisible;
            game.Objects[ObjectIds.Troll].Description1Id = 436;
            // !TROLL IN.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Troll, game.Player.Here, game))
            {
                MessageHandler.rspsub_(game, 437);
            }
            return ret_val;

            L1400:
            if (game.ParserVectors.prsa != VerbIds.frstqw)
            {
                goto L1500;
            }
            // !FIRST ENCOUNTER?
            ret_val = RoomHandler.prob_(game, 33, 66);
            // !33% TRUE UNLESS BADLK.
            return ret_val;

            L1500:
            if (game.ParserVectors.prsa != VerbIds.Move
                && game.ParserVectors.prsa != VerbIds.Take
                && game.ParserVectors.prsa != VerbIds.Mung
                && game.ParserVectors.prsa != VerbIds.Throw
                && game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L2000;
            }

            if (game.Objects[ObjectIds.Troll].Capacity >= 0)
            {
                goto L1550;
            }

            // !TROLL OUT?
            game.Objects[ObjectIds.Troll].Capacity = -game.Objects[ObjectIds.Troll].Capacity;
            // !YES, WAKE HIM.
            game.Objects[ObjectIds.Axe].Flag1 |= ObjectFlags.IsVisible;
            game.Flags.trollf = false;
            game.Objects[ObjectIds.Troll].Description1Id = 436;
            MessageHandler.rspsub_(game, 437);

            L1550:
            if (game.ParserVectors.prsa != VerbIds.Take && game.ParserVectors.prsa != VerbIds.Move)
            {
                goto L1600;
            }

            MessageHandler.rspsub_(game, 438);
            // !JOKE.
            return ret_val;

            L1600:
            if (game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L1700;
            }

            // !MUNG?
            MessageHandler.rspeak_(game, 439);
            // !JOKE.
            return ret_val;

            L1700:
            if (game.ParserVectors.DirectObject == 0)
            {
                goto L10;
            }
            // !NO OBJECT?
            i = 440;
            // !ASSUME THROW.
            if (game.ParserVectors.prsa == VerbIds.Give)
            {
                i = 441;
            }

            // !GIVE?
            MessageHandler.rspsub_(game, i, game.Objects[game.ParserVectors.DirectObject].Description2Id);

            // !TROLL TAKES.
            if (game.ParserVectors.DirectObject == ObjectIds.Knife)
            {
                goto L1900;
            }

            // !OBJ KNIFE?
            ObjectHandler.SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 442, 0, 0, 0, game);
            // !NO, EATS IT.
            return ret_val;

            L1900:
            MessageHandler.rspeak_(game, 443);
            // !KNIFE, THROWS IT BACK
            game.Objects[ObjectIds.Troll].Flag2 |= ObjectFlags2.IsFighting;
            return ret_val;

            L2000:
            if (!game.Flags.trollf || game.ParserVectors.prsa != VerbIds.Hello)
            {
                goto L10;
            }

            MessageHandler.rspeak_(game, 366);
            // !TROLL OUT.
            return ret_val;

            L10:
            ret_val = false;
            // !COULDNT HANDLE IT.
            return ret_val;
        }

        // CYCLOP-	CYCLOPS FUNCTION
        public static bool cyclop_(Game game, int arg)
        {
            int i__1, i__2;
            bool ret_val;
            int i;

            ret_val = true;
            // !ASSUME WINS.
            if (!game.Flags.cyclof)
            {
                goto L100;
            }

            // !ASLEEP?
            if (game.ParserVectors.prsa != VerbIds.alarmw
                && game.ParserVectors.prsa != VerbIds.Mung
                && game.ParserVectors.prsa != VerbIds.Hello
                && game.ParserVectors.prsa != VerbIds.Burn
                && game.ParserVectors.prsa != VerbIds.Kill
                && game.ParserVectors.prsa != VerbIds.Attack)
            {
                goto L10;
            }

            game.Flags.cyclof = false;
            // !WAKE CYCLOPS.
            MessageHandler.rspsub_(game, 187);
            // !DESCRIBE.
            game.Switches.rvcyc = Math.Abs(game.Switches.rvcyc);
            game.Objects[ObjectIds.Cyclops].Flag2 = (game.Objects[ObjectIds.Cyclops].Flag2 | ObjectFlags2.IsFighting) & ~ObjectFlags2.IsSleeping;
            return ret_val;

            L100:
            if (game.ParserVectors.prsa == VerbIds.Fight
                || game.ParserVectors.prsa == VerbIds.frstqw)
            {
                goto L10;
            }

            if (Math.Abs(game.Switches.rvcyc) <= 5)
            {
                goto L200;
            }

            // !ANNOYED TOO MUCH?
            game.Switches.rvcyc = 0;
            // !RESTART COUNT.
            AdventurerHandler.jigsup_(game, 188);
            // !YES, EATS PLAYER.
            return ret_val;

            L200:
            if (game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L500;
            }

            // !GIVE?
            if (game.ParserVectors.DirectObject != ObjectIds.Food || game.Switches.rvcyc < 0)
            {
                goto L300;
            }

            // !FOOD WHEN HUNGRY?
            ObjectHandler.SetNewObjectStatus(ObjectIds.Food, 189, 0, 0, 0, game);
            // !EATS PEPPERS.
            // Computing MIN
            i__1 = -1;
            i__2 = -game.Switches.rvcyc;
            game.Switches.rvcyc = Math.Min(i__1, i__2);
            // !GETS THIRSTY.
            return ret_val;

            L300:
            if (game.ParserVectors.DirectObject != ObjectIds.Water)
            {
                goto L400;
            }

            // !DRINK WHEN THIRSTY?
            if (game.Switches.rvcyc >= 0)
            {
                goto L350;
            }

            ObjectHandler.SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 190, 0, 0, 0, game);
            // !DRINKS AND
            game.Flags.cyclof = true;
            // !FALLS ASLEEP.
            game.Objects[ObjectIds.Cyclops].Flag2 = (game.Objects[ObjectIds.Cyclops].Flag2 | ObjectFlags2.IsSleeping) & ~ObjectFlags2.IsFighting;
            return ret_val;

            L350:
            MessageHandler.rspeak_(game, 191);
            // !NOT THIRSTY.
            L10:
            ret_val = false;
            // !FAILS.
            return ret_val;

            L400:
            i = 192;
            // !ASSUME INEDIBLE.
            if (game.ParserVectors.DirectObject == ObjectIds.Garlic)
            {
                i = 193;
            }

            // !GARLIC IS JOKE.
            L450:
            MessageHandler.rspeak_(game, i);

            // !DISDAIN IT.
            if (game.Switches.rvcyc < 0)
            {
                --game.Switches.rvcyc;
            }

            if (game.Switches.rvcyc >= 0)
            {
                ++game.Switches.rvcyc;
            }

            if (!game.Flags.cyclof)
            {
                i__1 = Math.Abs(game.Switches.rvcyc) + 193;
                MessageHandler.rspeak_(game, i__1);
            }

            return ret_val;

            L500:
            i = 0;
            // !ASSUME NOT HANDLED.
            if (game.ParserVectors.prsa == VerbIds.Hello)
            {
                goto L450;
            }

            // !HELLO IS NO GO.
            if (game.ParserVectors.prsa == VerbIds.Throw || game.ParserVectors.prsa == VerbIds.Mung)
            {
                i = game.rnd_(2) + 200;
            }

            if (game.ParserVectors.prsa == VerbIds.Take)
            {
                i = 202;
            }

            if (game.ParserVectors.prsa == VerbIds.Tie)
            {
                i = 203;
            }

            if (i <= 0)
            {
                goto L10;
            }
            else
            {
                goto L450;
            }

            // !SEE IF HANDLED.

        }

        // THIEFP-	THIEF FUNCTION
        public static bool thiefp_(Game game, int arg)
        {
            int i__1;
            bool ret_val;
            ObjectIds i, j;

            ret_val = true;
            // !ASSUME WINS.
            if (game.ParserVectors.prsa != VerbIds.Fight)
            {
                goto L100;
            }

            // !FIGHT?
            if (game.Objects[ObjectIds.Stilletto].Container == ObjectIds.Thief)
            {
                goto L10;
            }

            // !GOT STILLETTO?  F.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Stilletto, game.Hack.ThiefPosition, game))
            {
                goto L50;
            }

            // !CAN HE RECOVER IT?
            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, 0, 0, 0, 0, game);
            // !NO, VANISH.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Thief, game.Player.Here, game))
            {
                MessageHandler.rspeak_(game, 498);
            }

            // !IF HERO, TELL.
            return ret_val;

            L50:
            ObjectHandler.SetNewObjectStatus(ObjectIds.Stilletto, 0, 0, ObjectIds.Thief, 0, game);
            // !YES, RECOVER.
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Thief, game.Player.Here, game))
            {
                MessageHandler.rspeak_(game, 499);
            }

            // !IF HERO, TELL.
            return ret_val;

            L100:
            if (game.ParserVectors.prsa != VerbIds.Dead)
            {
                goto L200;
            }

            // !DEAD?
            game.Hack.IsThiefActive = false;
            // !DISABLE DEMON.
            game.Objects[ObjectIds.chali].Flag1 |= ObjectFlags.IsTakeable;
            j = 0;
            i__1 = game.Objects.Count;

            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !CARRYING ANYTHING?
                // L125:
                if (game.Objects[i].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
                {
                    j = (ObjectIds)500;
                }
            }

            MessageHandler.rspeak_(game, j);
            // !TELL IF BOOTY REAPPEARS.

            j = (ObjectIds)501;
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !LOOP.
                if (i == ObjectIds.chali
                    || i == ObjectIds.Thief
                    || game.Player.Here != RoomIds.Treasure
                    || !ObjectHandler.IsObjectInRoom(i, game.Player.Here, game))
                {
                    goto L135;
                }

                game.Objects[i].Flag1 |= ObjectFlags.IsVisible;
                // !DESCRIBE.
                MessageHandler.rspsub_(j, game.Objects[i].Description2Id, game);

                j = (ObjectIds)502;
                goto L150;

                L135:
                if (game.Objects[i].Adventurer == (ActorIds)(-(int)ObjectIds.Thief))
                {
                    ObjectHandler.SetNewObjectStatus((ObjectIds)i, 0, game.Player.Here, 0, 0, game);
                }
                L150:
                ;
            }
            return ret_val;

            L200:
            if (game.ParserVectors.prsa != VerbIds.frstqw)
            {
                goto L250;
            }
            // !FIRST ENCOUNTER?
            ret_val = RoomHandler.prob_(game, 20, 75);
            return ret_val;

            L250:
            if (game.ParserVectors.prsa != VerbIds.Hello || game.Objects[ObjectIds.Thief].Description1Id != 504)
            {
                goto L300;
            }

            MessageHandler.rspeak_(game, 626);
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != VerbIds.OutCold)
            {
                goto L400;
            }

            // !OUT?
            game.Hack.IsThiefActive = false;
            // !DISABLE DEMON.
            game.Objects[ObjectIds.Thief].Description1Id = 504;
            // !CHANGE DESCRIPTION.
            game.Objects[ObjectIds.Stilletto].Flag1 &= ~ObjectFlags.IsVisible;
            game.Objects[ObjectIds.chali].Flag1 |= ObjectFlags.IsTakeable;
            return ret_val;

            L400:
            if (game.ParserVectors.prsa != VerbIds.inxw)
            {
                goto L500;
            }

            // !IN?
            // !CAN HERO SEE?
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Thief, game.Player.Here, game))
            {
                MessageHandler.rspeak_(game, 505);
            }

            // !ENABLE DEMON.
            game.Hack.IsThiefActive = true;

            // !CHANGE DESCRIPTION.
            game.Objects[ObjectIds.Thief].Description1Id = 503;
            game.Objects[ObjectIds.Stilletto].Flag1 |= ObjectFlags.IsVisible;

            if (game.Player.Here == RoomIds.Treasure && ObjectHandler.IsObjectInRoom(ObjectIds.chali, game.Player.Here, game))
            {
                game.Objects[ObjectIds.chali].Flag1 &= ~ObjectFlags.IsTakeable;
            }

            return ret_val;

            L500:
            // !TAKE?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L600;
            }

            // !JOKE.
            MessageHandler.rspeak_(game, 506);
            return ret_val;

            L600:
            if (game.ParserVectors.prsa != VerbIds.Throw || game.ParserVectors.DirectObject != ObjectIds.Knife ||
                (game.Objects[ObjectIds.Thief].Flag2 & ObjectFlags2.IsFighting) != 0)
            {
                goto L700;
            }

            if (RoomHandler.prob_(game, 10, 10))
            {
                goto L650;
            }

            // !THREW KNIFE, 10%?
            MessageHandler.rspeak_(game, 507);
            // !NO, JUST MAKES
            game.Objects[ObjectIds.Thief].Flag2 |= ObjectFlags2.IsFighting;
            return ret_val;

            L650:
            j = (ObjectIds)508;
            // !THIEF DROPS STUFF.
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                if (game.Objects[i].Adventurer != (ActorIds)(-(int)ObjectIds.Thief))
                {
                    goto L675;
                }
                // !THIEF CARRYING?
                j = (ObjectIds)509;
                ObjectHandler.SetNewObjectStatus(i, 0, game.Player.Here, 0, 0, game);
                L675:
                ;
            }

            ObjectHandler.SetNewObjectStatus(ObjectIds.Thief, (int)j, 0, 0, 0, game);
            // !THIEF VANISHES.
            return ret_val;

            L700:
            if (game.ParserVectors.prsa != VerbIds.Throw
                && game.ParserVectors.prsa != VerbIds.Give
                || game.ParserVectors.DirectObject == 0
                || game.ParserVectors.DirectObject == ObjectIds.Thief)
            {
                goto L10;
            }

            if (game.Objects[ObjectIds.Thief].Capacity >= 0)
            {
                goto L750;
            }

            // !WAKE HIM UP.
            game.Objects[ObjectIds.Thief].Capacity = -game.Objects[ObjectIds.Thief].Capacity;
            game.Hack.IsThiefActive = true;
            game.Objects[ObjectIds.Stilletto].Flag1 |= ObjectFlags.IsVisible;
            game.Objects[ObjectIds.Thief].Description1Id = 503;
            MessageHandler.rspeak_(game, 510);

            L750:
            if (game.ParserVectors.DirectObject != ObjectIds.Brick
                || game.Objects[ObjectIds.Fuse].Container != ObjectIds.Brick
                || game.Clock.Ticks[(int)ClockIndices.cevfus - 1] == 0)
            {
                goto L800;
            }

            MessageHandler.rspsub_(game, 511);
            // !THIEF REFUSES BOMB.
            return ret_val;

            L800:
            i__1 = -(int)ObjectIds.Thief;
            ObjectHandler.SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 0, 0, 0, (ActorIds)i__1, game);

            // !THIEF TAKES GIFT.
            if (game.Objects[game.ParserVectors.DirectObject].otval > 0)
            {
                goto L900;
            }

            // !A TREASURE?
            MessageHandler.rspsub_(game, 512, game.Objects[game.ParserVectors.DirectObject].Description2Id);
            return ret_val;

            L900:
            MessageHandler.rspsub_(game, 627, game.Objects[game.ParserVectors.DirectObject].Description2Id);
            // !THIEF ENGROSSED.
            game.Flags.IsThiefEngrossed = true;
            return ret_val;

            L10:
            ret_val = false;
            return ret_val;
        }
    }
}