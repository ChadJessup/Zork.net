using System;
using System.Collections.Generic;

namespace Zork.Core
{
    public class ClockEvents
    {
        public Dictionary<ClockId, ClockEvent> Events { get; set; } = new Dictionary<ClockId, ClockEvent>();

        //public List<int> Ticks { get; } = new List<int>(25);
        //public List<int> Actions { get; } = new List<int>(25);
        //public List<bool> Flags { get; } = new List<bool>(25);

        /// <summary>
        /// clockd_ - Clock demon for intermove clock events.
        /// </summary>
        /// <returns></returns>
        public static bool clockd_(Game game)
        {
            bool ret_val = false;

            // !ASSUME NO ACTION.
            foreach (var clockEvent in game.Clock.Values)
            {
                if (!clockEvent.Flags || clockEvent.Ticks == 0)
                {
                    goto L100;
                }

                if (clockEvent.Ticks < 0)
                {
                    goto L50;
                }

                // !PERMANENT ENTRY?
                --clockEvent.Ticks;
                if (clockEvent.Ticks!= 0)
                {
                    goto L100;
                }

                // !TIMER EXPIRED?
                L50:
                ret_val = true;
                ClockEventRoutines(game, clockEvent.Actions);
                // !DO ACTION.
                L100:
                ;
            }

            return ret_val;
        }

        // CEVAPP- CLOCK EVENT APPLICABLES
        public static void ClockEventRoutines(Game game, int ri)
        {
            int[] cndtck = { 50, 20, 10, 5, 0, 156, 156, 156, 157, 0 };
            int[] lmptck = { 50, 30, 20, 10, 4, 0, 154, 154, 154, 154, 155, 0 };

            int i__1, i__2;
            bool f;
            int j;
            RoomIds br;
            ObjectIds i, bc;

            if (ri == 0)
            {
                return;
            }

            // !IGNORE DISABLED.
            switch (ri)
            {
                case 1: goto CURECLOCK;
                case 2: goto MAINTANENCEROOMWITHLEAK;
                case 3: goto LANTERNCLOCK;
                case 4: goto MATCHCLOCK;
                case 5: goto CANDLECLOCK;
                case 6: goto BALLOONCLOCK;
                case 7: goto BALLOONBURNUPCLOCK;
                case 8: goto FUSECLOCK;
                case 9: goto LEDGECLOCK;
                case 10: goto SAFECLOCK;
                case 11: goto VOLCANOGNOMECLOCK;
                case 12: goto VOLCANOGNOMEDISAPPEARSCLOCK;
                case 13: goto BUCKETCLOCK;
                case 14: goto SPHERECLOCK;
                case 15: goto ENDGAMEHERALDCLOCK;
                case 16: goto FORESTMURMURSCLOCK;
                case 17: goto SCOLALARMCLOCK;
                case 18: goto GNOMEOFZURICHCLOCK;
                case 19: goto EXITGNOMECLOCK;
                case 20: goto STARTOFENDGAMECLOCK;
                case 21: goto L21000;
                case 22: goto L22000;
                case 23: goto L23000;
                case 24: goto L24000;
            }

            throw new InvalidOperationException();
            //bug_(3, ri);

            // CEV1--	CURE CLOCK.  LET PLAYER SLOWLY RECOVER.

            CURECLOCK:
            // Computing MIN
            i__1 = 0;
            i__2 = game.Adventurers[ActorIds.Player].Strength + 1;
            game.Adventurers[ActorIds.Player].Strength = Math.Min(i__1, i__2);

            // !RECOVER.
            if (game.Adventurers[ActorIds.Player].Strength >= 0)
            {
                return;
            }

            // !FULLY RECOVERED?
            game.Clock[ClockId.cevcur].Ticks = 30;
            // !NO, WAIT SOME MORE.
            return;

            // CEV2--	MAINT-ROOM WITH LEAK.  RAISE THE WATER LEVEL.

            MAINTANENCEROOMWITHLEAK:
            if (game.Player.Here == RoomIds.Maintenance)
            {
                i__1 = game.Switches.IsReservoirLeaking / 2 + 71;
                MessageHandler.Speak(i__1, game);
            }

            // !DESCRIBE.
            ++game.Switches.IsReservoirLeaking;
            // !RAISE WATER LEVEL.
            if (game.Switches.IsReservoirLeaking <= 16)
            {
                return;
            }

            // !IF NOT FULL, EXIT.
            game.Clock[ClockId.cevmnt].Ticks = 0;
            // !FULL, DISABLE CLOCK.
            game.Rooms[RoomIds.Maintenance].Flags |= RoomFlags.RMUNG;
            game.Rooms[RoomIds.Maintenance].Action = 80;

            // !SAY IT IS FULL OF WATER.
            if (game.Player.Here == RoomIds.Maintenance)
            {
                AdventurerHandler.jigsup_(game, 81);
            }

            // !DROWN HIM IF PRESENT.
            return;

            // CEV3--	LANTERN.  DESCRIBE GROWING DIMNESS.

            LANTERNCLOCK:
            LightInterruptProcessor(game, ObjectIds.Lamp, game.Switches.orlamp, (int)ClockId.cevlnt, lmptck, 12);
            // !DO LIGHT INTERRUPT.
            return;

            // CEV4--	MATCH.  OUT IT GOES.

            MATCHCLOCK:
            MessageHandler.Speak(153, game);
            // !MATCH IS OUT.
            game.Objects[ObjectIds.Match].Flag1 &= ~ObjectFlags.IsOn;
            return;

            // CEV5--	CANDLE.  DESCRIBE GROWING DIMNESS.

            CANDLECLOCK:
            LightInterruptProcessor(game, ObjectIds.Candle, game.Switches.orcand, (int)ClockId.CandleClock, cndtck, 10);
            // !DO CANDLE INTERRUPT.
            return;
            // CEVAPP, PAGE 3

            // CEV6--	BALLOON

            BALLOONCLOCK:
            game.Clock[ClockId.cevbal].Ticks = 3;
            // !RESCHEDULE INTERRUPT.
            f = game.Adventurers[game.Player.Winner].VehicleId == (int)ObjectIds.Balloon;

            // !SEE IF IN BALLOON.
            if (game.State.BalloonLocation.Id == RoomIds.vlbot)
            {
                goto L6800;
            }
            // !AT BOTTOM?
            if (game.State.BalloonLocation.Id == RoomIds.Ledge2 || game.State.BalloonLocation.Id == RoomIds.Ledge3 ||
                game.State.BalloonLocation.Id == RoomIds.Ledge4 || game.State.BalloonLocation.Id == RoomIds.vlbot)
            {
                goto L6700;
            }

            // !ON LEDGE?
            if ((game.Objects[ObjectIds.recep].Flag2 & ObjectFlags2.IsOpen) != 0 && game.Switches.IsBalloonInflated != 0) {
                goto L6500;
            }

            // BALLOON IS IN MIDAIR AND IS DEFLATED (OR HAS RECEPTACLE CLOSED).
            // FALL TO NEXT ROOM.

            // !IN VAIR1?
            if (game.State.BalloonLocation.Id != RoomIds.InAir1)
            {
                goto L6300;
            }

            // !YES, NOW AT VLBOT.
            game.State.BalloonLocation.Id = RoomIds.vlbot;
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, game.State.BalloonLocation, 0, 0, game);
            if (f)
            {
                goto L6200;
            }

            // !IN BALLOON?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(530, game);
            }

            // !ON LEDGE, DESCRIBE.
            return;

            L6200:
            f = AdventurerHandler.MoveToNewRoom(game, game.State.BalloonLocation.Id, game.Player.Winner);
            // !MOVE HIM.
            if (game.Switches.IsBalloonInflated == 0) {
                goto L6250;
            }
            // !IN BALLOON.  INFLATED?
            MessageHandler.Speak(531, game);
            // !YES, LANDED.
            f = RoomHandler.RoomDescription(0, game);
            // !DESCRIBE.
            return;

            L6250:
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 532, 0, 0, 0, game);
            // !NO, BALLOON & CONTENTS DIE.
            ObjectHandler.SetNewObjectStatus(ObjectIds.dball, 0, game.State.BalloonLocation, 0, 0, game);
            // !INSERT DEAD BALLOON.
            game.Adventurers[game.Player.Winner].VehicleId = 0;

            // !NOT IN VEHICLE.
            game.Clock[ClockId.cevbal].Flags = false;
            // !DISABLE INTERRUPTS.
            game.Clock[ClockId.cevbrn].Flags = false;
            game.Switches.IsBalloonInflated = 0;
            game.Switches.IsBalloonTiedUp = 0;
            return;

            L6300:
            // TODO: chadj - this is broken now.
            --game.State.BalloonLocation.Id;
            // !NOT IN VAIR1, DESCEND.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, game.State.BalloonLocation, 0, 0, game);
            if (f) {
                goto L6400;
            }
            // !IS HE IN BALLOON?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(533, game);
            }
            // !IF ON LEDGE, DESCRIBE.
            return;

            L6400:
            f = AdventurerHandler.MoveToNewRoom(game, game.State.BalloonLocation.Id, game.Player.Winner);
            // !IN BALLOON, MOVE HIM.
            MessageHandler.Speak(534, game);
            // !DESCRIBE.
            f = RoomHandler.RoomDescription(0, game);
            return;

            // BALLOON IS IN MIDAIR AND IS INFLATED, UP-UP-AND-AWAY
            // !

            L6500:
            if (game.State.BalloonLocation.Id != RoomIds.InAir4)
            {
                goto L6600;
            }

            // !AT VAIR4?
            game.Clock[ClockId.cevbrn].Ticks = 0;
            game.Clock[ClockId.cevbal].Ticks = 0;
            game.Switches.IsBalloonInflated = 0;
            game.Switches.IsBalloonTiedUp = 0;
            game.State.BalloonLocation.Id = RoomIds.vlbot;
            // !FALL TO BOTTOM.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, 0, 0, 0, game);
            // !BALLOON & CONTENTS DIE.
            ObjectHandler.SetNewObjectStatus(ObjectIds.dball, 0, game.State.BalloonLocation, 0, 0, game);
            // !SUBSTITUTE DEAD BALLOON.
            if (f)
            {
                goto L6550;
            }

            // !WAS HE IN IT?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(535, game);
            }

            // !IF HE CAN SEE, DESCRIBE.
            return;

            L6550:
            // !IN BALLOON AT CRASH, DIE.
            AdventurerHandler.jigsup_(game, 536);
            return;

            L6600:
            // TODO: chadj - this will break now.
            ++game.State.BalloonLocation.Id;
            // !NOT AT VAIR4, GO UP.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, game.State.BalloonLocation, 0, 0, game);
            if (f)
            {
                goto L6650;
            }

            // !IN BALLOON?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(537, game);
            }
            // !CAN HE SEE IT?
            return;

            L6650:
            f = AdventurerHandler.MoveToNewRoom(game, game.State.BalloonLocation.Id, game.Player.Winner);
            // !MOVE PLAYER.
            MessageHandler.Speak(538, game);
            // !DESCRIBE.
            f = RoomHandler.RoomDescription(0, game);
            return;

            // ON LEDGE, GOES TO MIDAIR ROOM WHETHER INFLATED OR NOT.

            L6700:
            game.State.BalloonLocation.Id += (int)RoomIds.InAir2 - (int)RoomIds.Ledge2;
            // !MOVE TO MIDAIR.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, game.State.BalloonLocation, 0, 0, game);
            if (f)
            {
                goto L6750;
            }

            // !IN BALLOON?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(539, game);
            }

            // !NO, STRANDED.
            game.Clock[ClockId.cevvlg].Ticks = 10;
            // !MATERIALIZE GNOME.
            return;

            L6750:
            f = AdventurerHandler.MoveToNewRoom(game, game.State.BalloonLocation.Id, game.Player.Winner);
            // !MOVE TO NEW ROOM.
            MessageHandler.Speak(540, game);
            // !DESCRIBE.
            f = RoomHandler.RoomDescription(0, game);
            return;

            // AT BOTTOM, GO UP IF INFLATED, DO NOTHING IF DEFLATED.

            L6800:
            if (game.Switches.IsBalloonInflated == 0 || !((game.Objects[ObjectIds.recep].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                return;
            }

            game.State.BalloonLocation.Id = RoomIds.InAir1;
            // !INFLATED AND OPEN,
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, game.State.BalloonLocation, 0, 0, game);
            // !GO UP TO VAIR1.
            if (f)
            {
                goto L6850;
            }

            // !IN BALLOON?
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                MessageHandler.Speak(541, game);
            }

            // !IF CAN SEE, DESCRIBE.
            return;

            L6850:
            f = AdventurerHandler.MoveToNewRoom(game, game.State.BalloonLocation.Id, game.Player.Winner);
            // !MOVE PLAYER.
            MessageHandler.Speak(542, game);
            f = RoomHandler.RoomDescription(0, game);
            return;
            // CEVAPP, PAGE 4

            // CEV7--	BALLOON BURNUP

            BALLOONBURNUPCLOCK:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !FIND BURNING OBJECT
                if (ObjectIds.recep == game.Objects[i].Container && (game.Objects[i].Flag1 & ObjectFlags.FLAMBT) != 0)
                {
                    goto L7200;
                }
                // L7100:
            }

            throw new InvalidOperationException();
            //bug_(4, 0);

            L7200:
            // !VANISH OBJECT.
            ObjectHandler.SetNewObjectStatus(i, 0, 0, 0, 0, game);
            // !UNINFLATED.
            game.Switches.IsBalloonInflated = 0;
            if (game.Player.Here == game.State.BalloonLocation.Id)
            {
                MessageHandler.rspsub_(game, 292, game.Objects[i].Description2Id);
            }
            // !DESCRIBE.
            return;

            // CEV8--	FUSE FUNCTION

            FUSECLOCK:
            // !IGNITED BRICK?
            if (game.Objects[ObjectIds.Fuse].Container != ObjectIds.Brick)
            {
                goto L8500;
            }

            // !GET BRICK ROOM.
            br = RoomHandler.GetRoomThatContainsObject(ObjectIds.Brick, game).Id;
            // !GET CONTAINER.
            bc = game.Objects[ObjectIds.Brick].Container;

            if (br == 0 && bc != 0)
            {
                br = RoomHandler.GetRoomThatContainsObject(bc, game).Id;
            }

            // !KILL FUSE.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Fuse, 0, 0, 0, 0, game);
            // !KILL BRICK.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Brick, 0, 0, 0, 0, game);

            if (br != 0 && br != game.Player.Here)
            {
                goto L8100;
            }
            // !BRICK ELSEWHERE?

            // !MUNG ROOM.
            game.Rooms[game.Player.Here].Flags |= RoomFlags.RMUNG;
            game.Rooms[game.Player.Here].Action = 114;

            // !DEAD.
            AdventurerHandler.jigsup_(game, 150);
            return;

            L8100:
            // !BOOM.
            MessageHandler.Speak(151, game);

            // !SAVE ROOM THAT BLEW.
            game.State.mungrm.Id = br;
            // !SET SAFE INTERRUPT.
            game.Clock[ClockId.cevsaf].Ticks = 5;

            // !BLEW SAFE ROOM?
            if (br != RoomIds.Safe)
            {
                goto L8200;
            }

            // !WAS BRICK IN SAFE?
            if (bc != ObjectIds.SafeSlot)
            {
                return;
            }

            // !KILL SLOT.
            ObjectHandler.SetNewObjectStatus(ObjectIds.SafeSlot, 0, 0, 0, 0, game);
            // !INDICATE SAFE BLOWN.
            game.Objects[ObjectIds.safe].Flag2 |= ObjectFlags2.IsOpen;

            game.Flags.WasSafeBlown = true;
            return;

            L8200:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !BLEW WRONG ROOM.
                if (ObjectHandler.IsObjectInRoom(game, i, br) && (game.Objects[i].Flag1 & ObjectFlags.IsTakeable) != 0)
                {
                    ObjectHandler.SetNewObjectStatus(i, 0, 0, 0, 0, game);
                }
                // L8250:
            }

            if (br != RoomIds.LivingRoom)
            {
                return;
            }

            // !BLEW LIVING ROOM?
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                if (game.Objects[i].Container == ObjectIds.TrophyCase)
                {
                    ObjectHandler.SetNewObjectStatus(i, 0, 0, 0, 0, game);
                }
                // !KILL TROPHY CASE.
                // L8300:
            }

            return;

            L8500:
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Fuse, game.Player.Here, game) || game.Objects[ObjectIds.Fuse].Adventurer == game.Player.Winner)
            {
                MessageHandler.Speak(152, game);
            }

            ObjectHandler.SetNewObjectStatus(ObjectIds.Fuse, 0, 0, 0, 0, game);
            // !KILL FUSE.
            return;
            // CEVAPP, PAGE 5

            // CEV9--	LEDGE MUNGE.

            LEDGECLOCK:
            game.Rooms[RoomIds.Ledge4].Flags |= RoomFlags.RMUNG;
            game.Rooms[RoomIds.Ledge4].Action = 109;
            if (game.Player.Here == RoomIds.Ledge4)
            {
                goto L9100;
            }

            // !WAS HE THERE?
            MessageHandler.Speak(110, game);
            // !NO, NARROW ESCAPE.
            return;

            L9100:
            if (game.Adventurers[game.Player.Winner].VehicleId != 0)
            {
                goto L9200;
            }

            // !IN VEHICLE?
            AdventurerHandler.jigsup_(game, 111);
            // !NO, DEAD.
            return;

            L9200:
            if (game.Switches.IsBalloonTiedUp != 0)
            {
                goto L9300;
            }
            // !TIED TO LEDGE?
            MessageHandler.Speak(112, game);
            // !NO, NO PLACE TO LAND.
            return;

            L9300:
            game.State.BalloonLocation.Id = RoomIds.vlbot;
            // !YES, CRASH BALLOON.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Balloon, 0, 0, 0, 0, game);
            // !BALLOON & CONTENTS DIE.
            ObjectHandler.SetNewObjectStatus(ObjectIds.dball, 0, game.State.BalloonLocation, 0, 0, game);
            // !INSERT DEAD BALLOON.
            game.Switches.IsBalloonTiedUp = 0;
            game.Switches.IsBalloonInflated = 0;
            game.Clock[ClockId.cevbal].Flags = false;
            game.Clock[ClockId.cevbrn].Flags = false;
            AdventurerHandler.jigsup_(game, 113);
            // !DEAD
            return;

            // CEV10--	SAFE MUNG.

            SAFECLOCK:
            game.State.mungrm.Flags |= RoomFlags.RMUNG;
            game.State.mungrm.Action = 114;
            if (game.Player.Here == game.State.mungrm.Id)
            {
                goto L10100;
            }

            // !IS HE PRESENT?
            MessageHandler.Speak(115, game);
            // !LET HIM KNOW.
            if (game.State.mungrm.Id == RoomIds.Safe)
            {
                game.Clock[ClockId.cevled].Ticks = 8;
            }
            // !START LEDGE CLOCK.
            return;

            L10100:
            i = (ObjectIds)116;
            // !HE'S DEAD,
            if ((game.Rooms[game.Player.Here].Flags & RoomFlags.HOUSE) != 0)
            {
                i = (ObjectIds)117;
            }

            AdventurerHandler.jigsup_(game, (int)i);
            // !LET HIM KNOW.
            return;
            // CEVAPP, PAGE 6

            // CEV11--	VOLCANO GNOME

            VOLCANOGNOMECLOCK:
            if (game.Player.Here == RoomIds.Ledge2 || game.Player.Here == RoomIds.Ledge3 ||
                game.Player.Here == RoomIds.Ledge4 || game.Player.Here == RoomIds.vlbot)
            {
                goto L11100;
            }

            // !IS HE ON LEDGE?
            game.Clock[ClockId.cevvlg].Ticks = 1;
            // !NO, WAIT A WHILE.
            return;

            L11100:
            ObjectHandler.SetNewObjectStatus(ObjectIds.Gnome, 118, game.Player.Here, 0, 0, game);
            // !YES, MATERIALIZE GNOME.
            return;

            // CEV12--	VOLCANO GNOME DISAPPEARS

            VOLCANOGNOMEDISAPPEARSCLOCK:
            ObjectHandler.SetNewObjectStatus(ObjectIds.Gnome, 149, 0, 0, 0, game);
            // !DISAPPEAR THE GNOME.
            return;

            // CEV13--	BUCKET.

            BUCKETCLOCK:
            if (game.Objects[ObjectIds.Water].Container == ObjectIds.Bucket)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.Water, 0, 0, 0, 0, game);
            }

            return;

            // CEV14--	SPHERE.  IF EXPIRES, HE'S TRAPPED.

            SPHERECLOCK:
            game.Rooms[RoomIds.cager].Flags |= RoomFlags.RMUNG;
            game.Rooms[RoomIds.cager].Action = 147;
            AdventurerHandler.jigsup_(game, 148);
            // !MUNG PLAYER.
            return;

            // CEV15--	END GAME HERALD.

            ENDGAMEHERALDCLOCK:
            // !WE'RE IN ENDGAME.
            game.Flags.IsEndGame = true;
            // !INFORM OF ENDGAME.
            MessageHandler.Speak(119, game);
            return;
            // CEVAPP, PAGE 7

            // CEV16--	FOREST MURMURS

            FORESTMURMURSCLOCK:
            game.Clock[ClockId.cevfor].Flags = game.Player.Here == RoomIds.mtree
                || game.Player.Here >= RoomIds.Forest1 && game.Player.Here < RoomIds.ForestClearing;

            if (game.Clock[ClockId.cevfor].Flags && RoomHandler.prob_(game, 10, 10))
            {
                MessageHandler.Speak(635, game);
            }

            return;

            // CEV17--	SCOL ALARM

            SCOLALARMCLOCK:
            // !IF IN TWI, GNOME.
            if (game.Player.Here == RoomIds.bktwi)
            {
                game.Clock[ClockId.cevzgi].Flags = true;
            }

            // !IF IN VAU, DEAD.
            if (game.Player.Here == RoomIds.bkvau)
            {
                AdventurerHandler.jigsup_(game, 636);
            }

            return;

            // CEV18--	ENTER GNOME OF ZURICH

            GNOMEOFZURICHCLOCK:
            // !EXITS, TOO.
            game.Clock[ClockId.cevzgo].Flags = true;
            // !PLACE IN TWI.
            ObjectHandler.SetNewObjectStatus(ObjectIds.zgnom, 0, RoomIds.bktwi, 0, 0, game);

            // !ANNOUNCE.
            if (game.Player.Here == RoomIds.bktwi)
            {
                MessageHandler.Speak(637, game);
            }

            return;

            // CEV19--	EXIT GNOME

            EXITGNOMECLOCK:
            ObjectHandler.SetNewObjectStatus(ObjectIds.zgnom, 0, 0, 0, 0, game);
            // !VANISH.
            if (game.Player.Here == RoomIds.bktwi)
            {
                MessageHandler.Speak(638, game);
            }

            // !ANNOUNCE.
            return;
            // CEVAPP, PAGE 8

            // CEV20--	START OF ENDGAME

            STARTOFENDGAMECLOCK:
            if (game.Flags.spellf)
            {
                goto L20200;
            }

            // !SPELL HIS WAY IN?
            if (game.Player.Here != RoomIds.Crypt)
            {
                return;
            }

            // !NO, STILL IN TOMB?
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                goto L20100;
            }

            // !LIGHTS OFF?
            game.Clock[ClockId.cevste].Ticks = 3;
            // !RESCHEDULE.
            return;

            L20100:
            MessageHandler.Speak(727, game);
            // !ANNOUNCE.
            L20200:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !STRIP HIM OF OBJS.
                ObjectHandler.SetNewObjectStatus(i, 0, RoomHandler.GetRoomThatContainsObject(i, game).Id, game.Objects[i].Container, 0, game);
                // L20300:
            }

            // !GIVE HIM LAMP.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Lamp, 0, 0, 0, ActorIds.Player, game);
            // !GIVE HIM SWORD.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Sword, 0, 0, 0, ActorIds.Player, game);

            game.Objects[ObjectIds.Lamp].Flag1 = (game.Objects[ObjectIds.Lamp].Flag1 | ObjectFlags.LITEBT) & ~ObjectFlags.IsOn;
            game.Objects[ObjectIds.Lamp].Flag2 |= ObjectFlags2.WasTouched;
            // !LAMP IS GOOD AS NEW.
            game.Clock[ClockId.cevlnt].Flags = false;

            game.Clock[ClockId.cevlnt].Ticks = 350;
            game.Switches.orlamp = 0;
            game.Objects[ObjectIds.Sword].Flag2 |= ObjectFlags2.WasTouched;
            game.Hack.IsSwordActive = true;
            game.Hack.SwordStatus = 0;

            // !THIEF GONE.
            game.Hack.IsThiefActive = false;
            // !ENDGAME RUNNING.
            game.Flags.IsEndGame = true;
            // !MATCHES GONE,
            game.Clock[ClockId.MatchCountdown].Flags = false;
            // !CANDLES GONE.
            game.Clock[ClockId.CandleClock].Flags = false;

            // !SCORE CRYPT,
            AdventurerHandler.ScoreUpdate(game, game.Rooms[RoomIds.Crypt].Score);
            // !BUT ONLY ONCE.
            game.Rooms[RoomIds.Crypt].Score = 0;
            // !TO TOP OF STAIRS,
            f = AdventurerHandler.MoveToNewRoom(game, RoomIds.tstrs, game.Player.Winner);
            // !AND DESCRIBE.
            f = RoomHandler.RoomDescription(Verbosity.Full, game);
            return;
            // !BAM
            // !

            // CEV21--	MIRROR CLOSES.

            L21000:
            game.Flags.mrpshf = false;
            // !BUTTON IS OUT.
            game.Flags.mropnf = false;
            // !MIRROR IS CLOSED.
            if (game.Player.Here == RoomIds.mrant)
            {
                MessageHandler.Speak(728, game);
            }

            // !DESCRIBE BUTTON.
            if (game.Player.Here == RoomIds.inmir || RoomHandler.IsMirrorHere(game, game.Player.Here) == 1)
            {
                MessageHandler.Speak(729, game);
            }
            return;
            // CEVAPP, PAGE 9

            // CEV22--	DOOR CLOSES.

            L22000:
            if (game.Flags.wdopnf)
            {
                MessageHandler.Speak(730, game);
            }

            // !DESCRIBE.
            game.Flags.wdopnf = false;
            // !CLOSED.
            return;

            // CEV23--	INQUISITOR'S QUESTION

            L23000:
            if (game.Adventurers[ActorIds.Player].CurrentRoom.Id != RoomIds.FrontDoor)
            {
                return;
            }

            // !IF PLAYER LEFT, DIE.
            MessageHandler.Speak(769, game);
            i__1 = game.Switches.quesno + 770;
            MessageHandler.Speak(i__1, game);
            game.Clock[ClockId.cevinq].Ticks = 2;
            return;

            // CEV24--	MASTER FOLLOWS

            L24000:
            if (game.Adventurers[ActorIds.Master].CurrentRoom.Id == game.Player.Here)
            {
                return;
            }

            // !NO MOVEMENT, DONE.
            if (game.Player.Here != RoomIds.Cell && game.Player.Here != RoomIds.pcell)
            {
                goto L24100;
            }

            if (game.Flags.follwf)
            {
                MessageHandler.Speak(811, game);
            }

            // !WONT GO TO CELLS.
            game.Flags.follwf = false;
            return;

            L24100:
            game.Flags.follwf = true;
            // !FOLLOWING.
            i = (ObjectIds)812;
            // !ASSUME CATCHES UP.
            i__1 = (int)XSearch.xmax;
            i__2 = (int)XSearch.xmin;

            for (j = (int)XSearch.xmin; i__2 < 0 ? j >= i__1 : j <= i__1; j += i__2)
            {
                if (dso3.FindExit(game, j, game.Adventurers[ActorIds.Master].CurrentRoom.Id) && game.CurrentExit.xroom1 == game.Player.Here)
                {
                    i = (ObjectIds)813;
                }
                // L24200:
            }

            MessageHandler.Speak(i, game);
            ObjectHandler.SetNewObjectStatus(ObjectIds.Master, 0, game.Player.Here, 0, 0, game);
            // !MOVE MASTER OBJECT.
            game.Adventurers[ActorIds.Master].CurrentRoom.Id = game.Player.Here;

            // !MOVE MASTER PLAYER.
            return;
        }

        // LITINT-	LIGHT INTERRUPT PROCESSOR
        public static void LightInterruptProcessor(Game game, ObjectIds obj, int ctr, int cev, int[] ticks, int tickln)
        {
            // Parameter adjustments
            //--ticks;

            // Function Body
            ++(ctr);
            // !ADVANCE STATE CNTR.
            game.Clock[(ClockId)cev].Ticks = ticks[ctr];
            // !RESET INTERRUPT.
            if (game.Clock[(ClockId)cev].Ticks != 0)
            {
                goto L100;
            }

            // !EXPIRED?
            game.Objects[obj].Flag1 &= ~((int)ObjectFlags.LITEBT + (int)ObjectFlags.FLAMBT + ObjectFlags.IsOn);
            if (RoomHandler.GetRoomThatContainsObject(obj, game).Id == game.Player.Here || game.Objects[obj].Adventurer == game.Player.Winner)
            {
                MessageHandler.rspsub_(game, 293, game.Objects[obj].Description2Id);
            }
            return;

            L100:
            if (RoomHandler.GetRoomThatContainsObject(obj, game).Id == game.Player.Here || game.Objects[obj].Adventurer == game.Player.Winner)
            {
                MessageHandler.Speak(ticks[ctr + tickln / 2], game);
            }

            return;
        }
    }

    public class ClockEvent
    {
        public ClockId Id { get; set; }
        public int Ticks { get; set; }
        public int Actions { get; set; }
        public bool Flags { get; set; }
        public string Name { get; set; }
    }
}
