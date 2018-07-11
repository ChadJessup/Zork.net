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
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                return true;
            }

            var messageNumber = game.Objects[ObjectIds.Window].Flag2.HasFlag(ObjectFlags2.IsOpen)
                ? 12 // partially ajar
                : 13; // open

            MessageHandler.rspsub_(11, messageNumber, game);
            // !DESCRIBE.
            return true;
        }

        [RoomAction(RoomIds.Kitchen)]
        public static bool KitchenAction(Game game)
        {
            if (game.ParserVectors.prsa != VerbIds.Look)
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
            MessageHandler.rspsub_(14, messageId, game);
            return true;
        }

        // R3--	LIVING ROOM.  DESCRIPTION DEPENDS ON MAGICF (STATE OF
        // 	DOOR TO CYCLOPS ROOM), RUG (MOVED OR NOT), DOOR (OPEN OR CLOSED)
        [RoomAction(RoomIds.LivingRoom)]
        public static bool LivingRoomAction(Game game)
        {
            if (game.ParserVectors.prsa == VerbIds.Look)
            {
                // !ASSUME NO HOLE.
                int messageId = 15;

                if (game.Flags.IsDoorToCyclopsRoomOpen)
                {
                    messageId = 16;
                }

                MessageHandler.Speak(messageId, game);

                messageId = game.Switches.IsRugMoved + 17;

                // !DOOR OPEN?
                if (game.Objects[ObjectIds.TrapDoor].Flag2.HasFlag(ObjectFlags2.IsOpen))
                {
                    messageId += 2;
                }

                // !DESCRIBE.
                MessageHandler.Speak(messageId, game);

                return true;
            }

            if (game.ParserVectors.prsa != VerbIds.Take &&
                (game.ParserVectors.prsa != VerbIds.Put || game.ParserVectors.IndirectObject != ObjectIds.TrophyCase))
            {
                return true;
            }

            // !SCORE TROPHY CASE.
            // !RETAIN RAW SCORE AS WELL.
            game.Adventurers[game.Player.Winner].Score = game.State.RawScore;

            foreach (var obj in game.Objects[ObjectIds.TrophyCase].ContainedObjects)
            {
                game.Adventurers[game.Player.Winner].Score += obj.otval;
            }

            // !SEE IF ENDGAME TRIG.
            AdventurerHandler.ScoreUpdate(game, 0);

            return true;
        }


    }
}
