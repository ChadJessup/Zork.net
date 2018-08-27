
namespace Zork.Core
{
    public static class BalloonHandler
    {
        public static bool ballop_(Game game, int arg)
        {
            // System generated locals
            bool ret_val;

            ret_val = true;
            // !ASSUME WINS.
            if (arg != 2)
            {
                goto L200;
            }
            // !READOUT?
            if (game.ParserVectors.prsa != VerbId.Look)
            {
                goto L10;
            }

            // !ONLY PROCESS LOOK.
            if (game.Switches.IsBalloonInflated != 0)
            {
                goto L50;
            }
            // !INFLATED?
            MessageHandler.Speak(543, game);
            // !NO.
            goto L100;
            L50:
            MessageHandler.rspsub_(game, 544, game.Objects[(ObjectIds)game.Switches.IsBalloonInflated].Description2Id);
            // !YES.
            L100:
            if (game.Switches.IsBalloonTiedUp != 0)
            {
                MessageHandler.Speak(545, game);
            }

            // !HOOKED?
            return ret_val;

            L200:
            if (arg != 1)
            {
                goto L500;
            }
            // !READIN?
            if (game.ParserVectors.prsa != VerbId.Walk)
            {
                goto L300;
            }
            // !WALK?
            if (dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L250;
            }
            // !VALID EXIT?
            MessageHandler.Speak(546, game);
            // !NO, JOKE.
            return ret_val;

            L250:
            if (game.Switches.IsBalloonTiedUp == 0)
            {
                goto L275;
            }
            // !TIED UP?
            MessageHandler.Speak(547, game);
            // !YES, JOKE.
            return ret_val;

            L275:
            if (game.CurrentExit.ExitType != xpars_.xnorm)
            {
                goto L10;
            }
            // !NORMAL EXIT?
            if ((game.Rooms[game.CurrentExit.xroom1].Flags & RoomFlags.RMUNG) == 0)
            {
                game.State.BalloonLocation.Id = game.CurrentExit.xroom1;
            }
            L10:
            ret_val = false;
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != VerbId.Take || game.ParserVectors.DirectObject != (ObjectIds)game.Switches.IsBalloonInflated)
            {
                goto L350;
            }
            MessageHandler.rspsub_(game, 548, game.Objects[(ObjectIds)game.Switches.IsBalloonInflated].Description2Id);
            // !RECEP CONT TOO HOT.
            return ret_val;

            L350:
            if (game.ParserVectors.prsa != VerbId.Put
                || game.ParserVectors.IndirectObject != ObjectIds.recep
                || ObjectHandler.IsObjectEmpty(game, ObjectIds.recep))
            {
                goto L10;
            }

            MessageHandler.Speak(549, game);
            return ret_val;

            L500:
            if (game.ParserVectors.prsa != VerbId.unboaw || (game.Rooms[game.Player.Here].Flags & RoomFlags.LAND) == 0)
            {
                goto L600;
            }

            if (game.Switches.IsBalloonInflated != 0)
            {
                game.Clock[ClockId.cevbal].Ticks = 3;
            }
            // !HE GOT OUT, START BALLOON.
            goto L10;

            L600:
            if (game.ParserVectors.prsa != VerbId.Burn || game.Objects[game.ParserVectors.DirectObject].Container != ObjectIds.recep)
            {
                goto L700;
            }

            MessageHandler.rspsub_(game, 550, game.Objects[game.ParserVectors.DirectObject].Description2Id);
            // !LIGHT FIRE IN RECEP.
            game.Clock[ClockId.cevbrn].Ticks = game.Objects[game.ParserVectors.DirectObject].Size * 20;
            game.Objects[game.ParserVectors.DirectObject].Flag1 |= ((int)ObjectFlags.IsOn + ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT) & ~((int)ObjectFlags.IsTakeable + ObjectFlags.IsReadable);

            if (game.Switches.IsBalloonInflated != 0)
            {
                return ret_val;
            }

            if (!game.Flags.blabf)
            {
                ObjectHandler.SetNewObjectStatus(ObjectIds.blabe, 0, 0, ObjectIds.Balloon, 0, game);
            }

            game.Flags.blabf = true;
            game.Switches.IsBalloonInflated = (int)game.ParserVectors.DirectObject;
            game.Clock[ClockId.cevbal].Ticks = 3;
            MessageHandler.Speak(551, game);
            return ret_val;

            L700:
            if (game.ParserVectors.prsa == VerbId.unboaw
                && game.Switches.IsBalloonInflated != 0
                && (game.Rooms[game.Player.Here].Flags & RoomFlags.LAND) != 0)
            {
                game.Clock[ClockId.cevbal].Ticks = 3;
            }

            goto L10;
        }
    }
}
