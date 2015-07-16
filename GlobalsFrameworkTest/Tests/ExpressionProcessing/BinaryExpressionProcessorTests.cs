using System;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class BinaryExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void EqualExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value == 7).Count();
                Assert.AreEqual(1, result);

                var result1 = context.ADbSet.Where(i => 7 == i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Id.Value == i.C.Value.Id).Count();
                Assert.AreEqual(0, result2);

                var c = new TestB(0) { Array = new[] { new TestC { Id = 5 } } };
                var result3 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id == c.Array[0].Id).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id != i.TestBProperty.Array[1].Id).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => c.Array[0].Id != 4).Count();
                Assert.AreEqual(1, result5);

                var result6 = context.ADbSet.Where(i => c.Array == null).Count();
                Assert.AreEqual(0, result6);

                var result7 = context.ADbSet.Where(i => i.C == null).Count();
                Assert.AreEqual(0, result7);
            }
        }

        [Test]
        public void TestNotEqualExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value != 7).Count();
                Assert.AreEqual(0, result);

                var result2 = context.ADbSet.Where(i => null != i.C).Count();
                Assert.AreEqual(1, result2);
            }
        }

        [Test]
        public void TestLessThanExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value < 7).Count();
                Assert.AreEqual(0, result);

                TestC? testc = null;
                var result2 = context.ADbSet.Where(i => i.C < testc).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestLessThanOrEqualExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value <= 7).Count();
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void TestGreaterThanExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value > 7).Count();
                Assert.AreEqual(0, result);

                TestC? testc = null;
                var result2 = context.ADbSet.Where(i => i.C < testc).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestGreaterThanOrEqualExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i.TestBProperty.Id.Value >= 7).Count();
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void TestAddExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value + 3 == 10).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Id.Value + 3 == 9).Count();
                Assert.AreEqual(0, result2);

                var result3 = context.ADbSet.Where(i => i + 3 == 3).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => i + i == 0).Count();
                Assert.AreEqual(1, result4);

                TestC? testc = null;
                var result5 = context.ADbSet.Where(i => (i.C + testc) != null).Count();
                Assert.AreEqual(0, result5);

                var testc2 = new TestC();
                var result6 = context.ADbSet.Where(i => (i.C + testc2) == 0).Count();
                Assert.AreEqual(1, result6);

                Assert.DoesNotThrow(
                    () => context.ADbSet.Where(i => (i.TestBProperty.Id.Value + int.MaxValue) == 9).Count());
            }
        }

        [Test]
        public void TestAddCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value + 3) == 10).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value + 3) == 9).Count();
                Assert.AreEqual(0, result2);

                Assert.Throws<OverflowException>(
                    () => context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value + int.MaxValue) == 9).Count());
            }
        }

        [Test]
        public void TestSubtractExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value - 3 == 4).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Id.Value - 3 == 3).Count();
                Assert.AreEqual(0, result2);

                Assert.DoesNotThrow(
                    () => context.ADbSet.Where(i => unchecked(int.MinValue - i.TestBProperty.Id.Value) == 9).Count());
            }
        }

        [Test]
        public void TestSubtractCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value - 3) == 4).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value - 3) == 3).Count();
                Assert.AreEqual(0, result2);

                Assert.Throws<OverflowException>(
                    () => context.ADbSet.Where(i => checked(int.MinValue - i.TestBProperty.Id.Value) == 9).Count());
            }
        }

        [Test]
        public void TestMultiplyExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value * 3 == 21).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Id.Value * 3 == 22).Count();
                Assert.AreEqual(0, result2);

                Assert.DoesNotThrow(
                    () => context.ADbSet.Where(i => unchecked(int.MaxValue * i.TestBProperty.Id.Value) == 9).Count());
            }
        }

        [Test]
        public void TestMultiplyCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value * 3) == 21).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => checked(i.TestBProperty.Id.Value * 3) == 22).Count();
                Assert.AreEqual(0, result2);

                Assert.Throws<OverflowException>(
                    () => context.ADbSet.Where(i => checked(int.MaxValue * i.TestBProperty.Id.Value) == 9).Count());
            }
        }

        [Test]
        public void TestDivideExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.D.Value / 2.0 > 16).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.D.Value / 2.0 > 20).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestModuloExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value % 3 == 1).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Id.Value % 2 == 9).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestArrayIndexExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Array6[1][0].Id == 5).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Array[i.TestBProperty.Id.Value - 7].Id == 3).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.TestBProperty.Array6[0][1].Id == i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array6[1][0].Id == i.TestBProperty.Array6[0][1].Id).Count();
                Assert.AreEqual(0, result4);

                var result5 = context.ADbSet.Where(i => i.TestBProperty.Array7[0]).Count();
                Assert.AreEqual(1, result5);

                var c = new TestB(0) { Array = new[] { new TestC { Id = 5 } } };
                var result6 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id == c.Array[0].Id).Count();
                Assert.AreEqual(0, result6);
            }
        }

        [Test]
        public void TestExclusiveOrExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C.Value.Id ^ 2) == 7).Count();
                Assert.AreEqual(1, result1);
            }
        }

        [Test]
        public void TestCoalesceExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.D ?? 2.2) <= 2.2).Count();
                Assert.AreEqual(0, result1);

                TestC? c2 = new TestC();
                var result2 = context.ADbSet.Where(i => (c2 ?? i.C) != null).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => (i.C ?? i.C2) != null).Count();
                Assert.AreEqual(1, result3);
            }
        }

        [Test]
        public void TestLeftShiftExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C.Value.Id << 1) > 0).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.C.Value.Id << 1) < 0).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestRightShiftExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C.Value.Id >> 1) >= 1).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.C.Value.Id >> 1) < 1).Count();
                Assert.AreEqual(0, result2);
            }
        }

        [Test]
        public void TestAndExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C.Value.Id > 0) & (i.D.HasValue)).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.C.Value.Id & 3) == 1).Count();
                Assert.AreEqual(1, result2);

                TestC? testc = null;
                var result5 = context.ADbSet.Where(i => (i.C & testc) != null).Count();
                Assert.AreEqual(0, result5);
            }
        }

        [Test]
        public void TestAndAlsoExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C != null) && (i.TestBProperty.Array6[0].Length > 0)).Count();
                Assert.AreEqual(1, result1);

                TestC? nullableC = null;
                var result2 = context.ADbSet.Where(i => (i.C2 != null) && (nullableC.Value.Id > 0)).Count();
                Assert.AreEqual(0, result2);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => (i.C2 == null) && (nullableC.Value.Id > 0)).Count());

                var testC = new TestC();
                var result3 = context.ADbSet.Where(i => (i.C2.Value && testC).Id == 0).Count();
                Assert.AreEqual(1, result3);
            }
        }

        [Test]
        public void TestOrExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C.Value.Id > 0) | false).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => (i.C.Value.Id | 3) == 7).Count();
                Assert.AreEqual(1, result2);
            }
        }

        [Test]
        public void TestOrElseExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (i.C != null) || (i.TestBProperty.Array6[0].Length > 0)).Count();
                Assert.AreEqual(1, result1);

                TestC? nullableC = null;
                var result2 = context.ADbSet.Where(i => (i.C2 == null) || (nullableC.Value.Id > 0)).Count();
                Assert.AreEqual(1, result2);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => (i.C2 != null) || (nullableC.Value.Id > 0)).Count());

                var testC = new TestC();
                var result3 = context.ADbSet.Where(i => (i.C2.Value || testC).Id == 0).Count();
                Assert.AreEqual(1, result3);
            }
        }
    }
}
