using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.ExpressionCaching
{
    internal sealed class ExpressionHashCodeCalculator
    {
        private const int NullHashCode = 0x7abf3456;

        internal int CalculateHashCode(Expression e)
        {
            int result = NullHashCode;

            if (e == null)
                return result;

            switch (e.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    result = VisitUnary((UnaryExpression) e);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    result = VisitBinary((BinaryExpression) e);
                    break;
                case ExpressionType.TypeIs:
                    result = VisitTypeIs((TypeBinaryExpression) e);
                    break;
                case ExpressionType.Conditional:
                    result = VisitConditional((ConditionalExpression) e);
                    break;
                case ExpressionType.Constant:
                    result = VisitConstant((ConstantExpression) e);
                    break;
                case ExpressionType.Parameter:
                    result = VisitParameter((ParameterExpression) e);
                    break;
                case ExpressionType.MemberAccess:
                    result = VisitMemberAccess((MemberExpression) e);
                    break;
                case ExpressionType.Call:
                    result = VisitMethodCall((MethodCallExpression) e);
                    break;
                case ExpressionType.Lambda:
                    result = VisitLambda((LambdaExpression) e);
                    break;
                case ExpressionType.New:
                    result = VisitNew((NewExpression) e);
                    break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    result = VisitNewArray((NewArrayExpression) e);
                    break;
                case ExpressionType.Invoke:
                    result = VisitInvocation((InvocationExpression) e);
                    break;
                case ExpressionType.MemberInit:
                    result = VisitMemberInit((MemberInitExpression) e);
                    break;
                case ExpressionType.ListInit:
                    result = VisitListInit((ListInitExpression) e);
                    break;
                default:
                    result = VisitUnknown(e);
                    break;
            }

            var hash = (uint) (result ^ (int) e.NodeType ^ e.Type.GetHashCode());
            //transform bytes 0123 -> 1302
            hash = (hash & 0xFF00) >> 8 | (hash & 0xFF000000) >> 16 | (hash & 0xFF) << 16 | (hash & 0xFF0000) << 8;
            return (int) hash;
        }

        private int VisitUnary(UnaryExpression u)
        {
            return CalculateHashCode(u.Operand) ^ (u.Method == null ? 0 : u.Method.GetHashCode());
        }
        private int VisitBinary(BinaryExpression b)
        {
            return CalculateHashCode(b.Left) ^ CalculateHashCode(b.Right) ^ (b.Method == null ? 0 : b.Method.GetHashCode());
        }
        private int VisitTypeIs(TypeBinaryExpression tb)
        {
            return CalculateHashCode(tb.Expression) ^ tb.TypeOperand.GetHashCode();
        }
        private int VisitConstant(ConstantExpression c)
        {
            return c.Value != null ? c.Value.GetHashCode() : NullHashCode;
        }
        private int VisitConditional(ConditionalExpression c)
        {
            return CalculateHashCode(c.Test) ^ CalculateHashCode(c.IfTrue) ^ CalculateHashCode(c.IfFalse);
        }
        private int VisitParameter(ParameterExpression p)
        {
            return p.Name.GetHashCode() ^ p.Type.GetHashCode();
        }
        private int VisitMemberAccess(MemberExpression m)
        {
            return CalculateHashCode(m.Expression) ^ m.Member.GetHashCode();
        }
        private int VisitMethodCall(MethodCallExpression mc)
        {
            return CalculateHashCode(mc.Object) ^ mc.Method.GetHashCode() ^ HashExpressionSequence(mc.Arguments);
        }
        private int VisitLambda(LambdaExpression l)
        {
            return HashExpressionSequence(l.Parameters) ^ CalculateHashCode(l.Body);
        }
        private int VisitNew(NewExpression n)
        {
            var result = 0;
            result ^= n.Constructor.GetHashCode();
            result ^= n.Members.Aggregate(NullHashCode, (seed, current) => seed ^ current.GetHashCode());
            result ^= HashExpressionSequence(n.Arguments);
            return result;
        }
        private int VisitMemberInit(MemberInitExpression mi)
        {
            var result = CalculateHashCode(mi.NewExpression);
            return mi.Bindings.Aggregate(result,
                (current, b) => current ^ (b.BindingType.GetHashCode() ^ b.Member.GetHashCode()));
        }
        private int VisitListInit(ListInitExpression li)
        {
            var result = VisitNew(li.NewExpression);
            return li.Initializers.Aggregate(result,
                (current, e) => current ^ (e.AddMethod.GetHashCode() ^ HashExpressionSequence(e.Arguments)));
        }
        private int VisitNewArray(NewArrayExpression na)
        {
            return HashExpressionSequence(na.Expressions);
        }
        private int VisitInvocation(InvocationExpression i)
        {
            return HashExpressionSequence(i.Arguments) ^ CalculateHashCode(i.Expression);
        }
        private int VisitUnknown(Expression e)
        {
            return e.GetHashCode();
        }

        private int HashExpressionSequence(IEnumerable<Expression> expressions)
        {
            return expressions.Aggregate(0, (current, e) => current ^ CalculateHashCode(e));
        }
    }
}
