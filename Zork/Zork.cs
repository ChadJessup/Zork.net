using System;
using Zork.Core;

namespace Zork
{
    public class Zork
    {
        public static void Main(string[] args)
        {
            var zork = new Zork();
            zork.Go();
            Console.ReadLine();
        }

        public void Go()
        {
            var game = Game.Initialize();
            game.ReadInput = this.ReadInput;
            game.WriteOutput = this.WriteOutput;
            game.Play();
        }

        public void WriteOutput(string output) => Console.Write(output);
        public string ReadInput() => Console.ReadLine();
    }
}
