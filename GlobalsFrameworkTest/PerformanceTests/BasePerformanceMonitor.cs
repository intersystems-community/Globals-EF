using System;
using GlobalsFramework;
using GlobalsFrameworkTest.Data;
using GlobalsFrameworkTest.PerformanceDiagnostics;

namespace GlobalsFrameworkTest.PerformanceTests
{
    internal class BasePerformanceMonitor : PerformanceMonitor
    {
        protected sealed override void SetUpTestData()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(TestDataFactory.GetTestData());
                context.DDbSet.InsertOnSubmit(new TestD(0) { Id = new TestC { Id = 3, Value = "val" }, Value = 4 });

                context.SubmitChanges();
            }
        }

        protected sealed override void ClearTestData()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.DDbSet.DeleteAllOnSubmit(context.DDbSet);

                context.SubmitChanges();
            }
        }

        protected sealed override void ExecuteInContext(Action<DataContext> action)
        {
            using (var context = new TestDataContext())
            {
                action(context);
            }
        }
    }
}
