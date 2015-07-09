using System;

namespace GlobalsFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsDbGenerated { get; set; }
    }
}
