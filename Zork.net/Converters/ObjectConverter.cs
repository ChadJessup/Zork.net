using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Reads/Writes Object to JSON.
    /// </summary>
    public class ObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Object);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = new Object();
            serializer.Populate(reader, obj);

            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Object)value;

            serializer.Serialize(writer, new
            {
                id = (int)obj.Id,
                obj.Description1,
                obj.Description2,
                obj.odesco,
                obj.WrittenText,
                obj.Value,
                obj.Flag1,
                obj.Flag2,
                container = (int)obj.Container,
                obj.Size,
                obj.otval,
                obj.Capacity,
            });
        }
    }
}
