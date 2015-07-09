using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GlobalsFramework.Linq
{
    internal static class ExpressionHelper
    {
        internal static object InvokeLinqExpression(MethodCallExpression expression, object sourse)
        {
            //source parameter always at first place
            var methodParameters = expression.Method.GetParameters().Skip(1).ToList();
            var invocationParameters = new List<object>(methodParameters.Count()) {(sourse as IEnumerable).AsQueryable()};

            foreach (var methodParameter in methodParameters)
            {
                var expressionArgument = expression.Arguments.Single(arg => arg.Type == methodParameter.ParameterType);

                if (expressionArgument is UnaryExpression)
                    invocationParameters.Add((expressionArgument as UnaryExpression).Operand);
                else
                    invocationParameters.Add((expressionArgument as ConstantExpression).Value);
            }
            try
            {
                return expression.Method.Invoke(null, invocationParameters.ToArray());
            }
                //First, Single or Last expressions can throw exception
            catch(Exception e)
            {
                //TargetInvocationException must be displayed
                throw e.InnerException;
            }
        }

        internal static Type GetSourceParameterType(MethodCallExpression expression)
        {
            //source parameter always at first place
            var sourceParameter = expression.Method.GetParameters().First();

            return sourceParameter.ParameterType.IsGenericType
                ? sourceParameter.ParameterType.GetGenericArguments().Single()
                : sourceParameter.ParameterType;
        }

        internal static Type GetReturnParameterType(MethodCallExpression expression)
        {
            var returnParameter = expression.Method.ReturnParameter;

            return returnParameter.ParameterType.IsGenericType
                ? returnParameter.ParameterType.GetGenericArguments().Single()
                : returnParameter.ParameterType;
        }

        internal static List<MemberInfo> GetMembersChain(Expression expression)
        {
            var result = new List<MemberInfo>();

            var memberExpression = expression as MemberExpression;

            while (memberExpression!=null)
            {
                result.Add(memberExpression.Member);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            result.Reverse();

            return result;
        }

        //check is expression contains only either MemberExpression or ParameterExpression
        internal static bool IsFullyMemberAccessExpression(Expression expression)
        {
            if (expression is ParameterExpression)
                return true;

            var memberExpression = expression as MemberExpression;

            while (memberExpression != null)
            {
                if (memberExpression.Expression is ParameterExpression)
                    return true;
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            return false;
        }
    }
}
