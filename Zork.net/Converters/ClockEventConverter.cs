using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zork.Core.Converters
{
    public class ClockEventConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ClockEvent);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var clockEvent = new ClockEvent();
            serializer.Populate(reader, clockEvent);
            return clockEvent;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var clockEvent = (ClockEvent)value;

            serializer.Serialize(writer, new
            {
                id = (int)clockEvent.Id,
                name = clockEvent.Id,
                tick = clockEvent.Ticks,
                action = clockEvent.Actions,
                flag = clockEvent.Flags,
            });
        }
    }
}
