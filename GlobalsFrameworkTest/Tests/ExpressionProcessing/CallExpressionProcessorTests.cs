using System;
using System.Collections.Generic;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class CallExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestCallExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Array.Count(it => it.Id == 3) > 0).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.List.Where(it => it != null).FirstOrDefault(it => it.Array.Length > 0) != null).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.L1[1] == 23).Count();
                Assert.AreEqual(1, result3);

                var array = new[] { 3 };

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array.Count(it => it.Id == array[0]) > 0).Count();
                Assert.AreEqual(1, result4);

                array[0] = 5;
                var result5 = context.ADbSet.Where(i => i.TestBProperty.Array.Count(it => it.Id == array[0]) > 0).Count();
                Assert.AreEqual(0, result5);

                var array2 = new[] { 4 };
                var result6 = context.ADbSet.Where(i => array2.Intersect(i.L1).Count() > 0).Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.Where(i => i.C.Value.TestFunct(i.L1)).Count();
                Assert.AreEqual(1, result7);

                var result8 = context.ADbSet.Where(i => i.L1.Any()).Count();
                Assert.AreEqual(1, result8);

                var result9 = context.ADbSet.Where(i => i.List.Select(q => q.ToString().Count(r => r != '2') > 0).Count() > 0).Count();
                Assert.AreEqual(1, result9);

                var list = new List<int> { 3 };
                var result10 = context.ADbSet.Where(i => list[0] == 3).Select(i => i.C).Count();
                Assert.AreEqual(1, result10);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => i.List.All(item => item.Array.Length > 0)).Count());
            }
        }
    }
}
