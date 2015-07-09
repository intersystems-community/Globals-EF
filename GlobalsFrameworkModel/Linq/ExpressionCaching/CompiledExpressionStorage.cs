using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.ExpressionCaching
{
    internal static class CompiledExpressionStorage
    {
        private static readonly ExpressionHashCodeCalculator HashCodeCalculator = new ExpressionHashCodeCalculator();
        private static readonly ConcurrentDictionary<int, Delegate> CompiledLamdas = new ConcurrentDictionary<int, Delegate>();

        internal static Delegate GetOrAddCompiledLambda(LambdaExpression expression)
        {
            var hash = HashCodeCalculator.CalculateHashCode(expression);
            Delegate result;

            if (CompiledLamdas.TryGetValue(hash, out result))
                return result;

            result = expression.Compile();
            CompiledLamdas.TryAdd(hash, result);

            return result;
        }
    }
}
