using System;
using System.Collections.Generic;
using System.Text;
using Zork.Core.Attributes;

namespace Zork.Core.Verbs
{
    public class VerbActions
    {
        [VerbAction(VerbId.Read)]
        public static bool Read(Game game)
        {
            return true;
        }
    }
}
