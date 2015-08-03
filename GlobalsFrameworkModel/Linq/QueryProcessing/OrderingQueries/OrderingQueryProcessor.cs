using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionCaching;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing.OrderingQueries
{
    internal abstract class OrderingQueryProcessor : IQueryProcessor
    {
        public abstract bool CanProcess(MethodCallExpression query);

        public abstract ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult);

        protected LambdaExpression GetKeySelector(MethodCallExpression query)
        {
            return ((UnaryExpression)query.Arguments[1]).Operand as LambdaExpression;
        }

        protected object GetComparer(MethodCallExpression query)
        {
            var hasComparer = query.Arguments.Count > 2;
            return !hasComparer ? null : ((ConstantExpression)query.Arguments[2]).Value;
        }

        protected ProcessingResult GetKeys(LambdaExpression keySelector, ProcessingResult parentResult)
        {
            if (!parentResult.IsDeferred())
            {
                var keys = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(keySelector.ReturnType));
                var compiledSelector = CompiledExpressionStorage.GetOrAddCompiledLambda(keySelector);

                foreach (var item in parentResult.GetItems())
                {
                    keys.Add(compiledSelector.DynamicInvoke(item));
                }

                return new ProcessingResult(true, keys);
            }

            var nodeReferences = parentResult.GetDeferredList();
            var keysResult = ExpressionProcessingHelper.ProcessExpression(keySelector.Body, nodeReferences);
            if (!keysResult.IsSuccess)
                return ProcessingResult.Unsuccessful;

            if (keysResult.IsSingleItem)
            {
                keysResult = ExpressionProcessingHelper.CopyInstances(keysResult, nodeReferences.Count,
                    () => ExpressionProcessingHelper.ProcessExpression(keySelector.Body, nodeReferences).Result);
            }

            keysResult = new ProcessingResult(true, keysResult.GetLoadedItems(keySelector.ReturnType));
            return QueryProcessingHelper.NormalizeMultipleResult(keysResult, keySelector.ReturnType);
        }
    }
}
