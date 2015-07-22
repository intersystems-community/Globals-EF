using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GlobalsFramework.Exceptions;
using GlobalsFramework.Utils.InstanceCreation;
using GlobalsFramework.Utils.PrimaryKeyCalculation;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Access
{
    internal static class DatabaseManager
    {
        private const string IdentityValueSubscriptName = "%identityValue";
        private const string EnumerableLengthsSubscriptName = "%enumerableLengths";

        internal static void InsertEntity<TEntity>(TEntity entity, NodeReference node, Connection connection)
        {
            try
            {
                connection.StartTransaction();

                node.AcquireLock(NodeReference.EXCLUSIVE_LOCK,
                    NodeReference.LOCK_INCREMENTALLY,
                    NodeReference.RELEASE_AT_TRANSACTION_END);

                AppendPrimaryKey(entity, node);

                WriteItem(entity, node);

                connection.Commit();
            }
            catch
            {
                connection.Rollback();
                throw;
            }
        }

        internal static IEnumerable<NodeReference> GetEntitiesNodes(NodeReference node, Connection connection)
        {
            var result = new List<NodeReference>();

            node.AppendSubscript("");
 
            var subscript = node.NextSubscript();

            while (!subscript.Equals(""))
            {
                if (!string.Equals(subscript, IdentityValueSubscriptName))
                {
                    var entityNode = connection.CreateNodeReference(node.GetName());
                    entityNode.AppendSubscript(subscript);
                    result.Add(entityNode);
                }
                node.SetSubscript(node.GetSubscriptCount(), subscript);
                subscript = node.NextSubscript();
            }

            return result;
        }

        internal static void UpdateEntity<TEntity>(TEntity entity, NodeReference node, Connection connection)
        {
            try
            {
                connection.StartTransaction();

                node.AcquireLock(NodeReference.EXCLUSIVE_LOCK,
                    NodeReference.LOCK_INCREMENTALLY,
                    NodeReference.RELEASE_AT_TRANSACTION_END);

                var key = PrimaryKeyCalculator.GetPrimaryKey(entity);

                if (!IsKeyExists(key, node))
                    throw new GlobalsDbException("Unable to update entity. Entity with this primary key does not exist");

                node.SetSubscriptCount(0);

                node.AppendSubscript(key);

                WriteItem(entity, node);

                connection.Commit();
            }
            catch
            {
                connection.Rollback();
                throw;
            }
        }

        internal static void DeleteEntity<TEntity>(TEntity entity, NodeReference node, Connection connection)
        {
            try
            {
                connection.StartTransaction();

                node.AcquireLock(NodeReference.EXCLUSIVE_LOCK,
                    NodeReference.LOCK_INCREMENTALLY,
                    NodeReference.RELEASE_AT_TRANSACTION_END);

                var key = PrimaryKeyCalculator.GetPrimaryKey(entity);

                if (!IsKeyExists(key, node))
                    throw new GlobalsDbException("Unable to delete entity. Entity with this primary key does not exist");

                node.SetSubscriptCount(0);

                node.AppendSubscript(key);

                node.Kill();

                connection.Commit();
            }
            catch
            {
                connection.Rollback();
                throw;
            }
        }

        internal static object ReadNodes(IEnumerable<NodeReference> nodes, Type elementType)
        {
            var resultInstance = (IList)Activator.CreateInstance(typeof (List<>).MakeGenericType(elementType));

            foreach (var nodeReference in nodes)
            {
                resultInstance.Add(ReadNode(nodeReference, elementType));
            }

            return resultInstance;
        }

        internal static object ReadNode(NodeReference node, Type elementType)
        {
            object instance;

            if (EntityTypeDescriptor.IsSimpleType(elementType) || EntityTypeDescriptor.IsSimpleNullableType(elementType))
            {
                var nodeValue = node.GetObject();
                instance = ConvertValue(nodeValue, elementType);
            }
            else if (EntityTypeDescriptor.IsArrayType(elementType))
            {
                instance = ReadArray(node, elementType);
            }
            else if (EntityTypeDescriptor.IsSupportedEnumerable(elementType))
            {
                instance = ReadEnumerable(node, elementType);
            }
            else
            {
                instance = ReadComplexNode(node, elementType);
            }

            return instance;
        }

        internal static int GetEnumerableCount(NodeReference node)
        {
            var arrayLengthsString = node.GetString(EnumerableLengthsSubscriptName);
            var indexes = ToArrayIndexes(arrayLengthsString);

            return indexes.Aggregate(1, (current, index) => current*index);
        }

        internal static void NavigateToIndex(NodeReference node, int index)
        {
            node.AppendSubscript(ToSubscriptString(index));
        }

        private static object ReadComplexNode(NodeReference node, Type objectType)
        {
            var isNullable = EntityTypeDescriptor.IsNullableType(objectType);

            if(isNullable && IsNullableComplexColumnEmpty(node))
                return null;

            var underlyingType = !isNullable
                ? objectType
                : Nullable.GetUnderlyingType(objectType);

            var typeDescription = EntityTypeDescriptor.GetTypeDescription(underlyingType);

            var instance = InstanceCreator.CreateInstance(underlyingType);

            foreach (var column in typeDescription.Columns)
            {
                var subscript = column.ColumnAttribute.Name ?? column.ColumnInfo.Name;

                if (column.IsSimpleColumn)
                {
                    node.AppendSubscript(subscript);
                    var nodeValue = node.GetObject();
                    column.ColumnInfo.SetValue(instance, ConvertValue(nodeValue, column.ColumnInfo.PropertyType), null);
                }
                else if (column.IsArrayColumn)
                {
                    node.AppendSubscript(subscript);
                    column.ColumnInfo.SetValue(instance, ReadArray(node, column.ColumnInfo.PropertyType), null);
                }
                else if (column.IsEnumerableColumn)
                {
                    node.AppendSubscript(subscript);
                    column.ColumnInfo.SetValue(instance, ReadEnumerable(node, column.ColumnInfo.PropertyType), null);
                }
                else if (column.IsComplexColumn)
                {
                    node.AppendSubscript(subscript);
                    column.ColumnInfo.SetValue(instance,
                        ReadComplexNode(node, column.ColumnInfo.PropertyType), null);
                }

                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }

            return instance;
        }

        private static bool IsNullableComplexColumnEmpty(NodeReference node)
        {
            node.AppendSubscript("");
            var isEmpty = string.IsNullOrEmpty(node.NextSubscript());
            node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            return isEmpty;
        }

        private static void WriteItem<T>(T item, NodeReference node)
        {
            var entityType = item.GetType();

            if (EntityTypeDescriptor.IsSimpleType(entityType))
            {
                if (item != null)
                    node.Set(item.ToString());
            }
            else if (EntityTypeDescriptor.IsSimpleNullableType(entityType))
            {
                node.Set(item != null ? item.ToString() : null);
            }
            else if (EntityTypeDescriptor.IsArrayType(entityType))
            {
                WriteArray(node, item as Array);
            }
            else if (EntityTypeDescriptor.IsSupportedEnumerable(entityType))
            {
                var items = item as IEnumerable;

                if (items != null)
                    WriteEnumerable(node, items);
            }
            else
            {
                WriteComplexEntity(item, node);
            }
        }

        private static void WriteComplexEntity<TEntity>(TEntity entity, NodeReference node)
        {
            if (entity == null)
                return;

            var entityType = entity.GetType();
            var underlyingType = EntityTypeDescriptor.IsNullableType(entityType)
                ? Nullable.GetUnderlyingType(entityType)
                : entityType;

            var columns = EntityTypeDescriptor.GetTypeDescription(underlyingType).Columns;

            foreach (var column in columns)
            {
                node.AppendSubscript(column.ColumnAttribute.Name ?? column.ColumnInfo.Name);

                if (column.IsSimpleColumn)
                {
                    var value = column.ColumnInfo.GetValue(entity, null);

                    if (value != null)
                    {
                        if (column.IsNullableColumn)
                            node.Set(value != null ? value.ToString() : null);
                        else
                            node.Set(value.ToString());
                    }
                }
                else if (column.IsArrayColumn)
                {
                    var array = column.ColumnInfo.GetValue(entity, null) as Array;
                    WriteArray(node, array);
                }
                else if (column.IsEnumerableColumn)
                {
                    var items = column.ColumnInfo.GetValue(entity, null) as IEnumerable;

                    if (items != null)
                        WriteEnumerable(node, items);
                }
                else
                {
                    WriteComplexEntity(column.ColumnInfo.GetValue(entity, null), node);
                }

                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }
        }

        private static void AppendPrimaryKey<TEntity>(TEntity entity, NodeReference node)
        {
            var typeDescription = EntityTypeDescriptor.GetTypeDescription(entity.GetType());
            var identityKeys = typeDescription.IdentityKeys;

            if (identityKeys.Any())
            {
                //according to the validation rules
                var identityKey = identityKeys.Single();

                var newSeed = IncrementIdentity(node);

                var propertyType = identityKey.ColumnInfo.PropertyType;
                propertyType = !EntityTypeDescriptor.IsNullableType(propertyType)
                    ? propertyType
                    : Nullable.GetUnderlyingType(propertyType);

                var correctValue = Convert.ChangeType(newSeed, propertyType);
                identityKey.ColumnInfo.SetValue(entity, correctValue, null);
                node.AppendSubscript(newSeed);
            }
            else
            {
                var hash = PrimaryKeyCalculator.GetPrimaryKey(entity);

                if (IsKeyExists(hash, node))
                    throw new GlobalsDbException("Violation of Primary Key constraint. Cannot insert duplicate key");

                node.SetSubscriptCount(0);

                node.AppendSubscript(hash);
            }
        }

        private static object ConvertValue(object value, Type columnType)
        {
            if (value == null)
                return null;

            if (columnType.IsEnum)
                return Enum.Parse(columnType, value.ToString());

            if (columnType == typeof(DateTime))
                return DateTime.Parse(value.ToString());

            if (columnType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(value.ToString());

            if (EntityTypeDescriptor.IsNullableType(columnType))
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(columnType));

            return Convert.ChangeType(value, columnType);
        }

        private static bool IsKeyExists(string key, NodeReference node)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            node.AppendSubscript(GetPreviousSubscriptKey(key));

            var subscript = node.NextSubscript();

            while (!subscript.Equals(""))
            {
                if (string.Equals(subscript, key))
                    return true;
                node.SetSubscript(node.GetSubscriptCount(), subscript);
                subscript = node.NextSubscript();
            }

            return false;
        }

        //returns previous subscript name mask which is used for search due to database collation order 
        private static string GetPreviousSubscriptKey(string searchedIndex)
        {
            int number;
            if (int.TryParse(searchedIndex, out number))
                return (number - 1).ToString();

            if (searchedIndex.Length == 1)
            {
                return long.MaxValue.ToString();
            }

            return searchedIndex.Substring(0, searchedIndex.Length - 1);
        }

        private static long IncrementIdentity(NodeReference node)
        {
            var identityValue = node.GetObject(IdentityValueSubscriptName);

            var identitySeed = identityValue == null ? 0 : Convert.ToInt64(identityValue);

            node.Set(++identitySeed, IdentityValueSubscriptName);

            return identitySeed;
        }

        private static void WriteEnumerable(NodeReference node, IEnumerable items)
        {
            var index = 0;
            node.Kill();

            foreach (var item in items)
            {
                node.AppendSubscript(ToSubscriptString(index));

                WriteItem(item, node);

                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
                index++;
            }

            node.AppendSubscript(EnumerableLengthsSubscriptName);
            node.Set(ToSubscriptString(index));
            node.SetSubscriptCount(node.GetSubscriptCount() - 1);
        }

        private static object ReadEnumerable(NodeReference node, Type columnType)
        {
            var instance = (IList)Activator.CreateInstance(columnType);
            var elementType = columnType.IsGenericType ? columnType.GetGenericArguments().Single() : typeof(object);

            if(string.IsNullOrEmpty(node.NextSubscript("")))
                return null;

            var subscript = node.NextSubscript(EnumerableLengthsSubscriptName);

            while (!subscript.Equals(""))
            {
                node.AppendSubscript(subscript);

                var item = ReadNode(node, elementType);
                instance.Add(item);

                subscript = node.NextSubscript();
                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }

            return instance;
        }

        private static void WriteArray(NodeReference node, Array array)
        {
            node.Kill();

            if (array == null)
                return;

            node.AppendSubscript(EnumerableLengthsSubscriptName);
            node.Set(ToSubscriptString(GetArrayLengths(array)));
            node.SetSubscriptCount(node.GetSubscriptCount() - 1);

            var indexedElements = GetIndexedElements(array, new List<int>());

            foreach (var indexedElement in indexedElements)
            {
                node.AppendSubscript(ToSubscriptString(indexedElement.Item2));
                WriteItem(indexedElement.Item1, node);
                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }
        }

        private static object ReadArray(NodeReference node, Type columnType)
        {
            var arrayLengthsString = node.GetString(EnumerableLengthsSubscriptName);
            if (string.IsNullOrEmpty(arrayLengthsString))
                return null;

            var elementType = columnType.GetElementType();
            var arrayLengths = ToArrayIndexes(arrayLengthsString);
            var instance = Array.CreateInstance(elementType, arrayLengths);

            var subscript = node.NextSubscript(EnumerableLengthsSubscriptName);

            while (!subscript.Equals(""))
            {
                node.AppendSubscript(subscript);

                var item = ReadNode(node, elementType);
                var indexes = ToArrayIndexes(subscript);

                instance.SetValue(item, indexes);

                subscript = node.NextSubscript();
                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }

            return instance;
        }

        private static IEnumerable<Tuple<object, List<int>>> GetIndexedElements(Array array, List<int> indexes)
        {
            var result = new List<Tuple<object, List<int>>>();

            if (indexes.Count == array.Rank)
            {
                result.Add(new Tuple<object, List<int>>(array.GetValue(indexes.ToArray()), indexes));
                return result;
            }

            var dimension = indexes.Count;
            var length = array.GetLength(dimension);

            for (var i = 0; i < length; i++)
            {
                var indexesList = indexes.ToList();
                indexesList.Add(i);
                result.AddRange(GetIndexedElements(array, indexesList));
            }

            return result;
        }

        private static IEnumerable<int> GetArrayLengths(Array array)
        {
            var result = new List<int>();

            for (var dimension = 0; dimension < array.Rank; dimension++)
            {
                result.Add(array.GetLength(dimension));
            }

            return result;
        }

        private static string ToSubscriptString(IEnumerable<int> arrayIndexes)
        {
            return string.Format("{{{0}}}", string.Join(",", arrayIndexes));
        }

        private static string ToSubscriptString(int index)
        {
            return string.Format("{{{0}}}", index);
        }

        private static int[] ToArrayIndexes(string subscriptString)
        {
            var indexes = Regex.Replace(subscriptString, @"{(\S*)}", "$1");
            return indexes.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
        }
    }
}
