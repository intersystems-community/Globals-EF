using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal interface IQueryProcessor
    {
        bool CanProcess(MethodCallExpression query);
        ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult);
    }
}
