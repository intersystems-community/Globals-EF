using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class ConstantExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestConstantExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => false).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => true).Count();
                Assert.AreEqual(1, result2);
            }
        }
    }
}
