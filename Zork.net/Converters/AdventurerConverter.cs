using Newtonsoft.Json;
using System;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Reads/Writes Object to JSON.
    /// </summary>
    public class AdventurerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Adventurer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var actor = new Adventurer();
            serializer.Populate(reader, actor);
            return actor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var actor = (Adventurer)value;

            if (actor.Id == ActorIds.NoOne)
            {
                return;
            }

            serializer.Serialize(writer, new
            {
                id = (int)actor.Id,
                objectId = (int)actor.ObjectId,
                name = actor.Id,
                startRoom = (int)actor.CurrentRoom.Id,
                flags = actor.Flag,
            });
        }
    }
}
