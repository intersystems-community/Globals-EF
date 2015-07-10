using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GlobalsFrameworkTest.Data;

namespace GlobalsFrameworkTest.PerformanceDiagnostics
{
    internal static class PerformanceMonitor
    {
        internal static void TestPerformance(int milliseconds = 1000)
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(TestDataFactory.GetTestData());
                context.DDbSet.InsertOnSubmit(new TestD(0) {Id = new TestC {Id = 3, Value = "val"}, Value = 4});

                context.SubmitChanges();

                var methods = typeof (PerformanceMonitor).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(m => m.GetCustomAttribute<PerfWatchAttribute>() != null)
                    .ToList();

                foreach (var methodInfo in methods)
                    Console.WriteLine("{0} {1}", methodInfo.Name, GetExecutedTimes(() => methodInfo.Invoke(null, new []{ context }), milliseconds));

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.DDbSet.DeleteAllOnSubmit(context.DDbSet);
                context.SubmitChanges();
            }
        }

        private static int GetExecutedTimes(Action action, int milliseconds = 1000)
        {
            var stopwatch = new Stopwatch();
            var executedTimes = 0;
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                action();
                executedTimes++;
            }

            stopwatch.Stop();
            return executedTimes;
        }

        #region Test methods

        [PerfWatch]
        private static void MemberExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => c.Id >= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void MemberExpression_Nullable(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => c.C.Value.Id >= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ConstantExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(c => false).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ParameterExpression(TestDataContext context)
        {
            var res = context.DDbSet.Where(c => c != null).Select(item => item).ToList();
        }

        [PerfWatch]
        private static void EqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void NotEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value != 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void LessThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value < 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void LessThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value <= 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void GreaterThanExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value > 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void GreaterThanOrEqualExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value >= 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void AddExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value + 3 == 10).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void AddCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value + 3) == 10).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void SubtractExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value - 3 == 4).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void SubtractCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value - 3) == 4).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void MultiplyExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value * 3 == 21).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void MultiplyCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value * 3) == 21).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void DivideExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.D.Value / 2.0 > 16).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ConvertExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Where(i => i.D.Value < tuple.Item1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ConvertCheckedExpression(TestDataContext context)
        {
            var tuple = new Tuple<int>(7);
            var res = context.ADbSet.Where(i => i.D.Value < checked((double)tuple.Item1)).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ModuloExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Id.Value % 3 == 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ArrayIndexExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Array6[0][1].Id == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ArrayLengthExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.TestBProperty.Array6[0].Length == 2).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void LeftShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id << 1) > 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void RightShiftExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id >> 1) >= 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ExclusiveOrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id ^ 2) == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void CoalesceExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.D ?? 2.2) > 2.2).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void AndExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id & 3) == 1).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void AndAlsoExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => false && i != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void OrExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => (i.C.Value.Id | 3) == 7).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void OrElseExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => true || i != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void CallExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.L1.Count(it => it > 4) > 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void ConditionalExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => i.Id >= 0 ? true : false).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void InvokeExpression(TestDataContext context)
        {
            Func<int, bool> func = (n) => n >= 0;
            var res = context.ADbSet.Where(i => func(i.Id)).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void NegateExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => unchecked (-i.Id) <= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void NegateCheckedExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => checked (-i.Id) <= 0).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void NotExpression(TestDataContext context)
        {
            var res = context.ADbSet.Where(i => !i.C.Value).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void TypeAsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Where(i => (testC as TestC?) != null).Select(item => item.C).ToList();
        }

        [PerfWatch]
        private static void TypeIsExpression(TestDataContext context)
        {
            object testC = new TestC();
            var res = context.ADbSet.Where(i => i is TestA).Select(item => item.C).ToList();
        }

        #endregion
    }
}
