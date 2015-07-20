using System;
using System.Collections.Generic;
using System.Linq;
using GlobalsFrameworkTest.Data;
using GlobalsFrameworkTest.PerformanceDiagnostics;

namespace GlobalsFrameworkTest.PerformanceTests
{
    internal sealed class ExpressionNodesPerformanceMonitor : BasePerformanceMonitor
    {
        #region BinaryExpressions

        [PerfWatch]
        private void EqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value == 7);
        }

        [PerfWatch]
        private void NotEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value != 7);
        }

        [PerfWatch]
        private void LessThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value < 7);
        }

        [PerfWatch]
        private void LessThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value <= 7);
        }

        [PerfWatch]
        private void GreaterThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value > 7);
        }

        [PerfWatch]
        private void GreaterThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value >= 7);
        }

        [PerfWatch]
        private void AddExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value + 3 == 10);
        }

        [PerfWatch]
        private void AddCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => checked(i.TestBProperty.Id.Value + 3) == 10);
        }

        [PerfWatch]
        private void SubtractExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value - 3 == 4);
        }

        [PerfWatch]
        private void SubtractCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => checked(i.TestBProperty.Id.Value - 3) == 4);
        }

        [PerfWatch]
        private void MultiplyExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value * 3 == 21);
        }

        [PerfWatch]
        private void MultiplyCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => checked(i.TestBProperty.Id.Value * 3) == 21);
        }

        [PerfWatch]
        private void LeftShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.C.Value.Id << 1) > 0);
        }

        [PerfWatch]
        private void RightShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.C.Value.Id >> 1) >= 1);
        }

        [PerfWatch]
        private void DivideExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.D.Value / 2.0 > 16);
        }

        [PerfWatch]
        private void ModuloExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id.Value % 3 == 1);
        }

        [PerfWatch]
        private void ArrayIndexExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Array6[0][1].Id == 7);
        }

        [PerfWatch]
        private void ExclusiveOrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.C.Value.Id ^ 2) == 7);
        }

        [PerfWatch]
        private void CoalesceExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.D ?? 2.2) > 2.2);
        }

        [PerfWatch]
        private void AndExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.C.Value.Id & 3) == 1);
        }

        [PerfWatch]
        private void AndAlsoExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => false && i != null);
        }

        [PerfWatch]
        private void OrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (i.C.Value.Id | 3) == 7);
        }

        [PerfWatch]
        private void OrElseExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => true || i != null);
        }

        #endregion

        #region UnaryExpressions

        [PerfWatch]
        private void ConvertExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Count(i => i.D.Value < tuple.Item1);
        }

        [PerfWatch]
        private void ConvertCheckedExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Count(i => i.D.Value < checked((double)tuple.Item1));
        }

        [PerfWatch]
        private void ArrayLengthExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Array6[0].Length == 2);
        }

        [PerfWatch]
        private void NegateExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => unchecked(-i.Id) <= 0);
        }

        [PerfWatch]
        private void NegateCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => checked(-i.Id) <= 0);
        }

        [PerfWatch]
        private void NotExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => !i.C.Value);
        }

        [PerfWatch]
        private void TypeAsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Count(i => (testC as TestC?) != null);
        }

        [PerfWatch]
        private void UnaryPlusExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => (+i.C.Value) >= 0);
        }

        [PerfWatch]
        private void OnesComplementExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.TestBProperty.Id == -8);
        }

        #endregion

        [PerfWatch]
        private void MemberExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(c => c.Id >= 0);
        }

        [PerfWatch]
        private void MemberExpression_Nullable(TestDataContext context)
        {
            var res = context.ADbSet.Count(c => c.C.Value.Id >= 0);
        }

        [PerfWatch]
        private void ConstantExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(c => false);
        }

        [PerfWatch]
        private void ParameterExpression(TestDataContext context)
        {
            var res = context.DDbSet.Count(c => c != null);
        }

        [PerfWatch]
        private void CallExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.L1.Count(it => it > 4) > 0);
        }

        [PerfWatch]
        private void ConditionalExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => i.Id >= 0 ? true : false);
        }

        [PerfWatch]
        private void InvokeExpression(TestDataContext context)
        {
            Func<int, bool> func = (n) => n >= 0;
            var res = context.ADbSet.Count(i => func(i.Id));
        }

        [PerfWatch]
        private void TypeIsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Count(i => i is TestA);
        }

        [PerfWatch]
        private void NewExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => new TestA(0) != null);
        }

        [PerfWatch]
        private void NewArrayBoundsExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => new int[2] != null);
        }

        [PerfWatch]
        private void NewArrayInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => new[] { 2, 3 } != null);
        }

        [PerfWatch]
        private void MemberInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => new TestInit(i.Id) != null);
        }

        [PerfWatch]
        private void ListInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Count(i => new List<int> { 2, 3 } != null);
        }
    }
}
