using Zork.Core.Attributes;

namespace Zork.Core.Rooms
{
    // R3--	LIVING ROOM.  DESCRIPTION DEPENDS ON MAGICF (STATE OF
    // 	DOOR TO CYCLOPS ROOM), RUG (MOVED OR NOT), DOOR (OPEN OR CLOSED)
    [Room(RoomIds.LivingRoom)]
    public class LivingRoom : Room
    {
        public LivingRoom(Room room)
            : base(room)
        {
        }

        public override bool RoomAction(Game game)
        {
            if (game.ParserVectors.prsa == VerbId.Look)
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

            if (game.ParserVectors.prsa != VerbId.Take &&
                (game.ParserVectors.prsa != VerbId.Put || game.ParserVectors.IndirectObject != ObjectIds.TrophyCase))
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