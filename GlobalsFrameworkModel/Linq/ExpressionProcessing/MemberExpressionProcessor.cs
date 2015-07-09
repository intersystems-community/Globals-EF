using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GlobalsFramework.Access;
using GlobalsFramework.Attributes;
using GlobalsFramework.Utils.TypeDescription;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal sealed class MemberExpressionProcessor : IExpressionProcessor
    {
        public bool CanProcess(Expression expression)
        {
            return expression.NodeType == ExpressionType.MemberAccess;
        }

        public ProcessingResult ProcessExpression(Expression expression, List<NodeReference> references)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
                return ProcessingResult.Unsuccessful;

            //getting static member
            if (memberExpression.Expression == null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                    return new ProcessingResult(true, propertyInfo.GetValue(null), true);

                var fieldInfo = memberExpression.Member as FieldInfo;
                if (fieldInfo != null)
                    return new ProcessingResult(true, fieldInfo.GetValue(null), true);

                return ProcessingResult.Unsuccessful;
            }

            var parentResult = PredicateExpressionProcessor.ProcessExpression(memberExpression.Expression, references);
            if (!parentResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (parentResult.IsSingleItem)
            {
                var item = parentResult.GetLoadedItem(memberExpression.Type);
                return new ProcessingResult(true, ProcessLoadedEntityMember(item, memberExpression), true);
            }

            var nodes = parentResult.GetDeferredItems();

            if (nodes != null)
            {
                var columnAttribute = memberExpression.Member.GetCustomAttribute<ColumnAttribute>();
                return new ProcessingResult(true, ResolveColumn(columnAttribute, memberExpression, nodes));
            }

            var values = parentResult.GetItems();
            return new ProcessingResult(true, ProcessLoadedEntityMembers(values, memberExpression));
        }

        private static List<object> ProcessLoadedEntityMembers(IEnumerable loadedData, MemberExpression expression)
        {
            var result = loadedData;

            var propertyInfo = expression.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                result = from object item in result
                         select propertyInfo.GetValue(item, null);

                return (from object item in result select item).ToList();
            }

            var fieldInfo = expression.Member as FieldInfo;
            if (fieldInfo != null)
            {
                result = from object item in result
                         select fieldInfo.GetValue(item);

                return (from object item in result select item).ToList();
            }

            throw new NotSupportedException("Supported only properties or fields for member expression");
        }

        private static object ProcessLoadedEntityMember(object loadedData, MemberExpression expression)
        {
            var propertyInfo = expression.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(loadedData, null);
            }

            var fieldInfo = expression.Member as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(loadedData);
            }

            throw new NotSupportedException("Supported only properties or fields for member expression");
        }

        private static object ResolveColumn(ColumnAttribute columnAttribute, MemberExpression expression, IEnumerable<NodeReference> references)
        {
            if (columnAttribute != null)
            {
                foreach (var nodeReference in references)
                    nodeReference.AppendSubscript(columnAttribute.Name ?? expression.Member.Name);
                return references;
            }

            if (EntityTypeDescriptor.IsSupportedEnumerable(expression.Expression.Type) && expression.Member.Name == "Count")
            {
                var result = references.Select(DatabaseManager.GetEnumerableCount).ToList();
                return result;
            }

            if (EntityTypeDescriptor.IsArrayType(expression.Expression.Type) && expression.Member.Name == "Length")
            {
                var result = references.Select(DatabaseManager.GetEnumerableCount).ToList();
                return result;
            }

            if (EntityTypeDescriptor.IsArrayType(expression.Expression.Type) && expression.Member.Name == "LongLength")
            {
                var result = references.Select(DatabaseManager.GetEnumerableCount).ToList();
                return result;
            }

            if (EntityTypeDescriptor.IsNullableType(expression.Expression.Type) && expression.Member.Name == "Value")
                return references;

            var parentType = expression.Expression.Type;
            var data = DatabaseManager.ReadNodes(references, parentType) as IEnumerable;
            return ProcessLoadedEntityMembers(data, expression);

        }
    }
}
