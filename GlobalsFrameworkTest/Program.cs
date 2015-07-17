using System;
using System.Diagnostics;
using System.Linq;
using GlobalsFrameworkTest.Data;
using GlobalsFrameworkTest.PerformanceTests;

namespace GlobalsFrameworkTest
{
    class Program
    {
        private static void Main(string[] args)
        {
            var perfMonitor = new ExpressionNodesPerformanceMonitor();
            perfMonitor.TestPerformance();
        }
    }
}
