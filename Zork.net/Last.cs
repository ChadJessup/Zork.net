namespace Zork.Core
{
    /// <summary>
    /// This contains the object id of the last object the player
    /// has dealt with. This allows player to say open 'IT'.
    /// </summary>
    public class Last
    {
        public ObjectIds lastit { get; set; }
    }
}
