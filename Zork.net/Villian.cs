using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public class Villian
    {
        public ObjectIds Id { get; set; }
        public ObjectIds Opponent { get; set; }
        public int WakeupProbability { get; set; }
        public ObjectIds BestWeapon { get; set; }
        public int Melee { get; set; }
    }
}
