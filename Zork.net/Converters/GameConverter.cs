using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Read and writes the core Game object.
    /// Note: Actual per-game state would be loaded on top of this.
    /// </summary>
    public class GameConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Game);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var game = new Game();

            while (reader.Read())
            {
                switch (reader.Value)
                {
                    case var _ when reader.Path == "game.rtext" && reader.TokenType == JsonToken.StartArray:
                        game.Messages.rtext = serializer.Deserialize<List<int>>(reader);
                        break;
                    case var _ when reader.Path == "game.actors" && reader.TokenType == JsonToken.StartArray:
                        game.Adventurers = this.ParseAdventurers(reader, serializer);
                        break;
                    case var _ when reader.Path == "game.villians" && reader.TokenType == JsonToken.StartArray:
                        game.Villians = this.ParseVillians(reader, serializer);
                        break;
                    case var _ when reader.Path == "game.objects" && reader.TokenType == JsonToken.StartArray:
                        game.Objects = this.ParseObjects(reader, serializer);
                        break;
                    case var _ when reader.Path == "game.rooms" && reader.TokenType == JsonToken.StartArray:
                        game.Rooms = this.ParseRooms(reader, serializer);
                        break;
                    case string property when reader.TokenType == JsonToken.PropertyName:
                        this.ParseProperty(game, property, reader);
                        break;
                    default:
                        continue;
                }
            }

            return game;
        }

        private Dictionary<ActorIds, Adventurer> ParseAdventurers(JsonReader reader, JsonSerializer serializer)
        {
            var actor = serializer.Deserialize<List<Adventurer>>(reader);
            actor.Sort();

            return actor.ToDictionary(r => r.Id);
        }

        private Dictionary<ObjectIds, Villian> ParseVillians(JsonReader reader, JsonSerializer serializer)
        {
            var villians = serializer.Deserialize<List<Villian>>(reader);
            villians.Sort();

            return villians.ToDictionary(r => r.Id);
        }

        private Dictionary<ObjectIds, Object> ParseObjects(JsonReader reader, JsonSerializer serializer)
        {
            var objects = serializer.Deserialize<List<Object>>(reader);
            objects.Sort();

            return objects.ToDictionary(o => o.Id);
        }

        private Dictionary<RoomIds, Room> ParseRooms(JsonReader reader, JsonSerializer serializer)
        {
            var rooms = serializer.Deserialize<List<Room>>(reader);
            rooms.Sort();

            return rooms.ToDictionary(r => r.Id);
        }

        private void ParseProperty(Game game, string property, JsonReader reader)
        {
            switch (property)
            {
                case "egmxsc":
                    game.State.egmxsc = reader.ReadAsInt32().Value;
                    break;
                case "starBit":
                    game.Star.strbit = reader.ReadAsInt32().Value;
                    break;
                case "version":
                    game.Version = SemVersion.Parse(reader.ReadAsString());
                    break;
                case "maxScore":
                    game.State.MaxScore = reader.ReadAsInt32().GetValueOrDefault();
                    break;
                default:
                    break;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var game = (Game)value;
            var messages = new List<dynamic>();

            for (int i = 0; i < game.Messages.Count; i++)
            {
                messages.Add(new { id = game.Messages.rtext[i], message = game.Messages.text[i] });
            }

            var head = new JObject(
                new JProperty("game", new JObject(
                    new JProperty("version", game.Version.ToString()),
                    new JProperty("maxScore", game.State.MaxScore),
                    new JProperty("starBit", game.Star.strbit),
                    new JProperty("egmxsc", game.State.egmxsc),
                    new JProperty("rooms", JArray.FromObject(game.Rooms.Values, serializer)),
                    new JProperty("objects", JArray.FromObject(game.Objects.Values, serializer)),
                    new JProperty("villians", JArray.FromObject(game.Villians.Values, serializer)),
                    new JProperty("actors", JArray.FromObject(game.Adventurers.Values, serializer)),
                    new JProperty("messages", JArray.FromObject(messages, serializer))
                )));

            serializer.Serialize(writer, head);
        }
    }
}
