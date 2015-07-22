using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ElementAtQueryProcessor :IQueryProcessor
    {
        private static readonly Dictionary<string, Func<IEnumerator, Type, int, ProcessingResult>> OptimizedHandlers =
            new Dictionary<string, Func<IEnumerator, Type, int, ProcessingResult>>
            {
                {"ElementAt", ProcessElementAt},
                {"ElementAtOrDefault", ProcessElementAtOrDefault},
            };

        public bool CanProcess(MethodCallExpression query)
        {
            return OptimizedHandlers.ContainsKey(query.Method.Name);
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var index = (int) ((ConstantExpression) query.Arguments[1]).Value;

            var enumerator = parentResult.GetItems().GetEnumerator();
            var elementType = QueryProcessingHelper.GetReturnParameterType(query);

            return OptimizedHandlers[query.Method.Name](enumerator, elementType, index);
        }

        private static ProcessingResult ProcessElementAt(IEnumerator enumerator, Type elementType, int index)
        {
            while (index>=0)
            {
                if (!enumerator.MoveNext())
                    throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
                index--;
            }

            return new ProcessingResult(true, enumerator.Current, true);
        }

        private static ProcessingResult ProcessElementAtOrDefault(IEnumerator enumerator, Type elementType, int index)
        {
            while (index >= 0)
            {
                if (!enumerator.MoveNext())
                    return new ProcessingResult(true, InstanceCreator.GetDefaultValue(elementType), true);

                index--;
            }

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
