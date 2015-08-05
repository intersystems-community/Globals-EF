using System;

namespace GlobalsFramework.Attributes
{
    /// <summary>
    /// <see cref="T:GlobalsFramework.Attributes.DbSetAttribute"/> used for applying additional entity-to-global storing rules to the entity.
    /// Applying <see cref="T:GlobalsFramework.Attributes.DbSetAttribute"/> to entity class is not compulsory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DbSetAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of an entity, which will be used for storing in the database. 
        /// If value is not provided, then will be used name of entity class.
        /// </summary>
        public string Name { get; set; }
    }
}
