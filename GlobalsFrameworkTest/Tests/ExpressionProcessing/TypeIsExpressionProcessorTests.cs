using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class TypeIsExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestTypeIsExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (object)i is TestA).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.TestBProperty.Id.Value is int?) && (i.TestBProperty.Id >= 3)).Count();
                Assert.AreEqual(1, result2);

                object testC = new TestC();
                var result3 = context.ADbSet.Where(i => testC is TestA).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => testC is TestC?).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => (testC is TestC?) && (i.TestBProperty.Id >= 3)).Count();
                Assert.AreEqual(1, result5);

                var result6 = context.ADbSet.Where(i => i is object).Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.Where(i => i is TestA2).Count();
                Assert.AreEqual(0, result7);

                var test2 = new TestA2();
                var result8 = context.ADbSet.Where(i => test2 is TestA).Count();
                Assert.AreEqual(1, result8);
            }
        }
    }
}
