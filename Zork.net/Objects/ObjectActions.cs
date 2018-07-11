using System;
using System.Collections.Generic;
using System.Text;
using Zork.Core.Attributes;

namespace Zork.Core.Objects
{
    public class ObjectActions
    {
        [ObjectAction(ObjectIds.Mailbox)]
        public static bool MailboxAction(Game game)
        {
            return true;
        }
    }
}
