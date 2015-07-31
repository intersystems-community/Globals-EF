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
            var res = context.ADbSet.Where(a => a.Id >= 0).Count();
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
            var res = context.ADbSet.DefaultIfEmpty().Count();
        }

        [PerfWatch]
        private void OrderBy(TestDataContext context)
        {
            var res = context.ADbSet.OrderBy(a => a.Id).Count();
        }

        [PerfWatch]
        private void OrderByDescending(TestDataContext context)
        {
            var res = context.ADbSet.OrderByDescending(a => a.Id).Count();
        }

        [PerfWatch]
        private void ThenBy(TestDataContext context)
        {
            var res = context.ADbSet.OrderBy(a => 3).ThenBy(a => a.Id).Count();
        }

        [PerfWatch]
        private void ThenByDescending(TestDataContext context)
        {
            var res = context.ADbSet.OrderBy(a => 3).ThenByDescending(a => a.Id).Count();
        }

        [PerfWatch]
        private void Distinct(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Distinct().Count();
        }

        [PerfWatch]
        private void Except(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Except(new List<int> {-3}).Count();
        }

        [PerfWatch]
        private void Intersect(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Intersect(new List<int> {-3}).Count();
        }

        [PerfWatch]
        private void SequenceEqual(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).SequenceEqual(new List<int> {-3});
        }

        [PerfWatch]
        private void Union(TestDataContext context)
        {
            var res = context.ADbSet.Select(a => a.Id).Union(new List<int> {-3}).Count();
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

        [PerfWatch]
        private void SkipQuery(TestDataContext context)
        {
            var res = context.ADbSet.Skip(0).Count();
        }

        [PerfWatch]
        private void SkipWhileQuery(TestDataContext context)
        {
            var res = context.ADbSet.SkipWhile(a => a.Id >= 0).Count();
        }
    }
}
