using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class ParameterExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestParameterExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i == null).Count();
                Assert.AreEqual(0, result);
            }
        }
    }
}
