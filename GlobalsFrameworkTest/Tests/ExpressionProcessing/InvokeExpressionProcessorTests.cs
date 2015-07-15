using System;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class InvokeExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestInvokeExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.Func(i.Id) == i.Id * 2).Count();
                Assert.AreEqual(1, result1);

                Func<Predicate<int>, int, bool> a = (pred, i) => pred(i);
                var result2 = context.ADbSet.Where(i => a((n) => n + 2 == i.TestBProperty.Id.Value + 2, i.TestBProperty.Id.Value)).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => a((n) => n == 3, i.TestBProperty.Id.Value)).Count();
                Assert.AreEqual(0, result3);
            }
        }
    }
}
