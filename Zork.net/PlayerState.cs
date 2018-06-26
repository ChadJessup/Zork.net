namespace Zork.Core
{
    public class PlayerState
    {
        public int Moves { get; set; }
        public int Deaths { get; set; }
        public int RawScore { get; set; }
        public int MaxScore { get; set; }
        public int MaxLoad { get; set; }
        public int ltshft { get; set; }
        public Room BalloonLocation { get; set; }
        public Room mungrm { get; set; }

        /// <summary>
        /// Gets or set Hello Sailor count. (???)
        /// </summary>
        public int HelloSailor { get; set; }

        public int egscor { get; set; }
        public int egmxsc { get; set; }
    }
}
