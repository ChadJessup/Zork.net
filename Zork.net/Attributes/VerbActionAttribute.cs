using System;

namespace Zork.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class VerbActionAttribute : Attribute
    {
        public VerbId VerbId { get; set; }
        public VerbActionAttribute(VerbId verbId) => this.VerbId = verbId;
    }
}
