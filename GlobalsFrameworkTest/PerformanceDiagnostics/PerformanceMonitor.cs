using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GlobalsFramework;

namespace GlobalsFrameworkTest.PerformanceDiagnostics
{
    internal abstract class PerformanceMonitor
    {
        internal void TestPerformance(int milliseconds = 1000)
        {
            SetUpTestData();

            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<PerfWatchAttribute>() != null)
                .ToList();

            ExecuteInContext((context) =>
            {
                foreach (var methodInfo in methods)
                    Console.WriteLine("{0} {1}", methodInfo.Name,
                        GetExecutedTimes(() => methodInfo.Invoke(this, new object[] {context}), milliseconds));
            });

            ClearTestData();
        }

        protected abstract void SetUpTestData();
        protected abstract void ClearTestData();
        protected abstract void ExecuteInContext(Action<DataContext> action);

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
    }
}
