using Zork.Core;
using Zork.Core.Attributes;

namespace Zork.Rooms
{
    public class RoomActions
    {
        [RoomAction(RoomIds.BehindHouse)]
        public static bool BehindHouseAction(Game game)
        {
            // !LOOK?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return true;
            }

            var messageNumber = game.Objects[ObjectIds.Window].Flag2.HasFlag(ObjectFlags2.IsOpen)
                ? 12 // partially ajar
                : 13; // open

            MessageHandler.Speak(11, game.Messages.text[messageNumber - 1], game);
            // !DESCRIBE.
            return true;
        }

        [RoomAction(RoomIds.Kitchen)]
        public static bool KitchenAction(Game game)
        {
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                return true;
            }

            // !ASSUME CLOSED.
            int messageId = 13;
            // !IF OPEN, AJAR.
            if (game.Objects[ObjectIds.Window].Flag2.HasFlag(ObjectFlags2.IsOpen))
            {
                messageId = 12;
            }

            // !DESCRIBE.
            MessageHandler.Speak(14, game.Messages.text[messageId - 1], game);
            return true;
        }

        // R4--	CELLAR.  SHUT DOOR AND BAR IT IF HE JUST WALKED IN.
        [RoomAction(RoomIds.Cellar)]
        public static bool CellarRoomAction(Game game)
        {
            // !LOOK?
            if (game.ParserVectors.prsa == VerbId.Look)
            {
                // !DESCRIBE CELLAR.
                MessageHandler.Speak(21, game);

                return true;
            }

            if (game.ParserVectors.prsa != VerbId.WalkIn)
            {
                return true;
            }

            if ((game.Objects[ObjectIds.TrapDoor].Flag2 & (int)ObjectFlags2.IsOpen + ObjectFlags2.WasTouched) != ObjectFlags2.IsOpen)
            {
                return true;
            }

            game.Objects[ObjectIds.TrapDoor].Flag2 = (game.Objects[ObjectIds.TrapDoor].Flag2 | ObjectFlags2.WasTouched) & ~ObjectFlags2.IsOpen;

            // !SLAM AND BOLT DOOR.
            MessageHandler.Speak(22, game);
            return true;
        }
    }
}
