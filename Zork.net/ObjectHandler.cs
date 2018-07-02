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

            currentRoom.Objects.Remove(obj);
            room.Objects.Add(obj);

//            AdventurerHandler.AddObjectToAdventurer(obj, game.Adventurers[actorIds], game);
//            game.Objects[objectId].Container = containerObjectId;
            game.Objects[objectId].Adventurer = actorId;
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
            MessageHandler.rspsub_(descriptionId, game.Objects[objectId].Description2, game);
            // !PRINT HEADER.
            foreach (var obj in game.Objects.Values)
            {
                if (obj.Container == objectId)
                {
                    MessageHandler.rspsub_(502, obj.Description2, game);
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
            int i__1;
            bool ret_val;

            ObjectIds i;

            ret_val = false;
            // !ASSUME LOSE.
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
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

        public static bool nobjs_(Game game, int ri, int arg)
        {
            int i__1, i__2;
            bool ret_val;

            bool f;
            ObjectIds target;
            ObjectIds i;
            int j;
            int av, wl;
            int nxt, odi2 = 0, odo2 = 0;

            if (game.ParserVectors.prso != 0)
            {
                odo2 = game.Objects[game.ParserVectors.prso].Description2;
            }

            if (game.ParserVectors.prsi != 0)
            {
                odi2 = game.Objects[game.ParserVectors.prsi].Description2;
            }

            av = game.Adventurers[game.Player.Winner].VehicleId;
            ret_val = true;

            switch (ri - 31)
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
            }
            throw new InvalidOperationException();
            //bug_(6, ri);

            // RETURN HERE TO DECLARE FALSE RESULT

            L10:
            ret_val = false;
            return ret_val;

            // O32--	BILLS

            L1000:
            if (game.ParserVectors.prsa != VerbIds.eatw)
            {
                goto L1100;
            }
            // !EAT?
            MessageHandler.Speak(639, game);
            // !JOKE.
            return ret_val;

            L1100:
            if (game.ParserVectors.prsa == VerbIds.burnw)
            {
                MessageHandler.Speak(640, game);
            }
            // !BURN?  JOKE.
            goto L10;
            // !LET IT BE HANDLED.
            // NOBJS, PAGE 3

            // O33--	SCREEN OF LIGHT

            L2000:
            target = ObjectIds.scol;
            // !TARGET IS SCOL.
            L2100:
            if (game.ParserVectors.prso != target)
            {
                goto L2400;
            }
            // !PRSO EQ TARGET?
            if (game.ParserVectors.prsa != VerbIds.pushw && game.ParserVectors.prsa != VerbIds.movew &&
                game.ParserVectors.prsa != VerbIds.takew && game.ParserVectors.prsa != VerbIds.rubw)
            {
                goto L2200;
            }
            MessageHandler.Speak(673, game);
            // !HAND PASSES THRU.
            return ret_val;

            L2200:
            if (game.ParserVectors.prsa != VerbIds.killw && game.ParserVectors.prsa != VerbIds.attacw &&
                 game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L2400;
            }
            MessageHandler.rspsub_(674, odi2, game);
            // !PASSES THRU.
            return ret_val;

            L2400:
            if (game.ParserVectors.prsa != VerbIds.Throw || game.ParserVectors.prsi != target)
            {
                goto L10;
            }
            if (game.Player.Here == RoomIds.bkbox)
            {
                goto L2600;
            }
            // !THRU SCOL?
            SetNewObjectStatus(game.ParserVectors.prso, 0, RoomIds.bkbox, 0, 0, game);
            // !NO, THRU WALL.
            MessageHandler.rspsub_(675, odo2, game);
            // !ENDS UP IN BOX ROOM.
            game.Clock.Ticks[(int)ClockIndices.cevscl - 1] = 0;
            // !CANCEL ALARM.
            game.Screen.scolrm = 0;
            // !RESET SCOL ROOM.
            return ret_val;

            L2600:
            if (game.Screen.scolrm == 0)
            {
                goto L2900;
            }
            // !TRIED TO GO THRU?
            SetNewObjectStatus(game.ParserVectors.prso, 0, game.Screen.scolrm, 0, 0, game);
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

            if (game.Objects[game.ParserVectors.prso].otval != 0)
            {
                goto L3100;
            }

            // !THROW A TREASURE?
            SetNewObjectStatus(game.ParserVectors.prso, 641, 0, 0, 0, game);
            // !NO, GO POP.
            return ret_val;

            L3100:
            SetNewObjectStatus(game.ParserVectors.prso, 0, 0, 0, 0, game);
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
            if (game.ParserVectors.prsa != VerbIds.attacw
                && game.ParserVectors.prsa != VerbIds.killw
                && game.ParserVectors.prsa != VerbIds.mungw)
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
            if (game.ParserVectors.prsa != VerbIds.openw || game.ParserVectors.prso != ObjectIds.Egg)
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
            if (game.ParserVectors.prsi != 0)
            {
                goto L4200;
            }
            // !WITH SOMETHING?
            MessageHandler.Speak(650, game);
            // !NO, CANT.
            return ret_val;

            L4200:
            if (game.ParserVectors.prsi != ObjectIds.hands)
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
            if ((game.Objects[game.ParserVectors.prsi].Flag1 & ObjectFlags.IsTool) != 0
            || (game.Objects[game.ParserVectors.prsi].Flag2 & ObjectFlags2.WEAPBT) != 0)
            {
                goto L4600;
            }

            i = (ObjectIds)653;
            // !NOVELTY 1.
            if ((game.Objects[game.ParserVectors.prso].Flag2 & ObjectFlags2.FITEBT) != 0)
            {
                i = (ObjectIds)654;
            }

            game.Objects[game.ParserVectors.prso].Flag2 |= ObjectFlags2.FITEBT;
            MessageHandler.rspsub_((int)i, odi2, game);
            return ret_val;

            L4500:
            if (game.ParserVectors.prsa != VerbIds.openw && game.ParserVectors.prsa != VerbIds.mungw)
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

            if (game.Objects[ObjectIds.canar].Container != ObjectIds.Egg)
            {
                goto L4700;
            }
            // !WAS CANARY INSIDE?
            MessageHandler.Speak(game.Objects[ObjectIds.bcana].odesco, game);
            // !YES, DESCRIBE RESULT.
            game.Objects[ObjectIds.bcana].otval = 1;
            return ret_val;

            L4700:
            SetNewObjectStatus(ObjectIds.bcana, 0, 0, 0, 0, game);
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
            if (game.Objects[ObjectIds.canar].Container != ObjectIds.Egg)
            {
                goto L4700;
            }

            game.Objects[ObjectIds.bcana].otval = 1;
            // !BAD CANARY.
            return ret_val;
            // NOBJS, PAGE 5

            // O36--	CANARIES, GOOD AND BAD

            L5000:
            if (game.ParserVectors.prsa != VerbIds.windw)
            {
                goto L10;
            }
            // !WIND EM UP?
            if (game.ParserVectors.prso == ObjectIds.canar)
            {
                goto L5100;
            }
            // !RIGHT ONE?
            MessageHandler.Speak(645, game);
            // !NO, BAD NEWS.
            return ret_val;

            L5100:
            if (!game.Flags.singsf && (game.Player.Here == RoomIds.mtree || game.Player.Here >= RoomIds.Forest1 && game.Player.Here < RoomIds.ForestClearing))
            {
                goto L5200;
            }

            MessageHandler.Speak(646, game);
            // !NO, MEDIOCRE NEWS.
            return ret_val;

            L5200:
            game.Flags.singsf = true;
            // !SANG SONG.
            i = (ObjectIds)game.Player.Here;
            if (i == (ObjectIds)RoomIds.mtree)
            {
                i = (ObjectIds)RoomIds.Forest3;
            }

            // !PLACE BAUBLE.
            SetNewObjectStatus(ObjectIds.baubl, 647, (RoomIds)i, 0, 0, game);
            return ret_val;

            // O37--	WHITE CLIFFS

            L6000:
            if (game.ParserVectors.prsa != VerbIds.Climb && game.ParserVectors.prsa != VerbIds.ClimbUp &&
                 game.ParserVectors.prsa != VerbIds.ClimbDown)
            {
                goto L10;
            }
            MessageHandler.Speak(648, game);
            // !OH YEAH?
            return ret_val;

            // O38--	WALL
            L7000:
            i__1 = game.Player.Here - game.Switches.mloc;
            if (Math.Abs(i__1) != 1
                || RoomHandler.IsMirrorHere(game, game.Player.Here) != 0
                || game.ParserVectors.prsa != VerbIds.pushw)
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
            if (game.ParserVectors.prsa != VerbIds.pushw)
            {
                goto L10;
            }
            // !PUSH?
            for (i = (ObjectIds)1; i <= (ObjectIds)8; i += 2)
            {
                // !LOCATE WALL.
                if (game.ParserVectors.prso == (ObjectIds)PuzzleHandler.cpwl[(int)i - 1])
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

            L10000:
            if (game.ParserVectors.prsa != VerbIds.raisew)
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
            if (game.ParserVectors.prsa != VerbIds.lowerw && game.ParserVectors.prsa != VerbIds.pushw)
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

            L11000:
            if (game.ParserVectors.prsa != VerbIds.pushw)
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
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
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

            L12000:
            if (game.ParserVectors.prsa != VerbIds.takew || game.ParserVectors.prso != ObjectIds.rbeam)
            {
                goto L12100;
            }
            MessageHandler.Speak(759, game);
            // !TAKE BEAM, JOKE.
            return ret_val;

            L12100:
            i = game.ParserVectors.prso;
            // !ASSUME BLK WITH DIROBJ.
            if (game.ParserVectors.prsa == VerbIds.Put && game.ParserVectors.prsi == ObjectIds.rbeam)
            {
                goto L12200;
            }
            if (game.ParserVectors.prsa != VerbIds.mungw || game.ParserVectors.prso != ObjectIds.rbeam
                || game.ParserVectors.prsi == 0)
            {
                goto L10;
            }
            i = game.ParserVectors.prsi;
            L12200:
            if (game.Objects[i].Adventurer != game.Player.Winner)
            {
                goto L12300;
            }
            // !CARRYING?
            SetNewObjectStatus((ObjectIds)i, 0, game.Player.Here, 0, 0, game);
            // !DROP OBJ.
            MessageHandler.rspsub_(760, game.Objects[i].Description2, game);
            return ret_val;

            L12300:
            j = 761;
            // !ASSUME NOT IN ROOM.
            if (IsObjectInRoom((ObjectIds)j, game.Player.Here, game))
            {
                i = (ObjectIds)762;
            }
            // !IN ROOM?
            MessageHandler.rspsub_(j, game.Objects[i].Description2, game);
            // !DESCRIBE.
            return ret_val;

            // O44--	BRONZE DOOR

            L13000:
            if (game.Player.Here == RoomIds.ncell || game.Switches.lcell == 4 && (game.Player.Here == RoomIds.Cell || game.Player.Here == RoomIds.scorr))
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
            if (game.ParserVectors.prsa != VerbIds.openw && game.ParserVectors.prsa != VerbIds.closew)
            {

                goto L14100;
            }
            MessageHandler.Speak(767, game);
            // !DOOR WONT MOVE.
            return ret_val;

            L14100:
            if (game.ParserVectors.prsa != VerbIds.knockw)
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

            L15000:
            if (game.ParserVectors.prsa != VerbIds.openw)
            {
                goto L10;
            }
            // !OPEN?
            MessageHandler.Speak(778, game);
            // !CANT.
            return ret_val;

            // O47--	CELL DOOR

            L16000:
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.cdoor, 779, 780, game);
            // !OPEN/CLOSE?
            return ret_val;
            // NOBJS, PAGE 9

            // O48--	DIALBUTTON

            L17000:
            if (game.ParserVectors.prsa != VerbIds.pushw)
            {
                goto L10;
            }

            // !PUSH?
            MessageHandler.Speak(809, game);
            // !CLICK.
            if ((game.Objects[ObjectIds.cdoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                MessageHandler.Speak(810, game);
            }
            // !CLOSE CELL DOOR.

            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !RELOCATE OLD TO HYPER.
                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == RoomIds.Cell && (game.Objects[i].Flag1 & ObjectFlags.DOORBT) == 0)
                {
                    i__2 = game.Switches.lcell * game.hyper_.hfactr;
                    SetNewObjectStatus((ObjectIds)i, 0, (RoomIds)i__2, 0, 0, game);
                }

                if (RoomHandler.GetRoomThatContainsObject(i, game).Id == (RoomIds)(game.Switches.pnumb * game.hyper_.hfactr))
                {
                    SetNewObjectStatus((ObjectIds)i, 0, RoomIds.Cell, 0, 0, game);
                }
                // L17100:
            }

            game.Objects[ObjectIds.odoor].Flag2 &= ~ObjectFlags2.IsOpen;
            game.Objects[ObjectIds.cdoor].Flag2 &= ~ObjectFlags2.IsOpen;
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
            if (game.Switches.lcell != 4)
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
            game.Switches.lcell = game.Switches.pnumb;
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
            if (game.ParserVectors.prsa != VerbIds.movew && game.ParserVectors.prsa != VerbIds.Put &&
                game.ParserVectors.prsa != VerbIds.trntow)
            {
                goto L10;
            }

            if (game.ParserVectors.prsi != 0)
            {
                goto L18200;
            }

            // !TURN DIAL TO X?
            MessageHandler.Speak(806, game);
            // !MUST SPECIFY.
            return ret_val;

            L18200:
            if (game.ParserVectors.prsi >= ObjectIds.num1 && game.ParserVectors.prsi <= ObjectIds.num8)
            {
                goto L18300;
            }

            MessageHandler.Speak(807, game);
            // !MUST BE DIGIT.
            return ret_val;

            L18300:
            game.Switches.pnumb = game.ParserVectors.prsi - ObjectIds.num1 + 1;
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
            if (game.Player.Here != RoomIds.fdoor)
            {
                goto L20100;
            }
            // !AT FRONT DOOR?
            if (game.ParserVectors.prsa != VerbIds.openw && game.ParserVectors.prsa != VerbIds.closew)
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
            if (game.ParserVectors.prsa != VerbIds.Put || game.ParserVectors.prsi != ObjectIds.cslit)
            {
                goto L10;
            }
            if (game.ParserVectors.prso != ObjectIds.gcard)
            {
                goto L21100;
            }
            // !PUT CARD IN SLIT?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 863, 0, 0, 0, game);
            // !KILL CARD.
            game.Flags.cpoutf = true;
            // !OPEN DOOR.
            game.Objects[ObjectIds.stldr].Flag1 &= ~ObjectFlags.IsVisible;
            return ret_val;

            L21100:
            if ((game.Objects[game.ParserVectors.prso].Flag1 & ObjectFlags.VICTBT) == 0
                && (game.Objects[game.ParserVectors.prso].Flag2 & ObjectFlags2.VILLBT) == 0)
            {
                goto L21200;
            }

            i__1 = game.rnd_(5) + 552;
            MessageHandler.Speak(i__1, game);
            // !JOKE FOR VILL, VICT.
            return ret_val;

            L21200:
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, 0, game);
            // !KILL OBJECT.
            MessageHandler.rspsub_(864, odo2, game);
            // !DESCRIBE.
            return ret_val;
        }

        /// <summary>
        /// objact_ - Apply objects from parse vector
        /// </summary>
        /// <returns></returns>
        public static bool objact_(Game game)
        {
            bool ret_val;

            // !ASSUME WINS.
            ret_val = true;

            // !IND OBJECT?
            if (game.ParserVectors.prsi == 0)
            {
                goto L100;
            }

            // !YES, LET IT HANDLE.
            if (oappli_(game.Objects[(ObjectIds)game.ParserVectors.prsi].oactio, 0, game))
            {
                return ret_val;
            }

            L100:
            // !DIR OBJECT?
            if (game.ParserVectors.prso == 0)
            {
                goto L200;
            }

            // !YES, LET IT HANDLE.
            if (oappli_(game.Objects[game.ParserVectors.prso].oactio, 0, game))
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
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool oappli_(int ri, int arg, Game game)
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
            if (ri == 0)
            {
                goto L10;
            }

            // !SIMPLE OBJECT?
            if (ri <= mxsmp)
            {
                goto L100;
            }

            if (game.ParserVectors.prso > (ObjectIds)220)
            {
                goto L5;
            }

            if (game.ParserVectors.prso != 0)
            {
                odo2 = game.Objects[game.ParserVectors.prso].Description2;
            }

            L5:
            if (game.ParserVectors.prsi != 0)
            {
                odi2 = game.Objects[game.ParserVectors.prsi].Description2;
            }

            av = (ObjectIds)game.Adventurers[game.Player.Winner].VehicleId;

            flobts = (int)(ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.ONBT);
            ret_val = true;

            switch (ri - mxsmp)
            {
                case 1: goto L2000;
                case 2: goto L5000;
                case 3: goto L10000;
                case 4: goto L11000;
                case 5: goto L12000;
                case 6: goto L15000;
                case 7: goto L18000;
                case 8: goto L19000;
                case 9: goto L20000;
                case 10: goto L22000;
                case 11: goto L25000;
                case 12: goto L26000;
                case 13: goto L32000;
                case 14: goto L35000;
                case 15: goto L39000;
                case 16: goto L40000;
                case 17: goto L45000;
                case 18: goto L47000;
                case 19: goto L48000;
                case 20: goto L49000;
                case 21: goto L50000;
                case 22: goto L51000;
                case 23: goto L52000;
                case 24: goto L54000;
                case 25: goto L55000;
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
            if (ri < 32)
            {
                ret_val = sobjs_(game, ri, arg);
            }
            else
            {
                ret_val = ObjectHandler.nobjs_(game, ri, arg);
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
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.machi, 123, 124, game);
            return ret_val;

            // O101--	WATER FUNCTION

            L5000:
            if (game.ParserVectors.prsa != VerbIds.Fill)
            {
                goto L5050;
            }

            // !FILL X WITH Y IS
            // !MADE INTO
            game.ParserVectors.prsa = VerbIds.Put;
            i = game.ParserVectors.prsi;
            game.ParserVectors.prsi = game.ParserVectors.prso;
            game.ParserVectors.prso = i;
            // !PUT Y IN X.
            i = (ObjectIds)odi2;
            odi2 = odo2;
            odo2 = (int)i;
            L5050:
            if (game.ParserVectors.prso == ObjectIds.Water || game.ParserVectors.prso == ObjectIds.gwate)
            {
                goto L5100;
            }

            // !WATER IS IND OBJ,
            MessageHandler.Speak(561, game);
            // !PUNT.
            return ret_val;

            L5100:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L5400;
            }

            // !TAKE WATER?
            if (game.Objects[ObjectIds.Bottle].Adventurer == game.Player.Winner
             && game.Objects[game.ParserVectors.prso].Container != ObjectIds.Bottle)
            {
                goto L5500;
            }

            // !INSIDE ANYTHING?
            if (game.Objects[game.ParserVectors.prso].Container == 0)
            {
                goto L5200;
            }

            // !YES, OPEN?
            if ((game.Objects[game.Objects[game.ParserVectors.prso].Container].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L5200;
            }

            // !INSIDE, CLOSED, PUNT.
            MessageHandler.rspsub_(525, game.Objects[game.Objects[game.ParserVectors.prso].Container].Description2, game);

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
            if (av != 0 && game.ParserVectors.prsi == (ObjectIds)av)
            {
                goto L5800;
            }

            // !IN BOTTLE?
            if (game.ParserVectors.prsi == ObjectIds.Bottle)
            {
                goto L5500;
            }

            // !WONT GO ELSEWHERE.
            MessageHandler.rspsub_(297, odi2, game);

            // !VANISH WATER.
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, 0, game);

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
            if (game.ParserVectors.prsa != VerbIds.Drop
                && game.ParserVectors.prsa != VerbIds.Pour
                && game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L5900;
            }

            if (av != 0)
            {
                goto L5800;
            }

            // !INTO VEHICLE?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 133, 0, 0, 0, game);
            // !NO, VANISHES.
            return ret_val;

            L5800:
            SetNewObjectStatus(ObjectIds.Water, 0, 0, (ObjectIds)av, 0, game);
            // !WATER INTO VEHICLE.
            MessageHandler.rspsub_(296, game.Objects[av].Description2, game);
            // !DESCRIBE.
            return ret_val;

            L5900:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L10;
            }
            // !LAST CHANCE, THROW?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 132, 0, 0, 0, game);
            // !VANISHES.
            return ret_val;
            // OAPPLI, PAGE 4

            // O102--	LEAF PILE

            L10000:
            if (game.ParserVectors.prsa != VerbIds.burnw)
            {
                goto L10500;
            }
            // !BURN?
            if (RoomHandler.GetRoomThatContainsObject(game.ParserVectors.prso, game).Id == RoomIds.NoWhere)
            {
                goto L10100;
            }
            // !WAS HE CARRYING?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 158, 0, 0, 0, game);
            // !NO, BURN IT.
            return ret_val;

            L10100:
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, game.Player.Here, 0, 0, game);
            // !DROP LEAVES.
            AdventurerHandler.jigsup_(game, 159);
            // !BURN HIM.
            return ret_val;

            L10500:
            if (game.ParserVectors.prsa != VerbIds.movew)
            {
                goto L10600;
            }
            // !MOVE?
            MessageHandler.Speak(2, game);
            // !DONE.
            return ret_val;

            L10600:
            if (game.ParserVectors.prsa != VerbIds.lookuw || game.Switches.rvclr != 0)
            {
                goto L10;
            }
            MessageHandler.Speak(344, game);
            // !LOOK UNDER?
            return ret_val;

            // O103--	TROLL, DONE EXTERNALLY.

            L11000:
            ret_val = villns.trollp_(game, arg);
            // !TROLL PROCESSOR.
            return ret_val;

            // O104--	RUSTY KNIFE.

            L12000:
            if (game.ParserVectors.prsa != VerbIds.takew)
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
            if ((game.ParserVectors.prsa != VerbIds.attacw
                && game.ParserVectors.prsa != VerbIds.killw
                || game.ParserVectors.prsi != ObjectIds.rknif)
                && (game.ParserVectors.prsa != VerbIds.swingw && game.ParserVectors.prsa != VerbIds.Throw || game.ParserVectors.prso != ObjectIds.rknif))
            {
                goto L10;
            }
            SetNewObjectStatus(ObjectIds.rknif, 0, 0, 0, 0, game);
            // !KILL KNIFE.
            AdventurerHandler.jigsup_(game, 161);
            // !KILL HIM.
            return ret_val;
            // OAPPLI, PAGE 5

            // O105--	GLACIER

            L15000:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L15500;
            }
            // !THROW?
            if (game.ParserVectors.prso != ObjectIds.Torch)
            {
                goto L15400;
            }

            // !TORCH?
            SetNewObjectStatus(ObjectIds.Ice, 169, 0, 0, 0, game);
            // !MELT ICE.
            game.Objects[ObjectIds.Torch].Description1 = 174;
            // !MUNG TORCH
            game.Objects[ObjectIds.Torch].Description2 = 173;
            game.Objects[ObjectIds.Torch].Flag1 &= ~(ObjectFlags)flobts;
            SetNewObjectStatus(ObjectIds.Torch, 0, RoomIds.strea, 0, 0, game);
            // !MOVE TORCH.
            game.Flags.glacrf = true;
            // !GLACIER GONE.
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(170, game);
            }
            // !IN DARK?
            return ret_val;

            L15400:
            MessageHandler.Speak(171, game);
            // !JOKE IF NOT TORCH.
            return ret_val;

            L15500:
            if (game.ParserVectors.prsa != VerbIds.meltw || game.ParserVectors.prso != ObjectIds.Ice)
            {
                goto L10;
            }

            if ((game.Objects[game.ParserVectors.prsi].Flag1 & (ObjectFlags)flobts) == (ObjectFlags)flobts)
            {
                goto L15600;
            }

            MessageHandler.rspsub_(298, odi2, game);
            // !CANT MELT WITH THAT.
            return ret_val;

            L15600:
            game.Flags.glacmf = true;
            // !PARTIAL MELT.
            if (game.ParserVectors.prsi != ObjectIds.Torch)
            {
                goto L15700;
            }
            // !MELT WITH TORCH?
            game.Objects[ObjectIds.Torch].Description1 = 174;
            // !MUNG TORCH.
            game.Objects[ObjectIds.Torch].Description2 = 173;
            game.Objects[ObjectIds.Torch].Flag1 &= ~(ObjectFlags)flobts;

            L15700:
            AdventurerHandler.jigsup_(game, 172);
            // !DROWN.
            return ret_val;

            // O106--	BLACK BOOK

            L18000:
            if (game.ParserVectors.prsa != VerbIds.openw)
            {
                goto L18100;
            }
            // !OPEN?
            MessageHandler.Speak(180, game);
            // !JOKE.
            return ret_val;

            L18100:
            if (game.ParserVectors.prsa != VerbIds.closew)
            {
                goto L18200;
            }
            // !CLOSE?
            MessageHandler.Speak(181, game);
            return ret_val;

            L18200:
            if (game.ParserVectors.prsa != VerbIds.burnw)
            {
                goto L10;
            }
            // !BURN?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, 0, game);
            // !FATAL JOKE.
            AdventurerHandler.jigsup_(game, 182);
            return ret_val;
            // OAPPLI, PAGE 6

            // O107--	CANDLES, PROCESSED EXTERNALLY

            L19000:
            ret_val = LightHandler.lightp_(game, ObjectIds.Candle);
            return ret_val;

            // O108--	MATCHES, PROCESSED EXTERNALLY

            L20000:
            ret_val = LightHandler.lightp_(game, ObjectIds.match);
            return ret_val;

            // O109--	CYCLOPS, PROCESSED EXTERNALLY.

            L22000:
            ret_val = villns.cyclop_(game, arg);
            // !CYCLOPS
            return ret_val;

            // O110--	THIEF, PROCESSED EXTERNALLY

            L25000:
            ret_val = villns.thiefp_(game, arg);
            return ret_val;

            // O111--	WINDOW

            L26000:
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.Window, 208, 209, game);
            // !OPEN/CLS WINDOW.
            return ret_val;

            // O112--	PILE OF BODIES

            L32000:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L32500;
            }
            // !TAKE?
            MessageHandler.Speak(228, game);
            // !CANT.
            return ret_val;

            L32500:
            if (game.ParserVectors.prsa != VerbIds.burnw && game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L10;
            }
            if (game.Flags.onpolf)
            {
                return ret_val;
            }
            // !BURN OR MUNG?
            game.Flags.onpolf = true;
            // !SET HEAD ON POLE.
            SetNewObjectStatus(ObjectIds.hpole, 0, RoomIds.lld2, 0, 0, game);
            AdventurerHandler.jigsup_(game, 229);
            // !BEHEADED.
            return ret_val;

            // O113--	VAMPIRE BAT

            L35000:
            // !TIME TO FLY, JACK.
            MessageHandler.Speak(50, game);
            // !SELECT RANDOM DEST.
            f = AdventurerHandler.moveto_(game, (RoomIds)bats.batdrp[game.rnd_(9)], game.Player.Winner);
            f = RoomHandler.RoomDescription(0, game);
            return ret_val;
            // OAPPLI, PAGE 7

            // O114--	STICK

            L39000:
            if (game.ParserVectors.prsa != VerbIds.wavew)
            {
                goto L10;
            }
            // !WAVE?
            if (game.Player.Here == RoomIds.mrain)
            {
                goto L39500;
            }
            // !ON RAINBOW?
            if (game.Player.Here == RoomIds.pog || game.Player.Here == RoomIds.falls)
            {
                goto L39200;
            }
            MessageHandler.Speak(244, game);
            // !NOTHING HAPPENS.
            return ret_val;

            L39200:
            game.Objects[ObjectIds.pot].Flag1 |= ObjectFlags.IsVisible;
            game.Flags.rainbf = !game.Flags.rainbf;
            // !COMPLEMENT RAINBOW.
            i = (ObjectIds)245;
            // !ASSUME OFF.
            if (game.Flags.rainbf)
            {
                i = (ObjectIds)246;
            }
            // !IF ON, SOLID.
            MessageHandler.Speak((int)i, game);
            // !DESCRIBE.
            return ret_val;

            L39500:
            game.Flags.rainbf = false;
            // !ON RAINBOW,
            AdventurerHandler.jigsup_(game, 247);
            // !TAKE A FALL.
            return ret_val;

            // O115--	BALLOON, HANDLED EXTERNALLY

            L40000:
            ret_val = BalloonHandler.ballop_(game, arg);
            return ret_val;

            // O116--	HEADS

            L45000:
            if (game.ParserVectors.prsa != VerbIds.hellow)
            {
                goto L45100;
            }
            // !HELLO HEADS?
            MessageHandler.Speak(633, game);
            // !TRULY BIZARRE.
            return ret_val;

            L45100:
            if (game.ParserVectors.prsa == VerbIds.Read)
            {
                goto L10;
            }
            // !READ IS OK.
            SetNewObjectStatus(ObjectIds.lcase, 260, RoomIds.LivingRoom, 0, 0, game);
            // !MAKE LARGE CASE.
            i = (ObjectIds)dso4.RobAdventurer(game, game.Player.Winner, 0, ObjectIds.lcase, 0) + dso4.RobRoom(game, game.Player.Here, 100, 0, ObjectIds.lcase, 0);
            AdventurerHandler.jigsup_(game, 261);
            // !KILL HIM.
            return ret_val;
            // OAPPLI, PAGE 8

            // O117--	SPHERE

            L47000:
            if (game.Flags.cagesf || game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L10;
            }
            // !TAKE?
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
            SetNewObjectStatus(ObjectIds.spher, 0, 0, 0, 0, game);
            // !YOURE DEAD.
            game.Rooms[RoomIds.cager].Flags |= RoomFlags.RMUNG;
            game.Rooms[RoomIds.cager].Action = 147;
            AdventurerHandler.jigsup_(game, 148);
            // !MUNG PLAYER.
            return ret_val;

            L47500:
            SetNewObjectStatus(ObjectIds.spher, 0, 0, 0, 0, game);
            // !ROBOT TRIED,
            SetNewObjectStatus(ObjectIds.Robot, 264, 0, 0, 0, game);

            // !KILL HIM.
            SetNewObjectStatus(ObjectIds.cage, 0, game.Player.Here, 0, 0, game);
            // !INSERT MANGLED CAGE.
            return ret_val;

            // O118--	GEOMETRICAL BUTTONS

            L48000:
            if (game.ParserVectors.prsa != VerbIds.pushw)
            {
                goto L10;
            }
            // !PUSH?
            i = game.ParserVectors.prso - (int)ObjectIds.sqbut + 1;
            // !GET BUTTON INDEX.
            if (i <= 0 || i >= (ObjectIds)4)
            {
                goto L10;
            }
            // !A BUTTON?
            if (game.Player.Winner != ActorIds.Player)
            {
                switch (i)
                {
                    case (ObjectIds)1: goto L48100;
                    case (ObjectIds)2: goto L48200;
                    case (ObjectIds)3: goto L48300;
                }
            }
            AdventurerHandler.jigsup_(game, 265);
            // !YOU PUSHED, YOU DIE.
            return ret_val;

            L48100:
            i = (ObjectIds)267;
            if (game.Flags.carozf)
            {
                i = (ObjectIds)266;
            }
            // !SPEED UP?
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
            game.Flags.caroff = !game.Flags.caroff;
            // !FLIP CAROUSEL.
            if (!IsObjectInRoom(ObjectIds.irbox, RoomIds.Carousel, game))
            {
                return ret_val;
            }
            // !IRON BOX IN CAROUSEL?
            MessageHandler.Speak(269, game);
            // !YES, THUMP.
            game.Objects[ObjectIds.irbox].Flag1 ^= ObjectFlags.IsVisible;
            if (game.Flags.caroff)
            {
                game.Rooms[RoomIds.Carousel].Flags &= ~RoomFlags.SEEN;
            }
            return ret_val;

            // O119--	FLASK FUNCTION

            L49000:
            if (game.ParserVectors.prsa == VerbIds.openw)
            {
                goto L49100;
            }
            // !OPEN?
            if (game.ParserVectors.prsa != VerbIds.mungw && game.ParserVectors.prsa != VerbIds.Throw)
            {

                goto L10;
            }
            SetNewObjectStatus(ObjectIds.flask, 270, 0, 0, 0, game);
            // !KILL FLASK.
            L49100:
            game.Rooms[game.Player.Here].Flags |= RoomFlags.RMUNG;
            game.Rooms[game.Player.Here].Action = 271;
            AdventurerHandler.jigsup_(game, 272);
            // !POISONED.
            return ret_val;

            // O120--	BUCKET FUNCTION

            L50000:
            if (arg != 2)
            {
                goto L10;
            }

            // !READOUT?
            if (game.Objects[ObjectIds.Water].Container != ObjectIds.Bucket || game.Flags.bucktf)
            {
                goto L50500;
            }

            game.Flags.bucktf = true;
            // !BUCKET AT TOP.
            game.Clock.Ticks[(int)ClockIndices.cevbuc - 1] = 100;
            // !START COUNTDOWN.
            SetNewObjectStatus(ObjectIds.Bucket, 290, RoomIds.twell, 0, 0, game);
            // !REPOSITION BUCKET.
            goto L50900;
            // !FINISH UP.

            L50500:
            if (game.Objects[ObjectIds.Water].Container == ObjectIds.Bucket || !game.Flags.bucktf)
            {
                goto L10;
            }

            game.Flags.bucktf = false;
            // !BUCKET AT BOTTOM.
            SetNewObjectStatus(ObjectIds.Bucket, 291, RoomIds.BottomOfWell, 0, 0, game);

            L50900:
            if (av != ObjectIds.Bucket)
            {
                return ret_val;
            }
            // !IN BUCKET?
            f = AdventurerHandler.moveto_(game, RoomHandler.GetRoomThatContainsObject(ObjectIds.Bucket, game).Id, game.Player.Winner);
            // !MOVE ADVENTURER.
            f = RoomHandler.RoomDescription(0, game);
            // !DESCRIBE ROOM.
            return ret_val;
            // OAPPLI, PAGE 9

            // O121--	EATME CAKE

            L51000:
            if (game.ParserVectors.prsa != VerbIds.eatw || game.ParserVectors.prso != ObjectIds.ecake || game.Player.Here != RoomIds.alice)
            {
                goto L10;
            }

            SetNewObjectStatus(ObjectIds.ecake, 273, 0, 0, 0, game);
            // !VANISH CAKE.
            game.Objects[ObjectIds.Robot].Flag1 &= ~ObjectFlags.IsVisible;

            ret_val = AdventurerHandler.moveto_(game, RoomIds.alism, game.Player.Winner);
            // !MOVE TO ALICE SMALL.
            iz = 64;
            ir = RoomIds.alism;
            io = RoomIds.alice;
            goto L52405;

            // O122--	ICINGS

            L52000:
            if (game.ParserVectors.prsa != VerbIds.Read)
            {
                goto L52200;
            }

            // !READ?
            i = (ObjectIds)274;
            // !CANT READ.
            if (game.ParserVectors.prsi != 0)
            {
                i = (ObjectIds)275;
            }

            // !THROUGH SOMETHING?
            if (game.ParserVectors.prsi == ObjectIds.Bottle)
            {
                i = (ObjectIds)276;
            }

            // !THROUGH BOTTLE?
            if (game.ParserVectors.prsi == ObjectIds.flask)
            {
                i = game.ParserVectors.prso - (int)ObjectIds.orice + 277;
            }

            // !THROUGH FLASK?
            MessageHandler.Speak((int)i, game);
            // !READ FLASK.
            return ret_val;

            L52200:
            if (game.ParserVectors.prsa != VerbIds.Throw
                || game.ParserVectors.prso != ObjectIds.rdice
                || game.ParserVectors.prsi != ObjectIds.pool)
            {
                goto L52300;
            }

            SetNewObjectStatus(ObjectIds.pool, 280, 0, 0, 0, game);
            // !VANISH POOL.
            game.Objects[ObjectIds.saffr].Flag1 |= ObjectFlags.IsVisible;
            return ret_val;

            L52300:
            if (game.Player.Here != RoomIds.alice
                && game.Player.Here != RoomIds.alism
                && game.Player.Here != RoomIds.alitr)
            {
                goto L10;
            }
            if (game.ParserVectors.prsa != VerbIds.eatw
                && game.ParserVectors.prsa != VerbIds.Throw
                || game.ParserVectors.prso != ObjectIds.orice)
            {
                goto L52400;
            }

            SetNewObjectStatus(ObjectIds.orice, 0, 0, 0, 0, game);
            // !VANISH ORANGE ICE.
            game.Rooms[game.Player.Here].Flags |= RoomFlags.RMUNG;
            game.Rooms[game.Player.Here].Action = 281;
            AdventurerHandler.jigsup_(game, 282);
            // !VANISH ADVENTURER.
            return ret_val;

            L52400:
            if (game.ParserVectors.prsa != VerbIds.eatw || game.ParserVectors.prso != ObjectIds.blice)
            {
                goto L10;
            }
            SetNewObjectStatus(ObjectIds.blice, 283, 0, 0, 0, game);
            // !VANISH BLUE ICE.
            if (game.Player.Here != RoomIds.alism)
            {
                goto L52500;
            }
            // !IN REDUCED ROOM?
            game.Objects[ObjectIds.Robot].Flag1 |= ObjectFlags.IsVisible;

            io = game.Player.Here;
            ret_val = AdventurerHandler.moveto_(game, RoomIds.alice, game.Player.Winner);
            iz = 0;
            ir = RoomIds.alice;

            //  Do a size change, common loop used also by code at 51000

            L52405:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
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

            L54000:
            if (game.ParserVectors.prsa != VerbIds.burnw)
            {
                goto L10;
            }
            // !BURN?
            AdventurerHandler.jigsup_(game, 150);
            // !BOOM
            // !
            return ret_val;

            // O124--	MYSELF

            L55000:
            if (game.ParserVectors.prsa != VerbIds.Give)
            {
                goto L55100;
            }
            // !GIVE?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 2, 0, 0, ActorIds.Player, game);
            // !DONE.
            return ret_val;

            L55100:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L55200;
            }
            // !TAKE?
            MessageHandler.Speak(286, game);
            // !JOKE.
            return ret_val;

            L55200:
            if (game.ParserVectors.prsa != VerbIds.killw && game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L10;
            }
            AdventurerHandler.jigsup_(game, 287);
            // !KILL, NO JOKE.
            return ret_val;
            // OAPPLI, PAGE 10

            // O125--	PANELS INSIDE MIRROR

            L56000:
            if (game.ParserVectors.prsa != VerbIds.pushw)
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
            if (game.ParserVectors.prso == ObjectIds.rdwal || game.ParserVectors.prso == ObjectIds.ylwal)
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
            if (game.ParserVectors.prsa != VerbIds.pushw)
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
            if (game.ParserVectors.prso != ObjectIds.pindr)
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
            MessageHandler.Speak(736, game);
            // !NO, OPENS.
            game.Flags.wdopnf = true;
            // !INDICATE OPEN.
            game.Clock.Flags[(int)ClockIndices.cevpin - 1] = true;
            // !TIME OPENING.

            game.Clock.Ticks[(int)ClockIndices.cevpin - 1] = 5;
            return ret_val;

            L57200:
            MessageHandler.Speak(737, game);
            // !GDN SEES YOU, DIE.
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
            if (game.ParserVectors.prsa != VerbIds.attacw && game.ParserVectors.prsa != VerbIds.killw && game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L58100;
            }

            AdventurerHandler.jigsup_(game, 745);
            // !LOSE.
            return ret_val;

            L58100:
            if (game.ParserVectors.prsa != VerbIds.hellow)
            {
                goto L10;
            }
            // !HELLO?
            MessageHandler.Speak(746, game);
            // !NO REPLY.
            return ret_val;

            // O128--	GLOBAL MASTER

            L59000:
            if (game.ParserVectors.prsa != VerbIds.attacw && game.ParserVectors.prsa != VerbIds.killw && game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L59100;
            }
            AdventurerHandler.jigsup_(game, 747);
            // !BAD IDEA.
            return ret_val;

            L59100:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L10;
            }
            // !TAKE?
            MessageHandler.Speak(748, game);
            // !JOKE.
            return ret_val;

            // O129--	NUMERAL FIVE (FOR JOKE)

            L60000:
            if (game.ParserVectors.prsa != VerbIds.takew)
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
            if (!game.Flags.EndGame)
            {
                goto L45000;
            }
            // !IF NOT EG, DIE.
            if (game.ParserVectors.prsa != VerbIds.openw)
            {
                goto L61100;
            }
            // !OPEN?
            i = (ObjectIds)793;
            if ((game.Objects[ObjectIds.tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)794;
            }
            MessageHandler.Speak(i, game);
            game.Objects[ObjectIds.tomb].Flag2 |= ObjectFlags2.IsOpen;
            return ret_val;

            L61100:
            if (game.ParserVectors.prsa != VerbIds.closew)
            {
                goto L45000;
            }
            // !CLOSE?
            i = (ObjectIds)795;
            if ((game.Objects[ObjectIds.tomb].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                i = (ObjectIds)796;
            }
            MessageHandler.Speak(i, game);
            game.Objects[ObjectIds.tomb].Flag2 &= ~ObjectFlags2.IsOpen;
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

        public static bool sobjs_(Game game, int ri, int arg)
        {
            int i__1;
            bool ret_val;

            bool f;
            ObjectIds i;
            RoomIds mroom;
            int av;
            int odi2 = 0, odo2 = 0;

            if (game.ParserVectors.prso > (ObjectIds)220)
            {
                goto L5;
            }

            if (game.ParserVectors.prso != 0)
            {
                odo2 = game.Objects[game.ParserVectors.prso].Description2;
            }
            L5:
            if (game.ParserVectors.prsi != 0)
            {
                odi2 = game.Objects[game.ParserVectors.prsi].Description2;
            }

            av = game.Adventurers[game.Player.Winner].VehicleId;

            ret_val = true;

            switch (ri)
            {
                case 1: goto L1000;
                case 2: goto L3000;
                case 3: goto L4000;
                case 4: goto L6000;
                case 5: goto L7000;
                case 6: goto L8000;
                case 7: goto L9000;
                case 8: goto L13000;
                case 9: goto L14000;
                case 10: goto L16000;
                case 11: goto L17000;
                case 12: goto L21000;
                case 13: goto L23000;
                case 14: goto L24000;
                case 15: goto L27000;
                case 16: goto L28000;
                case 17: goto L29000;
                case 18: goto L30000;
                case 19: goto L31000;
                case 20: goto L33000;
                case 21: goto L34000;
                case 22: goto L36000;
                case 23: goto L37000;
                case 24: goto L38000;
                case 25: goto L41000;
                case 26: goto L42000;
                case 27: goto L43000;
                case 28: goto L44000;
                case 29: goto L46000;
                case 30: goto L53000;
                case 31: goto L56000;
            }
            throw new InvalidOperationException();
            //bug_(6, ri);

            // RETURN HERE TO DECLARE FALSE RESULT

            L10:
            ret_val = false;
            return ret_val;
            // SOBJS, PAGE 3

            // O1--	GUNK FUNCTION

            L1000:
            if (game.Objects[ObjectIds.gunk].Container == 0)
            {
                goto L10;
            }
            // !NOT INSIDE? F
            SetNewObjectStatus(ObjectIds.gunk, 122, 0, 0, 0, game);
            // !FALLS APART.
            return ret_val;

            // O2--	TROPHY CASE

            L3000:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L10;
            }
            // !TAKE?
            MessageHandler.Speak(128, game);
            // !CANT.
            return ret_val;

            // O3--	BOTTLE FUNCTION

            L4000:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L4100;
            }
            // !THROW?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 129, 0, 0, 0, game);
            // !BREAKS.
            return ret_val;

            L4100:
            if (game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L10;
            }
            // !MUNG?
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 131, 0, 0, 0, game);
            // !BREAKS.
            return ret_val;
            // SOBJS, PAGE 4

            // O4--	ROPE FUNCTION

            L6000:
            if (game.Player.Here == RoomIds.dome)
            {
                goto L6100;
            }
            // !IN DOME?
            game.Flags.domef = false;
            // !NO,
            if (game.ParserVectors.prsa != VerbIds.untiew)
            {
                goto L6050;
            }
            // !UNTIE?
            MessageHandler.Speak(134, game);
            // !CANT
            return ret_val;

            L6050:
            if (game.ParserVectors.prsa != VerbIds.tiew)
            {
                goto L10;
            }
            // !TIE?
            MessageHandler.Speak(135, game);
            // !CANT TIE
            return ret_val;

            L6100:
            if (game.ParserVectors.prsa != VerbIds.tiew || game.ParserVectors.prsi != ObjectIds.raili)
            {
                goto L6200;
            }
            if (game.Flags.domef)
            {
                goto L6150;
            }
            // !ALREADY TIED?
            game.Flags.domef = true;
            // !NO, TIE IT.
            game.Objects[ObjectIds.Rope].Flag1 |= ObjectFlags.HasNoDescription;
            game.Objects[ObjectIds.Rope].Flag2 |= ObjectFlags2.IsClimbable;
            SetNewObjectStatus(ObjectIds.Rope, 137, RoomIds.dome, 0, 0, game);
            return ret_val;

            L6150:
            MessageHandler.Speak(136, game);
            // !DUMMY.
            return ret_val;

            L6200:
            if (game.ParserVectors.prsa != VerbIds.untiew)
            {
                goto L6300;
            }
            // !UNTIE?
            if (game.Flags.domef)
            {
                goto L6250;
            }
            // !TIED?
            MessageHandler.Speak(134, game);
            // !NO, DUMMY.
            return ret_val;

            L6250:
            game.Flags.domef = false;
            // !YES, UNTIE IT.
            game.Objects[ObjectIds.Rope].Flag1 &= ~ObjectFlags.HasNoDescription;
            game.Objects[ObjectIds.Rope].Flag2 &= ~ObjectFlags2.IsClimbable;
            MessageHandler.Speak(139, game);
            return ret_val;

            L6300:
            if (game.Flags.domef || game.ParserVectors.prsa != VerbIds.Drop)
            {
                goto L6400;
            }
            // !DROP & UNTIED?
            SetNewObjectStatus(ObjectIds.Rope, 140, RoomIds.mtorc, 0, 0, game);
            // !YES, DROP.
            return ret_val;

            L6400:
            if (game.ParserVectors.prsa != VerbIds.takew || !game.Flags.domef)
            {
                goto L10;
            }
            MessageHandler.Speak(141, game);
            // !TAKE & TIED.
            return ret_val;

            // O5--	SWORD FUNCTION

            L7000:
            if (game.ParserVectors.prsa == VerbIds.takew && game.Player.Winner == ActorIds.Player)
            {
                game.Hack.IsSwordActive = true;
            }

            goto L10;

            // O6--	LANTERN

            L8000:
            if (game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L8100;
            }
            // !THROW?
            SetNewObjectStatus(ObjectIds.Lamp, 0, 0, 0, 0, game);
            // !KILL LAMP,
            SetNewObjectStatus(ObjectIds.blamp, 142, game.Player.Here, 0, 0, game);
            // !REPLACE WITH BROKEN.
            return ret_val;

            L8100:
            if (game.ParserVectors.prsa == VerbIds.trnonw)
            {
                game.Clock.Flags[(int)ClockIndices.cevlnt - 1] = true;
            }
            if (game.ParserVectors.prsa == VerbIds.trnofw)
            {
                game.Clock.Flags[(int)ClockIndices.cevlnt - 1] = false;
            }
            goto L10;

            // O7--	RUG FUNCTION

            L9000:
            if (game.ParserVectors.prsa != VerbIds.raisew)
            {
                goto L9100;
            }
            // !RAISE?
            MessageHandler.Speak(143, game);
            // !CANT
            return ret_val;

            L9100:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L9200;
            }
            // !TAKE?
            MessageHandler.Speak(144, game);
            // !CANT
            return ret_val;

            L9200:
            if (game.ParserVectors.prsa != VerbIds.movew)
            {
                goto L9300;
            }
            // !MOVE?
            i__1 = game.Switches.orrug + 145;
            MessageHandler.Speak(i__1, game);
            game.Switches.orrug = 1;
            game.Objects[ObjectIds.TrapDoor].Flag1 |= ObjectFlags.IsVisible;
            return ret_val;

            L9300:
            if (game.ParserVectors.prsa != VerbIds.lookuw
                || game.Switches.orrug != 0
                || (game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L10;
            }

            MessageHandler.Speak(345, game);
            return ret_val;
            // SOBJS, PAGE 5

            // O8--	SKELETON

            L13000:
            i = (ObjectIds)dso4.RobRoom(game, game.Player.Here, 100, RoomIds.lld2, 0, 0) + dso4.RobAdventurer(game, game.Player.Winner, RoomIds.lld2, 0, 0);

            if (i != 0)
            {
                MessageHandler.Speak(162, game);
            }

            // !IF ROBBED, SAY SO.
            return ret_val;

            // O9--	MIRROR

            L14000:
            if (game.Flags.mirrmf || game.ParserVectors.prsa != VerbIds.rubw)
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
            if (game.ParserVectors.prsa != VerbIds.lookw && game.ParserVectors.prsa != VerbIds.lookiw &&
                game.ParserVectors.prsa != VerbIds.Examine)
            {
                goto L14600;
            }
            i = (ObjectIds)164;
            // !MIRROR OK.
            if (game.Flags.mirrmf)
            {
                i = (ObjectIds)165;
            }
            // !MIRROR DEAD.
            MessageHandler.Speak(i, game);
            return ret_val;

            L14600:
            if (game.ParserVectors.prsa != VerbIds.takew)
            {
                goto L14700;
            }
            // !TAKE?
            MessageHandler.Speak(166, game);
            // !JOKE.
            return ret_val;

            L14700:
            if (game.ParserVectors.prsa != VerbIds.mungw && game.ParserVectors.prsa != VerbIds.Throw)
            {

                goto L10;
            }
            i = (ObjectIds)167;
            // !MIRROR BREAKS.
            if (game.Flags.mirrmf)
            {
                i = (ObjectIds)168;
            }
            // !MIRROR ALREADY BROKEN.
            game.Flags.mirrmf = true;
            game.Flags.badlkf = true;
            MessageHandler.Speak(i, game);
            return ret_val;
            // SOBJS, PAGE 6

            // O10--	DUMBWAITER

            L16000:
            if (game.ParserVectors.prsa != VerbIds.raisew)
            {
                goto L16100;
            }
            // !RAISE?
            if (game.Flags.cagetf)
            {
                goto L16400;
            }
            // !ALREADY AT TOP?
            SetNewObjectStatus(ObjectIds.tbask, 175, RoomIds.tshaf, 0, 0, game);
            // !NO, RAISE BASKET.
            SetNewObjectStatus(ObjectIds.fbask, 0, RoomIds.bshaf, 0, 0, game);
            game.Flags.cagetf = true;
            // !AT TOP.
            return ret_val;

            L16100:
            if (game.ParserVectors.prsa != VerbIds.lowerw)
            {
                goto L16200;
            }
            // !LOWER?
            if (!game.Flags.cagetf)
            {
                goto L16400;
            }
            // !ALREADY AT BOTTOM?
            SetNewObjectStatus(ObjectIds.tbask, 176, RoomIds.bshaf, 0, 0, game);
            // !NO, LOWER BASKET.
            SetNewObjectStatus(ObjectIds.fbask, 0, RoomIds.tshaf, 0, 0, game);
            game.Flags.cagetf = false;
            if (!RoomHandler.IsRoomLit(game.Player.Here, game))
            {
                MessageHandler.Speak(406, game);
            }
            // !IF DARK, DIE.
            return ret_val;

            L16200:
            if (game.ParserVectors.prso != ObjectIds.fbask && game.ParserVectors.prsi != ObjectIds.fbask)
            {
                goto L16300;
            }
            MessageHandler.Speak(130, game);
            // !WRONG BASKET.
            return ret_val;

            L16300:
            if (game.ParserVectors.prsa != VerbIds.takew)
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

            L17000:
            i = (ObjectIds)178;
            // !ASSUME DIRECT.
            if (game.ParserVectors.prso != ObjectIds.ghost)
            {
                i = (ObjectIds)179;
            }
            // !IF NOT, INDIRECT.
            MessageHandler.Speak(i, game);
            return ret_val;
            // !SPEAK AND EXIT.
            // SOBJS, PAGE 7

            // O12--	TUBE

            L21000:
            if (game.ParserVectors.prsa != VerbIds.Put || game.ParserVectors.prsi != ObjectIds.tube)
            {
                goto L10;
            }
            MessageHandler.Speak(186, game);
            // !CANT PUT BACK IN.
            return ret_val;

            // O13--	CHALICE

            L23000:
            if (game.ParserVectors.prsa != VerbIds.takew
                || game.Objects[game.ParserVectors.prso].Container != 0
                || RoomHandler.GetRoomThatContainsObject(game.ParserVectors.prso, game).Id  != RoomIds.Treasure
                || RoomHandler.GetRoomThatContainsObject(ObjectIds.thief, game).Id  != RoomIds.Treasure
                || (game.Objects[ObjectIds.thief].Flag2 & ObjectFlags2.FITEBT) == 0
                || !game.Hack.IsThiefActive)
            {
                goto L10;
            }

            MessageHandler.Speak(204, game);
            // !CANT TAKE.
            return ret_val;

            // O14--	PAINTING

            L24000:
            if (game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L10;
            }

            // !MUNG?
            MessageHandler.Speak(205, game);
            // !DESTROY PAINTING.
            game.Objects[game.ParserVectors.prso].ofval = 0;
            game.Objects[game.ParserVectors.prso].otval  = 0;
            game.Objects[game.ParserVectors.prso].Description1 = 207;
            game.Objects[game.ParserVectors.prso].Description2 = 206;
            return ret_val;
            // SOBJS, PAGE 8

            // O15--	BOLT

            L27000:
            if (game.ParserVectors.prsa != VerbIds.turnw)
            {
                goto L10;
            }

            // !TURN BOLT?
            if (game.ParserVectors.prsi != ObjectIds.wrenc)
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
            if (game.Flags.lwtidf)
            {
                goto L27200;
            }

            // !LOW TIDE NOW?
            game.Flags.lwtidf = true;

            // !NO, EMPTY DAM.
            MessageHandler.Speak(211, game);
            game.Objects[ObjectIds.Coffin].Flag2 &= ~ObjectFlags2.SCRDBT;
            game.Objects[ObjectIds.trunk].Flag1 |= ObjectFlags.IsVisible;
            game.Rooms[RoomIds.reser].Flags = (game.Rooms[RoomIds.reser].Flags | RoomFlags.LAND) & ~((int)RoomFlags.WATER + RoomFlags.SEEN);
            return ret_val;

            L27200:
            game.Flags.lwtidf = false;
            // !YES, FILL DAM.
            MessageHandler.Speak(212, game);
            if (ObjectHandler.IsObjectInRoom(ObjectIds.trunk, RoomIds.reser, game))
            {
                game.Objects[ObjectIds.trunk].Flag1 &= ~ObjectFlags.IsVisible;
            }

            game.Rooms[RoomIds.reser].Flags = (game.Rooms[RoomIds.reser].Flags | RoomFlags.WATER) & ~RoomFlags.LAND;
            return ret_val;

            L27500:
            MessageHandler.rspsub_(299, odi2, game);
            // !NOT WITH THAT.
            return ret_val;

            // O16--	GRATING

            L28000:
            if (game.ParserVectors.prsa != VerbIds.openw && game.ParserVectors.prsa != VerbIds.closew)
            {

                goto L10;
            }
            if (game.Flags.grunlf)
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
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.grate, (int)i, 885, game);
            // !OPEN/CLOSE.
            game.Rooms[RoomIds.mgrat].Flags &= ~RoomFlags.LIGHT;

            if ((game.Objects[ObjectIds.grate].Flag2 & ObjectFlags2.IsOpen) != 0)
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

            L29000:
            if (game.Player.Here != RoomIds.LivingRoom)
            {
                goto L29100;
            }
            // !FROM LIVING ROOM?
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.TrapDoor, 218, 219, game);
            // !OPEN/CLOSE.
            return ret_val;

            L29100:
            if (game.Player.Here != RoomIds.Cellar)
            {
                goto L10;
            }
            // !FROM CELLAR?
            if (game.ParserVectors.prsa != VerbIds.openw || (game.Objects[ObjectIds.TrapDoor].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L29200;
            }
            MessageHandler.Speak(220, game);
            // !CANT OPEN CLOSED DOOR.
            return ret_val;

            L29200:
            ret_val = RoomHandler.OpenCloseDoor(ObjectIds.TrapDoor, 0, 22, game);
            // !NORMAL OPEN/CLOSE.
            return ret_val;

            // O18--	DURABLE DOOR

            L30000:
            i = 0;
            // !ASSUME NO APPL.
            if (game.ParserVectors.prsa == VerbIds.openw)
            {
                i = (ObjectIds)221;
            }
            // !OPEN?
            if (game.ParserVectors.prsa == VerbIds.burnw)
            {
                i = (ObjectIds)222;
            }
            // !BURN?
            if (game.ParserVectors.prsa == VerbIds.mungw)
            {
                i = (ObjectIds)(game.rnd_(3) + 223);
            }
            // !MUNG?
            if (i == 0)
            {
                goto L10;
            }
            MessageHandler.Speak((int)i, game);
            return ret_val;

            // O19--	MASTER SWITCH

            L31000:
            if (game.ParserVectors.prsa != VerbIds.turnw)
            {
                goto L10;
            }

            // !TURN?
            if (game.ParserVectors.prsi != ObjectIds.screw)
            {
                goto L31500;
            }

            // !WITH SCREWDRIVER?
            if ((game.Objects[ObjectIds.machi].Flag2 & ObjectFlags2.IsOpen) != 0)
            {
                goto L31600;
            }

            // !LID UP?
            MessageHandler.Speak(226, game);

            // !NO, ACTIVATE.
            if (game.Objects[ObjectIds.coal].Container != ObjectIds.machi)
            {
                goto L31400;
            }

            // !COAL INSIDE?
            SetNewObjectStatus(ObjectIds.coal, 0, 0, 0, 0, game);
            // !KILL COAL,
            SetNewObjectStatus(ObjectIds.diamo, 0, 0, ObjectIds.machi, 0, game);
            // !REPLACE WITH DIAMOND.
            return ret_val;

            L31400:
            i__1 = game.Objects.Count;
            for (i = (ObjectIds)1; i <= (ObjectIds)i__1; ++i)
            {
                // !KILL NONCOAL OBJECTS.
                if (game.Objects[i].Container != ObjectIds.machi)
                {
                    goto L31450;
                }
                // !INSIDE MACHINE?
                SetNewObjectStatus((ObjectIds)i, 0, 0, 0, 0, game);
                // !KILL OBJECT AND CONTENTS.
                SetNewObjectStatus(ObjectIds.gunk, 0, 0, ObjectIds.machi, 0, game);
                // !REDUCE TO GUNK.
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

            L33000:
            if (game.ParserVectors.prso != ObjectIds.leak || game.ParserVectors.prsa != VerbIds.plugw || game.Switches.rvmnt <= 0)
            {
                goto L10;
            }

            if (game.ParserVectors.prsi != ObjectIds.putty)
            {
                goto L33100;
            }

            // !WITH PUTTY?
            game.Switches.rvmnt = -1;
            // !DISABLE LEAK.
            game.Clock.Ticks[(int)ClockIndices.cevmnt - 1] = 0;
            MessageHandler.Speak(577, game);
            return ret_val;

            L33100:
            MessageHandler.rspsub_(301, odi2, game);
            // !CANT WITH THAT.
            return ret_val;

            // O21--	DROWNING BUTTONS

            L34000:
            if (game.ParserVectors.prsa != VerbIds.pushw)
            {
                goto L10;
            }

            // !PUSH?
            switch (game.ParserVectors.prso - ObjectIds.rbutt + 1)
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
            if (game.Switches.rvmnt != 0)
            {
                goto L34500;
            }

            MessageHandler.Speak(233, game);
            // !NO, START LEAK.
            game.Switches.rvmnt = 1;
            game.Clock.Ticks[(int)ClockIndices.cevmnt - 1] = -1;
            return ret_val;

            L34500:
            // !BUTTON JAMMED.
            MessageHandler.Speak(234, game);
            return ret_val;

            // O22--	INFLATABLE BOAT

            L36000:
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
            if (game.ParserVectors.prsi != ObjectIds.Pump)
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
            if (game.ParserVectors.prsi != ObjectIds.lungs)
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
            if (game.ParserVectors.prsa != VerbIds.plugw)
            {
                goto L10;
            }
            // !PLUG?
            if (game.ParserVectors.prsi != ObjectIds.putty)
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
            if (game.ParserVectors.prsa != VerbIds.tiew
                || game.ParserVectors.prso != ObjectIds.brope
                || game.ParserVectors.prsi != ObjectIds.hook1
                && game.ParserVectors.prsi != ObjectIds.hook2)
            {
                goto L41500;
            }

            game.Switches.IsBalloonTiedUp = (int)game.ParserVectors.prsi;
            // !RECORD LOCATION.
            game.Clock.Flags[(int)ClockIndices.cevbal - 1] = false;
            // !STALL ASCENT.
            MessageHandler.Speak(248, game);
            return ret_val;

            L41500:
            if (game.ParserVectors.prsa != VerbIds.untiew || game.ParserVectors.prso != ObjectIds.brope)
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
            if (game.ParserVectors.prsa == VerbIds.takew)
            {
                i = (ObjectIds)251;
            }

            // !TAKE?
            if (game.ParserVectors.prsa == VerbIds.openw && game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)253;
            }

            // !OPEN AFTER BLAST?
            if (game.ParserVectors.prsa == VerbIds.openw && !game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)254;
            }

            // !OPEN BEFORE BLAST?
            if (game.ParserVectors.prsa == VerbIds.closew && game.Flags.WasSafeBlown)
            {
                i = (ObjectIds)253;
            }

            // !CLOSE AFTER?
            if (game.ParserVectors.prsa == VerbIds.closew && !game.Flags.WasSafeBlown)
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
            if (game.ParserVectors.prsa != VerbIds.burnw)
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

            if (game.Objects[game.ParserVectors.prso].otval == 0)
            {
                goto L44100;
            }

            // !TREASURE?
            MessageHandler.rspsub_(257, odo2, game);
            // !YES, GET DOOR.
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, 0, game);
            SetNewObjectStatus(ObjectIds.Gnome, 0, 0, 0, 0, game);
            // !VANISH GNOME.
            game.Flags.gnodrf = true;
            return ret_val;

            L44100:
            MessageHandler.rspsub_(258, odo2, game);
            // !NO, LOSE OBJECT.
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, 0, game);
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
            if (game.ParserVectors.prsa != VerbIds.Throw && game.ParserVectors.prsa != VerbIds.mungw)
            {
                goto L10;
            }

            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 262, 0, 0, 0, game);
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
            SetNewObjectStatus((ObjectIds)game.ParserVectors.prso, 0, 0, 0, ActorIds.Robot, game);
            // !PUT ON ROBOT.
            MessageHandler.rspsub_(302, odo2, game);
            return ret_val;

            L53200:
            if (game.ParserVectors.prsa != VerbIds.mungw && game.ParserVectors.prsa != VerbIds.Throw)
            {
                goto L10;
            }

            // !KILL ROBOT.
            SetNewObjectStatus(ObjectIds.Robot, 285, 0, 0, 0, game);

            return ret_val;

            // O31-- GRUE

            L56000:
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

            if (game.ParserVectors.prsa != VerbIds.movew && game.ParserVectors.prsa != VerbIds.openw)
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
                && game.ParserVectors.prsa != VerbIds.lookw)
            {
                goto L300;
            }

            i__1 = mrbf + 844;
            MessageHandler.Speak(game, i__1);
            // !LOOK IN MIRROR.
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != VerbIds.mungw)
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
            if (game.ParserVectors.prsa != VerbIds.pushw)
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