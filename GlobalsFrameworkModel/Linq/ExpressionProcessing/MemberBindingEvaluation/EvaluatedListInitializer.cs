using System.Collections.Generic;
using System.Reflection;

namespace GlobalsFramework.Linq.ExpressionProcessing.MemberBindingEvaluation
{
    internal sealed class EvaluatedListInitializer
    {
        internal EvaluatedListInitializer(MethodInfo method, List<ProcessingResult> arguments)
        {
            AddMethod = method;
            Arguments = arguments;
        }

        internal MethodInfo AddMethod { get; private set; }
        internal List<ProcessingResult> Arguments { get; private set; } 
    }
}
