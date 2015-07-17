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
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NotEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value != 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void LessThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value < 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void LessThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value <= 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void GreaterThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value > 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void GreaterThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value >= 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void AddExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value + 3 == 10).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void AddCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value + 3) == 10).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void SubtractExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value - 3 == 4).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void SubtractCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value - 3) == 4).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void MultiplyExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value * 3 == 21).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void MultiplyCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value * 3) == 21).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void LeftShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id << 1) > 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void RightShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id >> 1) >= 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void DivideExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.D.Value / 2.0 > 16).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ModuloExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value % 3 == 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ArrayIndexExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Array6[0][1].Id == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ExclusiveOrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id ^ 2) == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void CoalesceExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.D ?? 2.2) > 2.2).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void AndExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id & 3) == 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void AndAlsoExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => false && i != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void OrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id | 3) == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void OrElseExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => true || i != null).Select(item => item.C).ToList();
        }

        #endregion

        #region UnaryExpressions

        [PerfWatch]
        private void ConvertExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Where(i => i.D.Value < tuple.Item1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ConvertCheckedExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Where(i => i.D.Value < checked((double)tuple.Item1)).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ArrayLengthExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Array6[0].Length == 2).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NegateExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => unchecked(-i.Id) <= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NegateCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(-i.Id) <= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NotExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => !i.C.Value).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void TypeAsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Where(i => (testC as TestC?) != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void UnaryPlusExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (+i.C.Value) >= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void OnesComplementExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id == -8).Select(item => item.C).ToList();
        }

        #endregion

        [PerfWatch]
        private void MemberExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => c.Id >= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void MemberExpression_Nullable(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => c.C.Value.Id >= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ConstantExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => false).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ParameterExpression(TestDataContext context)
        {
            var res = context.DDbSet.Where(c => c != null).Select(item => item).ToList();
        }

        [PerfWatch]
        private void CallExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.L1.Count(it => it > 4) > 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ConditionalExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.Id >= 0 ? true : false).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void InvokeExpression(TestDataContext context)
        {
            Func<int, bool> func = (n) => n >= 0;
            var res = context.ADbSet.Where(i => func(i.Id)).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void TypeIsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Where(i => i is TestA).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NewExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => new TestA(0) != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NewArrayBoundsExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => new int[2] != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void NewArrayInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => new[] { 2, 3 } != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void MemberInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => new TestInit(i.Id) != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private void ListInitExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => new List<int> { 2, 3 } != null).Select(item => item.C).ToList();
        }
    }
}
