using System;

namespace GlobalsFramework.Attributes
{
    /// <summary>
    /// <see cref="T:GlobalsFramework.Attributes.ColumnAttribute"/> can be applied to the property of entity to indicate that this property must be stored in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of a column, which will be used for storing in the database. 
        /// If value is not provided, then will be used name of property in entity class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether this column represents a column that is part or all of the primary key of the database set.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets whether a column represents auto-generated primary key and contains value that the database auto-generates.
        /// </summary>
        public bool IsDbGenerated { get; set; }
    }
}
