using System;

namespace Zork.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RoomActionAttribute : Attribute
    {
        public RoomIds RoomId { get; set; }
        public RoomActionAttribute(RoomIds roomId) => this.RoomId = roomId;
    }
}
