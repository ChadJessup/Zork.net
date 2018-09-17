using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zork.Core.Attributes;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Read and writes the core Game object.
    /// Note: Actual per-game state would be loaded on top of this.
    /// </summary>
    public class GameConverter : JsonConverter
    {
        private List<(ObjectIds objId, ObjectIds containerId)> containers = null;
        private Dictionary<RoomIds, (RoomActionAttribute attrib, Func<Game, bool> action)> roomActions;
        private Dictionary<VerbId, (VerbActionAttribute attrib, Func<Game, bool> action)> verbActions;
        private Dictionary<RoomIds, Type> customRooms;

        public GameConverter(List<(ObjectIds objId, ObjectIds containerId)> containers)
        {
            this.containers = containers;
            this.roomActions = LoadRoomActions();
            this.customRooms = LoadCustomRooms();
            this.verbActions = LoadVerbs();
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Game);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var game = new Game();
            List<JObject> jsonRooms = new List<JObject>();

            while (reader.Read())
            {
                switch (reader.Value)
                {
                    case var _ when reader.Path == "game.messages" && reader.TokenType == JsonToken.StartArray:
                        this.ParseMessages(reader, serializer).ForEach(tuple =>
                        {
                            game.Messages.rtext.Add(tuple.Item1);
                            game.Messages.text.Add(tuple.Item2);
                        });
                        break;
                    case var _ when reader.Path == "game.clockEvents" && reader.TokenType == JsonToken.StartArray:
                        game.Clock = serializer.Deserialize<List<ClockEvent>>(reader).ToDictionary(ce => ce.Id);
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
                        jsonRooms = this.ParseRooms(reader, serializer);
                        break;
                    case var _ when reader.Path == "game.exits" && reader.TokenType == JsonToken.StartArray:
                        game.Exits.Travel = serializer.Deserialize<List<int>>(reader);
                        break;
                    case string property when reader.TokenType == JsonToken.PropertyName:
                        this.ParseProperty(game, property, reader);
                        break;
                    default:
                        continue;
                }
            }

            // By now, everything should be parsed. We need to connect some of the dots...
            // Create rooms, add actors, villians, and objects to the rooms.
            foreach (var room in jsonRooms)
            {
                var newRoom = new Room
                {
                    Id = (RoomIds)room.Value<int>("id"),
                    Name = room.Value<string>("name"),
                    Description = room.Value<string>("description1") ?? string.Empty,
                    ShortDescription = room.Value<string>("description2") ?? string.Empty,
                    Score = room.Value<int>("score"),
                    Flags = (RoomFlags)Enum.Parse(typeof(RoomFlags), room.Value<string>("flags")),
                    Exit = room.Value<int>("exit"),
                    Action = room.Value<int>("actionId"),
                };

                if (room.ContainsKey("objs"))
                {
                    foreach (var obj in room["objs"])
                    {
                        newRoom.Objects.Add(game.Objects[(ObjectIds)obj.Value<int>()]);
                    }
                }

                if (room.ContainsKey("actors"))
                {
                    foreach (var actor in room["actors"])
                    {
                        var tempActor = game.Adventurers[(ActorIds)actor.Value<int>()];
                        tempActor.CurrentRoom = newRoom;
                        newRoom.Adventurers.Add(tempActor);
                    }
                }

                if (room.ContainsKey("vills"))
                {
                    foreach (var villian in room["vills"])
                    {
                        newRoom.Villians.Add(game.Villians[(ObjectIds)villian.Value<int>()]);
                    }
                }

                if (this.roomActions.TryGetValue(newRoom.Id, out var value))
                {
                    newRoom.DoAction = value.action;
                }

                // We have a generic room now, let's see if this is actually a special room
                // e.g., a custom Room derived from the Room class - we connect via the RoomAttribute.
                if (this.customRooms.TryGetValue(newRoom.Id, out var customRoom))
                {
                    var custom = Activator.CreateInstance(customRoom, newRoom);

                    newRoom = (Room)custom;
                }

                game.Rooms.Add(newRoom.Id, newRoom);
            }

            // Move objs that are inside containers to inside the containers.
            foreach (var (objId, containerId) in this.containers)
            {
                game.Objects[containerId].ContainedObjects.Add(game.Objects[objId]);
                game.Objects[objId].Container = containerId;
            }

            if (game.Random == null)
            {
                game.Random = new Random(game.RandomSeed);
            }

            game.Parser.Verbs = this.verbActions;

            return game;
        }

        private List<(int,string)> ParseMessages(JsonReader reader, JsonSerializer serializer)
        {
            var jobj = JObject.ReadFrom(reader);
            var messages = new List<(int, string)>();

            foreach (var message in jobj)
            {
                messages.Add(((int)message["id"], (string)message["message"]));
            }

            return messages;
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

        private List<JObject> ParseRooms(JsonReader reader, JsonSerializer serializer)
            => serializer.Deserialize<List<JObject>>(reader);

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
                case "maxLoad":
                    game.State.MaxLoad = reader.ReadAsInt32().GetValueOrDefault();
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
                    new JProperty("maxLoad", game.State.MaxLoad),
                    new JProperty("rooms", JArray.FromObject(game.Rooms.Values, serializer)),
                    new JProperty("objects", JArray.FromObject(game.Objects.Values, serializer)),
                    new JProperty("villians", JArray.FromObject(game.Villians.Values, serializer)),
                    new JProperty("actors", JArray.FromObject(game.Adventurers.Values, serializer)),
                    new JProperty("messages", JArray.FromObject(messages, serializer)),
                    new JProperty("clockEvents", JArray.FromObject(game.Clock.Values, serializer)),
                    new JProperty("exits", JArray.FromObject(game.Exits.Travel, serializer))
                )));

            serializer.Serialize(writer, head);
        }

        private Dictionary<VerbId, (VerbActionAttribute attrib, Func<Game, bool>)> LoadVerbs()
        {
            var verbs = new Dictionary<VerbId, (VerbActionAttribute, Func<Game, bool>)>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<VerbActionAttribute>(inherit: true))
                    {
                        var func = (Func<Game, bool>)Delegate.CreateDelegate(typeof(Func<Game, bool>), null, method);
                        verbs.Add(attrib.VerbId, (attrib, func));
                    }
                }
            }

            return verbs;
        }

        private static Dictionary<RoomIds, Type> LoadCustomRooms()
        {
            var dic = new Dictionary<RoomIds, Type>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes().Where(t => t != typeof(Room) && t.IsSubclassOf(typeof(Room))))
            {
                foreach (var attrib in type.GetCustomAttributes<RoomAttribute>())
                {
                    dic.Add(attrib.RoomId, type);
                }
            }

            return dic;
        }

        private static Dictionary<RoomIds, (RoomActionAttribute attrib, Func<Game, bool> action)> LoadRoomActions()
        {
            var dic = new Dictionary<RoomIds, (RoomActionAttribute, Func<Game, bool>)>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<RoomActionAttribute>(inherit: true))
                    {
                        var func = (Func<Game, bool>)Delegate.CreateDelegate(typeof(Func<Game, bool>), null, method);
                        dic.Add(attrib.RoomId, (attrib, func));
                    }
                }
            }

            return dic;
        }
    }
}
