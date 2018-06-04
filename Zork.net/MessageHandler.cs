﻿using System;

namespace Zork.Core
{
    public static class MessageHandler
    {
        public static void Speak(int messageNumber, Game game) => MessageHandler.rspsb2nl_(messageNumber, 0, 0, true, game);

        /// <summary>
        /// OUTPUT RANDOM MESSAGE WITH SUBSTITUTABLE ARGUMENT
        /// </summary>
        /// <param name="messageNumber"></param>
        /// <param name="s1"></param>
        /// <param name="game"></param>
        public static void rspsub_(int messageNumber, int s1, Game game) => MessageHandler.rspsb2nl_(messageNumber, s1, 0, true, game);

        /// <summary>
        /// Display a substitutable message with an optional newline
        /// </summary>
        private static void rspsb2nl_(int messageNumber, int y, int z, bool newLine, Game game)
        {
            string zkey = "IanLanceTaylorJr";
            int x = messageNumber;

            if (x > 0)
            {
                x = game.Messages.rtext[x - 1];
            }

            // !IF >0, LOOK UP IN RTEXT.
            if (x == 0)
            {
                return;
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
                Console.Write('\n');
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
                    Console.Write('\n');
                    if (newLine)
                    {
                        // Console.Write('\n');
                    }
                }
                else if (i == '#' && y != 0)
                {
                    long iloc = game.DataPosition;

                    rspsb2nl_(y, 0, 0, false, game);
                    if (iloc > game.Data.Length)
                    {
                        throw new InvalidOperationException($"Error seeking database loc {iloc}");
                    }

                    y = z;
                    z = 0;
                }
                else
                {
                    Console.Write((char)i);
                }
            }

            if (newLine)
            {
                Console.Write('\n');
            }
        }
    }
}
