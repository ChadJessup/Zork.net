using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zork.Core.Object;
using Zork.Core.Room;

namespace Zork.Core.Helpers
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

            game.Rooms.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomDescriptions1, bytes, game);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomDescriptions2, bytes, game);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomExits, bytes, game);
            DataLoader.ReadPartialInts(game.Rooms.Count, game.Rooms.RoomActions, bytes, game);
            DataLoader.ReadPartialInts(game.Rooms.Count, game.Rooms.RoomValues, bytes, game);

            var tempFlags = new List<int>();
            DataLoader.ReadInts(game.Rooms.Count, tempFlags, bytes, game);

            for (int idx = 0; idx < tempFlags.Count; idx++)
            {
                game.Rooms.RoomFlags.Add((RoomFlags)tempFlags[idx]);
            }

            game.Exits.Count = DataLoader.ReadInt(bytes, game);
            DataLoader.ReadInts(game.Exits.Count, game.Exits.Travel, bytes, game);

            game.Objects.Count = DataLoader.ReadInt(bytes, game);

            DataLoader.ReadInts(game.Objects.Count, game.Objects.odesc1, bytes, game);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.odesc2, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.odesco, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oactio, bytes, game);

            var tempObjFlags = new List<int>();
            DataLoader.ReadInts(game.Objects.Count, tempObjFlags, bytes, game);
            for (int idxObj = 0; idxObj < tempObjFlags.Count; idxObj++)
            {
                game.Objects.oflag1.Add((ObjectFlags)tempObjFlags[idxObj]);
            }

            tempObjFlags.Clear();
            DataLoader.ReadPartialInts(game.Objects.Count, tempObjFlags, bytes, game);

            for (int idx = 0; idx < tempObjFlags.Count; idx++)
            {
                game.Objects.oflag2.Add((ObjectFlags2)tempObjFlags[idx]);
            }

            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ofval, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.otval, bytes, game);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.osize, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ocapac, bytes, game);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.oroom, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oadv, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ocan, bytes, game);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oread, bytes, game);

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

            game.Player.Winner = (int)AIndices.player;
            game.Last.lastit = game.Adventurers.Objects[(int)(AIndices.player - 1)];
            game.Player.Here = game.Adventurers.Rooms[game.Player.Winner - 1];

            game.State.bloc = game.Objects.oroom[(int)(ObjectIndices.ballo - 1)];
            game.Hack.thfpos = game.Objects.oroom[(int)(ObjectIndices.thief - 1)];

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
                if (value != 0 && value != 1)
                {

                }

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
