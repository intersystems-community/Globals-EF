using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class SingleQueryProcessor : IQueryProcessor
    {
        private static readonly Dictionary<string, Func<IEnumerator, Type, ProcessingResult>> OptimizedHandlers =
            new Dictionary<string, Func<IEnumerator, Type, ProcessingResult>>
            {
                {"Single", ProcessSingle},
                {"SingleOrDefault", ProcessSingleOrDefault},
            };

        public bool CanProcess(MethodCallExpression query)
        {
            return OptimizedHandlers.ContainsKey(query.Method.Name);
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            return QueryProcessingHelper.ProcessSingleResultQuery(query, parentResult, OptimizedHandlers[query.Method.Name]);
        }

        private static ProcessingResult ProcessSingle(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var result = enumerator.Current;

            if(enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return new ProcessingResult(true, result, true);
        }

        private static ProcessingResult ProcessSingleOrDefault(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                return new ProcessingResult(true, InstanceCreator.GetDefaultValue(elementType), true);

            var result = enumerator.Current;

            if(enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return new ProcessingResult(true, result, true);
        }
    }
}
