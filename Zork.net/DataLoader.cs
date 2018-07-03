using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zork.Core
{
    public static class DataLoader
    {
        public static Game LoadDataFile(string filename = "dtextc.dat")
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Unable to find data file: " + filename);
            }

            var bytes = File.ReadAllBytes(filename);
            var game = new Game(bytes);

            int i = DataLoader.ReadInt(bytes, game);
            int j = DataLoader.ReadInt(bytes, game);
            int k = DataLoader.ReadInt(bytes, game);

            if (i != 2 || j != 7)
            {
                throw new InvalidOperationException("Bad version on data file");
            }

            game.State.MaxScore = DataLoader.ReadInt(bytes, game);
            game.Star.strbit = DataLoader.ReadInt(bytes, game);
            game.State.egmxsc = DataLoader.ReadInt(bytes, game);

            var roomCount = DataLoader.ReadInt(bytes, game);
            var desc1 = new List<int>();
            var desc2 = new List<int>();
            var exits = new List<int>();
            var actions = new List<int>();
            var values = new List<int>();
            DataLoader.ReadInts(roomCount, desc1, bytes, game);
            DataLoader.ReadInts(roomCount, desc2, bytes, game);
            DataLoader.ReadInts(roomCount, exits, bytes, game);
            DataLoader.ReadPartialInts(roomCount, actions, bytes, game);
            DataLoader.ReadPartialInts(roomCount, values, bytes, game);

            var tempFlags = new List<int>();
            DataLoader.ReadInts(roomCount, tempFlags, bytes, game);

            for (int ridx = 1; ridx <= roomCount; ridx++)
            {
                var room = new Room
                {
                    Id = (RoomIds)ridx,
                    Action = actions[ridx - 1],
                    Description1 = desc1[ridx - 1],
                    Description2 = desc2[ridx - 1],
                    Exit = exits[ridx - 1],
                    Flags = (RoomFlags)tempFlags[ridx - 1],
                    Score = values[ridx - 1]
                };

                game.Rooms.Add(room.Id, room);
            }

            game.Rooms.Add(RoomIds.NoWhere, new Room
            {
                Id = RoomIds.NoWhere
            });

            game.Exits.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Exits.Count, game.Exits.Travel, bytes, game);

            var objectCount = DataLoader.ReadInt(bytes, game);
            var odesc1 = new List<int>();
            var odesc2 = new List<int>();
            var odesco = new List<int>();
            var oactio = new List<int>();
            var ofval = new List<int>();
            var otval = new List<int>();
            var Sizes = new List<int>();
            var ocapac = new List<int>();
            var oroom = new List<int>();
            var oadv = new List<int>();
            var ocan = new List<int>();
            var oread = new List<int>();
            var oflag1 = new List<ObjectFlags>();
            var oflag2 = new List<ObjectFlags2>();
            var tempObjFlags = new List<int>();

            DataLoader.ReadInts(objectCount, odesc1, bytes, game);
            DataLoader.ReadInts(objectCount, odesc2, bytes, game);
            DataLoader.ReadPartialInts(objectCount, odesco, bytes, game);
            DataLoader.ReadPartialInts(objectCount, oactio, bytes, game);

            DataLoader.ReadInts(objectCount, tempObjFlags, bytes, game);
            for (int idxObj = 0; idxObj < tempObjFlags.Count; idxObj++)
            {
                oflag1.Add((ObjectFlags)tempObjFlags[idxObj]);
            }

            tempObjFlags.Clear();
            DataLoader.ReadPartialInts(objectCount, tempObjFlags, bytes, game);

            for (int idx = 0; idx < tempObjFlags.Count; idx++)
            {
                oflag2.Add((ObjectFlags2)tempObjFlags[idx]);
            }

            DataLoader.ReadPartialInts(objectCount, ofval, bytes, game);
            DataLoader.ReadPartialInts(objectCount, otval, bytes, game);
            DataLoader.ReadInts(objectCount, Sizes, bytes, game);
            DataLoader.ReadPartialInts(objectCount, ocapac, bytes, game);
            DataLoader.ReadInts(objectCount, oroom, bytes, game);
            DataLoader.ReadPartialInts(objectCount, oadv, bytes, game);
            DataLoader.ReadPartialInts(objectCount, ocan, bytes, game);
            DataLoader.ReadPartialInts(objectCount, oread, bytes, game);

            var containerHolding = new List<Object>();

            for (int objIdx = 1; objIdx <= objectCount; objIdx++)
            {
                var newObject = new Object
                {
                    Id = (ObjectIds)objIdx,
                    Description1 = odesc1[objIdx - 1],
                    Description2 = odesc2[objIdx - 1],
                    odesco = odesco[objIdx - 1],
                    Action = oactio[objIdx - 1],
                    ofval = ofval[objIdx - 1],
                    otval = otval[objIdx - 1],
                    Size = Sizes[objIdx - 1],
                    Capacity = ocapac[objIdx - 1],
                    Adventurer = (ActorIds)oadv[objIdx - 1],
                    Container = (ObjectIds)ocan[objIdx - 1],
                    oread = oread[objIdx - 1],
                    Flag1 = oflag1[objIdx - 1],
                    Flag2 = oflag2[objIdx - 1],
                };

                // Some items exist, but not in a room specifically, they can be on an adventurer...
                if ((RoomIds)oroom[objIdx - 1] != RoomIds.NoWhere && game.Rooms.ContainsKey((RoomIds)oroom[objIdx - 1]))
                {
                    game.Rooms[(RoomIds)oroom[objIdx - 1]].Objects.Add(newObject);
                }

                if (newObject.Container != ObjectIds.Nothing)
                {
                    containerHolding.Add(newObject);
                }

                game.Objects.Add(newObject.Id, newObject);
            }

            foreach (var containedItem in containerHolding)
            {
                var container = game.Objects.First(o => o.Value.Id == containedItem.Container);
                container.Value.ContainedObjects.Add(containedItem);
            }

            game.Objects.Add(ObjectIds.Nothing, new Object());

            game.Rooms2.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.Rooms, bytes, game);
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.RRoom, bytes, game);

            game.Clock.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Clock.Count, game.Clock.Ticks, bytes, game);
            DataLoader.ReadInts(game.Clock.Count, game.Clock.Actions, bytes, game);
            DataLoader.ReadFlags(game.Clock.Count, game.Clock.Flags, bytes, game);

            game.Villians.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.villns, bytes, game);
            DataLoader.ReadPartialInts(game.Villians.Count, game.Villians.vprob, bytes, game);
            DataLoader.ReadPartialInts(game.Villians.Count, game.Villians.vopps, bytes, game);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.vbest, bytes, game);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.vmelee, bytes, game);

            var advCount = DataLoader.ReadInt(bytes, game);
            var advRooms = new List<int>();
            var advScores = new List<int>();
            var advVehicles = new List<int>();
            var advObjects = new List<int>();
            var advActions = new List<int>();
            var advStrengths = new List<int>();
            var advFlags = new List<int>();

            DataLoader.ReadInts(advCount, advRooms, bytes, game);
            DataLoader.ReadPartialInts(advCount, advScores, bytes, game);
            DataLoader.ReadPartialInts(advCount, advVehicles, bytes, game);
            DataLoader.ReadInts(advCount, advObjects, bytes, game);
            DataLoader.ReadInts(advCount, advActions, bytes, game);
            DataLoader.ReadInts(advCount, advStrengths, bytes, game);
            DataLoader.ReadPartialInts(advCount, advFlags, bytes, game);

            for (int advIdx = 1; advIdx <= advCount; advIdx++)
            {
                var newAdventurer = new Adventurer
                {
                    Id = (ActorIds)advIdx,
                    Action = advActions[advIdx - 1],
                    Strength = advStrengths[advIdx - 1],
                    CurrentRoom = game.Rooms[(RoomIds)advRooms[advIdx - 1]],
                    Score = advScores[advIdx - 1],
                    VehicleId = advVehicles[advIdx - 1],
                    ObjectId = (ObjectIds)advObjects[advIdx - 1]
                };

                newAdventurer.CurrentRoom.Adventurers.Add(newAdventurer);
                game.Adventurers.Add(newAdventurer.Id, newAdventurer);
            }

            game.Adventurers.Add(ActorIds.NoOne, new Adventurer());

            game.Star.mbase = DataLoader.ReadInt(bytes, game);
            game.Messages.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Messages.Count, game.Messages.rtext, bytes, game);
            // Location of beginning of message text
            game.Messages.mrloc = game.DataPosition;

            // TODO: See if we can just store the DateTime object, not sure how all this is used currently.
            var now = DateTime.Now;
            game.Time.shour = now.Hour;
            game.Time.smin = now.Minute;
            game.Time.ssec = now.Second;

            game.Player.Winner = ActorIds.Player;
            game.Last.lastit = game.Adventurers[ActorIds.Player].ObjectId;
            game.Player.Here = game.Adventurers[game.Player.Winner].CurrentRoom.Id;

            game.State.BalloonLocation = RoomHandler.GetRoomThatContainsObject(ObjectIds.Balloon, game);
            game.Hack.ThiefPosition = RoomHandler.GetRoomThatContainsObject(ObjectIds.thief, game).Id;

            return game;
        }

        private static void ReadPartialInts(int count, List<int> list, byte[] bytes, Game game)
        {
            // fill the list up to count
            list.AddRange(Enumerable.Repeat(0, count));

            while (true)
            {
                int index;

                if (count < 255)
                {
                    index = bytes[game.DataPosition++];
                    if (index == 255)
                    {
                        return;
                    }
                }
                else
                {
                    index = ReadInt(bytes, game);
                    if (index == -1)
                    {
                        return;
                    }
                }

                int value = ReadInt(bytes, game);
                list[index] = value;
            }
        }

        private static void ReadFlags(int count, List<bool> flags, byte[] bytes, Game game)
        {
            while (count-- != 0)
            {
                byte value = bytes[game.DataPosition++];
                flags.Add(value == 0 ? false : true);
            }
        }

        private static void ReadInts(int count, List<int> list, byte[] bytes, Game game)
        {
            while (count-- != 0)
            {
                list.Add(DataLoader.ReadInt(bytes, game));
            }
        }

        private static int ReadInt(byte[] bytes, Game game)
        {
            byte ch = bytes[game.DataPosition++];
            int chInt = (ch > 127 ? ch - 256 : ch);

            return chInt * 256 + bytes[game.DataPosition++];
        }
    }
}
