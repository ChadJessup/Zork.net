using System;
using Zork.Core;

namespace Zork
{
    public class Zork
    {
        public static void Main(string[] args)
        {
            var game = Game.Initialize();

            game.Play();
            Console.ReadLine();
        }
    }
}
