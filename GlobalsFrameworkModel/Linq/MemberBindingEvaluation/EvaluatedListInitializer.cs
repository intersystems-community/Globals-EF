using System.Collections.Generic;
using System.Reflection;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.MemberBindingEvaluation
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
