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

            for (int ridx = 0; ridx < roomCount; ridx++)
            {
                var room = new Room
                {
                    Id = (RoomIds)ridx,
                    Action = actions[ridx],
                    Description1 = desc1[ridx],
                    Description2 = desc2[ridx],
                    Exit = exits[ridx],
                    Flags = (RoomFlags)tempFlags[ridx],
                    Score = values[ridx]
                };

                game.Rooms.Add(room.Id, room);
            }

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

            for (int objIdx = 0; objIdx < objectCount; objIdx++)
            {
                var newObject = new Object
                {
                    Id = (ObjectIds)objIdx,
                    Description1 = odesc1[objIdx],
                    Description2 = odesc2[objIdx],
                    odesco = odesco[objIdx],
                    oactio = oactio[objIdx],
                    ofval = ofval[objIdx],
                    otval = otval[objIdx],
                    Size = Sizes[objIdx],
                    ocapac = ocapac[objIdx],
                    Room = (RoomIds)oroom[objIdx],
                    Adventurer = (ActorIds)oadv[objIdx],
                    Container = (ObjectIds)ocan[objIdx],
                    oread = oread[objIdx],
                    Flag1 = oflag1[objIdx],
                    Flag2 = oflag2[objIdx],
                };

                game.Objects.Add(newObject.Id, newObject);
            }

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

            game.Adventurers.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Rooms, bytes, game);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Scores, bytes, game);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Vehicles, bytes, game);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Objects, bytes, game);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Actions, bytes, game);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.astren, bytes, game);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Flags, bytes, game);

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
            game.Last.lastit = (ObjectIds)game.Adventurers.Objects[(int)(ActorIds.Player - 1)];
            game.Player.Here = (RoomIds)game.Adventurers.Rooms[(int)game.Player.Winner - 1];

            game.State.BalloonLocation = game.Objects[ObjectIds.Balloon].Room;
            game.Hack.ThiefPosition = game.Objects[ObjectIds.thief].Room;

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
