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

        [PerfWatch]
        private void CountQuery(TestDataContext context)
        {
            var res = context.ADbSet.Count(a => a.Id >= 0);
        }

        [PerfWatch]
        private void LongCountQuery(TestDataContext context)
        {
            var res = context.ADbSet.LongCount(a => a.Id >= 0);
        }

        [PerfWatch]
        private void AnyQuery(TestDataContext context)
        {
            var res = context.ADbSet.Any(a => a.Id >= 0);
        }

        [PerfWatch]
        private void FirstQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).First(id => id >= 0);
        }

        [PerfWatch]
        private void FirstOrDefaultQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).FirstOrDefault(id => id >= 0);
        }

        [PerfWatch]
        private void ElementAtQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).ElementAt(0);
        }

        [PerfWatch]
        private void ElementAtOrDefaultQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).ElementAtOrDefault(0);
        }

        [PerfWatch]
        private void SingleQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).Single(id => id >= 0);
        }

        [PerfWatch]
        private void SingleOrDefaultQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).SingleOrDefault(id => id >= 0);
        }
    }
}
