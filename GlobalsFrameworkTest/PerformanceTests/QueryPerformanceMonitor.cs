using System.Linq;
using GlobalsFrameworkTest.Data;
using GlobalsFrameworkTest.PerformanceDiagnostics;

namespace GlobalsFrameworkTest.PerformanceTests
{
    internal sealed class QueryPerformanceMonitor : BasePerformanceMonitor
    {
        [PerfWatch]
        private void SelectQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).Count();
        }

        [PerfWatch]
        private void WhereQuery(TestDataContext context)
        {
            var res = context.ADbSet.Where(a => a.Id >= 0).Select(a => a.Id).Count();
        }
    }
}
