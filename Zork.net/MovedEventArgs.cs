namespace Zork.Core
{
    public class MovedEventArgs
    {
        public Game Game { get; private set; }
        public MovedEventArgs(Game game) => this.Game = game;
    }
}