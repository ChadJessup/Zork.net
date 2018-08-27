using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Zork.Core.Attributes;

namespace Zork.Core.Converters
{
    /// <summary>
    /// Reads/Writes Object to JSON.
    /// </summary>
    public class ObjectConverter : JsonConverter
    {
        private List<(ObjectIds objId, ObjectIds containerId)> containers = null;
        private Dictionary<ObjectIds, (ObjectActionAttribute attrib, Func<Game, bool> action)> objectActions = null;

        public ObjectConverter(List<(ObjectIds objId, ObjectIds containerId)> container)
        {
            this.objectActions = LoadObjectActions();
            this.containers = container;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Object);

        private static Dictionary<ObjectIds, (ObjectActionAttribute attrib, Func<Game, bool> action)> LoadObjectActions()
        {
            var dic = new Dictionary<ObjectIds, (ObjectActionAttribute, Func<Game, bool>)>();

            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<ObjectActionAttribute>(inherit: true))
                    {
                        var func = (Func<Game, bool>)Delegate.CreateDelegate(typeof(Func<Game, bool>), null, method);
                        dic.Add(attrib.ObjectId, (attrib, func));
                    }
                }
            }

            return dic;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var newObj = new Object();
            newObj.Id = (ObjectIds)obj.Value<int>("id");
            newObj.Description = obj.Value<string>("description1");
            newObj.ShortDescription = obj.Value<string>("description2");
            newObj.LongDescription = obj.Value<string>("odesco");
            newObj.WrittenText = obj.Value<string>("writtenText");
            newObj.Value = obj.Value<int>("value");
            newObj.Flag1 = (ObjectFlags)Enum.Parse(typeof(ObjectFlags), obj.Value<string>("flag1"));
            newObj.Flag2 = (ObjectFlags2)Enum.Parse(typeof(ObjectFlags2), obj.Value<string>("flag2"));
            newObj.Size = obj.Value<int>("size");
            newObj.otval = obj.Value<int>("otval");
            newObj.Capacity = obj.Value<int>("capacity");
            newObj.Action = obj.Value<int>("actionId");

            var containerId = (ObjectIds)obj.Value<int>("container");
            this.containers.Add((newObj.Id, containerId));

            if (objectActions.TryGetValue(newObj.Id, out var value))
            {
                newObj.DoAction = value.action;
            }

            return newObj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Object)value;

            serializer.Serialize(writer, new
            {
                id = (int)obj.Id,
                obj.Description,
                obj.ShortDescription,
                obj.LongDescription,
                obj.WrittenText,
                obj.Value,
                obj.Flag1,
                obj.Flag2,
                container = (int)obj.Container,
                obj.Size,
                obj.otval,
                obj.Capacity,
                actionId = obj.Action,
            });
        }
    }
}
