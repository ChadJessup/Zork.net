using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core.Converters
{
    public class VillianConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Villian);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var villian = new Villian();
            serializer.Populate(reader, villian);
            return villian;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var villian = (Villian)value;

            serializer.Serialize(writer, new
            {
                id = (int)villian.Id,
                name = villian.Id,
                bestWeapon = (int)villian.BestWeapon,
                strength = villian.Melee,
            });
        }
    }
}
