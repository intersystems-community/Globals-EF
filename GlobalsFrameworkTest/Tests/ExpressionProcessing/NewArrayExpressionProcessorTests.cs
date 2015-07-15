using System;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class NewArrayExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestNewArrayBoundsExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => new int[2, 3] != null).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => new int[2, i.Id] != null).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => new int[i.Id][,] == null).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => new int[2, 3].Length == 6).Count();
                Assert.AreEqual(1, result4);
            }
        }

        [Test]
        public void TestNewArrayInitExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => new[] { 2, 3, 4 }.Length == 3).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => new[] { 2, 3, 4 }[1] == 3).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => new[] { new int[2, 3] } != null).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => new[] { 2, 3, i.Id }[2] == i.Id).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => new Func<int>[] { () => 5, () => 6 }[1]() == 6).Count();
                Assert.AreEqual(1, result5);

                context.ADbSet.InsertOnSubmit(new TestA(1) { C = new TestC() { Id = 8 } });
                context.SubmitChanges();

                var result6 = context.ADbSet.Where(i => new[] { 2, i.C.Value.Id }[1] == 8).Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.Where(i => new[] { 2, i.C.Value.Id }[1] == i.C.Value.Id).Count();
                Assert.AreEqual(2, result7);
            }
        }
    }
}
