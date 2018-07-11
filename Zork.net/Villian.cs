using System;

namespace Zork.Core
{
    public class Villian : IComparable, IComparable<Villian>
    {
        public ObjectIds Id { get; set; }
        public ObjectIds Opponent { get; set; }
        public int WakeupProbability { get; set; }
        public ObjectIds BestWeapon { get; set; }
        public int Melee { get; set; }

        public int CompareTo(Villian other) => ((int)this.Id).CompareTo((int)other.Id);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Villian other = obj as Villian;
            return this.CompareTo(other);
        }
    }
}
