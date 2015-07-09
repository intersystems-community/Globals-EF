using System;

namespace GlobalsFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DbSetAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
