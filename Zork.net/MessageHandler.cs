using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core
{
    public static class MessageHandler
    {
        public static string rspeak_(Game game, ObjectIds objectId) => MessageHandler.Speak(game, (int)objectId);
        public static string rspeak_(Game game, int messageNumber) => MessageHandler.Speak(game, messageNumber);
        public static string Speak(ObjectIds objectId, Game game) => MessageHandler.Speak(game, (int)objectId);
        public static string Speak(Game game, int messageNumber) => MessageHandler.Speak(messageNumber, game);
        public static string Speak(int messageNumber, Game game) => MessageHandler.rspsb2nl_(messageNumber, 0, 0, true, game);

        public static void more_output(Game game, string output) => game.WriteOutput(output);
        public static void more_input() { }

        private static Dictionary<int, string> messageLookup = new Dictionary<int, string>();

        /// <summary>
        /// OUTPUT RANDOM MESSAGE WITH SUBSTITUTABLE ARGUMENT
        /// </summary>
        /// <param name="messageNumber"></param>
        /// <param name="s1"></param>
        /// <param name="game"></param>
        public static string rspsub_(Game game, int messageNumber, int s1 = 0) => rspsub_(messageNumber, s1, game);
        public static string rspsub_(int messageNumber, int s1, Game game) => MessageHandler.rspsb2nl_(messageNumber, s1, 0, true, game);
        public static string rspsub_(ObjectIds messageNumber, int s1, Game game) => MessageHandler.rspsb2nl_((int)messageNumber, s1, 0, true, game);
        /// <summary>
        /// rspsb2_ - Output random message with up to two substitutable arguments.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="game"></param>
        public static string rspsb2_(Game game, int n, int s1, int s2) => rspsb2_(n, s1, s2, game);
        public static string rspsb2_(int n, int s1, int s2, Game game) => rspsb2nl_(n, s1, s2, true, game);

        /// <summary>
        /// Display a substitutable message with an optional newline
        /// </summary>
        private static string rspsb2nl_(int messageNumber, int y, int z, bool newLine, Game game)
        {
            return messageNumber == 0 ? "" : game.Messages.text[messageNumber - 1];

            if (MessageHandler.messageLookup.TryGetValue(messageNumber, out string message))
            {
                return message;
            }
            else
            {
                for (int i = 0; i < game.Messages.rtext.Count; i++)
                {
                    if (game.Messages.rtext[i] == messageNumber)
                    {
                        var output = game.Messages.text[i];
                        MessageHandler.messageLookup.Add(messageNumber, output);
                        return output;
                    }
                }
            }

            string zkey = "IanLanceTaylorJr";
            int x = messageNumber;
            StringBuilder finalOutput = new StringBuilder();

            if (x > 0)
            {
                x = game.Messages.rtext[x - 1];
            }

            // !IF >0, LOOK UP IN RTEXT.
            if (x == 0)
            {
                return string.Empty;
            }

            // !ANYTHING TO DO?
            game.Player.TelFlag = true;

            // !SAID SOMETHING.
            x = ((-x) - 1) * 8;
            game.DataPosition = x + game.Messages.mrloc;

            if (game.DataPosition > game.Data.Length)
            {
                throw new InvalidOperationException($"Error seeking database loc {x}");
            }

            if (newLine)
            {
                // game.WriteOutput?.Invoke(Environment.NewLine);
                // finalOutput.Append(Environment.NewLine);
            }

            while (true)
            {
                int i;

                try
                {
                    i = game.Data[game.DataPosition++];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException($"Error reading database loc {x}");
                }

                i ^= (int)(zkey[(int)(x & 0xf)] ^ (x & 0xff));

                x = x + 1;
                if (i == '\0')
                {
                    break;
                }
                else if (i == '\n')
                {
                    game.WriteOutput?.Invoke(Environment.NewLine);
                    finalOutput.Append(Environment.NewLine);

                    if (newLine)
                    {
                        // game.WriteOutput?.Invoke(Environment.NewLine);
                        // finalOutput.Append(Environment.NewLine);
                    }
                }
                else if (i == '#' && y != 0)
                {
                    long iloc = game.DataPosition;

                    finalOutput.Append(rspsb2nl_(y, 0, 0, false, game));
                    if (iloc > game.Data.Length)
                    {
                        throw new InvalidOperationException($"Error seeking database loc {iloc}");
                    }

                    game.DataPosition = (int)iloc;

                    y = z;
                    z = 0;
                }
                else
                {
                    game.WriteOutput?.Invoke(((char)i).ToString());
                    finalOutput.Append((char)i);
                }
            }

            if (newLine)
            {
                game.WriteOutput?.Invoke(Environment.NewLine);
            }

            return finalOutput.ToString();
        }

        /// <summary>
        /// yesno_ - Obtain Yes/No answer.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="questionStringId"></param>
        /// <param name="yesStringId"></param>
        /// <param name="noStringId"></param>
        /// <returns></returns>
        public static bool AskYesNoQuestion(Game game, int questionStringId, int yesStringId, int noStringId)
        {
            // System generated locals
            bool ret_val;

            // Local variables
            string ans = " ";

            L100:
            // !ASK
            MessageHandler.rspeak_(game, questionStringId);

            ans = game.ReadInput();
            MessageHandler.more_input();

            if (string.IsNullOrWhiteSpace(ans))
            {
                ans = " ";
            }

            // !GET ANSWER
            if (ans[0] == 'Y' || ans[0] == 'y')
            {
                goto L200;
            }

            if (ans[0] == 'N' || ans[0] == 'n')
            {
                goto L300;
            }

            MessageHandler.rspeak_(game, 6);
            // !SCOLD.
            goto L100;

            L200:
            // !YES,
            ret_val = true;
            // !OUT WITH IT.
            MessageHandler.rspeak_(game, yesStringId);
            return ret_val;

            L300:
            // !NO,
            ret_val = false;
            // !LIKEWISE.
            MessageHandler.rspeak_(game, noStringId);
            return ret_val;
        }
    }
}
