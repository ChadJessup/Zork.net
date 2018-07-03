
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
            if (game.ParserVectors.prsa != VerbIds.Look)
            {
                goto L10;
            }

            // !ONLY PROCESS LOOK.
            if (game.Switches.IsBalloonInflated != 0)
            {
                goto L50;
            }
            // !INFLATED?
            MessageHandler.Speak(game, 543);
            // !NO.
            goto L100;
            L50:
            MessageHandler.rspsub_(game, 544, game.Objects[(ObjectIds)game.Switches.IsBalloonInflated].Description2);
            // !YES.
            L100:
            if (game.Switches.IsBalloonTiedUp != 0)
            {
                MessageHandler.Speak(game, 545);
            }

            // !HOOKED?
            return ret_val;

            L200:
            if (arg != 1)
            {
                goto L500;
            }
            // !READIN?
            if (game.ParserVectors.prsa != VerbIds.Walk)
            {
                goto L300;
            }
            // !WALK?
            if (dso3.FindExit(game, (int)game.ParserVectors.DirectObject, game.Player.Here))
            {
                goto L250;
            }
            // !VALID EXIT?
            MessageHandler.Speak(game, 546);
            // !NO, JOKE.
            return ret_val;

            L250:
            if (game.Switches.IsBalloonTiedUp == 0)
            {
                goto L275;
            }
            // !TIED UP?
            MessageHandler.Speak(game, 547);
            // !YES, JOKE.
            return ret_val;

            L275:
            if (game.curxt_.xtype != xpars_.xnorm)
            {
                goto L10;
            }
            // !NORMAL EXIT?
            if ((game.Rooms[game.curxt_.xroom1].Flags & RoomFlags.RMUNG) == 0)
            {
                game.State.BalloonLocation.Id = game.curxt_.xroom1;
            }
            L10:
            ret_val = false;
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != VerbIds.Take || game.ParserVectors.DirectObject != (ObjectIds)game.Switches.IsBalloonInflated)
            {
                goto L350;
            }
            MessageHandler.rspsub_(game, 548, game.Objects[(ObjectIds)game.Switches.IsBalloonInflated].Description2);
            // !RECEP CONT TOO HOT.
            return ret_val;

            L350:
            if (game.ParserVectors.prsa != VerbIds.Put
                || game.ParserVectors.IndirectObject != ObjectIds.recep
                || ObjectHandler.IsObjectEmpty(game, ObjectIds.recep))
            {
                goto L10;
            }

            MessageHandler.Speak(game, 549);
            return ret_val;

            L500:
            if (game.ParserVectors.prsa != VerbIds.unboaw || (game.Rooms[game.Player.Here].Flags & RoomFlags.LAND) == 0)
            {
                goto L600;
            }

            if (game.Switches.IsBalloonInflated != 0)
            {
                game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            }
            // !HE GOT OUT, START BALLOON.
            goto L10;

            L600:
            if (game.ParserVectors.prsa != VerbIds.Burn || game.Objects[game.ParserVectors.DirectObject].Container != ObjectIds.recep)
            {
                goto L700;
            }

            MessageHandler.rspsub_(game, 550, game.Objects[game.ParserVectors.DirectObject].Description2);
            // !LIGHT FIRE IN RECEP.
            game.Clock.Ticks[(int)ClockIndices.cevbrn - 1] = game.Objects[game.ParserVectors.DirectObject].Size * 20;
            game.Objects[game.ParserVectors.DirectObject].Flag1 |= ((int)ObjectFlags.IsOn + ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT) & ~((int)ObjectFlags.IsTakeable + ObjectFlags.READBT);

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
            game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            MessageHandler.Speak(game, 551);
            return ret_val;

            L700:
            if (game.ParserVectors.prsa == VerbIds.unboaw
                && game.Switches.IsBalloonInflated != 0
                && (game.Rooms[game.Player.Here].Flags & RoomFlags.LAND) != 0)
            {
                game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            }

            goto L10;
        }
    }
}
