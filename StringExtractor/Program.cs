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
        private int currentRoomDesc = 0;
        private string currentString = string.Empty;

        public static void Main(string[] args)
        {
            var zork = new Program();
            zork.Go();
        }

        public void Go()
        {
            var game = Game.Initialize(useJson: false);
            Console.WriteLine("Game object initialized...");

            game.ReadInput = this.ReadInput;
            game.WriteOutput = this.WriteOutput;

            foreach (var strId in game.Messages.rtext)
            {
                game.Messages.text.Add(MessageHandler.Speak(strId, game).Replace("\r\n", " "));
            }

            Console.WriteLine($"{game.Messages.text.Count} messages parsed...");

            for (currentRoomDesc = 0; currentRoomDesc < game.Rooms.Count; currentRoomDesc++)
            {
                var room = game.Rooms[(RoomIds)currentRoomDesc];
                room.Description = MessageHandler.Speak(room.Description1Id, game).Replace("\r\n", " ");
                room.Name = MessageHandler.Speak(room.Description2Id, game);
            }

            Console.WriteLine($"{game.Rooms.Count} rooms parsed...");

            for (int objId = 0; objId < game.Objects.Count; objId++)
            {
                var obj = game.Objects[(ObjectIds)objId];

                obj.Description = MessageHandler.Speak(obj.Description1Id, game).Replace("\r\n", " ");
                obj.ShortDescription = MessageHandler.Speak(obj.Description2Id, game).Replace("\r\n", " ");
                obj.LongDescription = MessageHandler.Speak(obj.LongDescriptionId, game).Replace("\r\n", " ");
                obj.WrittenText = MessageHandler.Speak(obj.oreadId, game);
            }

            Console.WriteLine($"{game.Objects.Count} objects parsed...");

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            var objContainers = new List<(ObjectIds objId, ObjectIds containerId)>();

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new RoomConverter());
            settings.Converters.Add(new GameConverter(objContainers));
            settings.Converters.Add(new AdventurerConverter());
            settings.Converters.Add(new ObjectConverter(objContainers));
            settings.Converters.Add(new VillianConverter());
            settings.Converters.Add(new ClockEventConverter());

            var jsonPath = @"C:\Users\shabu\Desktop\zork.json";
            var json = JsonConvert.SerializeObject(game, settings);
            File.WriteAllText(jsonPath, json);
            Console.WriteLine($"Json file written to: {jsonPath}");

            var newGame = JsonConvert.DeserializeObject<Game>(File.ReadAllText(jsonPath), settings);
            Console.WriteLine($"Successfully loaded: {jsonPath}");
        }

        public void WriteOutput(string output) => this.currentString += output;
        public string ReadInput() => Console.ReadLine();
    }
}