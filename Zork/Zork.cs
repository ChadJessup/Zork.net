using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zork.Core;

namespace Zork
{
    public class Zork
    {
        private string lastRoomName = "West of House";
        private int lastScore = 0;
        private int lastMoveCount = 0;

        public static void Main(string[] args)
        {
            var zork = new Zork();
            zork.Go();
            Console.ReadLine();
        }

        public void Go()
        {
            var game = Game.Initialize(useJson: true);
            var gameOld = Game.Initialize(useJson: false);

            this.DrawHeader("", 0, 0);

            game.MoveOccurred += this.OnMoveOccurred;
            game.ReadInput = this.ReadInput;
            game.WriteOutput = this.WriteOutput;
            game.Play();
        }

        public void WriteOutput(string output)
        {
            Console.Write(output);
            if (output.Contains("\n"))
            {
                this.DrawHeader(this.lastRoomName, this.lastScore, this.lastMoveCount);
            }
        }

        public string ReadInput() => Console.ReadLine();

        private void OnMoveOccurred(object sender, MovedEventArgs e)
        {
            this.lastRoomName = e.Game.CurrentRoomName;
            this.lastScore = e.Game.CurrentScore;
            this.lastMoveCount = e.Game.MovesCount;

            this.DrawHeader(this.lastRoomName, this.lastScore, this.lastMoveCount);
        }

        private void DrawHeader(string roomName, int score, int moves)
        {
            var currentPosLeft = Console.CursorLeft;
            var currentPosTop = Console.CursorTop;

            var currentBackground = Console.BackgroundColor;
            var currentForeground = Console.ForegroundColor;

            Console.SetCursorPosition(0, Console.WindowTop);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            int padding = Console.WindowWidth - roomName.Length - 12 - 12;
            Console.WriteLine($"{roomName,0}{new string(' ', padding)}{"Score: " + score,5}{"Moves: " + moves,10}");

            Console.BackgroundColor = currentBackground;
            Console.ForegroundColor = currentForeground;
            Console.SetCursorPosition(currentPosLeft, currentPosTop);
        }
    }
}
