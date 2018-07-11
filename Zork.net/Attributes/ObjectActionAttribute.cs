using System;

namespace Zork.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ObjectActionAttribute : Attribute
    {
        public ObjectIds ObjectId { get; set; }
        public ObjectActionAttribute(ObjectIds objectId) => this.ObjectId = objectId;
    }
}
