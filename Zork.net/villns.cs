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
            if (game.ParserVectors.prsa != (int)VerbIndices.Fight) {
                goto L1100;
            }
            // !FIGHT?
            if (game.Objects.ocan[(int)ObjectIndices.axe - 1] == (int)ObjectIndices.troll) {
                goto L10;
            }
            // !GOT AXE?  NOTHING.
            i = 433;
            // !ASSUME CANT GET.
            if (!ObjectHandler.qhere_(game, (int)ObjectIndices.axe, game.Player.Here)) {
                goto L1050;
            }
            // !HERE?
            i = 434;
            // !YES, RECOVER.
            ObjectHandler.SetNewObjectStatus(game, (int)ObjectIndices.axe, 0, 0, (int)ObjectIndices.troll, 0);
            L1050:
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.troll, game.Player.Here))
            {
                MessageHandler.rspsub_(game, i);
            }
            // !IF PLAYER HERE.
            return ret_val;

            L1100:
            if (game.ParserVectors.prsa != (int)VerbIndices.deadxw) {
                goto L1200;
            }
            // !DEAD?
            game.Flags.trollf = true;
            // !PERMIT EXITS.
            return ret_val;

            L1200:
            if (game.ParserVectors.prsa != (int)VerbIndices.outxw) {
                goto L1300;
            }
            // !OUT?
            game.Flags.trollf = true;
            // !PERMIT EXITS.
            game.Objects.oflag1[(int)ObjectIndices.axe - 1] &= ~ObjectFlags.IsVisible;
            game.Objects.odesc1[(int)ObjectIndices.troll - 1] = 435;
            // !TROLL OUT.
            return ret_val;

            L1300:
            if (game.ParserVectors.prsa != (int)VerbIndices.inxw) {
                goto L1400;
            }
            // !WAKE UP?
            game.Flags.trollf = false;
            // !FORBID EXITS.
            game.Objects.oflag1[(int)ObjectIndices.axe - 1] |= ObjectFlags.IsVisible;
            game.Objects.odesc1[(int)ObjectIndices.troll - 1] = 436;
            // !TROLL IN.
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.troll, game.Player.Here)) {
                MessageHandler.rspsub_(game, 437);
            }
            return ret_val;

            L1400:
            if (game.ParserVectors.prsa != (int)VerbIndices.frstqw) {
                goto L1500;
            }
            // !FIRST ENCOUNTER?
            ret_val = RoomHandler.prob_(game, 33, 66);
            // !33% TRUE UNLESS BADLK.
            return ret_val;

            L1500:
            if (game.ParserVectors.prsa != (int)VerbIndices.movew && game.ParserVectors.prsa != (int)VerbIndices.takew &&
                game.ParserVectors.prsa != (int)VerbIndices.mungw && game.ParserVectors.prsa !=
                (int)VerbIndices.throww && game.ParserVectors.prsa != (int)VerbIndices.givew) {
                goto L2000;
            }
            if (game.Objects.ocapac[(int)ObjectIndices.troll - 1] >= 0) {
                goto L1550;
            }
            // !TROLL OUT?
            game.Objects.ocapac[(int)ObjectIndices.troll - 1] = -game.Objects.ocapac[(int)ObjectIndices.troll - 1]
                ;
            // !YES, WAKE HIM.
            game.Objects.oflag1[(int)ObjectIndices.axe - 1] |= ObjectFlags.IsVisible;
            game.Flags.trollf = false;
            game.Objects.odesc1[(int)ObjectIndices.troll - 1] = 436;
            MessageHandler.rspsub_(game, 437);

            L1550:
            if (game.ParserVectors.prsa != (int)VerbIndices.takew && game.ParserVectors.prsa != (int)VerbIndices.movew) {
                goto L1600;
            }
            MessageHandler.rspsub_(game, 438);
            // !JOKE.
            return ret_val;

            L1600:
            if (game.ParserVectors.prsa != (int)VerbIndices.mungw) {
                goto L1700;
            }
            // !MUNG?
            MessageHandler.rspeak_(game, 439);
            // !JOKE.
            return ret_val;

            L1700:
            if (game.ParserVectors.prso == 0) {
                goto L10;
            }
            // !NO OBJECT?
            i = 440;
            // !ASSUME THROW.
            if (game.ParserVectors.prsa == (int)VerbIndices.givew) {
                i = 441;
            }
            // !GIVE?
            MessageHandler.rspsub_(game, i, game.Objects.odesc2[game.ParserVectors.prso - 1]);
            // !TROLL TAKES.
            if (game.ParserVectors.prso == (int)ObjectIndices.Knife) {
                goto L1900;
            }
            // !OBJ KNIFE?
            ObjectHandler.SetNewObjectStatus(game, game.ParserVectors.prso, 442, 0, 0, 0);
            // !NO, EATS IT.
            return ret_val;

            L1900:
            MessageHandler.rspeak_(game, 443);
            // !KNIFE, THROWS IT BACK
            game.Objects.oflag2[(int)ObjectIndices.troll - 1] |= ObjectFlags2.FITEBT;
            return ret_val;

            L2000:
            if (!game.Flags.trollf || game.ParserVectors.prsa != (int)VerbIndices.hellow) {
                goto L10;
            }
            MessageHandler.rspeak_(game, 366);
            // !TROLL OUT.
            return ret_val;

            L10:
            ret_val = false;
            // !COULDNT HANDLE IT.
            return ret_val;
        } // trollp_

        // CYCLOP-	CYCLOPS FUNCTION

        // DECLARATIONS

        public static bool cyclop_(Game game, int arg)
        {
            // System generated locals
            int i__1, i__2;
            bool ret_val;

            // Local variables
            int i;

            ret_val = true;
            // !ASSUME WINS.
            if (!game.Flags.cyclof) {
                goto L100;
            }

            // !ASLEEP?
            if (game.ParserVectors.prsa != (int)VerbIndices.alarmw && game.ParserVectors.prsa != (int)VerbIndices.mungw &&
                 game.ParserVectors.prsa != (int)VerbIndices.hellow && game.ParserVectors.prsa !=
                (int)VerbIndices.burnw && game.ParserVectors.prsa != (int)VerbIndices.killw &&
                game.ParserVectors.prsa != (int)VerbIndices.attacw) {
                goto L10;
            }
            game.Flags.cyclof = false;
            // !WAKE CYCLOPS.
            MessageHandler.rspsub_(game, 187);
            // !DESCRIBE.
            game.Switches.rvcyc = Math.Abs(game.Switches.rvcyc);
            game.Objects.oflag2[(int)ObjectIndices.cyclo - 1] = (game.Objects.oflag2[(int)ObjectIndices.cyclo - 1] | ObjectFlags2.FITEBT) & ~ObjectFlags2.SLEPBT;
            return ret_val;

            L100:
            if (game.ParserVectors.prsa == (int)VerbIndices.Fight || game.ParserVectors.prsa == (int)VerbIndices.frstqw)
            {
                goto L10;
            }
            if (Math.Abs(game.Switches.rvcyc) <= 5) {
                goto L200;
            }
            // !ANNOYED TOO MUCH?
            game.Switches.rvcyc = 0;
            // !RESTART COUNT.
            AdventurerHandler.jigsup_(game, 188);
            // !YES, EATS PLAYER.
            return ret_val;

            L200:
            if (game.ParserVectors.prsa != (int)VerbIndices.givew) {
                goto L500;
            }
            // !GIVE?
            if (game.ParserVectors.prso != (int)ObjectIndices.food || game.Switches.rvcyc < 0) {
                goto L300;
            }
            // !FOOD WHEN HUNGRY?
            ObjectHandler.SetNewObjectStatus(game, (int)ObjectIndices.food, 189, 0, 0, 0);
            // !EATS PEPPERS.
            // Computing MIN
            i__1 = -1;
            i__2 = -game.Switches.rvcyc;
            game.Switches.rvcyc = Math.Min(i__1, i__2);
            // !GETS THIRSTY.
            return ret_val;

            L300:
            if (game.ParserVectors.prso != (int)ObjectIndices.Water) {
                goto L400;
            }
            // !DRINK WHEN THIRSTY?
            if (game.Switches.rvcyc >= 0) {
                goto L350;
            }
            ObjectHandler.SetNewObjectStatus(game, game.ParserVectors.prso, 190, 0, 0, 0);
            // !DRINKS AND
            game.Flags.cyclof = true;
            // !FALLS ASLEEP.
            game.Objects.oflag2[(int)ObjectIndices.cyclo - 1] = (game.Objects.oflag2[(int)ObjectIndices.cyclo - 1] | ObjectFlags2.SLEPBT) & ~ObjectFlags2.FITEBT;
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
            if (game.ParserVectors.prso == (int)ObjectIndices.garli) {
                i = 193;
            }
            // !GARLIC IS JOKE.
            L450:
            MessageHandler.rspeak_(game, i);
            // !DISDAIN IT.
            if (game.Switches.rvcyc < 0) {
                --game.Switches.rvcyc;
            }
            if (game.Switches.rvcyc >= 0) {
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
            if (game.ParserVectors.prsa == (int)VerbIndices.hellow) {
                goto L450;
            }
            // !HELLO IS NO GO.
            if (game.ParserVectors.prsa == (int)VerbIndices.throww || game.ParserVectors.prsa == (int)VerbIndices.mungw) {

                i = game.rnd_(2) + 200;
            }
            if (game.ParserVectors.prsa == (int)VerbIndices.takew) {
                i = 202;
            }
            if (game.ParserVectors.prsa == (int)VerbIndices.tiew) {
                i = 203;
            }
            if (i <= 0) {
                goto L10;
            } else {
                goto L450;
            }
            // !SEE IF HANDLED.

        } // cyclop_

        // THIEFP-	THIEF FUNCTION

        // DECLARATIONS

        public static bool thiefp_(Game game, int arg)
        {
            // System generated locals
            int i__1;
            bool ret_val;

            // Local variables
            int i, j;

            ret_val = true;
            // !ASSUME WINS.
            if (game.ParserVectors.prsa != (int)VerbIndices.Fight) {
                goto L100;
            }
            // !FIGHT?
            if (game.Objects.ocan[(int)ObjectIndices.still - 1] == (int)ObjectIndices.thief) {
                goto L10;
            }
            // !GOT STILLETTO?  F.
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.still, game.Hack.thfpos)) {
                goto L50;
            }
            // !CAN HE RECOVER IT?
            ObjectHandler.SetNewObjectStatus(game, (int)ObjectIndices.thief, 0, 0, 0, 0);
            // !NO, VANISH.
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.thief, game.Player.Here)) {
                MessageHandler.rspeak_(game, 498);
            }
            // !IF HERO, TELL.
            return ret_val;

            L50:
            ObjectHandler.SetNewObjectStatus(game, (int)ObjectIndices.still, 0, 0, (int)ObjectIndices.thief, 0);
            // !YES, RECOVER.
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.thief, game.Player.Here)) {
                MessageHandler.rspeak_(game, 499);
            }
            // !IF HERO, TELL.
            return ret_val;

            L100:
            if (game.ParserVectors.prsa != (int)VerbIndices.deadxw) {
                goto L200;
            }
            // !DEAD?
            game.Hack.thfact = false;
            // !DISABLE DEMON.
            game.Objects.oflag1[(int)ObjectIndices.chali - 1] |= ObjectFlags.TAKEBT;
            j = 0;
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i) {
                // !CARRYING ANYTHING?
                // L125:
                if (game.Objects.oadv[i - 1] == -(int)ObjectIndices.thief) {
                    j = 500;
                }
            }
            MessageHandler.rspeak_(game, j);
            // !TELL IF BOOTY REAPPEARS.

            j = 501;
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                // !LOOP.
                if (i == (int)ObjectIndices.chali
                    || i == (int)ObjectIndices.thief
                    || game.Player.Here != (int)RoomIndices.Treasure
                    || !ObjectHandler.qhere_(game, i, game.Player.Here))
                {
                    goto L135;
                }
                game.Objects.oflag1[i - 1] |= ObjectFlags.IsVisible;
                MessageHandler.rspsub_(game, j, game.Objects.odesc2[i - 1]);
                // !DESCRIBE.
                j = 502;
                goto L150;

                L135:
                if (game.Objects.oadv[i - 1] == -(int)ObjectIndices.thief)
                {
                    ObjectHandler.SetNewObjectStatus(game, i, 0, game.Player.Here, 0, 0);
                }
                L150:
                ;
            }
            return ret_val;

            L200:
            if (game.ParserVectors.prsa != (int)VerbIndices.frstqw) {
                goto L250;
            }
            // !FIRST ENCOUNTER?
            ret_val = RoomHandler.prob_(game, 20, 75);
            return ret_val;

            L250:
            if (game.ParserVectors.prsa != (int)VerbIndices.hellow || game.Objects.odesc1[(int)ObjectIndices.thief -
                1] != 504) {
                goto L300;
            }
            MessageHandler.rspeak_(game, 626);
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != (int)VerbIndices.outxw) {
                goto L400;
            }
            // !OUT?
            game.Hack.thfact = false;
            // !DISABLE DEMON.
            game.Objects.odesc1[(int)ObjectIndices.thief - 1] = 504;
            // !CHANGE DESCRIPTION.
            game.Objects.oflag1[(int)ObjectIndices.still - 1] &= ~ObjectFlags.IsVisible;
            game.Objects.oflag1[(int)ObjectIndices.chali - 1] |= ObjectFlags.TAKEBT;
            return ret_val;

            L400:
            if (game.ParserVectors.prsa != (int)VerbIndices.inxw) {
                goto L500;
            }
            // !IN?
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.thief, game.Player.Here)) {
                MessageHandler.rspeak_(game, 505);
            }
            // !CAN HERO SEE?
            game.Hack.thfact = true;
            // !ENABLE DEMON.
            game.Objects.odesc1[(int)ObjectIndices.thief - 1] = 503;
            // !CHANGE DESCRIPTION.
            game.Objects.oflag1[(int)ObjectIndices.still - 1] |= ObjectFlags.IsVisible;
            if (game.Player.Here == (int)RoomIndices.Treasure
                && ObjectHandler.qhere_(game, (int)ObjectIndices.chali, game.Player.Here))
            {
                game.Objects.oflag1[(int)ObjectIndices.chali - 1] &= ~ObjectFlags.TAKEBT;
            }
            return ret_val;

            L500:
            if (game.ParserVectors.prsa != (int)VerbIndices.takew) {
                goto L600;
            }
            // !TAKE?
            MessageHandler.rspeak_(game, 506);
            // !JOKE.
            return ret_val;

            L600:
            if (game.ParserVectors.prsa != (int)VerbIndices.throww || game.ParserVectors.prso != (int)ObjectIndices.Knife ||
                (game.Objects.oflag2[(int)ObjectIndices.thief - 1] & ObjectFlags2.FITEBT) != 0) {
                goto L700;
            }
            if (RoomHandler.prob_(game, 10, 10)) {
                goto L650;
            }
            // !THREW KNIFE, 10%?
            MessageHandler.rspeak_(game, 507);
            // !NO, JUST MAKES
            game.Objects.oflag2[(int)ObjectIndices.thief - 1] |= ObjectFlags2.FITEBT;
            return ret_val;

            L650:
            j = 508;
            // !THIEF DROPS STUFF.
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i) {
                if (game.Objects.oadv[i - 1] != -(int)ObjectIndices.thief) {
                    goto L675;
                }
                // !THIEF CARRYING?
                j = 509;
                ObjectHandler.SetNewObjectStatus(game, i, 0, game.Player.Here, 0, 0);
                L675:
                ;
            }
            ObjectHandler.SetNewObjectStatus(game, (int)ObjectIndices.thief, j, 0, 0, 0);
            // !THIEF VANISHES.
            return ret_val;

            L700:
            if (game.ParserVectors.prsa != (int)VerbIndices.throww && game.ParserVectors.prsa != (int)VerbIndices.givew ||
                game.ParserVectors.prso == 0 || game.ParserVectors.prso == (int)ObjectIndices.thief) {
                goto L10;
            }
            if (game.Objects.ocapac[(int)ObjectIndices.thief - 1] >= 0) {
                goto L750;
            }
            // !WAKE HIM UP.
            game.Objects.ocapac[(int)ObjectIndices.thief - 1] = -game.Objects.ocapac[(int)ObjectIndices.thief - 1];
            game.Hack.thfact = true;
            game.Objects.oflag1[(int)ObjectIndices.still - 1] |= ObjectFlags.IsVisible;
            game.Objects.odesc1[(int)ObjectIndices.thief - 1] = 503;
            MessageHandler.rspeak_(game, 510);

            L750:
            if (game.ParserVectors.prso != (int)ObjectIndices.brick || game.Objects.ocan[(int)ObjectIndices.fuse - 1] !=
                (int)ObjectIndices.brick || game.Clock.Ticks[(int)ClockIndices.cevfus - 1] == 0) {
                goto L800;
            }
            MessageHandler.rspsub_(game, 511);
            // !THIEF REFUSES BOMB.
            return ret_val;

            L800:
            i__1 = -(int)ObjectIndices.thief;
            ObjectHandler.SetNewObjectStatus(game, game.ParserVectors.prso, 0, 0, 0, i__1);
            // !THIEF TAKES GIFT.
            if (game.Objects.otval[game.ParserVectors.prso - 1] > 0) {
                goto L900;
            }
            // !A TREASURE?
            MessageHandler.rspsub_(game, 512, game.Objects.odesc2[game.ParserVectors.prso - 1]);
            return ret_val;

            L900:
            MessageHandler.rspsub_(game, 627, game.Objects.odesc2[game.ParserVectors.prso - 1]);
            // !THIEF ENGROSSED.
            game.Flags.thfenf = true;
            return ret_val;

            L10:
            ret_val = false;
            return ret_val;
        }

    }
}