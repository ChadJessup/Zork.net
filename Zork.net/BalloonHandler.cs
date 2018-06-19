using Zork.Core.Room;

namespace Zork.Core
{
    public static class BalloonHandler
    {
        public static bool ballop_(Game game, int arg)
        {
            /* System generated locals */
            bool ret_val;

            ret_val = true;
            /* 						!ASSUME WINS. */
            if (arg != 2)
            {
                goto L200;
            }
            /* 						!READOUT? */
            if (game.ParserVectors.prsa != (int)VIndices.lookw)
            {
                goto L10;
            }
            /* 						!ONLY PROCESS LOOK. */
            if (game.Switches.binff != 0)
            {
                goto L50;
            }
            /* 						!INFLATED? */
            MessageHandler.Speak(game, 543);
            /* 						!NO. */
            goto L100;
            L50:
            MessageHandler.rspsub_(game, 544, game.Objects.odesc2[game.Switches.binff - 1]);
            /* 						!YES. */
            L100:
            if (game.Switches.btief != 0)
            {
                MessageHandler.Speak(game, 545);
            }
            /* 						!HOOKED? */
            return ret_val;

            L200:
            if (arg != 1)
            {
                goto L500;
            }
            /* 						!READIN? */
            if (game.ParserVectors.prsa != (int)VIndices.walkw)
            {
                goto L300;
            }
            /* 						!WALK? */
            if (dso3.findxt_(game, game.ParserVectors.prso, game.Player.Here))
            {
                goto L250;
            }
            /* 						!VALID EXIT? */
            MessageHandler.Speak(game, 546);
            /* 						!NO, JOKE. */
            return ret_val;

            L250:
            if (game.Switches.btief == 0)
            {
                goto L275;
            }
            /* 						!TIED UP? */
            MessageHandler.Speak(game, 547);
            /* 						!YES, JOKE. */
            return ret_val;

            L275:
            if (game.curxt_.xtype != xpars_.xnorm)
            {
                goto L10;
            }
            /* 						!NORMAL EXIT? */
            if ((game.Rooms.RoomFlags[game.curxt_.xroom1 - 1] & RoomFlags.RMUNG) == 0)
            {
                game.State.bloc = game.curxt_.xroom1;
            }
            L10:
            ret_val = false;
            return ret_val;

            L300:
            if (game.ParserVectors.prsa != (int)VIndices.takew || game.ParserVectors.prso != game.Switches.binff)
            {
                goto L350;
            }
            MessageHandler.rspsub_(game, 548, game.Objects.odesc2[game.Switches.binff - 1]);
            /* 						!RECEP CONT TOO HOT. */
            return ret_val;

            L350:
            if (game.ParserVectors.prsa != (int)VIndices.putw
                || game.ParserVectors.prsi != (int)ObjectIndices.recep
                || ObjectHandler.qempty_(game, ObjectIndices.recep))
            {
                goto L10;
            }

            MessageHandler.Speak(game, 549);
            return ret_val;

            L500:
            if (game.ParserVectors.prsa != (int)VIndices.unboaw || (game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RLAND) == 0)
            {
                goto L600;
            }
            if (game.Switches.binff != 0)
            {
                game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            }
            /* 						!HE GOT OUT, START BALLOON. */
            goto L10;

            L600:
            if (game.ParserVectors.prsa != (int)VIndices.burnw || game.Objects.ocan[game.ParserVectors.prso - 1] != (int)ObjectIndices.recep)
            {
                goto L700;
            }

            MessageHandler.rspsub_(game, 550, game.Objects.odesc2[game.ParserVectors.prso - 1]);
            /* 						!LIGHT FIRE IN RECEP. */
            game.Clock.Ticks[(int)ClockIndices.cevbrn - 1] = game.Objects.osize[game.ParserVectors.prso - 1] * 20;
            game.Objects.oflag1[game.ParserVectors.prso - 1] |= ((int)ObjectFlags.ONBT + ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT) & ~((int)ObjectFlags.TAKEBT + ObjectFlags.READBT);

            if (game.Switches.binff != 0)
            {
                return ret_val;
            }
            if (!game.Flags.blabf)
            {
                ObjectHandler.newsta_(ObjectIndices.blabe, 0, 0, ObjectIndices.ballo, 0, game);
            }
            game.Flags.blabf = true;
            game.Switches.binff = game.ParserVectors.prso;
            game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            MessageHandler.Speak(game, 551);
            return ret_val;

            L700:
            if (game.ParserVectors.prsa == (int)VIndices.unboaw && game.Switches.binff != 0
                && (game.Rooms.RoomFlags[game.Player.Here - 1] & RoomFlags.RLAND) != 0)
            {
                game.Clock.Ticks[(int)ClockIndices.cevbal - 1] = 3;
            }

            goto L10;
        }
    }
}
