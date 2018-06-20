namespace Zork.Core
{
    public class Hack
    {
        /// <summary>
        /// Gets or sets the Thief's position.
        /// </summary>
        public int ThiefPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if Thief was introduced.
        /// </summary>
        public bool WasThiefIntroduced { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if Thief is active.
        /// </summary>
        public bool IsThiefActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if sword is active.
        /// </summary>
        public bool IsSwordActive { get; set; }

        /// <summary>
        /// Gets or sets a value based on if the sword is on.  0 == OFF.
        /// </summary>
        public int SwordStatus { get; set; }
    }
}
