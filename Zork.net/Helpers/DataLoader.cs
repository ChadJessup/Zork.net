using System;
using System.Collections.Generic;
using System.IO;

namespace Zork.Core.Helpers
{
    public static class DataLoader
    {
        private static int DataPosition { get; set; } = 0;

        public static Game LoadDataFile(string filename = "dtextc.dat")
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Unable to find data file: " + filename);
            }

            var bytes = File.ReadAllBytes(filename);

            int i = DataLoader.ReadInt(bytes);
            int j = DataLoader.ReadInt(bytes);
            int k = DataLoader.ReadInt(bytes);

            if (i != 2 || j != 7)
            {
                throw new InvalidOperationException("Bad version on data file");
            }

            var game = new Game();

            game.State.MaxScore = DataLoader.ReadInt(bytes);
            game.Star.strbit = DataLoader.ReadInt(bytes);
            game.State.egmxsc = DataLoader.ReadInt(bytes);

            Console.WriteLine($"Rooms Position: {DataLoader.DataPosition}");
            game.Rooms.Count = DataLoader.ReadInt(bytes);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomDescriptions1, bytes);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomDescriptions2, bytes);
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomExits, bytes);
            DataLoader.ReadPartialInts(game.Rooms.Count, game.Rooms.RoomActions, bytes);
            DataLoader.ReadPartialInts(game.Rooms.Count, game.Rooms.RoomValues, bytes);
            Console.WriteLine($"After read partial ints Position: {DataLoader.DataPosition}");
            DataLoader.ReadInts(game.Rooms.Count, game.Rooms.RoomFlags, bytes);

            Console.WriteLine($"Exits Position: {DataLoader.DataPosition}");
            game.Exits.Count = DataLoader.ReadInt(bytes);
            Console.WriteLine($"Exits: {game.Exits.Count}");
            DataLoader.ReadInts(game.Exits.Count, game.Exits.Travel, bytes);

            Console.WriteLine($"Objects Position: {DataLoader.DataPosition}");
            game.Objects.Count = DataLoader.ReadInt(bytes);
            Console.WriteLine($"Objects Count: {game.Objects.Count}");

            DataLoader.ReadInts(game.Objects.Count, game.Objects.odesc1, bytes);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.odesc2, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.odesco, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oactio, bytes);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.oflag1, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oflag2, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ofval, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.otval, bytes);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.osize, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ocapac, bytes);
            DataLoader.ReadInts(game.Objects.Count, game.Objects.oroom, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oadv, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.ocan, bytes);
            DataLoader.ReadPartialInts(game.Objects.Count, game.Objects.oread, bytes);

            Console.WriteLine($"Rooms 2 Position: {DataLoader.DataPosition}");

            game.Rooms2.Count = DataLoader.ReadInt(bytes);
            Console.WriteLine($"Rooms 2 Count: {game.Rooms2.Count}");
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.Rooms, bytes);
            DataLoader.ReadInts(game.Rooms2.Count, game.Rooms2.RRoom, bytes);


            game.Clock.Count = DataLoader.ReadInt(bytes);
            Console.WriteLine($"Clock Events Count: {game.Clock.Count}");
            DataLoader.ReadInts(game.Clock.Count, game.Clock.Ticks, bytes);
            DataLoader.ReadInts(game.Clock.Count, game.Clock.Actions, bytes);
            DataLoader.ReadFlags(game.Clock.Count, game.Clock.Flags, bytes);

            Console.WriteLine($"Villians Position: {DataLoader.DataPosition}");
            game.Villians.Count = DataLoader.ReadInt(bytes);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.villns, bytes);
            DataLoader.ReadPartialInts(game.Villians.Count, game.Villians.vprob, bytes);
            DataLoader.ReadPartialInts(game.Villians.Count, game.Villians.vopps, bytes);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.vbest, bytes);
            DataLoader.ReadInts(game.Villians.Count, game.Villians.vmelee, bytes);

            game.Adventurers.Count = DataLoader.ReadInt(bytes);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Rooms, bytes);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Scores, bytes);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Vehicles, bytes);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Objects, bytes);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.Actions, bytes);
            DataLoader.ReadInts(game.Adventurers.Count, game.Adventurers.astren, bytes);
            DataLoader.ReadPartialInts(game.Adventurers.Count, game.Adventurers.Flags, bytes);

            game.Star.mbase = DataLoader.ReadInt(bytes);
            game.Messages.Count = DataLoader.ReadInt(bytes);
            DataLoader.ReadInts(game.Messages.Count, game.Messages.rtext, bytes);
            // Location of beginning of message text
            game.Messages.mrloc = DataLoader.DataPosition;
/*
                        itime_(&time_1.shour, &time_1.smin, &time_1.ssec);

                        play_1.winner = aindex_1.player;
                        last_1.lastit = advs_1.aobj[aindex_1.player - 1];
                        play_1.here = advs_1.aroom[play_1.winner - 1];
                        hack_1.thfpos = objcts_1.oroom[oindex_1.thief - 1];
                        state_1.bloc = objcts_1.oroom[oindex_1.ballo - 1];
            */

            return game;
        }

        private static void ReadPartialInts(int count, List<int> list, byte[] bytes)
        {
            while (true)
            {
                int i;

                if (count < 255)
                {
                    i = bytes[DataLoader.DataPosition++];
                    //Console.Write($"File pos: {DataLoader.DataPosition - 1}, First i: {i} ");
                    if (i == 255)
                    {
                      //  Console.WriteLine();
                        return;
                    }
                }
                else
                {
                    i = ReadInt(bytes);
                  //  Console.Write($"Second i: {i} ");
                    if (i == -1)
                    {
                    //    Console.WriteLine();
                        return;
                    }
                }

                i = ReadInt(bytes);
                //Console.WriteLine($"Third int: {i} C: {count}");
                list.Add(i);
            }

        }

        private static void ReadFlags(int count, List<bool> flags, byte[] bytes)
        {

        }

        private static void ReadInts(int count, List<int> list, byte[] bytes)
        {
            while (count-- != 0)
            {
                list.Add(DataLoader.ReadInt(bytes));
            }
        }

        private static int ReadInt(byte[] bytes)
        {
            byte ch = bytes[DataLoader.DataPosition++];
            ch = (byte)(ch > 127 ? ch - 256 : ch);

            return ch * 256 + bytes[DataLoader.DataPosition++];
        }
    }
}
