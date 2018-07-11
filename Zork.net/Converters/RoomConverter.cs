using Newtonsoft.Json;
using System;
using System.Linq;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Reads/Writes Rooms to JSON.
    /// </summary>
    public class RoomConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Room);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var room = new Room();
            serializer.Populate(reader, room);
            return room;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var room = (Room)value;

            serializer.Serialize(writer, new
            {
                id = (int)room.Id,
                room.Name,
                room.Description1,
                room.Description2,
                room.Score,
                room.Flags,
                actors = room.Adventurers.Any() ? room.Adventurers.Select(a => (int)a.Id) : null,
                objs = room.Objects.Any() ? room.Objects.Select(o => (int)o.Id) : null,
                vills = room.Villians.Any() ? room.Villians.Select(v => (int)v.Id) : null
            });
        }
    }
}
