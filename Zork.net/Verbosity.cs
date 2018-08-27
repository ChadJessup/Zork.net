using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public enum Verbosity
    {
        // 0/1/2/3= SHORT/OBJ/ROOM/FULL
        Short = 0,
        ObjectsOnly = 1,
        RoomAndContents = 2,
        Full = 3,

    }
}
