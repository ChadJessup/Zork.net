using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zork.Core;
using Zork.Core.Converters;

namespace StringExtractor
{
    public class Program
    {
        private Dictionary<int, string> speaks = new Dictionary<int, string>();
        private Dictionary<int, string> roomDescriptions = new Dictionary<int, string>();
        private int currentSpeaks = 0;
        private int currentRoomDesc = 0;
        private string currentString = string.Empty;

        public static void Main(string[] args)
        {
            var zork = new Program();
            zork.Go();
        }

        public void Go()
        {
            var game = Game.Initialize();
            game.ReadInput = this.ReadInput;
            game.WriteOutput = this.WriteOutput;

            foreach (var strId in game.Messages.rtext)
            {
                game.Messages.text.Add(MessageHandler.Speak(game, strId).Replace("\r\n", " "));
            }

            for (currentRoomDesc = 0; currentRoomDesc < game.Rooms.Count; currentRoomDesc++)
            {
                var room = game.Rooms[(RoomIds)currentRoomDesc];
                room.Description1 = MessageHandler.Speak(game, room.Description1Id).Replace("\r\n", " ");
                room.Name = MessageHandler.Speak(game, room.Description2Id);
            }

            for (int objId = 0; objId < game.Objects.Count; objId++)
            {
                var obj = game.Objects[(ObjectIds)objId];

                obj.Description1 = MessageHandler.Speak(game, obj.Description1Id).Replace("\r\n", " ");
                obj.Description2 = MessageHandler.Speak(game, obj.Description2Id).Replace("\r\n", " ");
                obj.odesco = MessageHandler.Speak(game, obj.odescoId).Replace("\r\n", " ");
                obj.WrittenText = MessageHandler.Speak(game, obj.oreadId);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new RoomConverter());
            settings.Converters.Add(new GameConverter());
            settings.Converters.Add(new AdventurerConverter());
            settings.Converters.Add(new ObjectConverter());
            settings.Converters.Add(new VillianConverter());

            var jsonPath = @"C:\Users\shabu\Desktop\zork.json";
            var json = JsonConvert.SerializeObject(game, settings);
            File.WriteAllText(jsonPath, json);

            var newGame = JsonConvert.DeserializeObject<Game>(File.ReadAllText(jsonPath), settings);
        }

        public void WriteOutput(string output) => this.currentString += output;
        public string ReadInput() => Console.ReadLine();
    }
}