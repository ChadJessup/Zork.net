using System;
using System.Collections.Generic;
using Zork.Core.Object;
using Zork.Core.Room;

namespace Zork.Core.Clock
{
    public class ClockEvents
    {
        public int Count { get; set; }
        public List<int> Ticks { get; } = new List<int>(25);
        public List<int> Actions { get; } = new List<int>(25);
        public List<bool> Flags { get; } = new List<bool>(25);

        /// <summary>
        /// clockd_ - Clock demon for intermove clock events.
        /// </summary>
        /// <returns></returns>
        public static bool clockd_(Game game)
        {
            /* System generated locals */
            int i__1;
            bool ret_val;

            /* Local variables */
            int i;

            ret_val = false;
            /* 						!ASSUME NO ACTION. */
            i__1 = game.Clock.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (!game.Clock.Flags[i - 1] || game.Clock.Ticks[i - 1] == 0)
                {
                    goto L100;
                }

                if (game.Clock.Ticks[i - 1] < 0)
                {
                    goto L50;
                }

                /* 						!PERMANENT ENTRY? */
                --game.Clock.Ticks[i - 1];
                if (game.Clock.Ticks[i - 1] != 0)
                {
                    goto L100;
                }
                /* 						!TIMER EXPIRED? */
                L50:
                ret_val = true;
                cevapp_(game, game.Clock.Actions[i - 1]);
                /* 						!DO ACTION. */
                L100:
                ;
            }
            return ret_val;

        }

        /* CEVAPP- CLOCK EVENT APPLICABLES */
        public static void cevapp_(Game game, int ri)
        {
            int[] cndtck = { 50, 20, 10, 5, 0, 156, 156, 156, 157, 0 };
            int[] lmptck = { 50, 30, 20, 10, 4, 0, 154, 154, 154, 154, 155, 0 };

            int i__1, i__2;
            bool f;
            int i, j, bc, br;

            if (ri == 0) {
                return;
            }
            /* 						!IGNORE DISABLED. */
            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L2000;
                case 3: goto L3000;
                case 4: goto L4000;
                case 5: goto L5000;
                case 6: goto L6000;
                case 7: goto L7000;
                case 8: goto L8000;
                case 9: goto L9000;
                case 10: goto L10000;
                case 11: goto L11000;
                case 12: goto L12000;
                case 13: goto L13000;
                case 14: goto L14000;
                case 15: goto L15000;
                case 16: goto L16000;
                case 17: goto L17000;
                case 18: goto L18000;
                case 19: goto L19000;
                case 20: goto L20000;
                case 21: goto L21000;
                case 22: goto L22000;
                case 23: goto L23000;
                case 24: goto L24000;
            }

            throw new InvalidOperationException();
            //bug_(3, ri);

            /* CEV1--	CURE CLOCK.  LET PLAYER SLOWLY RECOVER. */

            L1000:
            /* Computing MIN */
            i__1 = 0;
            i__2 = game.Adventurers.astren[(int)AIndices.player - 1] + 1;
            game.Adventurers.astren[(int)AIndices.player - 1] = Math.Min(i__1, i__2);

            /* 						!RECOVER. */
            if (game.Adventurers.astren[(int)AIndices.player - 1] >= 0)
            {
                return;
            }

            /* 						!FULLY RECOVERED? */
            game.Clock.Ticks[(int)ClockIndices.cevcur - 1] = 30;
            /* 						!NO, WAIT SOME MORE. */
            return;

            /* CEV2--	MAINT-ROOM WITH LEAK.  RAISE THE WATER LEVEL. */

            L2000:
            if (game.Player.Here == (int)RoomIndices.maint)
            {
                i__1 = game.Switch.rvmnt / 2 + 71;
                MessageHandler.rspeak_(game, i__1);
            }

            /* 						!DESCRIBE. */
            ++game.Switch.rvmnt;
            /* 						!RAISE WATER LEVEL. */
            if (game.Switch.rvmnt <= 16)
            {
                return;
            }

            /* 						!IF NOT FULL, EXIT. */
            game.Clock.Ticks[(int)ClockIndices.cevmnt - 1] = 0;
            /* 						!FULL, DISABLE CLOCK. */
            game.Rooms.RoomFlags[(int)RoomIndices.maint - 1] |= RoomFlags.RMUNG;
            game.Rooms.RoomActions[(int)RoomIndices.maint - 1] = 80;

            /* 						!SAY IT IS FULL OF WATER. */
            if (game.Player.Here == (int)RoomIndices.maint)
            {
                AdventurerHandler.jigsup_(game, 81);
            }

            /* 						!DROWN HIM IF PRESENT. */
            return;

            /* CEV3--	LANTERN.  DESCRIBE GROWING DIMNESS. */

            L3000:
            litint_(game, (int)ObjectIndices.lamp, game.Switch.orlamp, (int)ClockIndices.cevlnt, lmptck, 12);
            /* 						!DO LIGHT INTERRUPT. */
            return;

            /* CEV4--	MATCH.  OUT IT GOES. */

            L4000:
            MessageHandler.rspeak_(game, 153);
            /* 						!MATCH IS OUT. */
            game.Objects.oflag1[(int)ObjectIndices.match - 1] &= ~ObjectFlags.ONBT;
            return;

            /* CEV5--	CANDLE.  DESCRIBE GROWING DIMNESS. */

            L5000:
            litint_(game, (int)ObjectIndices.candl, game.Switch.orcand, (int)ClockIndices.cevcnd, cndtck, 10);
            /* 						!DO CANDLE INTERRUPT. */
            return;
            /* CEVAPP, PAGE 3 */

            /* CEV6--	BALLOON */

            L6000:
            game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            /* 						!RESCHEDULE INTERRUPT. */
            f = game.Adventurers.Vehicles[game.Player.Winner - 1] == (int)ObjectIndices.ballo;
            /* 						!SEE IF IN BALLOON. */
            if (game.State.bloc == (int)RoomIndices.vlbot)
            {
                goto L6800;
            }
            /* 						!AT BOTTOM? */
            if (game.State.bloc == (int)RoomIndices.ledg2 || game.State.bloc == (int)RoomIndices.ledg3 ||
                game.State.bloc == (int)RoomIndices.ledg4 || game.State.bloc == (int)RoomIndices.vlbot)
            {
                goto L6700;
            }

            /* 						!ON LEDGE? */
            if ((game.Objects.oflag2[(int)ObjectIndices.recep - 1] & ObjectFlags2.OPENBT) != 0 && game.Switch.binff != 0) {
                goto L6500;
            }

            /* BALLOON IS IN MIDAIR AND IS DEFLATED (OR HAS RECEPTACLE CLOSED). */
            /* FALL TO NEXT ROOM. */

            if (game.State.bloc != (int)RoomIndices.vair1) {
                goto L6300;
            }
            /* 						!IN VAIR1? */
            game.State.bloc = (int)RoomIndices.vlbot;
            /* 						!YES, NOW AT VLBOT. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, game.State.bloc, 0, 0);
            if (f) {
                goto L6200;
            }

            /* 						!IN BALLOON? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot)
            {
                MessageHandler.rspeak_(game, 530);
            }

            /* 						!ON LEDGE, DESCRIBE. */
            return;

            L6200:
            f = AdventurerHandler.moveto_(game, game.State.bloc, game.Player.Winner);
            /* 						!MOVE HIM. */
            if (game.Switch.binff == 0) {
                goto L6250;
            }
            /* 						!IN BALLOON.  INFLATED? */
            MessageHandler.rspeak_(game, 531);
            /* 						!YES, LANDED. */
            f = RoomHandler.RoomDescription(0, game);
            /* 						!DESCRIBE. */
            return;

            L6250:
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 532, 0, 0, 0);
            /* 						!NO, BALLOON & CONTENTS DIE. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.dball, 0, game.State.bloc, 0, 0);
            /* 						!INSERT DEAD BALLOON. */
            game.Adventurers.Vehicles[game.Player.Winner - 1] = 0;
            /* 						!NOT IN VEHICLE. */
            game.Clock.Flags[(int)ClockIndices.cevbal - 1] = false;
            /* 						!DISABLE INTERRUPTS. */
            game.Clock.Flags[(int)ClockIndices.cevbrn - 1] = false;
            game.Switch.binff = 0;
            game.Switch.btief = 0;
            return;

            L6300:
            --game.State.bloc;
            /* 						!NOT IN VAIR1, DESCEND. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, game.State.bloc, 0, 0);
            if (f) {
                goto L6400;
            }
            /* 						!IS HE IN BALLOON? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot)
            {
                MessageHandler.rspeak_(game, 533);
            }
            /* 						!IF ON LEDGE, DESCRIBE. */
            return;

            L6400:
            f = AdventurerHandler.moveto_(game, game.State.bloc, game.Player.Winner);
            /* 						!IN BALLOON, MOVE HIM. */
            MessageHandler.rspeak_(game, 534);
            /* 						!DESCRIBE. */
            f = RoomHandler.RoomDescription(0, game);
            return;

            /* BALLOON IS IN MIDAIR AND IS INFLATED, UP-UP-AND-AWAY */
            /* 						! */

            L6500:
            if (game.State.bloc != (int)RoomIndices.vair4)
            {
                goto L6600;
            }

            /* 						!AT VAIR4? */
            game.Clock.Ticks[(int)ClockIndices.cevbrn - 1] = 0;
            game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 0;
            game.Switch.binff = 0;
            game.Switch.btief = 0;
            game.State.bloc = (int)RoomIndices.vlbot;
            /* 						!FALL TO BOTTOM. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, 0, 0, 0);
            /* 						!BALLOON & CONTENTS DIE. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.dball, 0, game.State.bloc, 0, 0);
            /* 						!SUBSTITUTE DEAD BALLOON. */
            if (f) {
                goto L6550;
            }
            /* 						!WAS HE IN IT? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot) {
                MessageHandler.rspeak_(game, 535);
            }
            /* 						!IF HE CAN SEE, DESCRIBE. */
            return;

            L6550:
            AdventurerHandler.jigsup_(game, 536);
            /* 						!IN BALLOON AT CRASH, DIE. */
            return;

            L6600:
            ++game.State.bloc;
            /* 						!NOT AT VAIR4, GO UP. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, game.State.bloc, 0, 0);
            if (f)
            {
                goto L6650;
            }

            /* 						!IN BALLOON? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot)
            {
                MessageHandler.rspeak_(game, 537);
            }
            /* 						!CAN HE SEE IT? */
            return;

            L6650:
            f = AdventurerHandler.moveto_(game, game.State.bloc, game.Player.Winner);
            /* 						!MOVE PLAYER. */
            MessageHandler.rspeak_(game, 538);
            /* 						!DESCRIBE. */
            f = RoomHandler.RoomDescription(0, game);
            return;

            /* ON LEDGE, GOES TO MIDAIR ROOM WHETHER INFLATED OR NOT. */

            L6700:
            game.State.bloc += (int)RoomIndices.vair2 - (int)RoomIndices.ledg2;
            /* 						!MOVE TO MIDAIR. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, game.State.bloc, 0, 0);
            if (f) {
                goto L6750;
            }
            /* 						!IN BALLOON? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot) {
                MessageHandler.rspeak_(game, 539);
            }
            /* 						!NO, STRANDED. */
            game.Clock.Ticks[(int)ClockIndices.cevvlg - 1] = 10;
            /* 						!MATERIALIZE GNOME. */
            return;

            L6750:
            f = AdventurerHandler.moveto_(game, game.State.bloc, game.Player.Winner);
            /* 						!MOVE TO NEW ROOM. */
            MessageHandler.rspeak_(game, 540);
            /* 						!DESCRIBE. */
            f = RoomHandler.RoomDescription(0, game);
            return;

            /* AT BOTTOM, GO UP IF INFLATED, DO NOTHING IF DEFLATED. */

            L6800:
            if (game.Switch.binff == 0 || !((game.Objects.oflag2[(int)ObjectIndices.recep - 1] & ObjectFlags2.OPENBT) != 0))
            {
                return;
            }

            game.State.bloc = (int)RoomIndices.vair1;
            /* 						!INFLATED AND OPEN, */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, game.State.bloc, 0, 0);
            /* 						!GO UP TO VAIR1. */
            if (f) {
                goto L6850;
            }
            /* 						!IN BALLOON? */
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot) {
                MessageHandler.rspeak_(game, 541);
            }
            /* 						!IF CAN SEE, DESCRIBE. */
            return;

            L6850:
            f = AdventurerHandler.moveto_(game, game.State.bloc, game.Player.Winner);
            /* 						!MOVE PLAYER. */
            MessageHandler.rspeak_(game, 542);
            f = RoomHandler.RoomDescription(0, game);
            return;
            /* CEVAPP, PAGE 4 */

            /* CEV7--	BALLOON BURNUP */

            L7000:
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!FIND BURNING OBJECT */
                if ((int)ObjectIndices.recep == game.Objects.ocan[i - 1] && (game.Objects.oflag1[i - 1] & ObjectFlags.FLAMBT) != 0)
                {
                    goto L7200;
                }
                /* L7100: */
            }

            throw new InvalidOperationException();
            //bug_(4, 0);

            L7200:
            ObjectHandler.newsta_(game, i, 0, 0, 0, 0);
            /* 						!VANISH OBJECT. */
            game.Switch.binff = 0;
            /* 						!UNINFLATED. */
            if (game.Player.Here == game.State.bloc)
            {
                MessageHandler.rspsub_(game, 292, game.Objects.odesc2[i - 1]);
            }
            /* 						!DESCRIBE. */
            return;

            /* CEV8--	FUSE FUNCTION */

            L8000:
            if (game.Objects.ocan[(int)ObjectIndices.fuse - 1] != (int)ObjectIndices.brick) {
                goto L8500;
            }
            /* 						!IGNITED BRICK? */
            br = game.Objects.oroom[(int)ObjectIndices.brick - 1];
            /* 						!GET BRICK ROOM. */
            bc = game.Objects.ocan[(int)ObjectIndices.brick - 1];
            /* 						!GET CONTAINER. */
            if (br == 0 && bc != 0) {
                br = game.Objects.oroom[bc - 1];
            }
            ObjectHandler.newsta_(game, (int)ObjectIndices.fuse, 0, 0, 0, 0);
            /* 						!KILL FUSE. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.brick, 0, 0, 0, 0);
            /* 						!KILL BRICK. */
            if (br != 0 && br != game.Player.Here) {
                goto L8100;
            }
            /* 						!BRICK ELSEWHERE? */

            game.Rooms.RoomFlags[game.Player.Here - 1] |= RoomFlags.RMUNG;
            game.Rooms.RoomActions[game.Player.Here - 1] = 114;
            /* 						!MUNG ROOM. */
            AdventurerHandler.jigsup_(game, 150);
            /* 						!DEAD. */
            return;

            L8100:
            MessageHandler.rspeak_(game, 151);
            /* 						!BOOM. */
            game.State.mungrm = br;
            /* 						!SAVE ROOM THAT BLEW. */
            game.Clock.Ticks[(int)ClockIndices.cevsaf - 1] = 5;
            /* 						!SET SAFE INTERRUPT. */
            if (br != (int)RoomIndices.msafe) {
                goto L8200;
            }
            /* 						!BLEW SAFE ROOM? */
            if (bc != (int)ObjectIndices.sslot) {
                return;
            }
            /* 						!WAS BRICK IN SAFE? */
            ObjectHandler.newsta_(game, (int)ObjectIndices.sslot, 0, 0, 0, 0);
            /* 						!KILL SLOT. */
            game.Objects.oflag2[(int)ObjectIndices.safe - 1] |= ObjectFlags2.OPENBT;
            game.Flags.safef = true;
            /* 						!INDICATE SAFE BLOWN. */
            return;

            L8200:
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!BLEW WRONG ROOM. */
                if (ObjectHandler.qhere_(game, i, br) && (game.Objects.oflag1[i - 1] & ObjectFlags.TAKEBT) != 0)
                {
                    ObjectHandler.newsta_(game, i, 0, 0, 0, 0);
                }
                /* L8250: */
            }

            if (br != (int)RoomIndices.lroom)
            {
                return;
            }
            /* 						!BLEW LIVING ROOM? */
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (game.Objects.ocan[i - 1] == (int)ObjectIndices.tcase)
                {
                    ObjectHandler.newsta_(game, i, 0, 0, 0, 0);
                }
                /* 						!KILL TROPHY CASE. */
                /* L8300: */
            }
            return;

            L8500:
            if (ObjectHandler.qhere_(game, (int)ObjectIndices.fuse, game.Player.Here) || game.Objects.oadv[(int)ObjectIndices.fuse - 1] == game.Player.Winner)
            {
                MessageHandler.rspeak_(game, 152);
            }
            ObjectHandler.newsta_(game, (int)ObjectIndices.fuse, 0, 0, 0, 0);
            /* 						!KILL FUSE. */
            return;
            /* CEVAPP, PAGE 5 */

            /* CEV9--	LEDGE MUNGE. */

            L9000:
            game.Rooms.RoomFlags[(int)RoomIndices.ledg4 - 1] |= RoomFlags.RMUNG;
            game.Rooms.RoomActions[(int)RoomIndices.ledg4 - 1] = 109;
            if (game.Player.Here == (int)RoomIndices.ledg4)
            {
                goto L9100;
            }

            /* 						!WAS HE THERE? */
            MessageHandler.rspeak_(game, 110);
            /* 						!NO, NARROW ESCAPE. */
            return;

            L9100:
            if (game.Adventurers.Vehicles[game.Player.Winner - 1] != 0)
            {
                goto L9200;
            }

            /* 						!IN VEHICLE? */
            AdventurerHandler.jigsup_(game, 111);
            /* 						!NO, DEAD. */
            return;

            L9200:
            if (game.Switch.btief != 0)
            {
                goto L9300;
            }
            /* 						!TIED TO LEDGE? */
            MessageHandler.rspeak_(game, 112);
            /* 						!NO, NO PLACE TO LAND. */
            return;

            L9300:
            game.State.bloc = (int)RoomIndices.vlbot;
            /* 						!YES, CRASH BALLOON. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.ballo, 0, 0, 0, 0);
            /* 						!BALLOON & CONTENTS DIE. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.dball, 0, game.State.bloc, 0, 0);
            /* 						!INSERT DEAD BALLOON. */
            game.Switch.btief = 0;
            game.Switch.binff = 0;
            game.Clock.Flags[(int)ClockIndices.cevbal - 1] = false;
            game.Clock.Flags[(int)ClockIndices.cevbrn - 1] = false;
            AdventurerHandler.jigsup_(game, 113);
            /* 						!DEAD */
            return;

            /* CEV10--	SAFE MUNG. */

            L10000:
            game.Rooms.RoomFlags[game.State.mungrm - 1] |= RoomFlags.RMUNG;
            game.Rooms.RoomActions[game.State.mungrm - 1] = 114;
            if (game.Player.Here == game.State.mungrm)
            {
                goto L10100;
            }

            /* 						!IS HE PRESENT? */
            MessageHandler.rspeak_(game, 115);
            /* 						!LET HIM KNOW. */
            if (game.State.mungrm == (int)RoomIndices.msafe)
            {
                game.Clock.Ticks[(int)ClockIndices.cevled - 1] = 8;
            }
            /* 						!START LEDGE CLOCK. */
            return;

            L10100:
            i = 116;
            /* 						!HE'S DEAD, */
            if ((game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RHOUSE) != 0)
            {
                i = 117;
            }

            AdventurerHandler.jigsup_(game, i);
            /* 						!LET HIM KNOW. */
            return;
            /* CEVAPP, PAGE 6 */

            /* CEV11--	VOLCANO GNOME */

            L11000:
            if (game.Player.Here == (int)RoomIndices.ledg2 || game.Player.Here == (int)RoomIndices.ledg3 ||
                game.Player.Here == (int)RoomIndices.ledg4 || game.Player.Here == (int)RoomIndices.vlbot) {
                goto L11100;
            }
            /* 						!IS HE ON LEDGE? */
            game.Clock.Ticks[(int)ClockIndices.cevvlg - 1] = 1;
            /* 						!NO, WAIT A WHILE. */
            return;

            L11100:
            ObjectHandler.newsta_(game, (int)ObjectIndices.gnome, 118, game.Player.Here, 0, 0);
            /* 						!YES, MATERIALIZE GNOME. */
            return;

            /* CEV12--	VOLCANO GNOME DISAPPEARS */

            L12000:
            ObjectHandler.newsta_(game, (int)ObjectIndices.gnome, 149, 0, 0, 0);
            /* 						!DISAPPEAR THE GNOME. */
            return;

            /* CEV13--	BUCKET. */

            L13000:
            if (game.Objects.ocan[(int)ObjectIndices.water - 1] == (int)ObjectIndices.bucke) {
                ObjectHandler.newsta_(game, (int)ObjectIndices.water, 0, 0, 0, 0);
            }
            return;

            /* CEV14--	SPHERE.  IF EXPIRES, HE'S TRAPPED. */

            L14000:
            game.Rooms.RoomFlags[(int)RoomIndices.cager - 1] |= RoomFlags.RMUNG;
            game.Rooms.RoomActions[(int)RoomIndices.cager - 1] = 147;
            AdventurerHandler.jigsup_(game, 148);
            /* 						!MUNG PLAYER. */
            return;

            /* CEV15--	END GAME HERALD. */

            L15000:
            game.Flags.endgmf = true;
            /* 						!WE'RE IN ENDGAME. */
            MessageHandler.rspeak_(game, 119);
            /* 						!INFORM OF ENDGAME. */
            return;
            /* CEVAPP, PAGE 7 */

            /* CEV16--	FOREST MURMURS */

            L16000:
            game.Clock.Flags[(int)ClockIndices.cevfor - 1] = game.Player.Here == (int)RoomIndices.mtree
                || game.Player.Here >= (int)RoomIndices.fore1 && game.Player.Here < (int)RoomIndices.clear;

            if (game.Clock.Flags[(int)ClockIndices.cevfor - 1] && RoomHandler.prob_(game, 10, 10))
            {
                MessageHandler.rspeak_(game, 635);
            }

            return;

            /* CEV17--	SCOL ALARM */

            L17000:
            if (game.Player.Here == (int)RoomIndices.bktwi) {
                game.Clock.Flags[(int)ClockIndices.cevzgi - 1] = true;
            }
            /* 						!IF IN TWI, GNOME. */
            if (game.Player.Here == (int)RoomIndices.bkvau) {
                AdventurerHandler.jigsup_(game, 636);
            }
            /* 						!IF IN VAU, DEAD. */
            return;

            /* CEV18--	ENTER GNOME OF ZURICH */

            L18000:
            game.Clock.Flags[(int)ClockIndices.cevzgo - 1] = true;
            /* 						!EXITS, TOO. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.zgnom, 0, (int)RoomIndices.bktwi, 0, 0);
            /* 						!PLACE IN TWI. */
            if (game.Player.Here == (int)RoomIndices.bktwi)
            {
                MessageHandler.rspeak_(game, 637);
            }
            /* 						!ANNOUNCE. */
            return;

            /* CEV19--	EXIT GNOME */

            L19000:
            ObjectHandler.newsta_(game, (int)ObjectIndices.zgnom, 0, 0, 0, 0);
            /* 						!VANISH. */
            if (game.Player.Here == (int)RoomIndices.bktwi) {
                MessageHandler.rspeak_(game, 638);
            }
            /* 						!ANNOUNCE. */
            return;
            /* CEVAPP, PAGE 8 */

            /* CEV20--	START OF ENDGAME */

            L20000:
            if (game.Flags.spellf) {
                goto L20200;
            }
            /* 						!SPELL HIS WAY IN? */
            if (game.Player.Here != (int)RoomIndices.crypt)
            {
                return;
            }
            /* 						!NO, STILL IN TOMB? */
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L20100;
            }
            /* 						!LIGHTS OFF? */
            game.Clock.Ticks[(int)ClockIndices.cevste - 1] = 3;
            /* 						!RESCHEDULE. */
            return;

            L20100:
            MessageHandler.rspeak_(game, 727);
            /* 						!ANNOUNCE. */
            L20200:
            i__1 = game.Objects.Count;
            for (i = 1; i <= i__1; ++i)
            {
                /* 						!STRIP HIM OF OBJS. */
                ObjectHandler.newsta_(game, i, 0, game.Objects.oroom[i - 1], game.Objects.ocan[i - 1], 0);
                /* L20300: */
            }
            ObjectHandler.newsta_(game, (int)ObjectIndices.lamp, 0, 0, 0, (int)AIndices.player);
            /* 						!GIVE HIM LAMP. */
            ObjectHandler.newsta_(game, (int)ObjectIndices.sword, 0, 0, 0, (int)AIndices.player);
            /* 						!GIVE HIM SWORD. */

            game.Objects.oflag1[(int)ObjectIndices.lamp - 1] = (game.Objects.oflag1[(int)ObjectIndices.lamp - 1] | ObjectFlags.LITEBT) & ~ObjectFlags.ONBT;
            game.Objects.oflag2[(int)ObjectIndices.lamp - 1] |= ObjectFlags2.TCHBT;
            game.Clock.Flags[(int)ClockIndices.cevlnt - 1] = false;
            /* 						!LAMP IS GOOD AS NEW. */
            game.Clock.Ticks[(int)ClockIndices.cevlnt - 1] = 350;
            game.Switch.orlamp = 0;
            game.Objects.oflag2[(int)ObjectIndices.sword - 1] |= ObjectFlags2.TCHBT;
            game.Hack.swdact = true;
            game.Hack.swdsta = 0;

            game.Hack.thfact = false;
            /* 						!THIEF GONE. */
            game.Flags.endgmf = true;
            /* 						!ENDGAME RUNNING. */
            game.Clock.Flags[(int)ClockIndices.cevmat - 1] = false;
            /* 						!MATCHES GONE, */
            game.Clock.Flags[(int)ClockIndices.cevcnd - 1] = false;
            /* 						!CANDLES GONE. */

            AdventurerHandler.scrupd_(game, game.Rooms.RoomValues[(int)RoomIndices.crypt - 1]);
            /* 						!SCORE CRYPT, */
            game.Rooms.RoomValues[(int)RoomIndices.crypt - 1] = 0;
            /* 						!BUT ONLY ONCE. */
            f = AdventurerHandler.moveto_(game, (int)RoomIndices.tstrs, game.Player.Winner);
            /* 						!TO TOP OF STAIRS, */
            f = RoomHandler.RoomDescription(3, game);
            /* 						!AND DESCRIBE. */
            return;
            /* 						!BAM */
            /* 						! */

            /* CEV21--	MIRROR CLOSES. */

            L21000:
            game.Flags.mrpshf = false;
            /* 						!BUTTON IS OUT. */
            game.Flags.mropnf = false;
            /* 						!MIRROR IS CLOSED. */
            if (game.Player.Here == (int)RoomIndices.mrant)
            {
                MessageHandler.rspeak_(game, 728);
            }

            /* 						!DESCRIBE BUTTON. */
            if (game.Player.Here == (int)RoomIndices.inmir || RoomHandler.mrhere_(game, game.Player.Here) == 1)
            {
                MessageHandler.rspeak_(game, 729);
            }
            return;
            /* CEVAPP, PAGE 9 */

            /* CEV22--	DOOR CLOSES. */

            L22000:
            if (game.Flags.wdopnf) {
                MessageHandler.rspeak_(game, 730);
            }
            /* 						!DESCRIBE. */
            game.Flags.wdopnf = false;
            /* 						!CLOSED. */
            return;

            /* CEV23--	INQUISITOR'S QUESTION */

            L23000:
            if (game.Adventurers.Rooms[(int)AIndices.player - 1] != (int)RoomIndices.fdoor) {
                return;
            }
            /* 						!IF PLAYER LEFT, DIE. */
            MessageHandler.rspeak_(game, 769);
            i__1 = game.Switch.quesno + 770;
            MessageHandler.rspeak_(game, i__1);
            game.Clock.Ticks[(int)ClockIndices.cevinq - 1] = 2;
            return;

            /* CEV24--	MASTER FOLLOWS */

            L24000:
            if (game.Adventurers.Rooms[(int)AIndices.amastr - 1] == game.Player.Here) {
                return;
            }
            /* 						!NO MOVEMENT, DONE. */
            if (game.Player.Here != (int)RoomIndices.cell && game.Player.Here != (int)RoomIndices.pcell)
            {
                goto L24100;
            }

            if (game.Flags.follwf)
            {
                MessageHandler.rspeak_(game, 811);
            }

            /* 						!WONT GO TO CELLS. */
            game.Flags.follwf = false;
            return;

            L24100:
            game.Flags.follwf = true;
            /* 						!FOLLOWING. */
            i = 812;
            /* 						!ASSUME CATCHES UP. */
            i__1 = (int)XSearch.xmax;
            i__2 = (int)XSearch.xmin;
            for (j = (int)XSearch.xmin; i__2 < 0 ? j >= i__1 : j <= i__1; j += i__2)
            {
                if (dso3.findxt_(game, j, game.Adventurers.Rooms[(int)AIndices.amastr - 1]) && game.curxt_.xroom1 == game.Player.Here)
                {
                    i = 813;
                }
                /* L24200: */
            }

            MessageHandler.rspeak_(game, i);
            ObjectHandler.newsta_(game, (int)ObjectIndices.master, 0, game.Player.Here, 0, 0);
            /* 						!MOVE MASTER OBJECT. */
            game.Adventurers.Rooms[(int)AIndices.amastr - 1] = game.Player.Here;
            /* 						!MOVE MASTER PLAYER. */
            return;

        }

        /* LITINT-	LIGHT INTERRUPT PROCESSOR */
        public static void litint_(Game game, int obj, int ctr, int cev, int[] ticks, int tickln)
        {
            /* Parameter adjustments */
            //--ticks;

            /* Function Body */
            ++(ctr);
            /* 						!ADVANCE STATE CNTR. */
            game.Clock.Ticks[cev - 1] = ticks[ctr];
            /* 						!RESET INTERRUPT. */
            if (game.Clock.Ticks[cev - 1] != 0) {
                goto L100;
            }
            /* 						!EXPIRED? */
            game.Objects.oflag1[obj - 1] &= ~((int)ObjectFlags.LITEBT + (int)ObjectFlags.FLAMBT + ObjectFlags.ONBT);
            if (game.Objects.oroom[obj - 1] == game.Player.Here || game.Objects.oadv[obj - 1] == game.Player.Winner)
            {
                MessageHandler.rspsub_(game, 293, game.Objects.odesc2[obj - 1]);
            }
            return;

            L100:
            if (game.Objects.oroom[obj - 1] == game.Player.Here || game.Objects.oadv[obj - 1] == game.Player.Winner)
            {
                MessageHandler.rspeak_(game, ticks[ctr + tickln / 2]);
            }

            return;
        }
    }
}
