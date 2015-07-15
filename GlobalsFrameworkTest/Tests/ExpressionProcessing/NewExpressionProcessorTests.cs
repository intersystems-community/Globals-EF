using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class NewExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestNewExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => new TestA(0) != null).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => new TestA2() != null).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => new TestA(i.Id) != null).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => new TestA2((n) => n > 0, new TestA(2)) == null).Count();
                Assert.AreEqual(0, result4);

                var test = new TestA(0);
                var result5 = context.ADbSet.Where(i => new TestA2((n) => n > 0, test) != null).Count();
                Assert.AreEqual(1, result5);
            }
        }

    }
}
