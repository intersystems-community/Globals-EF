using System.Collections.Generic;
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

        [PerfWatch]
        private void LastQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).Last(id => id >= 0);
        }

        [PerfWatch]
        private void LastOrDefaultQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).LastOrDefault(id => id >= 0);
        }

        [PerfWatch]
        private void AllQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id).All(id => id >= 0);
        }

        [PerfWatch]
        private void AverageQuery(TestDataContext context)
        {
            var res = context.ADbSet.Average(a => a.Id);
        }

        [PerfWatch]
        private void ConcatQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Concat(new List<int> {2, 3}).Count();
        }

        [PerfWatch]
        private void ContainsQuery(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.TestBProperty.Id.Value).Contains(7);
        }

        [PerfWatch]
        private void DefaultIfEmpty(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).DefaultIfEmpty().ToList();
        }

        [PerfWatch]
        private void OrderBy(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).OrderBy(i => i).ToList();
        }

        [PerfWatch]
        private void OrderByDescending(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).OrderByDescending(i => i).ToList();
        }

        [PerfWatch]
        private void ThenBy(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).OrderBy(i => 3).ThenBy(i => i).ToList();
        }

        [PerfWatch]
        private void ThenByDescending(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).OrderBy(i => 3).ThenByDescending(i => i).ToList();
        }

        [PerfWatch]
        private void Distinct(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Distinct().ToList();
        }

        [PerfWatch]
        private void Except(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Except(new List<int> {-3}).ToList();
        }

        [PerfWatch]
        private void Intersect(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Intersect(new List<int> {-3}).ToList();
        }

        [PerfWatch]
        private void SequenceEqual(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).SequenceEqual(new List<int> {-3});
        }

        [PerfWatch]
        private void Union(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Union(new List<int> {-3}).ToList();
        }

        [PerfWatch]
        private void MaxQuery(TestDataContext context)
        {
            var res = context.ADbSet.Max(a => a.Id);
        }

        [PerfWatch]
        private void MinQuery(TestDataContext context)
        {
            var res = context.ADbSet.Min(a => a.Id);
        }

        [PerfWatch]
        private void SumQuery(TestDataContext context)
        {
            var res = context.ADbSet.Sum(a => a.Id);
        }
    }
}
