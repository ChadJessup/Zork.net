using System;

namespace Zork.Core
{
    public static class LightHandler
    {
        public static bool lightp_(Game game, ObjectIds obj)
        {
            bool ret_val;

            ObjectFlags flobts;
            int i;

            ret_val = true;
            // !ASSUME WINS
            flobts = ObjectFlags.FLAMBT + (int)ObjectFlags.LITEBT + (int)ObjectFlags.IsOn;
            if (obj != ObjectIds.Candle)
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
            game.Clock[ClockId.CandleClock].Ticks = 50;

            L19100:
            if (game.ParserVectors.IndirectObject == ObjectIds.Candle)
            {
                goto L10;
            }
            // !IGNORE IND REFS.
            if (game.ParserVectors.prsa != VerbId.TurnOff)
            {
                goto L19200;
            }
            // !TURN OFF?
            i = 513;
            // !ASSUME OFF.
            if ((game.Objects[ObjectIds.Candle].Flag1 & ObjectFlags.IsOn) != 0)
            {
                i = 514;
            }

            // !IF ON, DIFFERENT.
            game.Clock[ClockId.CandleClock].Flags = false;
            // !DISABLE COUNTDOWN.

            game.Objects[ObjectIds.Candle].Flag1 &= ~ObjectFlags.IsOn;
            MessageHandler.Speak(i, game);
            return ret_val;

            L19200:
            if (game.ParserVectors.prsa != VerbId.Burn && game.ParserVectors.prsa != VerbId.TurnOn)
            {
                goto L10;
            }

            // !CANDLES TOO SHORT.
            if ((game.Objects[ObjectIds.Candle].Flag1.HasFlag(ObjectFlags.LITEBT)))
            {
                goto L19300;
            }

            MessageHandler.Speak(515, game);
            return ret_val;

            L19300:
            if (game.ParserVectors.IndirectObject != 0)
            {
                goto L19400;
            }

            // !ANY FLAME?
            MessageHandler.Speak(516, game);
            // !NO, LOSE.
            game.ParserVectors.prswon = false;
            return ret_val;

            L19400:
            if (game.ParserVectors.IndirectObject != ObjectIds.Match || !((game.Objects[ObjectIds.Match].Flag1 & ObjectFlags.IsOn) != 0))
            {
                goto L19500;
            }

            i = 517;
            // !ASSUME OFF.
            if ((game.Objects[ObjectIds.Candle].Flag1.HasFlag(ObjectFlags.IsOn)))
            {
                i = 518;
            }

            // !IF ON, JOKE.
            game.Objects[ObjectIds.Candle].Flag1 |= ObjectFlags.IsOn;

            // !RESUME COUNTDOWN.
            game.Clock[ClockId.CandleClock].Flags = true;

            MessageHandler.Speak(i, game);
            return ret_val;

            L19500:
            if (game.ParserVectors.IndirectObject != ObjectIds.Torch || !((game.Objects[ObjectIds.Torch].Flag1 & ObjectFlags.IsOn) != 0))
            {
                goto L19600;
            }

            // !ALREADY ON?
            if ((game.Objects[ObjectIds.Candle].Flag1 & ObjectFlags.IsOn) != 0)
            {
                goto L19700;
            }

            // !NO, VAPORIZE.
            ObjectHandler.SetNewObjectStatus(ObjectIds.Candle, 521, 0, 0, 0, game);

            return ret_val;

            L19600:
            // !CANT LIGHT WITH THAT.
            MessageHandler.Speak(519, game);

            return ret_val;

            L19700:
            MessageHandler.Speak(520, game);
            // !ALREADY ON.
            return ret_val;

            L20000:
            if (obj != ObjectIds.Match)
            {
                throw new InvalidOperationException();
                //bug_(6, obj);
            }

            if (game.ParserVectors.prsa != VerbId.TurnOn || game.ParserVectors.DirectObject != ObjectIds.Match)
            {
                goto L20500;
            }

            if (game.Switches.MatchCount != 0)
            {
                goto L20100;
            }

            // !ANY MATCHES LEFT?
            MessageHandler.Speak(183, game);
            // !NO, LOSE.
            return ret_val;

            L20100:
            // !DECREMENT NO MATCHES.
            --game.Switches.MatchCount;

            game.Objects[ObjectIds.Match].Flag1 |= flobts;
            // !COUNTDOWN.
            game.Clock[ClockId.MatchCountdown].Ticks = 2;

            MessageHandler.Speak(184, game);
            return ret_val;

            L20500:
            if (game.ParserVectors.prsa != VerbId.TurnOff || (game.Objects[ObjectIds.Match].Flag1 & ObjectFlags.IsOn) == 0)
            {
                goto L10;
            }

            game.Objects[ObjectIds.Match].Flag1 &= ~flobts;
            game.Clock[ClockId.MatchCountdown].Ticks = 0;
            MessageHandler.Speak(185, game);
            return ret_val;

            // HERE FOR FALSE RETURN

            L10:
            ret_val = false;
            return ret_val;
        }
    }
}