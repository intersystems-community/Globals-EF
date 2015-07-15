using System;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class UnaryExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestConvertExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (int)i.D.Value > 30).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.D.Value < i.TestBProperty.Id).Count();
                Assert.AreEqual(0, result2);

                var tuple = new Tuple<int>(7);

                var result3 = context.ADbSet.Where(i => i.D.Value < tuple.Item1).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => i.E == TestEnum.One).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => i == 5).Count();
                Assert.AreEqual(0, result5);

                var result6 = context.ADbSet.Where(i => (TestB)i == i.TestBProperty).Count();
                Assert.AreEqual(0, result6);

                var tuple2 = new Tuple<long>(long.MaxValue);
                Assert.DoesNotThrow(() => context.ADbSet.Where(i => unchecked(i.Id3 > (int)tuple2.Item1)).Count());

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result7 = context.ADbSet.Where(i => i.D.Value > tuple.Item1).Count();
                Assert.AreEqual(0, result7);
            }
        }

        [Test]
        public void TestConvertCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked((int)i.D.Value) > 30).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => checked((short)i) == 5).Count();
                Assert.AreEqual(0, result2);

                var tuple = new Tuple<long>(long.MaxValue);

                Assert.Throws<OverflowException>(() => context.ADbSet.Where(i => checked(i.Id3 > (int)tuple.Item1)).Count());
            }
        }

        [Test]
        public void TestArrayLengthExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Array6.Length == 2).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Array6[0].Length == 2).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.TestBProperty.Array5.Length == 8).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array.Length == 2).Count();
                Assert.AreEqual(0, result4);

                var c = new TestB(0) { Array = new[] { new TestC { Id = 5 } } };
                var result5 = context.ADbSet.Where(i => c.Array.Length == 1).Count();
                Assert.AreEqual(1, result5);

                var result6 = context.ADbSet.Where(i => c.Array.LongLength == 1).Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.Where(i => i.TestBProperty.Array.LongLength == 2).Count();
                Assert.AreEqual(0, result7);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => i.List[1].Array.Length > 0).Count());
            }
        }

        [Test]
        public void TestNegateExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => unchecked(-i) != 0).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => unchecked(-i.TestBProperty.Id) < 0).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => unchecked(-i.TestBProperty).Id == 4).Count();
                Assert.AreEqual(1, result3);

                var value = UInt16.MaxValue;
                Assert.DoesNotThrow(() => context.ADbSet.Where(i => unchecked((ushort)(-value)) <= 0).Count());
            }
        }

        [Test]
        public void TestNegateCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked(-i) != 0).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => checked(-i.TestBProperty.Id) < 0).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => checked(-i.TestBProperty).Id == 4).Count();
                Assert.AreEqual(1, result3);

                var value = UInt16.MaxValue;
                Assert.Throws<OverflowException>(() => context.ADbSet.Where(i => checked((ushort)(-value)) <= 0).Count());

            }
        }

        [Test]
        public void TestTypeAsExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i as TestA).TestBProperty.Id >= 0).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.TestBProperty.Id.Value as int?) == i.TestBProperty.Id).Count();
                Assert.AreEqual(1, result2);

                object testC = new TestC();
                var result3 = context.ADbSet.Where(i => (testC as TestA) != null).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => (testC as TestC?) != null).Count();
                Assert.AreEqual(1, result4);
            }
        }

        [Test]
        public void TestNotExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => !i.C.Value).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => !(i.TestBProperty.Id >= 0)).Count();
                Assert.AreEqual(0, result2);

                var b = false;
                var result3 = context.ADbSet.Where(i => !b).Count();
                Assert.AreEqual(1, result3);
            }
        }

        [Test]
        public void TestUnaryPlusExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (+i.C.Value) >= 0).Count();
                Assert.AreEqual(1, result1);
            }
        }
    }
}
