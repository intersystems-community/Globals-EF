using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class ConditionalExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestConditionalExpression()
        {
            using (var context = new TestDataContext())
            {
                var a = true;
                var result1 = context.ADbSet.Where(i => a ? false : true).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => a ? i.Id > 0 : false).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.Id > 0 ? false : true).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => i.Id > 0 ? i.Id > 0 : false).Count();
                Assert.AreEqual(1, result4);

                bool[] arr = null;
                var result5 = context.ADbSet.Where(i => arr != null ? arr[0] : false).Count();
                Assert.AreEqual(0, result5);
            }
        }
    }
}
