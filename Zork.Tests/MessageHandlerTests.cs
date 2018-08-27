using System;
using Xunit;
using Zork.Core;

namespace Zork.Tests
{
    public class MessageHandlerTests
    {
        [Fact]
        public void SpeakWrittenToHandlerGoodValue()
        {
            var game = Game.Initialize();
            bool wasOutput = false;
            string output = string.Empty;
            game.WriteOutput = (o) =>
            {
                output += o;
                wasOutput = true;
            };

            MessageHandler.Speak(662, game);
            Assert.Equal("\r\nYou are on a wide ledge high into the volcano.  The rim of the\r\nvolcano is about 200 feet above, and there is a precipitous drop\r\nbelow to the floor.\r\n", output);
            Assert.True(wasOutput);
        }
    }
}
