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
            ClearTestData();
            SetUpTestData();

            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<PerfWatchAttribute>() != null)
                .ToList();

            foreach (var methodInfo in methods)
            {
                var method = methodInfo;
                var executedTimes = 0;

                ExecuteInContext(context => executedTimes = GetExecutedTimes(() => method.Invoke(this, new object[] {context}), milliseconds));
                Console.WriteLine("{0} {1}", method.Name, executedTimes);
            }

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
