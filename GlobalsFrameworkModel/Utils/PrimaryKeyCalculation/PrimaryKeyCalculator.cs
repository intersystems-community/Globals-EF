using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using GlobalsFramework.Utils.TypeDescription;

namespace GlobalsFramework.Utils.PrimaryKeyCalculation
{
    internal static class PrimaryKeyCalculator
    {
        public static string GetPrimaryKey<TEntity>(TEntity entity)
        {
            var typeDescription = EntityTypeDescriptor.GetTypeDescription(entity.GetType());
            var identityKeys = typeDescription.IdentityKeys;

            if (identityKeys.Any())
            {
                //according to the validation rules
                return identityKeys.Single().ColumnInfo.GetValue(entity, null).ToString();
            }

            var primaryKeys = new NameValueCollection();
            foreach (var key in typeDescription.CustomKeys)
            {
                primaryKeys.Add(key.ColumnAttribute.Name ?? key.ColumnInfo.Name, SerializeColumnToString(key.ColumnInfo.GetValue(entity, null)));
            }

            return CryptoProvider.CalculatePrimaryKeyHash(primaryKeys);
        }

        private static string SerializeColumnToString<T>(T column)
        {
            if (column == null)
                return "null";

            var columnType = column.GetType();
            var result = new StringBuilder();

            if (EntityTypeDescriptor.IsSimpleType(columnType) || EntityTypeDescriptor.IsNullableType(columnType))
            {
                result.Append(column);
            }
            else if (EntityTypeDescriptor.IsSupportedEnumerable(columnType))
            {
                var enumerable = column as IEnumerable;

                foreach (var item in enumerable)
                {
                    result.AppendFormat("{0},", SerializeColumnToString(item));
                }
            }
            else
            {
                var typeDescription = EntityTypeDescriptor.GetTypeDescription(columnType);
                foreach (var item in typeDescription.Columns)
                {
                    result.AppendFormat("{0}={1}\n", item.ColumnAttribute.Name ?? item.ColumnInfo.Name,
                        SerializeColumnToString(item.ColumnInfo.GetValue(column, null)));
                }
            }

            return result.ToString();
        }
    }
}
