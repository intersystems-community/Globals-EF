using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using GlobalsFramework.Attributes;

namespace GlobalsFramework.Utils.TypeDescription
{
    internal static class EntityTypeDescriptor
    {
        private static readonly ConcurrentDictionary<Type, EntityTypeDescription> Descriptions =
            new ConcurrentDictionary<Type, EntityTypeDescription>();

        internal static bool IsSimpleType(Type type)
        {
            if (type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(object) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset))
                return true;
            return false;
        }
        internal static bool IsSupportedEnumerable(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IList)))
                return true;
            return false;
        }
        internal static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }
        internal static bool IsSimpleNullableType(Type type)
        {
            return IsNullableType(type) && IsSimpleType(Nullable.GetUnderlyingType(type));
        }
        internal static bool IsArrayType(Type type)
        {
            return type.IsArray;
        }

        internal static EntityTypeDescription GetTypeDescription(Type entityType)
        {
            EntityTypeDescription result;
            Descriptions.TryGetValue(entityType, out result);

            return result ?? CreateTypeDescription(entityType);
        }

        private static EntityTypeDescription CreateTypeDescription(Type entityType)
        {
            var result = new EntityTypeDescription
            {
                DbSetAttribute =
                    (DbSetAttribute) entityType.GetCustomAttributes(typeof (DbSetAttribute), false).SingleOrDefault()
            };

            var properties = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(ColumnAttribute), false));

            foreach (var property in properties)
            {
                var attribute = ((ColumnAttribute)(property.GetCustomAttributes(typeof(ColumnAttribute), false).Single()));

                if (IsSimpleType(property.PropertyType))
                {
                    result.Columns.Add(new EntityTypeDescription
                    {
                        IsSimpleColumn = true,
                        ColumnAttribute = attribute,
                        ColumnInfo = property
                    });
                }
                else if (IsSimpleNullableType(property.PropertyType))
                {
                    result.Columns.Add(new EntityTypeDescription
                    {
                        IsNullableColumn = true,
                        IsSimpleColumn = true,
                        ColumnAttribute = attribute,
                        ColumnInfo = property
                    });
                }
                else if (IsArrayType(property.PropertyType))
                {
                    result.Columns.Add(new EntityTypeDescription
                    {
                        IsArrayColumn = true,
                        ColumnAttribute = attribute,
                        ColumnInfo = property
                    });
                }
                else if (IsSupportedEnumerable(property.PropertyType))
                {
                    result.Columns.Add(new EntityTypeDescription
                    {
                        IsEnumerableColumn = true,
                        ColumnAttribute = attribute,
                        ColumnInfo = property
                    });
                }
                else
                {
                    var isNullableType = IsNullableType(property.PropertyType);

                    var complexColumn = isNullableType
                        ? CreateTypeDescription(Nullable.GetUnderlyingType(property.PropertyType))
                        : CreateTypeDescription(property.PropertyType);

                    complexColumn.IsComplexColumn = true;
                    complexColumn.IsNullableColumn = isNullableType;
                    complexColumn.ColumnAttribute = attribute;
                    complexColumn.ColumnInfo = property;

                    result.Columns.Add(complexColumn);
                }
            }

            DescribeKeys(result);

            RegisterDescription(entityType, result);

            return result;
        }

        private static void RegisterDescription(Type entityType, EntityTypeDescription description)
        {
            if (!Descriptions.ContainsKey(entityType))
            {
                Descriptions.TryAdd(entityType, description);
            }
        }
        private static void DescribeKeys(EntityTypeDescription typeDescription)
        {
            foreach (var column in typeDescription.Columns.Where(column => column.ColumnAttribute.IsPrimaryKey))
            {
                if (column.ColumnAttribute.IsDbGenerated)
                {
                    typeDescription.IdentityKeys.Add(column);
                }
                else
                {
                    typeDescription.CustomKeys.Add(column);
                }
                typeDescription.AllKeys.Add(column);
            }
        }

    }
}
