using System;
using GlobalsFrameworkTest.PerformanceTests;

namespace GlobalsFrameworkTest
{
    class Program
    {
        private static void Main()
        {
            TestPerformance();
        }

        private static void TestPerformance()
        {
            Console.WriteLine("Expression nodes performance tests:\n");

            var expressionNodesPerfMonitor = new ExpressionNodesPerformanceMonitor();
            expressionNodesPerfMonitor.TestPerformance();

            Console.WriteLine("\nLINQ queries performance tests:\n");

            var queryPerfMonitor = new QueryPerformanceMonitor();
            queryPerfMonitor.TestPerformance();
        }
    }
}
