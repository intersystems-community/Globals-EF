using System.Collections.Generic;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class ListInitExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestListInitExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => new List<int> {2, 3}[1] == 3).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => new List<TestInit> {new TestInit(0) {Id = 3, List = {3, 4}}}[0].Id == 3).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => new TestInit2 {{2, 3}, {2, i.Id}}.List[1].Item2 == i.Id).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet
                    .Where(i => new List<int>(i.TestBProperty.Id.Value) {2, 3}.Capacity == i.TestBProperty.Id.Value)
                    .Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet
                    .Where(i => new List<int>(i.TestBProperty.Id.Value) { 2, i.TestBProperty.Id.Value }[1] == i.TestBProperty.Id.Value)
                    .Count();
                Assert.AreEqual(1, result5);

                var result6 = context.ADbSet.Where(i => new List<int>().Count == 0).Count();
                Assert.AreEqual(1, result6);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result7 = context.ADbSet
                    .Where(i => new List<int>(i.TestBProperty.Id.Value) { 2, i.TestBProperty.Id.Value }[1] == i.TestBProperty.Id.Value)
                    .Count();
                Assert.AreEqual(0, result7);
            }
        }
    }
}
