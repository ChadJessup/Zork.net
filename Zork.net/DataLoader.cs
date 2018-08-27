﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Zork.Core.Attributes;
using Zork.Core.Converters;

namespace Zork.Core
{
    public static class DataLoader
    {
        /// <summary>
        /// Loads game data from the original Dungeon data file dtextc.dat.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Game LoadDataFile(string filename = "dtextc.dat", bool useJson = false)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Unable to find data file: " + filename);
            }

            Game game = null;

            if (useJson)
            {
                game = LoadFromJson(@"C:\Users\shabu\Desktop\zork.json");
            }
            else
            {
                var bytes = File.ReadAllBytes(filename);
                game = new Game(bytes);

                // Order matters of everything below!

                int major = DataLoader.ReadInt(bytes, game);
                int minor = DataLoader.ReadInt(bytes, game);
                int revision = DataLoader.ReadInt(bytes, game);

                if (major != 2 || minor != 7)
                {
                    throw new InvalidOperationException("Bad version on data file");
                }

                LoadStars(bytes, game);

                LoadRooms(bytes, game);

                LoadExits(bytes, game);

                LoadObjects(bytes, game);

                LoadRoom2(bytes, game);

                LoadClockEvents(bytes, game);

                LoadVillians(bytes, game);

                LoadAdventurers(bytes, game);

                game.Star.mbase = DataLoader.ReadInt(bytes, game);
                game.Messages.Count = DataLoader.ReadInt(bytes, game);
                DataLoader.ReadInts(game.Messages.Count, game.Messages.rtext, bytes, game);

                // Location of beginning of message text
                game.Messages.mrloc = game.DataPosition;
            }

            // TODO: See if we can just store the DateTime object, not sure how all this is used currently.
            var now = DateTime.Now;
            game.Time.shour = now.Hour;
            game.Time.smin = now.Minute;
            game.Time.ssec = now.Second;

            game.Player.Winner = ActorIds.Player;
            game.Last.lastit = game.Adventurers[ActorIds.Player].ObjectId;
            game.Player.Here = game.Adventurers[game.Player.Winner].CurrentRoom.Id;

            game.State.BalloonLocation = RoomHandler.GetRoomThatContainsObject(ObjectIds.Balloon, game);
            game.Hack.ThiefPosition = RoomHandler.GetRoomThatContainsObject(ObjectIds.Thief, game).Id;

            SetRoomNames(game, game.Rooms);

            return game;
        }

        private static Game LoadFromJson(string path)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            // To avoid polluting our game models as much as possible,
            // some of the metadata in our json is stored during parsing
            // in these temporary collections to help connect some of
            // the models together...
            var objContainers = new List<(ObjectIds objId, ObjectIds containerId)>();

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new RoomConverter());
            settings.Converters.Add(new GameConverter(objContainers));
            settings.Converters.Add(new AdventurerConverter());
            settings.Converters.Add(new ObjectConverter(objContainers));
            settings.Converters.Add(new VillianConverter());
            settings.Converters.Add(new ClockEventConverter());

            return JsonConvert.DeserializeObject<Game>(File.ReadAllText(path), settings);
        }

        private static Dictionary<RoomIds, (RoomActionAttribute attrib, Func<Game, bool> action)> LoadRoomActions()
        {
            var dic = new Dictionary<RoomIds, (RoomActionAttribute, Func<Game, bool>)>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<RoomActionAttribute>(inherit: true))
                    {
                        var func = (Func<Game, bool>)Delegate.CreateDelegate(typeof(Func<Game, bool>), null, method);
                        dic.Add(attrib.RoomId, (attrib, func));
                    }
                }
            }

            return dic;
        }

        private static Dictionary<ObjectIds, (ObjectActionAttribute attrib, Func<Game, bool> action)> LoadObjectActions()
        {
            var dic = new Dictionary<ObjectIds, (ObjectActionAttribute, Func<Game, bool>)>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<ObjectActionAttribute>(inherit: true))
                    {
                        var func = (Func<Game, bool>)Delegate.CreateDelegate(typeof(Func<Game, bool>), null, method);
                        dic.Add(attrib.ObjectId, (attrib, func));
                    }
                }
            }

            return dic;
        }

        private static void SetRoomNames(Game game, Dictionary<RoomIds, Room> rooms)
        {
            foreach (var room in rooms.Values)
            {
                room.Name = MessageHandler.Speak(room.Description2Id, game);
            }
        }

        private static void LoadStars(byte[] bytes, Game game)
        {
            game.State.MaxScore = DataLoader.ReadInt(bytes, game);
            game.Star.strbit = DataLoader.ReadInt(bytes, game);
            game.State.egmxsc = DataLoader.ReadInt(bytes, game);
        }

        private static void LoadExits(byte[] bytes, Game game)
        {
            game.Exits.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Exits.Count, game.Exits.Travel, bytes, game);
        }

        private static void LoadRoom2(byte[] bytes, Game game)
        {
            game.Rooms2.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.Rooms, bytes, game);
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.RRoom, bytes, game);
        }

        private static void LoadClockEvents(byte[] bytes, Game game)
        {
            var count = DataLoader.ReadInt(bytes, game);
            var ticks = new List<int>();
            var actions = new List<int>();
            var flags = new List<bool>();

            DataLoader.ReadInts(count, ticks, bytes, game);
            DataLoader.ReadInts(count, actions, bytes, game);
            DataLoader.ReadFlags(count, flags, bytes, game);

            for (int i = 1; i <= count; i++)
            {
                game.Clock.Add((ClockId)i, new ClockEvent
                {
                    Id = (ClockId)i,
                    Ticks = ticks[i - 1],
                    Actions = actions[i - 1],
                    Flags = flags[i - 1],
                });
            }
        }

        private static void LoadVillians(byte[] bytes, Game game)
        {
            var count = DataLoader.ReadInt(bytes, game);
            var villns = new List<int>();
            var vprob = new List<int>();
            var vopps = new List<int>();
            var vbest = new List<int>();
            var vmelee = new List<int>();

            DataLoader.ReadInts(count, villns, bytes, game);
            DataLoader.ReadPartialInts(count, vprob, bytes, game);
            DataLoader.ReadPartialInts(count, vopps, bytes, game);
            DataLoader.ReadInts(count, vbest, bytes, game);
            DataLoader.ReadInts(count, vmelee, bytes, game);

            for (int i = 1; i <= count; i++)
            {
                var newVillian = new Villian
                {
                    Melee = vmelee[i - 1],
                    Id = (ObjectIds)villns[i - 1],
                    WakeupProbability = vprob[i - 1],
                    Opponent = (ObjectIds)vopps[i - 1],
                    BestWeapon = (ObjectIds)vbest[i - 1],
                };

                game.Villians.Add(newVillian.Id, newVillian);
            }
        }

        private static void LoadAdventurers(byte[] bytes, Game game)
        {
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
        }

        private static void LoadObjects(byte[] bytes, Game game)
        {
            var objectActions = LoadObjectActions();

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
                    Description1Id = odesc1[objIdx - 1],
                    Description2Id = odesc2[objIdx - 1],
                    LongDescriptionId = odesco[objIdx - 1],
                    Action = oactio[objIdx - 1],
                    Value = ofval[objIdx - 1],
                    otval = otval[objIdx - 1],
                    Size = Sizes[objIdx - 1],
                    Capacity = ocapac[objIdx - 1],
                    Adventurer = (ActorIds)oadv[objIdx - 1],
                    Container = (ObjectIds)ocan[objIdx - 1],
                    oreadId = oread[objIdx - 1],
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

                if (objectActions.TryGetValue(newObject.Id, out var value))
                {
                    newObject.DoAction = value.action;
                }

                game.Objects.Add(newObject.Id, newObject);
            }

            foreach (var containedItem in containerHolding)
            {
                var container = game.Objects.First(o => o.Value.Id == containedItem.Container);
                container.Value.ContainedObjects.Add(containedItem);
            }


            game.Objects.Add(ObjectIds.Nothing, new Object());
        }

        private static void LoadRooms(byte[] bytes, Game game)
        {
            var roomActions = LoadRoomActions();

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
                    Description1Id = desc1[ridx - 1],
                    Description2Id = desc2[ridx - 1],
                    Exit = exits[ridx - 1],
                    Flags = (RoomFlags)tempFlags[ridx - 1],
                    Score = values[ridx - 1],
                };

                if (roomActions.TryGetValue(room.Id, out var value))
                {
                    room.DoAction = value.action;
                }

                game.Rooms.Add(room.Id, room);
            }

            game.Rooms.Add(RoomIds.NoWhere, new Room
            {
                Id = RoomIds.NoWhere
            });

            var room2 = game.Rooms[RoomIds.WestOfHouse];
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
