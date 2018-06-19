namespace Zork.Core
{
    public class Hack
    {
        public int thfpos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if Thief was introduced.
        /// </summary>
        public bool thfflg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if Thief is active.
        /// </summary>
        public bool thfact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if sword is active.
        /// </summary>
        public bool swdact { get; set; }

        /// <summary>
        /// Gets or sets a value based on if the sword is on.  0 == OFF.
        /// </summary>
        public int swdsta { get; set; }
    }
}
