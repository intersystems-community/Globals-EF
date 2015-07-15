using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    public class ExpressionProcessorTests
    {
        protected readonly TestA TestEntity = TestDataFactory.GetTestData();

        [SetUp]
        public void Init()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(TestEntity);
                context.SubmitChanges();
            }
        }

        [TearDown]
        public void ClearData()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();
            }
        }
    }
}
