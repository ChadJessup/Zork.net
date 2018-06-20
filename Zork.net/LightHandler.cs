using System;

namespace Zork.Core
{
    public static class LightHandler
    {
        public static bool lightp_(Game game, ObjectIndices obj)
        {
            bool ret_val;

            ObjectFlags flobts;
            int i;

            ret_val = true;
            // !ASSUME WINS
            flobts = ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.ONBT;
            if (obj != ObjectIndices.Candle)
            {
                goto L20000;
            }

            // !CANDLE?
            if (game.Switches.orcand != 0)
            {
                goto L19100;
            }

            // !FIRST REF?
            // !YES, CANDLES ARE
            game.Switches.orcand = 1;
            // !BURNING WHEN SEEN.
            game.Clock.Ticks[(int)ClockIndices.cevcnd - 1] = 50;

            L19100:
            if (game.ParserVectors.prsi == (int)ObjectIndices.Candle)
            {
                goto L10;
            }
            // !IGNORE IND REFS.
            if (game.ParserVectors.prsa != (int)VerbIndices.trnofw)
            {
                goto L19200;
            }
            // !TURN OFF?
            i = 513;
            // !ASSUME OFF.
            if ((game.Objects.oflag1[(int)ObjectIndices.Candle - 1] & ObjectFlags.ONBT) != 0)
            {
                i = 514;
            }

            // !IF ON, DIFFERENT.
            game.Clock.Flags[(int)ClockIndices.cevcnd - 1] = false;
            // !DISABLE COUNTDOWN.

            game.Objects.oflag1[(int)ObjectIndices.Candle - 1] &= ~ObjectFlags.ONBT;
            MessageHandler.Speak(i, game);
            return ret_val;

            L19200:
            if (game.ParserVectors.prsa != (int)VerbIndices.burnw && game.ParserVectors.prsa != (int)VerbIndices.trnonw)
            {
                goto L10;
            }

            if ((game.Objects.oflag1[(int)ObjectIndices.Candle - 1] & ObjectFlags.LITEBT) != 0)
            {
                goto L19300;
            }

            // !CANDLES TOO SHORT.
            MessageHandler.Speak(game, 515);
            return ret_val;

            L19300:
            if (game.ParserVectors.prsi != 0)
            {
                goto L19400;
            }

            // !ANY FLAME?
            MessageHandler.Speak(game, 516);
            // !NO, LOSE.
            game.ParserVectors.prswon = false;
            return ret_val;

            L19400:
            if (game.ParserVectors.prsi != (int)ObjectIndices.match || !((game.Objects.oflag1[(int)ObjectIndices.match - 1] & ObjectFlags.ONBT) != 0))
            {
                goto L19500;
            }

            i = 517;
            // !ASSUME OFF.
            if ((game.Objects.oflag1[(int)ObjectIndices.Candle - 1] & ObjectFlags.ONBT) != 0)
            {
                i = 518;
            }

            // !IF ON, JOKE.
            game.Objects.oflag1[(int)ObjectIndices.Candle - 1] |= ObjectFlags.ONBT;
            game.Clock.Flags[(int)ClockIndices.cevcnd - 1] = true;

            // !RESUME COUNTDOWN.
            MessageHandler.Speak(i, game);
            return ret_val;

            L19500:
            if (game.ParserVectors.prsi != (int)ObjectIndices.torch || !((game.Objects.oflag1[(int)ObjectIndices.torch - 1] & ObjectFlags.ONBT) != 0))
            {
                goto L19600;
            }

            if ((game.Objects.oflag1[(int)ObjectIndices.Candle - 1] & ObjectFlags.ONBT) != 0)
            {
                goto L19700;
            }
            // !ALREADY ON?

            ObjectHandler.SetNewObjectStatus(ObjectIndices.Candle, 521, 0, 0, 0, game);
            // !NO, VAPORIZE.
            return ret_val;

            L19600:
            MessageHandler.Speak(game, 519);
            // !CANT LIGHT WITH THAT.
            return ret_val;

            L19700:
            MessageHandler.Speak(game, 520);
            // !ALREADY ON.
            return ret_val;

            L20000:
            if (obj != ObjectIndices.match)
            {
                throw new InvalidOperationException();
                //bug_(6, obj);
            }

            if (game.ParserVectors.prsa != (int)VerbIndices.trnonw
                || game.ParserVectors.prso != (int)ObjectIndices.match)
            {
                goto L20500;
            }

            if (game.Switches.ormtch != 0)
            {
                goto L20100;
            }

            // !ANY MATCHES LEFT?
            MessageHandler.Speak(183, game);
            // !NO, LOSE.
            return ret_val;

            L20100:
            // !DECREMENT NO MATCHES.
            --game.Switches.ormtch;

            game.Objects.oflag1[(int)ObjectIndices.match - 1] |= flobts;
            game.Clock.Ticks[(int)ClockIndices.cevmat - 1] = 2;
            // !COUNTDOWN.
            MessageHandler.Speak(184, game);
            return ret_val;

            L20500:
            if (game.ParserVectors.prsa != (int)VerbIndices.trnofw || (game.Objects.oflag1[(int)ObjectIndices.match - 1] & ObjectFlags.ONBT) == 0)
            {
                goto L10;
            }

            game.Objects.oflag1[(int)ObjectIndices.match - 1] &= ~flobts;
            game.Clock.Ticks[(int)ClockIndices.cevmat - 1] = 0;
            MessageHandler.Speak(185, game);
            return ret_val;

            // HERE FOR FALSE RETURN

            L10:
            ret_val = false;
            return ret_val;
        }
    }
}