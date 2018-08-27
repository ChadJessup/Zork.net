using System;

namespace Zork.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RoomAttribute : Attribute
    {
        public RoomIds RoomId { get; set; }
        public RoomAttribute(RoomIds roomId) => this.RoomId = roomId;
    }
}
