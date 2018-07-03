using System;
using System.Collections.Generic;
using Zork.Core;

namespace StringExtractor
{
    public class Program
    {
        private Dictionary<int, string> speaks = new Dictionary<int, string>();
        private Dictionary<int, string> roomDescriptions = new Dictionary<int, string>();
        private int currentSpeaks = 0;
        private int currentRoomDesc = 0;
        private string currentString = string.Empty;

        public static void Main(string[] args)
        {
            var zork = new Program();
            zork.Go();
            Console.ReadLine();
        }

        public void Go()
        {
            var game = Game.Initialize();
            game.ReadInput = this.ReadInput;
            game.WriteOutput = this.WriteOutput;

            for (currentSpeaks = 0; currentSpeaks < 1023; currentSpeaks++)
            {
                MessageHandler.Speak(game, currentSpeaks);
                this.speaks.Add(this.currentSpeaks, this.currentString);
                this.currentString = string.Empty;
            }

            for (currentRoomDesc = 0; currentRoomDesc < 217; currentRoomDesc++)
            {
                game.Player.Here = (RoomIds)this.currentRoomDesc;
                game.Adventurers[ActorIds.Player].CurrentRoom.Id = game.Player.Here;

                RoomHandler.RoomDescription(game, 3);

                this.roomDescriptions.Add(this.currentRoomDesc, this.currentString);
                this.currentString = string.Empty;
            }
        }

        public void WriteOutput(string output) => this.currentString += output;
        public string ReadInput() => Console.ReadLine();
    }
}