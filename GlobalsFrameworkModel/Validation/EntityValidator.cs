using System;
using System.Linq;
using GlobalsFramework.Exceptions;
using GlobalsFramework.Utils.TypeDescription;

namespace GlobalsFramework.Validation
{
    internal static class EntityValidator
    {
        internal static void ValidateDefinitionAndThrow(Type entityType)
        {
            if (!entityType.IsVisible)
                throw new EntityValidationException(
                    string.Format("Class <{0}> must be visible from current assembly. Declare class as public ",
                        entityType.Name));

            var typeDescription = EntityTypeDescriptor.GetTypeDescription(entityType);

            if (!typeDescription.AllKeys.Any())
            {
                throw new EntityValidationException(
                    string.Format(
                        "Entity <{0}> must declare at least one primary key with <IsPrimaryKey> property of <Column> attribute",
                        entityType.Name));
            }

            if (typeDescription.IdentityKeys.Any(k => !IsIdentitySupportedType(k.ColumnInfo.PropertyType)))
            {
                throw new EntityValidationException(
                    "Only integral types (short, int or long) supported as DbGenerated column");
            }

            if (typeDescription.IdentityKeys.Count() > 1)
            {
                throw new EntityValidationException("Only one DbGenerated column is allowed");
            }

            if (typeDescription.IdentityKeys.Any() && typeDescription.CustomKeys.Any())
            {
                throw new EntityValidationException("Either DbGenerated primary key or custom primary keys are allowed");
            }

        }

        private static bool IsIdentitySupportedType(Type type)
        {
            return type.IsAssignableFrom(typeof (Int16)) ||
                   type.IsAssignableFrom(typeof (Int32)) ||
                   type.IsAssignableFrom(typeof (Int64));
        }
    }
}
