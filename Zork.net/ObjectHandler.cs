using System;
using System.Linq;

namespace Zork.Core
{
    public static class ObjectHandler
    {
        public static bool IsInRoom(RoomIds roomId, ObjectIds obj, Game game)
        {
            bool ret_val;

            ret_val = true;
            var foundRoomId = RoomHandler.GetRoomThatContainsObject(obj, game)?.Id ?? RoomIds.NoWhere;
            if (foundRoomId == roomId)
            {
                return ret_val;
            }

            // !IN ROOM?
            for (int i = 1; i <= game.Rooms2.Count; ++i)
            {
                // !NO, SCH ROOM2.
                if (game.Rooms2.Rooms[i - 1] == (int)obj && game.Rooms2.RRoom[i - 1] == (int)roomId)
                {
                    return ret_val;
                }
                // L100:
            }
            ret_val = false;

            // !NOT PRESENT.
            return ret_val;
        }

        /// <summary>
        /// newsta_ - Set new status for object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="messageId"></param>
        /// <param name="roomId"></param>
        /// <param name="containerObjectId"></param>
        /// <param name="adventurerId"></param>
        /// <param name="game"></param>
        public static void SetNewObjectStatus(ObjectIds objectId, int messageId, RoomIds roomId, ObjectIds containerObjectId, ActorIds actorIds, Game game)
            => SetNewObjectStatus(objectId, messageId, game.Rooms[roomId], containerObjectId, actorIds, game);

        public static void SetNewObjectStatus(ObjectIds objectId, int messageId, Room room, ObjectIds containerObjectId, ActorIds actorId, Game game)
        {
            MessageHandler.Speak(messageId, game);
            var currentRoom = RoomHandler.GetRoomThatContainsObject(objectId, game);

            var obj = game.Objects[objectId];

            ObjectHandler.RemoveObjectFromContainer(obj, game);
            ObjectHandler.AddObjectToContainer(obj, game.Objects[containerObjectId], game);

            currentRoom?.Objects.Remove(obj);
            room.Objects.Add(obj);

            var currentAdv = game.Adventurers.Values.FirstOrDefault(a => a.HasObject(obj.Id));
            currentAdv?.DropObject(obj);

            if (actorId != ActorIds.NoOne)
            {
                game.Adventurers[actorId].PickupObject(obj);
            }
        }

        private static void AddObjectToContainer(Object obj, Object containerObj, Game game)
        {
            containerObj.ContainedObjects.Add(obj);
            obj.Container = containerObj.Id;
        }

        private static void RemoveObjectFromContainer(Object obj, Game game)
        {
            var container = game.Objects[obj.Container];
            container.ContainedObjects.Remove(obj);
            obj.Container = ObjectIds.Nothing;
        }

        /// <summary>
        /// princo_ - PRINT CONTENTS OF OBJECT
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="descriptionId"></param>
        /// <param name="game"></param>
        public static void PrintDescription(ObjectIds objectId, int descriptionId, Game game)
        {
            MessageHandler.rspsub_(descriptionId, game.Objects[objectId].Description2Id, game);
            // !PRINT HEADER.
            foreach (var obj in game.Objects.Values)
            {
                if (obj.Container == objectId)
                {
                    MessageHandler.rspsub_(502, obj.Description2Id, game);
                }
            }
        }

        /// <summary>
        /// oactor_ - Get Actor associated with object.
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static ActorIds GetActor(ObjectIds objId, Game game)
        {
            foreach (var actor in game.Adventurers.Values)
            {
                if (actor.HeldObjects.Any(o => o.Id == objId))
                {
                    return actor.Id;
                }
            }

            return ActorIds.NoOne;
        }

        /// <summary>
        /// qempty_ Test for object empty
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool IsObjectEmpty(ObjectIds objId, Game game) => !game.Objects.Values.Any(o => o.Container == objId);

        /// <summary>
        /// qhere_ - Test for object in room
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rm"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool IsObjectInRoom(ObjectIds obj, RoomIds rm, Game game) => game.Rooms[rm].HasObject(obj);

        /// <summary>
        /// weight_ - Returns sum of weight of qualifying objects.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="objId"></param>
        /// <param name="actorId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static int GetWeight(RoomIds room, ObjectIds objId, ActorIds actorId, Game game)
            => game.Objects[objId].Weight;

        /// <summary>
        /// qempty_ - Test for object empty.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsObjectEmpty(Game game, ObjectIds obj)
        {
            bool ret_val = false;

            ObjectIds i;

            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                if (game.Objects[i].Container == obj)
                {
                    return ret_val;
                }
                // !INSIDE TARGET?
                // L100:
            }

            ret_val = true;
            return ret_val;
        }

        /// <summary>
        /// qhere_ - Test for object in room.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="obj"></param>
        /// <param name="rm"></param>
        /// <returns></returns>
        public static bool IsObjectInRoom(Game game, ObjectIds obj, RoomIds rm)
        {
            var room = game.Rooms[rm];
            if (room.HasObject(obj))
            {
                return true;
            }

            // !IN ROOM?
            for (int i = 1; i <= game.Rooms2.Count; ++i)
            {
                // !NO, SCH ROOM2.
                if (game.Rooms2.Rooms[i - 1] == (int)obj && game.Rooms2.RRoom[i - 1] == (int)rm)
                {
                    return true;
                }
                // L100:
            }

            return false;
        }

        public static bool nobjs_(Game game, Object obj, int arg)
        {
            int i__1, i__2;
            bool ret_val;

            bool f;
            ObjectIds target;
            ObjectIds i;
            int j;
            int av, wl;
            int nxt, odi2 = 0, odo2 = 0;

            if (game.ParserVectors.DirectObject != 0)
            {
                odo2 = game.Objects[game.ParserVectors.DirectObject].Description2Id;
            }

            if (game.ParserVectors.IndirectObject != 0)
            {
                odi2 = game.Objects[game.ParserVectors.IndirectObject].Description2Id;
            }

            av = game.Adventurers[game.Player.Winner].VehicleId;
            ret_val = true;

            if (obj.DoAction != null)
            {
                return obj.DoAction(game);
            }

            switch (obj.Action - 31)
            {
                case 1: goto BILLS;
                case 2: goto SCREENOFLIGHT;
                case 3: goto L3000;
                case 4: goto L4000;
                case 5: goto CANARIES;
                case 6: goto L6000;
                case 7: goto WALL;
                case 8: goto L8000;
                case 9: goto L9000;
                case 10: goto SHORTPOLE;
                case 11: goto MIRRORSWITCH;
                case 12: goto BEAMFUNCTION;
                case 13: goto BRONZEDOOR;
                case 14: goto L14000;
                case 15: goto LOCKEDDOOR;
                case 16: goto CELLDOOR;
                case 17: goto L17000;
                case 18: goto L18000;
                case 19: goto L19000;
                case 20: goto L20000;
                case 21: goto L21000;
            }
            throw new InvalidOperationException();
            //bug_(6, ri);

            // RETURN HERE TO DECLARE FALSE RESULT

            L10:
            ret_val = false;
            return ret_val;

            // O32--	BILLS

            BILLS:
            // !EAT?
            if (game.ParserVectors.prsa != VerbIds.Eat)
            {
                goto L1100;
            }

            // !JOKE.
            MessageHandler.Speak(639, game);
            return ret_val;

            L1100:
            if (game.ParserVectors.prsa == VerbIds.Burn)
            {
                MessageHandler.Speak(640, game);
            }
            // !BURN?  JOKE.
            goto L10;
            // !LET IT BE HANDLED.
            // NOBJS, PAGE 3

            // O33--	SCREEN OF LIGHT

            SCREENOFLIGHT:
            target = ObjectIds.ScreenOfLight;
            // !TARGET IS SCOL.
            L2100:
            if (game.ParserVectors.DirectObject != target)
            {
                goto L2400;
            }
            // !PRSO EQ TARGET?
            if (game.ParserVectors.prsa != VerbIds.Push && game.ParserVectors.prsa != VerbIds.Move &&
                game.ParserVectors.prsa != VerbIds.Take && game.ParserVectors.prsa != VerbIds.Rub)
            {
                goto L2200;
            }

            // !HAND PASSES THRU.
            MessageHandler.Speak(673, game);
            return ret_val;

            L2200:
            if (game.ParserVectors.prsa != VerbIds.Kill &&
                game.ParserVectors.prsa != VerbIds.Attack &&
                 game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L2400;
            }
            MessageHandler.rspsub_(674, odi2, game);
            // !PASSES THRU.
            return ret_val;

            L2400:
            if (game.ParserVectors.prsa != VerbIds.Throw || game.ParserVectors.IndirectObject != target)
            {
                goto L10;
            }
            if (game.Player.Here == RoomIds.bkbox)
            {
                goto L2600;
            }
            // !THRU SCOL?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, RoomIds.bkbox, 0, 0, game);
            // !NO, THRU WALL.
            MessageHandler.rspsub_(675, odo2, game);
            // !ENDS UP IN BOX ROOM.
            // !CANCEL ALARM.
            game.Clock.Ticks[(int)ClockIndices.cevscl - 1] = 0;
            // !RESET SCOL ROOM.
            game.Screen.scolrm = 0;

            return ret_val;

            L2600:
            if (game.Screen.scolrm == 0)
            {
                goto L2900;
            }
            // !TRIED TO GO THRU?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, game.Screen.scolrm, 0, 0, game);
            // !SUCCESS.
            MessageHandler.rspsub_(676, odo2, game);
            // !ENDS UP SOMEWHERE.
            game.Clock.Ticks[(int)ClockIndices.cevscl - 1] = 0;
            // !CANCEL ALARM.
            game.Screen.scolrm = 0;
            // !RESET SCOL ROOM.
            return ret_val;

            L2900:
            MessageHandler.Speak(213, game);
            // !CANT DO IT.
            return ret_val;
            // NOBJS, PAGE 4

            // O34--	GNOME OF ZURICH

            L3000:
            if (game.ParserVectors.prsa != VerbIds.Give && game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L3200;
            }

            if (game.Objects[game.ParserVectors.DirectObject].otval != 0)
            {
                goto L3100;
            }

            // !THROW A TREASURE?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 641, 0, 0, 0, game);
            // !NO, GO POP.
            return ret_val;

            L3100:
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            // !YES, BYE BYE TREASURE.
            MessageHandler.rspsub_(642, odo2, game);
            SetNewObjectStatus(ObjectIds.zgnom, 0, 0, 0, 0, game);
            // !BYE BYE GNOME.
            game.Clock.Ticks[(int)ClockIndices.cevzgo - 1] = 0;
            // !CANCEL EXIT.
            f = AdventurerHandler.moveto_(game, RoomIds.bkent, game.Player.Winner);
            // !NOW IN BANK ENTRANCE.
            return ret_val;

            L3200:
            if (game.ParserVectors.prsa != VerbIds.Attack
                && game.ParserVectors.prsa != VerbIds.Kill
                && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L3300;
            }

            SetNewObjectStatus(ObjectIds.zgnom, 643, 0, 0, 0, game);
            // !VANISH GNOME.
            game.Clock.Ticks[(int)ClockIndices.cevzgo - 1] = 0;
            // !CANCEL EXIT.
            return ret_val;

            L3300:
            MessageHandler.Speak(644, game);
            // !GNOME IS IMPATIENT.
            return ret_val;

            // O35--	EGG

            L4000:
            if (game.ParserVectors.prsa != VerbIds.Open || game.ParserVectors.DirectObject != ObjectIds.Egg)
            {
                goto L4500;
            }

            if (!((game.Objects[ObjectIds.Egg].Flag2 & ObjectFlags2.IsOpen) != 0))
            {
                goto L4100;
            }

            // !OPEN ALREADY?
            MessageHandler.Speak(649, game);
            // !YES.
            return ret_val;

            L4100:
            if (game.ParserVectors.IndirectObject != 0)
            {
                goto L4200;
            }
            // !WITH SOMETHING?
            MessageHandler.Speak(650, game);
            // !NO, CANT.
            return ret_val;

            L4200:
            if (game.ParserVectors.IndirectObject != ObjectIds.hands)
            {
                goto L4300;
            }
            // !WITH HANDS?
            MessageHandler.Speak(651, game);
            // !NOT RECOMMENDED.
            return ret_val;

            L4300:
            i = (ObjectIds)652;
            // !MUNG MESSAGE.
            if (game.Objects[game.ParserVectors.IndirectObject].Flag1.HasFlag(ObjectFlags.IsTool) ||
                game.Objects[game.ParserVectors.IndirectObject].Flag2.HasFlag(ObjectFlags2.IsWeapon))
            {
                goto L4600;
            }

            i = (ObjectIds)653;
            // !NOVELTY 1.
            if ((game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsFighting) != 0)
            {
                i = (ObjectIds)654;
            }

            game.Objects[game.ParserVectors.DirectObject].Flag2 |= ObjectFlags2.IsFighting;
            MessageHandler.rspsub_((int)i, odi2, game);
            return ret_val;

            L4500:
            if (game.ParserVectors.prsa != VerbIds.Open && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L4800;
            }

            i = (ObjectIds)655;
            // !YOU BLEW IT.
            L4600:
            SetNewObjectStatus(ObjectIds.BadEgg, (int)i, RoomHandler.GetRoomThatContainsObject(ObjectIds.Egg, game).Id, game.Objects[ObjectIds.Egg].Container, game.Objects[ObjectIds.Egg].Adventurer, game);
            SetNewObjectStatus(ObjectIds.Egg, 0, 0, 0, 0, game);
            // !VANISH EGG.
            game.Objects[ObjectIds.BadEgg].otval = 2;
            // !BAD EGG HAS VALUE.

            if (game.Objects[ObjectIds.Canary].Container != ObjectIds.Egg)
            {
                goto L4700;
            }
            // !WAS CANARY INSIDE?
            MessageHandler.Speak(game.Objects[ObjectIds.BadCanary].odescoId, game);
            // !YES, DESCRIBE RESULT.
            game.Objects[ObjectIds.BadCanary].otval = 1;
            return ret_val;

            L4700:
            SetNewObjectStatus(ObjectIds.BadCanary, 0, 0, 0, 0, game);
            // !NO, VANISH IT.
            return ret_val;

            L4800:
            if (game.ParserVectors.prsa != VerbIds.Drop || game.Player.Here != RoomIds.mtree)
            {
                goto L10;
            }

            SetNewObjectStatus(ObjectIds.BadEgg, 658, RoomIds.Forest3, 0, 0, game);
            // !DROPPED EGG.
            SetNewObjectStatus(ObjectIds.Egg, 0, 0, 0, 0, game);
            game.Objects[ObjectIds.BadEgg].otval = 2;
            if (game.Objects[ObjectIds.Canary].Container != ObjectIds.Egg)
            {
                goto L4700;
            }

            game.Objects[ObjectIds.BadCanary].otval = 1;
            // !BAD CANARY.
            return ret_val;
            // NOBJS, PAGE 5

            // O36--	CANARIES, GOOD AND BAD

            CANARIES:
            // !WIND EM UP?
            if (game.ParserVectors.prsa != VerbIds.Wind)
            {
                goto L10;
            }

            // !RIGHT ONE?
            if (game.ParserVectors.DirectObject == ObjectIds.Canary)
            {
                goto L5100;
            }

            // !NO, BAD NEWS.
            MessageHandler.Speak(645, game);
            return ret_val;

            L5100:
            if (!game.Flags.HasBirdSangSong && (game.Player.Here == RoomIds.mtree || game.Player.Here >= RoomIds.Forest1 && game.Player.Here < RoomIds.ForestClearing))
            {
                goto L5200;
            }

            MessageHandler.Speak(646, game);
            // !NO, MEDIOCRE NEWS.
            return ret_val;

            L5200:
            // !SANG SONG.
            game.Flags.HasBirdSangSong = true;
            i = (ObjectIds)game.Player.Here;
            if (i == (ObjectIds)RoomIds.mtree)
            {
                i = (ObjectIds)RoomIds.Forest3;
            }

            // !PLACE BAUBLE.
            SetNewObjectStatus(ObjectIds.Bauble, 647, (RoomIds)i, 0, 0, game);
            return ret_val;

            // O37--	WHITE CLIFFS

            L6000:
            if (game.ParserVectors.prsa != VerbIds.Climb &&
                game.ParserVectors.prsa != VerbIds.ClimbUp &&
                 game.ParserVectors.prsa != VerbIds.ClimbDown)
            {
                goto L10;
            }

            // !OH YEAH?
            MessageHandler.Speak(648, game);
            return ret_val;

            // O38--	WALL
            WALL:
            i__1 = game.Player.Here - game.Switches.mloc;
            if (Math.Abs(i__1) != 1
                || RoomHandler.IsMirrorHere(game, game.Player.Here) != 0
                || game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L7100;
            }

            MessageHandler.Speak(860, game);
            // !PUSHED MIRROR WALL.
            return ret_val;

            L7100:
            if ((game.Rooms[game.Player.Here].Flags & RoomFlags.NOWALL) == 0)
            {
                goto L10;
            }
            MessageHandler.Speak(662, game);
            // !NO WALL.
            return ret_val;
            // NOBJS, PAGE 6

            // O39--	SONG BIRD GLOBAL

            L8000:
            if (game.ParserVectors.prsa != VerbIds.Find)
            {
                goto L8100;
            }
            // !FIND?
            MessageHandler.Speak(666, game);
            return ret_val;

            L8100:
            if (game.ParserVectors.prsa != VerbIds.Examine)
            {
                goto L10;
            }
            // !EXAMINE?
            MessageHandler.Speak(667, game);
            return ret_val;

            // O40--	PUZZLE/SCOL WALLS

            L9000:
            if (game.Player.Here != RoomIds.cpuzz)
            {
                goto L9500;
            }
            // !PUZZLE WALLS?
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }
            // !PUSH?
            for (i = (ObjectIds)1; i <= (ObjectIds)8; i += 2)
            {
                // !LOCATE WALL.
                if (game.ParserVectors.DirectObject == (ObjectIds)PuzzleHandler.cpwl[(int)i - 1])
                {
                    goto L9200;
                }
                // L9100:
            }
            throw new InvalidOperationException();
            //bug_(80, game.ParserVectors.prso);
            // !WHAT?

            L9200:
            j = PuzzleHandler.cpwl[(int)i];
            // !GET DIRECTIONAL OFFSET.
            nxt = game.Switches.cphere + j;
            // !GET NEXT STATE.
            wl = PuzzleHandler.cpvec[nxt - 1];
            // !GET C(NEXT STATE).
            switch (wl + 4)
            {
                case 1: goto L9300;
                case 2: goto L9300;
                case 3: goto L9300;
                case 4: goto L9250;
                case 5: goto L9350;
            }
            // !PROCESS.

            L9250:
            MessageHandler.Speak(876, game);
            // !CLEAR CORRIDOR.
            return ret_val;

            L9300:
            if (PuzzleHandler.cpvec[nxt + j - 1] == 0)
            {
                goto L9400;
            }
            // !MOVABLE, ROOM TO MOVE?
            L9350:
            MessageHandler.Speak(877, game);
            // !IMMOVABLE, NO ROOM.
            return ret_val;

            L9400:
            i = (ObjectIds)878;
            // !ASSUME FIRST PUSH.
            if (game.Flags.cpushf)
            {
                i = (ObjectIds)879;
            }
            // !NOT?
            game.Flags.cpushf = true;
            PuzzleHandler.cpvec[nxt + j - 1] = wl;
            // !MOVE WALL.
            PuzzleHandler.cpvec[nxt - 1] = 0;
            // !VACATE NEXT STATE.
            dso7.cpgoto_(game, nxt);
            // !ONWARD.
            dso7.cpinfo_(game, (int)i, nxt);
            // !DESCRIBE.
            RoomHandler.PrintRoomContents(true, game.Player.Here, game);
            // !PRINT ROOMS CONTENTS.
            game.Rooms[game.Player.Here].Flags |= RoomFlags.SEEN;
            return ret_val;

            L9500:
            if (game.Player.Here != game.Screen.scolac)
            {
                goto L9700;
            }
            // !IN SCOL ACTIVE ROOM?
            for (i = (ObjectIds)1; i <= (ObjectIds)12; i += 3)
            {
                target = (ObjectIds)game.Screen.scolwl[(int)i];
                // !ASSUME TARGET.
                if (game.Screen.scolwl[(int)i - 1] == (int)game.Player.Here)
                {
                    goto L2100;
                }
                // !TREAT IF FOUND.
                // L9600:
            }

            L9700:
            if (game.Player.Here != RoomIds.bkbox)
            {
                goto L10;
            }
            // !IN BOX ROOM?
            target = ObjectIds.wnort;
            goto L2100;
            // NOBJS, PAGE 7

            // O41--	SHORT POLE

            SHORTPOLE:
            if (game.ParserVectors.prsa != VerbIds.Raise)
            {
                goto L10100;
            }
            // !LIFT?
            i = (ObjectIds)749;
            // !ASSUME UP.
            if (game.Switches.poleuf == 2)
            {
                i = (ObjectIds)750;
            }
            // !ALREADY UP?
            MessageHandler.Speak(i, game);
            game.Switches.poleuf = 2;
            // !POLE IS RAISED.
            return ret_val;

            L10100:
            if (game.ParserVectors.prsa != VerbIds.Lower && game.ParserVectors.prsa != VerbIds.Push)
            {

                goto L10;
            }
            if (game.Switches.poleuf != 0)
            {
                goto L10200;
            }
            // !ALREADY LOWERED?
            MessageHandler.Speak(751, game);
            // !CANT DO IT.
            return ret_val;

            L10200:
            if (game.Switches.mdir % 180 != 0)
            {
                goto L10300;
            }
            // !MIRROR N-S?
            game.Switches.poleuf = 0;
            // !YES, LOWER INTO
            MessageHandler.Speak(752, game);
            // !CHANNEL.
            return ret_val;

            L10300:
            if (game.Switches.mdir != 270 || game.Switches.mloc != RoomIds.mrb)
            {
                goto L10400;
            }
            game.Switches.poleuf = 0;
            // !LOWER INTO HOLE.
            MessageHandler.Speak(753, game);
            return ret_val;

            L10400:
            i__1 = game.Switches.poleuf + 753;
            MessageHandler.Speak(i__1, game);
            // !POLEUF = 1 OR 2.
            game.Switches.poleuf = 1;
            // !NOW ON FLOOR.
            return ret_val;

            // O42--	MIRROR SWITCH

            MIRRORSWITCH:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }
            // !PUSH?
            if (game.Flags.mrpshf)
            {
                goto L11300;
            }
            // !ALREADY PUSHED?
            MessageHandler.Speak(756, game);
            // !BUTTON GOES IN.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !BLOCKED?
                if (IsObjectInRoom(i, RoomIds.mreye, game) && i != ObjectIds.rbeam)
                {
                    goto L11200;
                }
                // L11100:
            }

            MessageHandler.Speak(757, game);
            // !NOTHING IN BEAM.
            return ret_val;

            L11200:
            game.Clock.Flags[(int)ClockIndices.cevmrs - 1] = true;
            // !MIRROR OPENS.
            game.Clock.Ticks[(int)ClockIndices.cevmrs - 1] = 7;
            game.Flags.mrpshf = true;
            game.Flags.mropnf = true;
            return ret_val;

            L11300:
            MessageHandler.Speak(758, game);
            // !MIRROR ALREADYOPEN.
            return ret_val;
            // NOBJS, PAGE 8

            // O43--	BEAM FUNCTION

            BEAMFUNCTION:
            if (game.ParserVectors.prsa != VerbIds.Take ||
                game.ParserVectors.DirectObject != ObjectIds.rbeam)
            {
                goto L12100;
            }
            MessageHandler.Speak(759, game);
            // !TAKE BEAM, JOKE.
            return ret_val;

            L12100:
            i = game.ParserVectors.DirectObject;
            // !ASSUME BLK WITH DIROBJ.
            if (game.ParserVectors.prsa == VerbIds.Put && game.ParserVectors.IndirectObject == ObjectIds.rbeam)
            {
                goto L12200;
            }
            if (game.ParserVectors.prsa != VerbIds.Mung ||
                game.ParserVectors.DirectObject != ObjectIds.rbeam ||
                game.ParserVectors.IndirectObject == 0)
            {
                goto L10;
            }

            i = game.ParserVectors.IndirectObject;
            L12200:
            if (game.Objects[i].Adventurer != game.Player.Winner)
            {
                goto L12300;
            }
            // !CARRYING?
            SetNewObjectStatus(i, 0, game.Player.Here, 0, 0, game);
            // !DROP OBJ.
            MessageHandler.rspsub_(760, game.Objects[i].Description2Id, game);
            return ret_val;

            L12300:
            j = 761;
            // !ASSUME NOT IN ROOM.
            if (IsObjectInRoom((ObjectIds)j, game.Player.Here, game))
            {
                i = (ObjectIds)762;
            }
            // !IN ROOM?
            MessageHandler.rspsub_(j, game.Objects[i].Description2Id, game);
            // !DESCRIBE.
            return ret_val;

            // O44--	BRONZE DOOR

            BRONZEDOOR:
            if (game.Player.Here == RoomIds.ncell || game.Switches.LeftCell == 4 && (game.Player.Here == RoomIds.Cell || game.Player.Here == RoomIds.scorr))
            {
                goto L13100;
            }

            MessageHandler.Speak(763, game);
            // !DOOR NOT THERE.
            return ret_val;

            L13100:
            if (!RoomHandler.OpenCloseDoor(ObjectIds.odoor, 764, 765, game))
            {
                goto L10;
            }

            // !OPEN/CLOSE?
            if (game.Player.Here == RoomIds.ncell && (game.Objects[ObjectIds.odoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                MessageHandler.Speak(766, game);
            }

            return ret_val;

            // O45--	QUIZ DOOR

            L14000:
            if (game.ParserVectors.prsa != VerbIds.Open && game.ParserVectors.prsa != VerbIds.Close)
            {

                goto L14100;
            }
            MessageHandler.Speak(767, game);
            // !DOOR WONT MOVE.
            return ret_val;

            L14100:
            if (game.ParserVectors.prsa != VerbIds.Knock)
            {
                goto L10;
            }
            // !KNOCK?
            if (game.Flags.inqstf)
            {
                goto L14200;
            }
            // !TRIED IT ALREADY?
            game.Flags.inqstf = true;
            // !START INQUISITION.
            game.Clock.Flags[(int)ClockIndices.cevinq - 1] = true;
            game.Clock.Ticks[(int)ClockIndices.cevinq - 1] = 2;
            game.Switches.quesno = game.rnd_(8);
            // !SELECT QUESTION.
            game.Switches.nqatt = 0;
            game.Switches.corrct = 0;
            MessageHandler.Speak(768, game);
            // !ANNOUNCE RULES.
            MessageHandler.Speak(769, game);
            i__1 = game.Switches.quesno + 770;
            MessageHandler.Speak(i__1, game);
            // !ASK QUESTION.
            return ret_val;

            L14200:
            MessageHandler.Speak(798, game);
            // !NO REPLY.
            return ret_val;

            // O46--	LOCKED DOOR

            LOCKEDDOOR:
            if (game.ParserVectors.prsa != VerbIds.Open)
            {
                goto L10;
            }
            // !OPEN?
            MessageHandler.Speak(778, game);
            // !CANT.
            return ret_val;

            // O47--	CELL DOOR

            CELLDOOR:
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.CellDoor, 779, 780, game);
            // !OPEN/CLOSE?
            return ret_val;
            // NOBJS, PAGE 9

            // O48--	DIALBUTTON

            L17000:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }

            // !PUSH?
            MessageHandler.Speak(809, game);
            // !CLICK.
            if ((game.Objects[ObjectIds.CellDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                MessageHandler.Speak(810, game);
            }
            // !CLOSE CELL DOOR.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !RELOCATE OLD TO HYPER.
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == RoomIds.Cell && (game.Objects[i].Flag1 & ObjectFlags.IsDoor) == 0)
                {
                    i__2 = game.Switches.LeftCell * game.hyper_.hfactr;
                    SetNewObjectStatus(i, 0, (RoomIds)i__2, 0, 0, game);
                }

                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == (RoomIds)(game.Switches.pnumb * game.hyper_.hfactr))
                {
                    SetNewObjectStatus(i, 0, RoomIds.Cell, 0, 0, game);
                }
                // L17100:
            }

            game.Objects[ObjectIds.odoor].Flag2 &= ~ObjectFlags2.IsOpen;
            game.Objects[ObjectIds.CellDoor].Flag2 &= ~ObjectFlags2.IsOpen;
            game.Objects[ObjectIds.odoor].Flag1 &= ~ObjectFlags.IsVisible;

            if (game.Switches.pnumb == 4)
            {
                game.Objects[ObjectIds.odoor].Flag1 |= ObjectFlags.IsVisible;
            }

            // !PLAYER IN CELL?
            if (game.Adventurers[ActorIds.Player].CurrentRoom != game.Rooms[RoomIds.Cell])
            {
                goto L17400;
            }

            // !IN RIGHT CELL?
            if (game.Switches.LeftCell != 4)
            {
                goto L17200;
            }

            // !YES, MOVETO NCELL.
            game.Objects[ObjectIds.odoor].Flag1 |= ObjectFlags.IsVisible;
            f = AdventurerHandler.moveto_(game, RoomIds.ncell, ActorIds.Player);
            goto L17400;
            L17200:
            // !NO, MOVETO PCELL.
            f = AdventurerHandler.moveto_(game, RoomIds.pcell, ActorIds.Player);

            L17400:
            game.Switches.LeftCell = game.Switches.pnumb;
            return ret_val;
            // NOBJS, PAGE 10

            // O49--	DIAL INDICATOR

            L18000:
            if (game.ParserVectors.prsa != VerbIds.Spin)
            {
                goto L18100;
            }
            // !SPIN?
            game.Switches.pnumb = game.rnd_(8) + 1;
            // !WHEE
            // !
            i__1 = game.Switches.pnumb + 712;
            MessageHandler.rspsub_(797, i__1, game);
            return ret_val;

            L18100:
            if (game.ParserVectors.prsa != VerbIds.Move &&
                game.ParserVectors.prsa != VerbIds.Put &&
                game.ParserVectors.prsa != VerbIds.trntow)
            {
                goto L10;
            }

            if (game.ParserVectors.IndirectObject != 0)
            {
                goto L18200;
            }

            // !TURN DIAL TO X?
            MessageHandler.Speak(806, game);
            // !MUST SPECIFY.
            return ret_val;

            L18200:
            if (game.ParserVectors.IndirectObject >= ObjectIds.num1 && game.ParserVectors.IndirectObject <= ObjectIds.num8)
            {
                goto L18300;
            }

            MessageHandler.Speak(807, game);
            // !MUST BE DIGIT.
            return ret_val;

            L18300:
            game.Switches.pnumb = game.ParserVectors.IndirectObject - ObjectIds.num1 + 1;
            // !SET UP NEW.
            i__1 = game.Switches.pnumb + 712;
            MessageHandler.rspsub_(808, i__1, game);
            return ret_val;

            // O50--	GLOBAL MIRROR

            L19000:
            ret_val = mirpan_(game, 832, false);
            return ret_val;

            // O51--	GLOBAL PANEL

            L20000:
            if (game.Player.Here != RoomIds.FrontDoor)
            {
                goto L20100;
            }
            // !AT FRONT DOOR?
            if (game.ParserVectors.prsa != VerbIds.Open && game.ParserVectors.prsa != VerbIds.Close)
            {

                goto L10;
            }
            MessageHandler.Speak(843, game);
            // !PANEL IN DOOR, NOGO.
            return ret_val;

            L20100:
            ret_val = mirpan_(game, 838, true);
            return ret_val;

            // O52--	PUZZLE ROOM SLIT

            L21000:
            if (game.ParserVectors.prsa != VerbIds.Put || game.ParserVectors.IndirectObject != ObjectIds.cslit)
            {
                goto L10;
            }
            if (game.ParserVectors.DirectObject != ObjectIds.gcard)
            {
                goto L21100;
            }
            // !PUT CARD IN SLIT?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 863, 0, 0, 0, game);
            // !KILL CARD.
            game.Flags.cpoutf = true;
            // !OPEN DOOR.
            game.Objects[ObjectIds.stldr].Flag1 &= ~ObjectFlags.IsVisible;
            return ret_val;

            L21100:
            if ((game.Objects[game.ParserVectors.DirectObject].Flag1 & ObjectFlags.VICTBT) == 0
                && (game.Objects[game.ParserVectors.DirectObject].Flag2 & ObjectFlags2.IsVillian) == 0)
            {
                goto L21200;
            }

            i__1 = game.rnd_(5) + 552;
            MessageHandler.Speak(i__1, game);
            // !JOKE FOR VILL, VICT.
            return ret_val;

            L21200:
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            // !KILL OBJECT.
            MessageHandler.rspsub_(864, odo2, game);
            // !DESCRIBE.
            return ret_val;
        }

        /// <summary>
        /// objact_ - Apply objects from parse vector
        /// </summary>
        /// <returns></returns>
        public static bool ApplyObjectsFromParseVector(Game game)
        {
            // !ASSUME WINS.
            bool ret_val = true;

            // !IND OBJECT?
            if (game.ParserVectors.IndirectObject == ObjectIds.Nothing)
            {
                goto L100;
            }

            // !YES, LET IT HANDLE.
            if (DoObjectSpecialAction(game.Objects[game.ParserVectors.IndirectObject], 0, game))
            {
                return ret_val;
            }

            L100:
            // !DIR OBJECT?
            if (game.ParserVectors.DirectObject == ObjectIds.Nothing)
            {
                goto L200;
            }

            // !YES, LET IT HANDLE.
            if (DoObjectSpecialAction(game.Objects[game.ParserVectors.DirectObject], 0, game))
            {
                return ret_val;
            }

            L200:
            // !LOSES.
            ret_val = false;

            return ret_val;
        }

        /// <summary>
        /// oappli_ - Object special action routines
        /// </summary>
        /// <param name="actionId">The Id of the action</param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool DoObjectSpecialAction(Object obj, int arg, Game game)
        {
            const int mxsmp = 99;

            int i__1;
            bool ret_val;

            bool f;
            int flobts;
            ObjectIds i;
            int j, iz;
            ObjectIds av;
            RoomIds ir, io;
            int odi2 = 0, odo2 = 0;
            int nloc;

            // !ZERO IS FALSE APP.
            if (obj.Action == 0)
            {
                goto L10;
            }

            // !SIMPLE OBJECT?
            if (obj.Action <= mxsmp)
            {
                goto L100;
            }

            if (game.ParserVectors.DirectObject > (ObjectIds)220)
            {
                goto L5;
            }

            if (game.ParserVectors.DirectObject != ObjectIds.Nothing)
            {
                odo2 = game.Objects[game.ParserVectors.DirectObject].Description2Id;
            }

            L5:
            if (game.ParserVectors.IndirectObject != ObjectIds.Nothing)
            {
                odi2 = game.Objects[game.ParserVectors.IndirectObject].Description2Id;
            }

            av = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;

            flobts = (int)(ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.IsOn);
            ret_val = true;

            if (obj.DoAction != null)
            {
                return obj.DoAction(game);
            }

            switch (obj.Action - mxsmp)
            {
                case 1: goto L2000;
                case 2: goto WATER;
                case 3: goto LEAFPILE;
                case 4: goto TROLL;
                case 5: goto RUSTYKNIFE;
                case 6: goto GLACIER;
                case 7: goto BLACKBOOK;
                case 8: goto CANDLES;
                case 9: goto MATCHES;
                case 10: goto CYCLOPS;
                case 11: goto THIEF;
                case 12: goto OPENCLOSEWINDOW;
                case 13: goto PILEOFBODIES;
                case 14: goto VAMPIREBAT;
                case 15: goto STICK;
                case 16: goto BALLOON;
                case 17: goto HEADS;
                case 18: goto SPHERE;
                case 19: goto L48000;
                case 20: goto FLASK;
                case 21: goto BUCKET;
                case 22: goto EATMECAKE;
                case 23: goto ICINGS;
                case 24: goto BRICK;
                case 25: goto MYSELF;
                case 26: goto L56000;
                case 27: goto L57000;
                case 28: goto L58000;
                case 29: goto L59000;
                case 30: goto L60000;
                case 31: goto L61000;
                case 32: goto L62000;
            }

            throw new InvalidOperationException("6");
            //            bug_(6, ri);


            // RETURN HERE TO DECLARE FALSE RESULT
            L10:
            ret_val = false;
            return ret_val;

            // SIMPLE OBJECTS, PROCESSED EXTERNALLY.

            L100:
            if (obj.Action < 32)
            {
                ret_val = DoSimpleObjectAction(game, obj, arg);
            }
            else
            {
                ret_val = ObjectHandler.nobjs_(game, obj, arg);
            }
            return ret_val;
            // OAPPLI, PAGE 3

            // O100--	MACHINE FUNCTION

            L2000:
            if (game.Player.Here != RoomIds.mmach)
            {
                goto L10;
            }

            // !NOT HERE? F
            // !HANDLE OPN/CLS.
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.Machine, 123, 124, game);
            return ret_val;

            // O101--	WATER FUNCTION

            WATER:
            if (game.ParserVectors.prsa != VerbIds.Fill)
            {
                goto L5050;
            }

            // !FILL X WITH Y IS
            // !MADE INTO
            game.ParserVectors.prsa = VerbIds.Put;
            i = game.ParserVectors.IndirectObject;
            game.ParserVectors.IndirectObject = game.ParserVectors.DirectObject;
            game.ParserVectors.DirectObject = i;
            // !PUT Y IN X.
            i = (ObjectIds)odi2;
            odi2 = odo2;
            odo2 = (int)i;
            L5050:
            if (game.ParserVectors.DirectObject == ObjectIds.Water || game.ParserVectors.DirectObject == ObjectIds.gwate)
            {
                goto L5100;
            }

            // !WATER IS IND OBJ,
            MessageHandler.Speak(561, game);
            // !PUNT.
            return ret_val;

            L5100:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L5400;
            }

            // !TAKE WATER?
            if (game.Objects[ObjectIds.Bottle].Adventurer == game.Player.Winner &&
                game.Objects[game.ParserVectors.DirectObject].Container != ObjectIds.Bottle)
            {
                goto L5500;
            }

            // !INSIDE ANYTHING?
            if (game.Objects[game.ParserVectors.DirectObject].Container == 0)
            {
                goto L5200;
            }

            // !YES, OPEN?
            if ((game.Objects[game.Objects[game.ParserVectors.DirectObject].Container].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L5200;
            }

            // !INSIDE, CLOSED, PUNT.
            MessageHandler.rspsub_(525, game.Objects[game.Objects[game.ParserVectors.DirectObject].Container].Description2Id, game);

            return ret_val;

            L5200:
            // !NOT INSIDE OR OPEN,
            MessageHandler.Speak(615, game);
            // !SLIPS THRU FINGERS.
            return ret_val;

            L5400:
            // !PUT WATER IN X?
            if (game.ParserVectors.prsa != VerbIds.Put)
            {
                goto L5700;
            }

            // !IN VEH?
            if (av != 0 && game.ParserVectors.IndirectObject == av)
            {
                goto L5800;
            }

            // !IN BOTTLE?
            if (game.ParserVectors.IndirectObject == ObjectIds.Bottle)
            {
                goto L5500;
            }

            // !WONT GO ELSEWHERE.
            MessageHandler.rspsub_(297, odi2, game);

            // !VANISH WATER.
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, 0, game);

            return ret_val;

            L5500:
            // !BOTTLE OPEN?
            if ((game.Objects[ObjectIds.Bottle].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L5550;
            }

            // !NO, LOSE.
            MessageHandler.Speak(612, game);
            return ret_val;

            L5550:
            // !OPEN, EMPTY?
            if (ObjectHandler.IsObjectEmpty(ObjectIds.Bottle, game))
            {
                goto L5600;
            }

            // !NO, ALREADY FULL.
            MessageHandler.Speak(613, game);
            return ret_val;

            L5600:
            // !TAKE WATER TO BOTTLE.
            SetNewObjectStatus(ObjectIds.Water, 614, 0, ObjectIds.Bottle, 0, game);
            return ret_val;

            L5700:
            if (game.ParserVectors.prsa != VerbIds.Drop &&
                game.ParserVectors.prsa != VerbIds.Pour &&
                game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L5900;
            }

            if (av != 0)
            {
                goto L5800;
            }

            // !INTO VEHICLE?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 133, 0, 0, 0, game);
            // !NO, VANISHES.
            return ret_val;

            L5800:
            // !WATER INTO VEHICLE.
            SetNewObjectStatus(ObjectIds.Water, 0, 0, av, 0, game);
            // !DESCRIBE.
            MessageHandler.rspsub_(296, game.Objects[av].Description2Id, game);

            return ret_val;

            L5900:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L10;
            }
            // !LAST CHANCE, THROW?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 132, 0, 0, 0, game);
            // !VANISHES.
            return ret_val;
            // OAPPLI, PAGE 4

            // O102--	LEAF PILE

            LEAFPILE:
            if (game.ParserVectors.prsa != VerbIds.Burn)
            {
                goto L10500;
            }
            // !BURN?
            if (RoomHandler.GetRoomThatContainsObject(game.ParserVectors.DirectObject, game).Id == RoomIds.NoWhere)
            {
                goto L10100;
            }
            // !WAS HE CARRYING?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 158, 0, 0, 0, game);
            // !NO, BURN IT.
            return ret_val;

            L10100:
            // !DROP LEAVES.
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, game.Player.Here, 0, 0, game);
            // !BURN HIM.
            AdventurerHandler.jigsup_(game, 159);

            return ret_val;

            L10500:
            if (game.ParserVectors.prsa != VerbIds.Move)
            {
                goto L10600;
            }
            // !MOVE?
            MessageHandler.Speak(2, game);
            // !DONE.
            return ret_val;

            L10600:
            // !LOOK UNDER?
            if (game.ParserVectors.prsa != VerbIds.LookUnder || game.Switches.rvclr != 0)
            {
                goto L10;
            }
            MessageHandler.Speak(344, game);
            return ret_val;

            // O103--	TROLL, DONE EXTERNALLY.

            TROLL:
            ret_val = villns.trollp_(game, arg);
            // !TROLL PROCESSOR.
            return ret_val;

            // O104--	RUSTY KNIFE.

            RUSTYKNIFE:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L12100;
            }

            // !TAKE?
            if (game.Objects[ObjectIds.Sword].Adventurer == game.Player.Winner)
            {
                MessageHandler.Speak(160, game);
            }
            // !PULSE SWORD.
            goto L10;

            L12100:
            if ((game.ParserVectors.prsa != VerbIds.Attack &&
                    game.ParserVectors.prsa != VerbIds.Kill ||
                    game.ParserVectors.IndirectObject != ObjectIds.RustyKnife) &&
                (game.ParserVectors.prsa != VerbIds.swingw &&
                    game.ParserVectors.prsa != VerbIds.Throw ||
                    game.ParserVectors.DirectObject != ObjectIds.RustyKnife))
            {
                goto L10;
            }

            SetNewObjectStatus(ObjectIds.RustyKnife, 0, 0, 0, 0, game);
            // !KILL KNIFE.
            AdventurerHandler.jigsup_(game, 161);
            // !KILL HIM.
            return ret_val;
            // OAPPLI, PAGE 5

            // O105--	GLACIER

            GLACIER:
            // !THROW?
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L15500;
            }

            // !TORCH?
            if (game.ParserVectors.DirectObject != ObjectIds.Torch)
            {
                goto L15400;
            }

            // !MELT ICE.
            SetNewObjectStatus(ObjectIds.Ice, 169, 0, 0, 0, game);

            // !MUNG TORCH
            game.Objects[ObjectIds.Torch].Description1Id = 174;
            game.Objects[ObjectIds.Torch].Description2Id = 173;
            game.Objects[ObjectIds.Torch].Flag1 &= ~(ObjectFlags)flobts;

            // !MOVE TORCH.
            SetNewObjectStatus(ObjectIds.Torch, 0, RoomIds.strea, 0, 0, game);

            // !GLACIER GONE.
            game.Flags.IsGlacierFullyMelted = true;

            // !IN DARK?
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(170, game);
            }

            return ret_val;

            L15400:
            // !JOKE IF NOT TORCH.
            MessageHandler.Speak(171, game);
            return ret_val;

            L15500:
            if (game.ParserVectors.prsa != VerbIds.Melt || game.ParserVectors.DirectObject != ObjectIds.Ice)
            {
                goto L10;
            }

            if ((game.Objects[game.ParserVectors.IndirectObject].Flag1 & (ObjectFlags)flobts) == (ObjectFlags)flobts)
            {
                goto L15600;
            }

            // !CANT MELT WITH THAT.
            MessageHandler.rspsub_(298, odi2, game);
            return ret_val;

            L15600:
            // !PARTIAL MELT.
            game.Flags.IsGlacierPartiallyMelted = true;

            // !MELT WITH TORCH?
            if (game.ParserVectors.IndirectObject != ObjectIds.Torch)
            {
                goto L15700;
            }

            // !MUNG TORCH.
            game.Objects[ObjectIds.Torch].Description1Id = 174;

            game.Objects[ObjectIds.Torch].Description2Id = 173;
            game.Objects[ObjectIds.Torch].Flag1 &= ~(ObjectFlags)flobts;

            L15700:
            // !DROWN.
            AdventurerHandler.jigsup_(game, 172);
            return ret_val;

            // O106--	BLACK BOOK

            BLACKBOOK:
            if (game.ParserVectors.prsa != VerbIds.Open)
            {
                goto L18100;
            }
            // !OPEN?
            MessageHandler.Speak(180, game);
            // !JOKE.
            return ret_val;

            L18100:
            if (game.ParserVectors.prsa != VerbIds.Close)
            {
                goto L18200;
            }
            // !CLOSE?
            MessageHandler.Speak(181, game);
            return ret_val;

            L18200:
            // !BURN?
            if (game.ParserVectors.prsa != VerbIds.Burn)
            {
                goto L10;
            }
            SetNewObjectStatus(game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            // !FATAL JOKE.
            AdventurerHandler.jigsup_(game, 182);
            return ret_val;
            // OAPPLI, PAGE 6

            // O107--	CANDLES, PROCESSED EXTERNALLY

            CANDLES:
            ret_val = LightHandler.lightp_(game, ObjectIds.Candle);
            return ret_val;

            // O108--	MATCHES, PROCESSED EXTERNALLY

            MATCHES:
            ret_val = LightHandler.lightp_(game, ObjectIds.Match);
            return ret_val;

            // O109--	CYCLOPS, PROCESSED EXTERNALLY.

            CYCLOPS:
            ret_val = villns.cyclop_(game, arg);
            // !CYCLOPS
            return ret_val;

            // O110--	THIEF, PROCESSED EXTERNALLY

            THIEF:
            ret_val = villns.thiefp_(game, arg);
            return ret_val;

            // O111--	WINDOW

            OPENCLOSEWINDOW:
            // !OPEN/CLS WINDOW.
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.Window, 208, 209, game);
            return ret_val;

            // O112--	PILE OF BODIES

            PILEOFBODIES:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L32500;
            }

            // !TAKE?
            MessageHandler.Speak(228, game);
            // !CANT.
            return ret_val;

            L32500:
            if (game.ParserVectors.prsa != VerbIds.Burn && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L10;
            }

            // !BURN OR MUNG?
            if (game.Flags.IsHeadOnPole)
            {
                return ret_val;
            }

            game.Flags.IsHeadOnPole = true;
            // !SET HEAD ON POLE.

            SetNewObjectStatus(ObjectIds.HeadOnPole, 0, RoomIds.lld2, 0, 0, game);
            // !BEHEADED.
            AdventurerHandler.jigsup_(game, 229);
            return ret_val;

            // O113--	VAMPIRE BAT

            VAMPIREBAT:
            // !TIME TO FLY, JACK.
            MessageHandler.Speak(50, game);
            // !SELECT RANDOM DEST.
            f = AdventurerHandler.moveto_(game, (RoomIds)bats.batdrp[game.rnd_(9)], game.Player.Winner);
            f = RoomHandler.RoomDescription(0, game);
            return ret_val;
            // OAPPLI, PAGE 7

            // O114--	STICK

            STICK:
            // !WAVE?
            if (game.ParserVectors.prsa != VerbIds.Wave)
            {
                goto L10;
            }

            // !ON RAINBOW?
            if (game.Player.Here == RoomIds.Rainbow)
            {
                goto L39500;
            }

            if (game.Player.Here == RoomIds.pog || game.Player.Here == RoomIds.falls)
            {
                goto L39200;
            }
            MessageHandler.Speak(244, game);
            // !NOTHING HAPPENS.
            return ret_val;

            L39200:
            game.Objects[ObjectIds.pot].Flag1 |= ObjectFlags.IsVisible;
            // !COMPLEMENT RAINBOW.
            game.Flags.IsRainbowOn = !game.Flags.IsRainbowOn;
            // !ASSUME OFF.
            i = (ObjectIds)245;

            // !IF ON, SOLID.
            if (game.Flags.IsRainbowOn)
            {
                i = (ObjectIds)246;
            }

            // !DESCRIBE.
            MessageHandler.Speak((int)i, game);
            return ret_val;

            L39500:
            // !ON RAINBOW,
            game.Flags.IsRainbowOn = false;
            // !TAKE A FALL.
            AdventurerHandler.jigsup_(game, 247);
            return ret_val;

            // O115--	BALLOON, HANDLED EXTERNALLY

            BALLOON:
            ret_val = BalloonHandler.ballop_(game, arg);
            return ret_val;

            // O116--	HEADS

            HEADS:
            // !HELLO HEADS?
            if (game.ParserVectors.prsa != VerbIds.Hello)
            {
                goto L45100;
            }

            // !TRULY BIZARRE.
            MessageHandler.Speak(633, game);

            return ret_val;

            L45100:
            // !READ IS OK.
            if (game.ParserVectors.prsa == VerbIds.Read)
            {
                goto L10;
            }

            // !MAKE LARGE CASE.
            SetNewObjectStatus(ObjectIds.lcase, 260, RoomIds.LivingRoom, 0, 0, game);

            i = (ObjectIds)dso4.RobAdventurer(game, game.Player.Winner, 0, ObjectIds.lcase, 0) + dso4.RobRoom(game, game.Player.Here, 100, 0, ObjectIds.lcase, 0);
            // !KILL HIM.
            AdventurerHandler.jigsup_(game, 261);
            return ret_val;
            // OAPPLI, PAGE 8

            // O117--	SPHERE

            SPHERE:
            // !TAKE?
            if (game.Flags.cagesf || game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L10;
            }
            if (game.Player.Winner != ActorIds.Player)
            {
                goto L47500;
            }
            // !ROBOT TAKE?
            MessageHandler.Speak(263, game);
            // !NO, DROP CAGE.
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.Robot, game).Id != game.Player.Here)
            {
                goto L47200;
            }

            // !ROBOT HERE?
            f = AdventurerHandler.moveto_(game, RoomIds.caged, game.Player.Winner);
            // !YES, MOVE INTO CAGE.
            SetNewObjectStatus(ObjectIds.Robot, 0, RoomIds.caged, 0, 0, game);
            // !MOVE ROBOT.
            game.Adventurers[ActorIds.Robot].CurrentRoom.Id = RoomIds.caged;
            game.Objects[ObjectIds.Robot].Flag1 |= ObjectFlags.HasNoDescription;

            game.Clock.Ticks[(int)ClockIndices.cevsph - 1] = 10;
            // !GET OUT IN 10 OR ELSE.
            return ret_val;

            L47200:
            SetNewObjectStatus(ObjectIds.Sphere, 0, 0, 0, 0, game);
            // !YOURE DEAD.
            game.Rooms[RoomIds.cager].Flags |= RoomFlags.RMUNG;
            game.Rooms[RoomIds.cager].Action = 147;

            // !MUNG PLAYER.
            AdventurerHandler.jigsup_(game, 148);
            return ret_val;

            L47500:
            SetNewObjectStatus(ObjectIds.Sphere, 0, 0, 0, 0, game);
            // !ROBOT TRIED,
            SetNewObjectStatus(ObjectIds.Robot, 264, 0, 0, 0, game);

            // !KILL HIM.
            SetNewObjectStatus(ObjectIds.cage, 0, game.Player.Here, 0, 0, game);
            // !INSERT MANGLED CAGE.
            return ret_val;

            // O118--	GEOMETRICAL BUTTONS

            L48000:
            // !PUSH?
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }

            // !GET BUTTON INDEX.
            i = game.ParserVectors.DirectObject - (int)ObjectIds.sqbut + 1;

            // !A BUTTON?
            if (i <= 0 || i >= (ObjectIds)4)
            {
                goto L10;
            }

            if (game.Player.Winner != ActorIds.Player)
            {
                switch (i)
                {
                    case (ObjectIds)1: goto L48100;
                    case (ObjectIds)2: goto L48200;
                    case (ObjectIds)3: goto L48300;
                }
            }

            // !YOU PUSHED, YOU DIE.
            AdventurerHandler.jigsup_(game, 265);
            return ret_val;

            L48100:
            i = (ObjectIds)267;
            // !SPEED UP?
            if (game.Flags.carozf)
            {
                i = (ObjectIds)266;
            }
            game.Flags.carozf = true;
            MessageHandler.Speak((int)i, game);
            return ret_val;

            L48200:
            i = (ObjectIds)266;
            // !ASSUME NO CHANGE.
            if (game.Flags.carozf)
            {
                i = (ObjectIds)268;
            }

            game.Flags.carozf = false;
            MessageHandler.Speak((int)i, game);
            return ret_val;

            L48300:
            // !FLIP CAROUSEL.
            game.Flags.IsCarouselOff = !game.Flags.IsCarouselOff;

            // !IRON BOX IN CAROUSEL?
            if (!IsObjectInRoom(ObjectIds.IronBox, RoomIds.Carousel, game))
            {
                return ret_val;
            }

            MessageHandler.Speak(269, game);
            // !YES, THUMP.
            game.Objects[ObjectIds.IronBox].Flag1 ^= ObjectFlags.IsVisible;
            if (game.Flags.IsCarouselOff)
            {
                game.Rooms[RoomIds.Carousel].Flags &= ~RoomFlags.SEEN;
            }
            return ret_val;

            // O119--	FLASK FUNCTION

            FLASK:
            if (game.ParserVectors.prsa == VerbIds.Open)
            {
                goto L49100;
            }

            // !OPEN?
            if (game.ParserVectors.prsa != VerbIds.Mung && game.ParserVectors.prsa != VerbIds.Throw)
            {

                goto L10;
            }

            // !KILL FLASK.
            SetNewObjectStatus(ObjectIds.Flask, 270, 0, 0, 0, game);

            L49100:
            game.Rooms[game.Player.Here].Flags |= RoomFlags.RMUNG;
            game.Rooms[game.Player.Here].Action = 271;

            // !POISONED.
            AdventurerHandler.jigsup_(game, 272);
            return ret_val;

            // O120--	BUCKET FUNCTION

            BUCKET:
            if (arg != 2)
            {
                goto L10;
            }

            // !READOUT?
            if (game.Objects[ObjectIds.Water].Container != ObjectIds.Bucket || game.Flags.IsBucketAtTop)
            {
                goto L50500;
            }

            // !BUCKET AT TOP.
            game.Flags.IsBucketAtTop = true;

            // !START COUNTDOWN.
            game.Clock.Ticks[(int)ClockIndices.cevbuc - 1] = 100;

            // !REPOSITION BUCKET.
            SetNewObjectStatus(ObjectIds.Bucket, 290, RoomIds.twell, 0, 0, game);

            goto L50900;
            // !FINISH UP.

            L50500:
            if (game.Objects[ObjectIds.Water].Container == ObjectIds.Bucket || !game.Flags.IsBucketAtTop)
            {
                goto L10;
            }

            // !BUCKET AT BOTTOM.
            game.Flags.IsBucketAtTop = false;
            SetNewObjectStatus(ObjectIds.Bucket, 291, RoomIds.BottomOfWell, 0, 0, game);

            L50900:
            // !IN BUCKET?
            if (av != ObjectIds.Bucket)
            {
                return ret_val;
            }

            // !MOVE ADVENTURER.
            f = AdventurerHandler.moveto_(game, RoomHandler.GetRoomThatContainsObject(ObjectIds.Bucket, game).Id, game.Player.Winner);
            // !DESCRIBE ROOM.
            f = RoomHandler.RoomDescription(0, game);

            return ret_val;
            // OAPPLI, PAGE 9

            // O121--	EATME CAKE

            EATMECAKE:
            if (game.ParserVectors.prsa != VerbIds.Eat ||
                game.ParserVectors.DirectObject != ObjectIds.EatMeCake ||
                game.Player.Here != RoomIds.Alice)
            {
                goto L10;
            }

            // !VANISH CAKE.
            SetNewObjectStatus(ObjectIds.EatMeCake, 273, 0, 0, 0, game);

            game.Objects[ObjectIds.Robot].Flag1 &= ~ObjectFlags.IsVisible;

            // !MOVE TO ALICE SMALL.
            ret_val = AdventurerHandler.moveto_(game, RoomIds.AliceSmall, game.Player.Winner);
            iz = 64;
            ir = RoomIds.AliceSmall;
            io = RoomIds.Alice;
            goto L52405;

            // O122--	ICINGS

            ICINGS:
            if (game.ParserVectors.prsa != VerbIds.Read)
            {
                goto L52200;
            }

            // !READ?
            i = (ObjectIds)274;
            // !CANT READ.
            if (game.ParserVectors.IndirectObject != 0)
            {
                i = (ObjectIds)275;
            }

            // !THROUGH SOMETHING?
            if (game.ParserVectors.IndirectObject == ObjectIds.Bottle)
            {
                i = (ObjectIds)276;
            }

            // !THROUGH BOTTLE?
            if (game.ParserVectors.IndirectObject == ObjectIds.Flask)
            {
                i = game.ParserVectors.DirectObject - (int)ObjectIds.OrangeIce + 277;
            }

            // !THROUGH FLASK?
            MessageHandler.Speak((int)i, game);
            // !READ FLASK.
            return ret_val;

            L52200:
            if (game.ParserVectors.prsa != VerbIds.Throw
                || game.ParserVectors.DirectObject != ObjectIds.rdice
                || game.ParserVectors.IndirectObject != ObjectIds.Pool)
            {
                goto L52300;
            }

            SetNewObjectStatus(ObjectIds.Pool, 280, 0, 0, 0, game);
            // !VANISH POOL.
            game.Objects[ObjectIds.saffr].Flag1 |= ObjectFlags.IsVisible;
            return ret_val;

            L52300:
            if (game.Player.Here != RoomIds.Alice
                && game.Player.Here != RoomIds.AliceSmall
                && game.Player.Here != RoomIds.alitr)
            {
                goto L10;
            }
            if (game.ParserVectors.prsa != VerbIds.Eat
                && game.ParserVectors.prsa != VerbIds.Throw
                || game.ParserVectors.DirectObject != ObjectIds.OrangeIce)
            {
                goto L52400;
            }

            SetNewObjectStatus(ObjectIds.OrangeIce, 0, 0, 0, 0, game);
            // !VANISH ORANGE ICE.
            game.Rooms[game.Player.Here].Flags |= RoomFlags.RMUNG;
            game.Rooms[game.Player.Here].Action = 281;
            AdventurerHandler.jigsup_(game, 282);
            // !VANISH ADVENTURER.
            return ret_val;

            L52400:
            if (game.ParserVectors.prsa != VerbIds.Eat || game.ParserVectors.DirectObject != ObjectIds.BlueIce)
            {
                goto L10;
            }

            SetNewObjectStatus(ObjectIds.BlueIce, 283, 0, 0, 0, game);
            // !VANISH BLUE ICE.
            if (game.Player.Here != RoomIds.AliceSmall)
            {
                goto L52500;
            }
            // !IN REDUCED ROOM?
            game.Objects[ObjectIds.Robot].Flag1 |= ObjectFlags.IsVisible;

            io = game.Player.Here;
            ret_val = AdventurerHandler.moveto_(game, RoomIds.Alice, game.Player.Winner);
            iz = 0;
            ir = RoomIds.Alice;

            //  Do a size change, common loop used also by code at 51000

            L52405:
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !ENLARGE WORLD.
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id != io || game.Objects[i].Size == 10000)
                {
                    goto L52450;
                }

                RoomHandler.GetRoomThatContainsObject(i, game).Id = ir;
                game.Objects[i].Size *= iz;
                L52450:
                ;
            }
            return ret_val;

            L52500:
            AdventurerHandler.jigsup_(game, 284);
            // !ENLARGED IN WRONG ROOM.
            return ret_val;

            // O123--	BRICK

            BRICK:
            // !BURN?
            if (game.ParserVectors.prsa != VerbIds.Burn)
            {
                goto L10;
            }
            // !BOOM
            AdventurerHandler.jigsup_(game, 150);
            // !
            return ret_val;

            // O124--	MYSELF

            MYSELF:
            // !GIVE?
            if (game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L55100;
            }
            // !DONE.
            SetNewObjectStatus(game.ParserVectors.DirectObject, 2, 0, 0, ActorIds.Player, game);
            return ret_val;

            L55100:
            // !TAKE?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L55200;
            }
            // !JOKE.
            MessageHandler.Speak(286, game);
            return ret_val;

            L55200:
            if (game.ParserVectors.prsa != VerbIds.Kill && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L10;
            }

            // !KILL, NO JOKE.
            AdventurerHandler.jigsup_(game, 287);

            return ret_val;
            // OAPPLI, PAGE 10

            // O125--	PANELS INSIDE MIRROR

            L56000:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }
            // !PUSH?
            if (game.Switches.poleuf != 0)
            {
                goto L56100;
            }
            // !SHORT POLE UP?
            i = (ObjectIds)731;
            // !NO, WONT BUDGE.
            if (game.Switches.mdir % 180 == 0)
            {
                i = (ObjectIds)732;
            }
            // !DIFF MSG IF N-S.
            MessageHandler.Speak((int)i, game);
            // !TELL WONT MOVE.
            return ret_val;

            L56100:
            if (game.Switches.mloc != RoomIds.mrg)
            {
                goto L56200;
            }
            // !IN GDN ROOM?
            MessageHandler.Speak(733, game);
            // !YOU LOSE.
            AdventurerHandler.jigsup_(game, 685);
            return ret_val;

            L56200:
            i = (ObjectIds)831;
            // !ROTATE L OR R.
            if (game.ParserVectors.DirectObject == ObjectIds.rdwal || game.ParserVectors.DirectObject == ObjectIds.ylwal)
            {
                i = (ObjectIds)830;
            }
            MessageHandler.Speak((int)i, game);
            // !TELL DIRECTION.
            game.Switches.mdir = (game.Switches.mdir + 45 + ((int)i - 830) * 270) % 360;
            // !CALCULATE NEW DIR.
            i__1 = game.Switches.mdir / 45 + 695;
            MessageHandler.rspsub_(734, i__1, game);
            // !TELL NEW DIR.
            if (game.Flags.wdopnf)
            {
                MessageHandler.Speak(730, game);
            }
            // !IF PANEL OPEN, CLOSE.
            game.Flags.wdopnf = false;
            return ret_val;
            // !DONE.

            // O126--	ENDS INSIDE MIRROR

            L57000:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }
            // !PUSH?
            if (game.Switches.mdir % 180 == 0)
            {
                goto L57100;
            }
            // !MIRROR N-S?
            MessageHandler.Speak(735, game);
            // !NO, WONT BUDGE.
            return ret_val;

            L57100:
            if (game.ParserVectors.DirectObject != ObjectIds.pindr)
            {
                goto L57300;
            }

            // !PUSH PINE WALL?
            if (game.Switches.mloc == RoomIds.mrc
                && game.Switches.mdir == 180
                || game.Switches.mloc == RoomIds.mrd
                && game.Switches.mdir == 0
                || game.Switches.mloc == RoomIds.mrg)
            {
                goto L57200;
            }
            // !NO, OPENS.
            MessageHandler.Speak(736, game);

            // !INDICATE OPEN.
            game.Flags.wdopnf = true;

            // !TIME OPENING.
            game.Clock.Flags[(int)ClockIndices.cevpin - 1] = true;

            game.Clock.Ticks[(int)ClockIndices.cevpin - 1] = 5;
            return ret_val;

            L57200:
            // !GDN SEES YOU, DIE.
            MessageHandler.Speak(737, game);
            AdventurerHandler.jigsup_(game, 685);
            return ret_val;

            L57300:
            nloc = (int)game.Switches.mloc - 1;
            // !NEW LOC IF SOUTH.
            if (game.Switches.mdir == 0)
            {
                nloc = (int)game.Switches.mloc + 1;
            }
            // !NEW LOC IF NORTH.
            if (nloc >= (int)RoomIds.mra && nloc <= (int)RoomIds.mrd)
            {
                goto L57400;
            }
            MessageHandler.Speak(738, game);
            // !HAVE REACHED END.
            return ret_val;

            L57400:
            i = (ObjectIds)699;
            // !ASSUME SOUTH.
            if (game.Switches.mdir == 0)
            {
                i = (ObjectIds)695;
            }
            // !NORTH.
            j = 739;
            // !ASSUME SMOOTH.
            if (game.Switches.poleuf != 0)
            {
                j = 740;
            }
            // !POLE UP, WOBBLES.
            MessageHandler.rspsub_(game, j, (int)i);
            // !DESCRIBE.
            game.Switches.mloc = (RoomIds)nloc;
            if (game.Switches.mloc != RoomIds.mrg)
            {
                return ret_val;
            }
            // !NOW IN GDN ROOM?

            if (game.Switches.poleuf != 0)
            {
                goto L57500;
            }
            // !POLE UP, GDN SEES.
            if (game.Flags.mropnf || game.Flags.wdopnf)
            {
                goto L57600;
            }
            // !DOOR OPEN, GDN SEES.
            if (game.Flags.mr1f && game.Flags.mr2f)
            {
                return ret_val;
            }
            // !MIRRORS INTACT, OK.
            MessageHandler.Speak(742, game);
            // !MIRRORS BROKEN, DIE.
            AdventurerHandler.jigsup_(game, 743);
            return ret_val;

            L57500:
            MessageHandler.Speak(741, game);
            // !POLE UP, DIE.
            AdventurerHandler.jigsup_(game, 743);
            return ret_val;

            L57600:
            MessageHandler.Speak(744, game);
            // !DOOR OPEN, DIE.
            AdventurerHandler.jigsup_(game, 743);
            return ret_val;
            // OAPPLI, PAGE 11

            // O127--	GLOBAL GUARDIANS

            L58000:
            if (game.ParserVectors.prsa != VerbIds.Attack && game.ParserVectors.prsa != VerbIds.Kill && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L58100;
            }

            AdventurerHandler.jigsup_(game, 745);
            // !LOSE.
            return ret_val;

            L58100:
            if (game.ParserVectors.prsa != VerbIds.Hello)
            {
                goto L10;
            }
            // !HELLO?
            MessageHandler.Speak(746, game);
            // !NO REPLY.
            return ret_val;

            // O128--	GLOBAL MASTER

            L59000:
            if (game.ParserVectors.prsa != VerbIds.Attack && game.ParserVectors.prsa != VerbIds.Kill && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L59100;
            }
            AdventurerHandler.jigsup_(game, 747);
            // !BAD IDEA.
            return ret_val;

            L59100:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L10;
            }
            // !TAKE?
            MessageHandler.Speak(748, game);
            // !JOKE.
            return ret_val;

            // O129--	NUMERAL FIVE (FOR JOKE)

            L60000:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L10;
            }
            // !TAKE FIVE?
            MessageHandler.Speak(419, game);
            // !TIME PASSES.
            for (i = (ObjectIds)1; i <= (ObjectIds)3; ++i)
            {
                // !WAIT A WHILE.
                if (ClockEvents.clockd_(game))
                {
                    return ret_val;
                }
                // L60100:
            }
            return ret_val;

            // O130--	CRYPT FUNCTION

            L61000:
            if (!game.Flags.IsEndGame)
            {
                goto HEADS;
            }
            // !IF NOT EG, DIE.
            if (game.ParserVectors.prsa != VerbIds.Open)
            {
                goto L61100;
            }
            // !OPEN?
            i = (ObjectIds)793;
            if ((game.Objects[ObjectIds.Tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)794;
            }
            MessageHandler.Speak(i, game);
            game.Objects[ObjectIds.Tomb].Flag2 |= ObjectFlags2.IsOpen;
            return ret_val;

            L61100:
            if (game.ParserVectors.prsa != VerbIds.Close)
            {
                goto HEADS;
            }
            // !CLOSE?
            i = (ObjectIds)795;
            if ((game.Objects[ObjectIds.Tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)796;
            }
            MessageHandler.Speak(i, game);
            game.Objects[ObjectIds.Tomb].Flag2 &= ~ObjectFlags2.IsOpen;
            if (game.Player.Here == RoomIds.Crypt)
            {
                game.Clock.Ticks[(int)ClockIndices.cevste - 1] = 3;
            }
            // !IF IN CRYPT, START EG.
            return ret_val;
            // OAPPLI, PAGE 12

            // O131--	GLOBAL LADDER

            L62000:
            if (PuzzleHandler.cpvec[game.Switches.cphere] == -2 || PuzzleHandler.cpvec[game.Switches.cphere - 2] == -3)
            {
                goto L62100;
            }
            MessageHandler.Speak(865, game);
            // !NO, LOSE.
            return ret_val;

            L62100:
            if (game.ParserVectors.prsa == VerbIds.Climb || game.ParserVectors.prsa == VerbIds.ClimbUp)
            {

                goto L62200;
            }
            MessageHandler.Speak(866, game);
            // !CLIMB IT?
            return ret_val;

            L62200:
            if (game.Switches.cphere == 10 && PuzzleHandler.cpvec[game.Switches.cphere] == -2)
            {
                goto L62300;
            }

            MessageHandler.Speak(867, game);
            // !NO, HIT YOUR HEAD.
            return ret_val;

            L62300:
            f = AdventurerHandler.moveto_(game, RoomIds.cpant, game.Player.Winner);
            // !TO ANTEROOM.
            f = RoomHandler.RoomDescription(3, game);
            // !DESCRIBE.
            return ret_val;
        }

        public static bool DoSimpleObjectAction(Game game, Object obj, int arg)
        {
            int i__1;
            bool ret_val;

            bool f;
            ObjectIds i;
            RoomIds mroom;
            int av;
            int odi2 = 0, odo2 = 0;

            if (game.ParserVectors.DirectObject > (ObjectIds)220)
            {
                goto L5;
            }

            if (game.ParserVectors.DirectObject != 0)
            {
                odo2 = game.Objects[game.ParserVectors.DirectObject].Description2Id;
            }
            L5:
            if (game.ParserVectors.IndirectObject != 0)
            {
                odi2 = game.Objects[game.ParserVectors.IndirectObject].Description2Id;
            }

            av = game.Adventurers[game.Player.Winner].VehicleId;

            ret_val = true;

            if (obj.DoAction != null)
            {
                return obj.DoAction(game);
            }

            switch (obj.Action)
            {
                case 1: goto GUNK;
                case 2: goto TROPHYCASE;
                case 3: goto BOTTLE;
                case 4: goto ROPE;
                case 5: goto SWORD;
                case 6: goto LANTERN;
                case 7: goto RUG;
                case 8: goto SKELETON;
                case 9: goto MIRROR;
                case 10: goto DUMBWAITER;
                case 11: goto GHOST;
                case 12: goto TUBE;
                case 13: goto CHALICE;
                case 14: goto PAINTING;
                case 15: goto BOLT;
                case 16: goto GRATING;
                case 17: goto TRAPDOOR;
                case 18: goto DURABLEDOOR;
                case 19: goto MASTERSWITCH;
                case 20: goto LEAK;
                case 21: goto L34000;
                case 22: goto INFLATABLEBOAT;
                case 23: goto L37000;
                case 24: goto L38000;
                case 25: goto L41000;
                case 26: goto L42000;
                case 27: goto L43000;
                case 28: goto L44000;
                case 29: goto L46000;
                case 30: goto L53000;
                case 31: goto GRUE;
            }
            throw new InvalidOperationException();
            //bug_(6, ri);

            // RETURN HERE TO DECLARE FALSE RESULT

            L10:
            ret_val = false;
            return ret_val;
            // SOBJS, PAGE 3

            // O1--	GUNK FUNCTION

            GUNK:
            if (game.Objects[ObjectIds.Gunk].Container == 0)
            {
                goto L10;
            }
            // !NOT INSIDE? F
            SetNewObjectStatus(ObjectIds.Gunk, 122, 0, 0, 0, game);
            // !FALLS APART.
            return ret_val;

            // O2--	TROPHY CASE

            TROPHYCASE:
            // !TAKE?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L10;
            }

            // !CANT.
            MessageHandler.Speak(128, game);
            return ret_val;

            // O3--	BOTTLE FUNCTION

            BOTTLE:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L4100;
            }
            // !THROW?
            SetNewObjectStatus(game.ParserVectors.DirectObject, 129, 0, 0, 0, game);
            // !BREAKS.
            return ret_val;

            L4100:
            // !MUNG?
            if (game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L10;
            }

            // !BREAKS.
            SetNewObjectStatus(game.ParserVectors.DirectObject, 131, 0, 0, 0, game);

            return ret_val;
            // SOBJS, PAGE 4

            // O4--	ROPE FUNCTION

            ROPE:
            // !IN DOME?
            if (game.Player.Here == RoomIds.Dome)
            {
                goto L6100;
            }

            // !NO,
            game.Flags.IsRopeTiedToRailingInDomeRoom = false;
            // !UNTIE?
            if (game.ParserVectors.prsa != VerbIds.Untie)
            {
                goto L6050;
            }

            // !CANT
            MessageHandler.Speak(134, game);
            return ret_val;

            L6050:
            // !TIE?
            if (game.ParserVectors.prsa != VerbIds.Tie)
            {
                goto L10;
            }

            // !CANT TIE
            MessageHandler.Speak(135, game);
            return ret_val;

            L6100:
            if (game.ParserVectors.prsa != VerbIds.Tie || game.ParserVectors.IndirectObject != ObjectIds.Railing)
            {
                goto L6200;
            }

            // !ALREADY TIED?
            if (game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                goto L6150;
            }

            // !NO, TIE IT.
            game.Flags.IsRopeTiedToRailingInDomeRoom = true;

            game.Objects[ObjectIds.Rope].Flag1 |= ObjectFlags.HasNoDescription;
            game.Objects[ObjectIds.Rope].Flag2 |= ObjectFlags2.IsClimbable;
            SetNewObjectStatus(ObjectIds.Rope, 137, RoomIds.Dome, 0, 0, game);

            return ret_val;

            L6150:
            // !DUMMY.
            MessageHandler.Speak(136, game);
            return ret_val;

            L6200:
            // !UNTIE?
            if (game.ParserVectors.prsa != VerbIds.Untie)
            {
                goto L6300;
            }

            // !TIED?
            if (game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                goto L6250;
            }

            // !NO, DUMMY.
            MessageHandler.Speak(134, game);
            return ret_val;

            L6250:
            // !YES, UNTIE IT.
            game.Flags.IsRopeTiedToRailingInDomeRoom = false;
            game.Objects[ObjectIds.Rope].Flag1 &= ~ObjectFlags.HasNoDescription;
            game.Objects[ObjectIds.Rope].Flag2 &= ~ObjectFlags2.IsClimbable;

            MessageHandler.Speak(139, game);
            return ret_val;

            L6300:
            // !DROP & UNTIED?
            if (game.Flags.IsRopeTiedToRailingInDomeRoom || game.ParserVectors.prsa != VerbIds.Drop)
            {
                goto L6400;
            }

            // !YES, DROP.
            SetNewObjectStatus(ObjectIds.Rope, 140, RoomIds.mtorc, 0, 0, game);
            return ret_val;

            L6400:
            // !TAKE & TIED.
            if (game.ParserVectors.prsa != VerbIds.Take || !game.Flags.IsRopeTiedToRailingInDomeRoom)
            {
                goto L10;
            }

            MessageHandler.Speak(141, game);
            return ret_val;

            // O5--	SWORD FUNCTION

            SWORD:
            if (game.ParserVectors.prsa == VerbIds.Take && game.Player.Winner == ActorIds.Player)
            {
                game.Hack.IsSwordActive = true;
            }

            goto L10;

            // O6--	LANTERN

            LANTERN:
            // !THROW?
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L8100;
            }

            // !KILL LAMP,
            SetNewObjectStatus(ObjectIds.Lamp, 0, 0, 0, 0, game);

            // !REPLACE WITH BROKEN.
            SetNewObjectStatus(ObjectIds.BrokenLamp, 142, game.Player.Here, 0, 0, game);
            return ret_val;

            L8100:
            if (game.ParserVectors.prsa == VerbIds.TurnOn)
            {
                game.Clock.Flags[(int)ClockIndices.cevlnt - 1] = true;
            }

            if (game.ParserVectors.prsa == VerbIds.TurnOff)
            {
                game.Clock.Flags[(int)ClockIndices.cevlnt - 1] = false;
            }

            goto L10;

            // O7--	RUG FUNCTION

            RUG:
            // !RAISE?
            if (game.ParserVectors.prsa != VerbIds.Raise)
            {
                goto L9100;
            }

            // !CANT
            MessageHandler.Speak(143, game);
            return ret_val;

            L9100:
            // !TAKE?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L9200;
            }

            // !CANT
            MessageHandler.Speak(144, game);
            return ret_val;

            L9200:
            // !MOVE?
            if (game.ParserVectors.prsa != VerbIds.Move)
            {
                goto L9300;
            }

            i__1 = game.Switches.IsRugMoved + 145;
            MessageHandler.Speak(i__1, game);
            game.Switches.IsRugMoved = 1;

            game.Objects[ObjectIds.TrapDoor].Flag1 |= ObjectFlags.IsVisible;
            return ret_val;

            L9300:
            if (game.ParserVectors.prsa != VerbIds.LookUnder
                || game.Switches.IsRugMoved != 0
                || (game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L10;
            }

            MessageHandler.Speak(345, game);
            return ret_val;
            // SOBJS, PAGE 5

            // O8--	SKELETON

            SKELETON:
            i = (ObjectIds)dso4.RobRoom(game, game.Player.Here, 100, RoomIds.lld2, 0, 0) + dso4.RobAdventurer(game, game.Player.Winner, RoomIds.lld2, 0, 0);

            if (i != 0)
            {
                MessageHandler.Speak(162, game);
            }

            // !IF ROBBED, SAY SO.
            return ret_val;

            // O9--	MIRROR

            MIRROR:
            if (game.Flags.IsMirrorBroken || game.ParserVectors.prsa != VerbIds.Rub)
            {
                goto L14500;
            }

            mroom = (RoomIds)((int)game.Player.Here ^ 1);
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !INTERCHANGE OBJS.
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == game.Player.Here)
                {
                    RoomHandler.GetRoomThatContainsObject(i, game).Id = RoomIds.UNKNOWN;
                }
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == mroom)
                {
                    RoomHandler.GetRoomThatContainsObject(i, game).Id = game.Player.Here;
                }
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == RoomIds.UNKNOWN)
                {
                    RoomHandler.GetRoomThatContainsObject(i, game).Id = mroom;
                }
                // L14100:
            }

            f = AdventurerHandler.moveto_(game, (RoomIds)mroom, game.Player.Winner);
            MessageHandler.Speak(163, game);
            // !SHAKE WORLD.
            return ret_val;

            L14500:
            if (game.ParserVectors.prsa != VerbIds.Look &&
                game.ParserVectors.prsa != VerbIds.lookiw &&
                game.ParserVectors.prsa != VerbIds.Examine)
            {
                goto L14600;
            }

            // !MIRROR OK.
            i = (ObjectIds)164;

            // !MIRROR DEAD.
            if (game.Flags.IsMirrorBroken)
            {
                i = (ObjectIds)165;
            }

            MessageHandler.Speak(i, game);
            return ret_val;

            L14600:
            // !TAKE?
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L14700;
            }

            // !JOKE.
            MessageHandler.Speak(166, game);
            return ret_val;

            L14700:
            if (game.ParserVectors.prsa != VerbIds.Mung &&
                game.ParserVectors.prsa != VerbIds.Throw)
            {

                goto L10;
            }

            // !MIRROR BREAKS.
            i = (ObjectIds)167;
            // !MIRROR ALREADY BROKEN.
            if (game.Flags.IsMirrorBroken)
            {
                i = (ObjectIds)168;
            }

            game.Flags.IsMirrorBroken = true;
            game.Flags.HasBadLuck = true;
            MessageHandler.Speak(i, game);
            return ret_val;
            // SOBJS, PAGE 6

            // O10--	DUMBWAITER

            DUMBWAITER:
            // !RAISE?
            if (game.ParserVectors.prsa != VerbIds.Raise)
            {
                goto L16100;
            }

            // !ALREADY AT TOP?
            if (game.Flags.IsCageAtTop)
            {
                goto L16400;
            }

            // !NO, RAISE BASKET.
            SetNewObjectStatus(ObjectIds.tbask, 175, RoomIds.TopOfShaft, 0, 0, game);

            SetNewObjectStatus(ObjectIds.fbask, 0, RoomIds.BottomOfShaft, 0, 0, game);
            // !AT TOP.
            game.Flags.IsCageAtTop = true;

            return ret_val;

            L16100:
            // !LOWER?
            if (game.ParserVectors.prsa != VerbIds.Lower)
            {
                goto L16200;
            }

            // !ALREADY AT BOTTOM?
            if (!game.Flags.IsCageAtTop)
            {
                goto L16400;
            }

            // !NO, LOWER BASKET.
            SetNewObjectStatus(ObjectIds.tbask, 176, RoomIds.BottomOfShaft, 0, 0, game);

            SetNewObjectStatus(ObjectIds.fbask, 0, RoomIds.TopOfShaft, 0, 0, game);
            game.Flags.IsCageAtTop = false;
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(406, game);
            }
            // !IF DARK, DIE.
            return ret_val;

            L16200:
            if (game.ParserVectors.DirectObject != ObjectIds.fbask && game.ParserVectors.IndirectObject != ObjectIds.fbask)
            {
                goto L16300;
            }
            MessageHandler.Speak(130, game);
            // !WRONG BASKET.
            return ret_val;

            L16300:
            if (game.ParserVectors.prsa != VerbIds.Take)
            {
                goto L10;
            }
            // !TAKE?
            MessageHandler.Speak(177, game);
            // !JOKE.
            return ret_val;

            L16400:
            i__1 = game.rnd_(3) + 125;
            MessageHandler.Speak(i__1, game);
            // !DUMMY.
            return ret_val;

            // O11--	GHOST FUNCTION

            GHOST:
            // !ASSUME DIRECT.
            i = (ObjectIds)178;

            // !IF NOT, INDIRECT.
            if (game.ParserVectors.DirectObject != ObjectIds.Ghost)
            {
                i = (ObjectIds)179;
            }

            MessageHandler.Speak(i, game);
            return ret_val;
            // !SPEAK AND EXIT.
            // SOBJS, PAGE 7

            // O12--	TUBE

            TUBE:
            // !CANT PUT BACK IN.
            if (game.ParserVectors.prsa != VerbIds.Put || game.ParserVectors.IndirectObject != ObjectIds.tube)
            {
                goto L10;
            }
            MessageHandler.Speak(186, game);
            return ret_val;

            // O13--	CHALICE

            CHALICE:
            if (game.ParserVectors.prsa != VerbIds.Take
                || game.Objects[game.ParserVectors.DirectObject].Container != 0
                || RoomHandler.GetRoomThatContainsObject(game.ParserVectors.DirectObject, game).Id  != RoomIds.Treasure
                || RoomHandler.GetRoomThatContainsObject(ObjectIds.Thief, game).Id  != RoomIds.Treasure
                || (game.Objects[ObjectIds.Thief].Flag2 & ObjectFlags2.IsFighting) == 0
                || !game.Hack.IsThiefActive)
            {
                goto L10;
            }

            MessageHandler.Speak(204, game);
            // !CANT TAKE.
            return ret_val;

            // O14--	PAINTING

            PAINTING:
            if (game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L10;
            }

            // !MUNG?
            MessageHandler.Speak(205, game);
            // !DESTROY PAINTING.
            game.Objects[game.ParserVectors.DirectObject].Value = 0;
            game.Objects[game.ParserVectors.DirectObject].otval  = 0;
            game.Objects[game.ParserVectors.DirectObject].Description1Id = 207;
            game.Objects[game.ParserVectors.DirectObject].Description2Id = 206;
            return ret_val;
            // SOBJS, PAGE 8

            // O15--	BOLT

            BOLT:
            if (game.ParserVectors.prsa != VerbIds.Turn)
            {
                goto L10;
            }

            // !TURN BOLT?
            if (game.ParserVectors.IndirectObject != ObjectIds.Wrench)
            {
                goto L27500;
            }

            // !WITH WRENCH?
            if (game.Flags.gatef)
            {
                goto L27100;
            }
            // !PROPER BUTTON PUSHED?
            MessageHandler.Speak(210, game);
            // !NO, LOSE.
            return ret_val;

            L27100:
            if (game.Flags.IsLowTide)
            {
                goto L27200;
            }

            // !LOW TIDE NOW?
            game.Flags.IsLowTide = true;

            // !NO, EMPTY DAM.
            MessageHandler.Speak(211, game);
            game.Objects[ObjectIds.Coffin].Flag2 &= ~ObjectFlags2.SCRDBT;
            game.Objects[ObjectIds.Trunk].Flag1 |= ObjectFlags.IsVisible;
            game.Rooms[RoomIds.Reservoir].Flags = (game.Rooms[RoomIds.Reservoir].Flags | RoomFlags.LAND) & ~((int)RoomFlags.WATER + RoomFlags.SEEN);
            return ret_val;

            L27200:
            // !YES, FILL DAM.
            game.Flags.IsLowTide = false;

            MessageHandler.Speak(212, game);
            if (ObjectHandler.IsObjectInRoom(ObjectIds.Trunk, RoomIds.Reservoir, game))
            {
                game.Objects[ObjectIds.Trunk].Flag1 &= ~ObjectFlags.IsVisible;
            }

            game.Rooms[RoomIds.Reservoir].Flags = (game.Rooms[RoomIds.Reservoir].Flags | RoomFlags.WATER) & ~RoomFlags.LAND;
            return ret_val;

            L27500:
            MessageHandler.rspsub_(299, odi2, game);
            // !NOT WITH THAT.
            return ret_val;

            // O16--	GRATING

            GRATING:
            if (game.ParserVectors.prsa != VerbIds.Open && game.ParserVectors.prsa != VerbIds.Close)
            {

                goto L10;
            }
            if (game.Flags.IsGrateUnlocked)
            {
                goto L28200;
            }
            // !UNLOCKED?
            MessageHandler.Speak(214, game);
            // !NO, LOCKED.
            return ret_val;

            L28200:
            i = (ObjectIds)215;
            // !UNLOCKED, VIEW FRM CLR.
            if (game.Player.Here != RoomIds.ForestClearing)
            {
                i = (ObjectIds)216;
            }

            // !VIEW FROM BELOW.
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.Grate, (int)i, 885, game);
            // !OPEN/CLOSE.
            game.Rooms[RoomIds.mgrat].Flags &= ~RoomFlags.LIGHT;

            if ((game.Objects[ObjectIds.Grate].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                game.Rooms[RoomIds.mgrat].Flags |= RoomFlags.LIGHT;
            }

            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(406, game);
            }

            // !IF DARK, DIE.
            return ret_val;

            // O17--	TRAP DOOR

            TRAPDOOR:
            // !FROM LIVING ROOM?
            if (game.Player.Here != RoomIds.LivingRoom)
            {
                goto L29100;
            }

            // !OPEN/CLOSE.
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.TrapDoor, 218, 219, game);

            return ret_val;

            L29100:
            // !FROM CELLAR?
            if (game.Player.Here != RoomIds.Cellar)
            {
                goto L10;
            }

            if (game.ParserVectors.prsa != VerbIds.Open || (game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L29200;
            }

            // !CANT OPEN CLOSED DOOR.
            MessageHandler.Speak(220, game);
            return ret_val;

            L29200:
            // !NORMAL OPEN/CLOSE.
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.TrapDoor, 0, 22, game);
            return ret_val;

            // O18--	DURABLE DOOR

            DURABLEDOOR:
            // !ASSUME NO APPL.
            i = 0;
            // !OPEN?
            if (game.ParserVectors.prsa == VerbIds.Open)
            {
                i = (ObjectIds)221;
            }

            // !BURN?
            if (game.ParserVectors.prsa == VerbIds.Burn)
            {
                i = (ObjectIds)222;
            }

            // !MUNG?
            if (game.ParserVectors.prsa == VerbIds.Mung)
            {
                i = (ObjectIds)(game.rnd_(3) + 223);
            }

            if (i == 0)
            {
                goto L10;
            }
            MessageHandler.Speak((int)i, game);
            return ret_val;

            // O19--	MASTER SWITCH

            MASTERSWITCH:
            // !TURN?
            if (game.ParserVectors.prsa != VerbIds.Turn)
            {
                goto L10;
            }

            // !WITH SCREWDRIVER?
            if (game.ParserVectors.IndirectObject != ObjectIds.ScrewDriver)
            {
                goto L31500;
            }

            // !LID UP?
            if ((game.Objects[ObjectIds.Machine].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L31600;
            }

            // !NO, ACTIVATE.
            MessageHandler.Speak(226, game);

            // !COAL INSIDE?
            if (game.Objects[ObjectIds.coal].Container != ObjectIds.Machine)
            {
                goto L31400;
            }

            // !KILL COAL,
            SetNewObjectStatus(ObjectIds.coal, 0, 0, 0, 0, game);
            // !REPLACE WITH DIAMOND.
            SetNewObjectStatus(ObjectIds.Diamond, 0, 0, ObjectIds.Machine, 0, game);

            return ret_val;

            L31400:;
                // !KILL NONCOAL OBJECTS.
            for (i = (ObjectIds)1; i <= (ObjectIds)game.Objects.Count; ++i)
            {
                // !INSIDE MACHINE?
                if (game.Objects[i].Container != ObjectIds.Machine)
                {
                    goto L31450;
                }
                // !KILL OBJECT AND CONTENTS.
                SetNewObjectStatus(i, 0, 0, 0, 0, game);
                // !REDUCE TO GUNK.
                SetNewObjectStatus(ObjectIds.Gunk, 0, 0, ObjectIds.Machine, 0, game);
                L31450:
                ;
            }
            return ret_val;

            L31500:
            MessageHandler.rspsub_(300, odi2, game);
            // !CANT TURN WITH THAT.
            return ret_val;

            L31600:
            MessageHandler.Speak(227, game);
            // !LID IS UP.
            return ret_val;
            // SOBJS, PAGE 9

            // O20--	LEAK

            LEAK:
            if (game.ParserVectors.DirectObject != ObjectIds.Leak || game.ParserVectors.prsa != VerbIds.Plug || game.Switches.IsReservoirLeaking <= 0)
            {
                goto L10;
            }

            // !WITH PUTTY?
            if (game.ParserVectors.IndirectObject != ObjectIds.Putty)
            {
                goto L33100;
            }

            // !DISABLE LEAK.
            game.Switches.IsReservoirLeaking = -1;

            game.Clock.Ticks[(int)ClockIndices.cevmnt - 1] = 0;
            MessageHandler.Speak(577, game);
            return ret_val;

            L33100:
            MessageHandler.rspsub_(301, odi2, game);
            // !CANT WITH THAT.
            return ret_val;

            // O21--	DROWNING BUTTONS

            L34000:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L10;
            }

            // !PUSH?
            switch (game.ParserVectors.DirectObject - ObjectIds.rbutt + 1)
            {
                case 1: goto L34100;
                case 2: goto L34200;
                case 3: goto L34300;
                case 4: goto L34400;
            }

            goto L10;
            // !NOT A BUTTON.

            L34100:
            game.Rooms[game.Player.Here].Flags ^= RoomFlags.LIGHT;
            i = (ObjectIds)230;

            if ((game.Rooms[game.Player.Here].Flags & RoomFlags.LIGHT) != 0)
            {
                i = (ObjectIds)231;
            }

            MessageHandler.Speak(i, game);
            return ret_val;

            L34200:
            // !RELEASE GATE.
            game.Flags.gatef = true;
            MessageHandler.Speak(232, game);
            return ret_val;

            L34300:
            // !INTERLOCK GATE.
            game.Flags.gatef = false;
            MessageHandler.Speak(232, game);
            return ret_val;

            L34400:
            // !LEAK ALREADY STARTED?
            if (game.Switches.IsReservoirLeaking != 0)
            {
                goto L34500;
            }

            MessageHandler.Speak(233, game);
            // !NO, START LEAK.
            game.Switches.IsReservoirLeaking = 1;
            game.Clock.Ticks[(int)ClockIndices.cevmnt - 1] = -1;
            return ret_val;

            L34500:
            // !BUTTON JAMMED.
            MessageHandler.Speak(234, game);
            return ret_val;

            // O22--	INFLATABLE BOAT

            INFLATABLEBOAT:
            // !INFLATE?
            if (game.ParserVectors.prsa != VerbIds.Inflate)
            {
                goto L10;
            }

            // !IN ROOM?
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.iboat, game).Id != 0)
            {
                goto L36100;
            }

            // !NO, JOKE.
            MessageHandler.Speak(235, game);
            return ret_val;

            L36100:
            if (game.ParserVectors.IndirectObject != ObjectIds.Pump)
            {
                goto L36200;
            }
            // !WITH PUMP?
            SetNewObjectStatus(ObjectIds.iboat, 0, 0, 0, 0, game);
            // !KILL DEFL BOAT,
            SetNewObjectStatus(ObjectIds.rboat, 236, game.Player.Here, 0, 0, game);
            // !REPL WITH INF.
            game.Flags.deflaf = false;
            return ret_val;

            L36200:
            i = (ObjectIds)237;
            // !JOKES.
            if (game.ParserVectors.IndirectObject != ObjectIds.lungs)
            {
                i = (ObjectIds)303;
            }
            MessageHandler.rspsub_((int)i, odi2, game);
            return ret_val;

            // O23--	DEFLATED BOAT

            L37000:
            if (game.ParserVectors.prsa != VerbIds.Inflate)
            {
                goto L37100;
            }
            // !INFLATE?
            MessageHandler.Speak(238, game);
            // !JOKE.
            return ret_val;

            L37100:
            if (game.ParserVectors.prsa != VerbIds.Plug)
            {
                goto L10;
            }
            // !PLUG?
            if (game.ParserVectors.IndirectObject != ObjectIds.Putty)
            {
                goto L33100;
            }
            // !WITH PUTTY?
            SetNewObjectStatus(ObjectIds.iboat, 239, RoomHandler.GetRoomThatContainsObject(ObjectIds.dboat, game).Id,
                game.Objects[ObjectIds.dboat].Container, game.Objects[ObjectIds.dboat].Adventurer, game);

            SetNewObjectStatus(ObjectIds.dboat, 0, 0, 0, 0, game);
            // !KILL DEFL BOAT, REPL.
            return ret_val;
            // SOBJS, PAGE 10

            // O24--	RUBBER BOAT

            L38000:
            if (arg != 0)
            {
                goto L10;
            }
            // !DISMISS READIN, OUT.
            if (game.ParserVectors.prsa != VerbIds.boardw || game.Objects[ObjectIds.stick].Adventurer != game.Player.Winner)
            {
                goto L38100;
            }
            SetNewObjectStatus(ObjectIds.rboat, 0, 0, 0, 0, game);
            // !KILL INFL BOAT,
            SetNewObjectStatus(ObjectIds.dboat, 240, game.Player.Here, 0, 0, game);
            // !REPL WITH DEAD.
            game.Flags.deflaf = true;
            return ret_val;

            L38100:
            if (game.ParserVectors.prsa != VerbIds.Inflate)
            {
                goto L38200;
            }
            // !INFLATE?
            MessageHandler.Speak(367, game);
            // !YES, JOKE.
            return ret_val;

            L38200:
            if (game.ParserVectors.prsa != VerbIds.Deflate)
            {
                goto L10;
            }

            // !DEFLATE?
            if (av == (int)ObjectIds.rboat)
            {
                goto L38300;
            }

            // !IN BOAT?
            if (RoomHandler.GetRoomThatContainsObject(ObjectIds.rboat, game).Id == 0)
            {
                goto L38400;
            }

            // !ON GROUND?
            SetNewObjectStatus(ObjectIds.rboat, 0, 0, 0, 0, game);
            // !KILL INFL BOAT,
            SetNewObjectStatus(ObjectIds.iboat, 241, game.Player.Here, 0, 0, game);
            // !REPL WITH DEFL.
            game.Flags.deflaf = true;
            return ret_val;

            L38300:
            MessageHandler.Speak(242, game);
            // !IN BOAT.
            return ret_val;

            L38400:
            MessageHandler.Speak(243, game);
            // !NOT ON GROUND.
            return ret_val;

            // O25--	BRAIDED ROPE

            L41000:
            if (game.ParserVectors.prsa != VerbIds.Tie
                || game.ParserVectors.DirectObject != ObjectIds.brope
                || game.ParserVectors.IndirectObject != ObjectIds.hook1
                && game.ParserVectors.IndirectObject != ObjectIds.hook2)
            {
                goto L41500;
            }

            game.Switches.IsBalloonTiedUp = (int)game.ParserVectors.IndirectObject;
            // !RECORD LOCATION.
            game.Clock.Flags[(int)ClockIndices.cevbal - 1] = false;
            // !STALL ASCENT.
            MessageHandler.Speak(248, game);
            return ret_val;

            L41500:
            if (game.ParserVectors.prsa != VerbIds.Untie || game.ParserVectors.DirectObject != ObjectIds.brope)
            {

                goto L10;
            }

            if (game.Switches.IsBalloonTiedUp != 0)
            {
                goto L41600;
            }

            // !TIED UP?
            MessageHandler.Speak(249, game);
            // !NO, JOKE.
            return ret_val;

            L41600:
            MessageHandler.Speak(250, game);
            game.Switches.IsBalloonTiedUp = 0;
            // !UNTIE.

            game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            // !RESTART CLOCK.
            game.Clock.Flags[(int)ClockIndices.cevbal - 1] = true;
            return ret_val;

            // O26--	SAFE

            L42000:
            i = 0;
            // !ASSUME UNPROCESSED.
            if (game.ParserVectors.prsa == VerbIds.Take)
            {
                i = (ObjectIds)251;
            }

            // !TAKE?
            if (game.ParserVectors.prsa == VerbIds.Open && game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)253;
            }

            // !OPEN AFTER BLAST?
            if (game.ParserVectors.prsa == VerbIds.Open && !game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)254;
            }

            // !OPEN BEFORE BLAST?
            if (game.ParserVectors.prsa == VerbIds.Close && game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)253;
            }

            // !CLOSE AFTER?
            if (game.ParserVectors.prsa == VerbIds.Close && !game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)255;
            }

            if (i == 0)
            {
                goto L10;
            }

            MessageHandler.Speak(i, game);
            return ret_val;

            // O27--	FUSE

            L43000:
            if (game.ParserVectors.prsa != VerbIds.Burn)
            {
                goto L10;
            }

            // !BURN?
            MessageHandler.Speak(256, game);
            game.Clock.Ticks[(int)ClockIndices.cevfus - 1] = 2;
            // !START COUNTDOWN.
            return ret_val;

            // O28--	GNOME

            L44000:
            if (game.ParserVectors.prsa != VerbIds.Give && game.ParserVectors.prsa != VerbIds.Throw)
            {

                goto L44500;
            }

            if (game.Objects[game.ParserVectors.DirectObject].otval == 0)
            {
                goto L44100;
            }

            // !TREASURE?
            MessageHandler.rspsub_(257, odo2, game);
            // !YES, GET DOOR.
            SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            SetNewObjectStatus(ObjectIds.Gnome, 0, 0, 0, 0, game);
            // !VANISH GNOME.
            game.Flags.gnodrf = true;
            return ret_val;

            L44100:
            MessageHandler.rspsub_(258, odo2, game);
            // !NO, LOSE OBJECT.
            SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 0, 0, 0, 0, game);
            return ret_val;

            L44500:
            MessageHandler.Speak(259, game);
            // !NERVOUS GNOME.
            if (!game.Flags.gnomef)
            {
                game.Clock.Ticks[(int)ClockIndices.cevgno - 1] = 5;
            }

            // !SCHEDULE BYEBYE.
            game.Flags.gnomef = true;
            return ret_val;

            // O29--	COKE BOTTLES

            L46000:
            if (game.ParserVectors.prsa != VerbIds.Throw && game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L10;
            }

            SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 262, 0, 0, 0, game);
            // !MUNG BOTTLES.
            return ret_val;
            // SOBJS, PAGE 11

            // O30--	ROBOT

            L53000:
            if (game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L53200;
            }

            // !GIVE?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.DirectObject, 0, 0, 0, ActorIds.Robot, game);
            // !PUT ON ROBOT.
            MessageHandler.rspsub_(302, odo2, game);
            return ret_val;

            L53200:
            if (game.ParserVectors.prsa != VerbIds.Mung && game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L10;
            }

            // !KILL ROBOT.
            SetNewObjectStatus(ObjectIds.Robot, 285, 0, 0, 0, game);

            return ret_val;

            // O31-- GRUE

            GRUE:
            // !EXAMINE?
            if (game.ParserVectors.prsa != VerbIds.Examine)
            {
                goto L56100;
            }

            MessageHandler.Speak(288, game);
            return ret_val;

            L56100:
            // !FIND?
            if (game.ParserVectors.prsa != VerbIds.Find)
            {
                goto L10;
            }

            MessageHandler.Speak(289, game);
            return ret_val;
        }

        // MIRPAN--	PROCESSOR FOR GLOBAL MIRROR/PANEL
        public static bool mirpan_(Game game, int st, bool pnf)
        {
            int i__1;
            bool ret_val;

            // Local variables
            int num;
            int mrbf;

            ret_val = true;
            // !GET MIRROR NUM.
            num = RoomHandler.IsMirrorHere(game, game.Player.Here);

            // !ANY HERE?
            if (num != 0)
            {
                goto L100;
            }

            // !NO, LOSE.
            MessageHandler.Speak(game, st);

            return ret_val;

            L100:
            mrbf = 0;
            // !ASSUME MIRROR OK.
            if (num == 1 && !game.Flags.mr1f || num == 2 && !game.Flags.mr2f)
            {
                mrbf = 1;
            }

            if (game.ParserVectors.prsa != VerbIds.Move && game.ParserVectors.prsa != VerbIds.Open)
            {
                goto L200;
            }

            i__1 = st + 1;
            MessageHandler.Speak(game, i__1);
            // !CANT OPEN OR MOVE.
            return ret_val;

            L200:
            if (pnf
                || game.ParserVectors.prsa != VerbIds.lookiw
                && game.ParserVectors.prsa != VerbIds.Examine
                && game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L300;
            }

            i__1 = mrbf + 844;
            MessageHandler.Speak(game, i__1);
            // !LOOK IN MIRROR.
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != VerbIds.Mung)
            {
                goto L400;
            }

            // !BREAK?
            i__1 = st + 2 + mrbf;
            MessageHandler.Speak(game, i__1);

            // !DO IT.
            if (num == 1 && !(pnf))
            {
                game.Flags.mr1f = false;
            }

            if (num == 2 && !(pnf))
            {
                game.Flags.mr2f = false;
            }

            return ret_val;

            L400:
            if (pnf || mrbf == 0)
            {
                goto L500;
            }
            // !BROKEN MIRROR?
            MessageHandler.Speak(game, 846);
            return ret_val;

            L500:
            if (game.ParserVectors.prsa != VerbIds.Push)
            {
                goto L600;
            }
            // !PUSH?
            i__1 = st + 3 + num;
            MessageHandler.Speak(game, i__1);
            return ret_val;

            L600:
            ret_val = false;
            // !CANT HANDLE IT.
            return ret_val;
        }
    }
}