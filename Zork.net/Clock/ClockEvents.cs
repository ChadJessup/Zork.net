using System.Collections.Generic;

namespace Zork.Core.Clock
{
    public class ClockEvents
    {
        public int Count { get; set; }
        public List<int> Ticks { get; } = new List<int>(25);
        public List<int> Actions { get; } = new List<int>(25);
        public List<bool> Flags { get; } = new List<bool>(25);

        /// <summary>
        /// clockd_ - Clock demon for intermove clock events.
        /// </summary>
        /// <returns></returns>
        public static bool clockd_(Game game)
        {
            /* System generated locals */
            int i__1;
            bool ret_val;

            /* Local variables */
            int i;

            ret_val = false;
            /* 						!ASSUME NO ACTION. */
            i__1 = game.Clock.Count;
            for (i = 1; i <= i__1; ++i)
            {
                if (!game.Clock.Flags[i - 1] || game.Clock.Ticks[i - 1] == 0)
                {
                    goto L100;
                }

                if (game.Clock.Ticks[i - 1] < 0)
                {
                    goto L50;
                }

                /* 						!PERMANENT ENTRY? */
                --game.Clock.Ticks[i - 1];
                if (game.Clock.Ticks[i - 1] != 0)
                {
                    goto L100;
                }
                /* 						!TIMER EXPIRED? */
                L50:
                ret_val = true;
                cevapp_(game.Clock.Actions[i - 1]);
                /* 						!DO ACTION. */
                L100:
                ;
            }
            return ret_val;

        }
    }
}
