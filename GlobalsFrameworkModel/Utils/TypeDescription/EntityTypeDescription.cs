using System.Collections.Generic;
using System.Reflection;
using GlobalsFramework.Attributes;

namespace GlobalsFramework.Utils.TypeDescription
{
    internal class EntityTypeDescription
    {
        internal ColumnAttribute ColumnAttribute { get; set; }
        internal DbSetAttribute DbSetAttribute { get; set; }
        internal PropertyInfo ColumnInfo { get; set; }

        internal bool IsSimpleColumn { get; set; }
        internal bool IsEnumerableColumn { get; set; }
        internal bool IsComplexColumn { get; set; }
        internal bool IsNullableColumn { get; set; }
        internal bool IsArrayColumn { get; set; }

        internal List<EntityTypeDescription> Columns { get; set; }

        internal List<EntityTypeDescription> IdentityKeys { get; set; }
        internal List<EntityTypeDescription> CustomKeys { get; set; }
        internal List<EntityTypeDescription> AllKeys { get; set; }

        internal EntityTypeDescription()
        {
            Columns = new List<EntityTypeDescription>();
            
            IdentityKeys = new List<EntityTypeDescription>();
            CustomKeys = new List<EntityTypeDescription>();
            AllKeys = new List<EntityTypeDescription>();
        }
    }
}
