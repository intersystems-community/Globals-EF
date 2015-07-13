using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests
{
    [TestFixture]
    public class PredicateExpressionTests
    {
        private readonly TestA _testEntity = TestDataFactory.GetTestData();

        [SetUp]
        public void Init()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();
            }
        }

        [TearDown]
        public void ClearData()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();
            }
        }

        [Test]
        public void TestMemberExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value == 7).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(_testEntity.TestBProperty.Id.Value, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id == 7).Select(i => i).ToList();
                Assert.AreEqual(0, result2.Count);

                var result3 = context.ADbSet.Where(i => i.L1[0].ToString(CultureInfo.InvariantCulture) == "1").Select(i => i.L1[0]).Single();
                Assert.AreEqual(_testEntity.L1[0], result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Value == null).Select(i => i.E).Single();
                Assert.AreEqual(_testEntity.E, result4);

                var result5 = context.ADbSet.Where(i => i.TestBProperty.Id != null).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(_testEntity.TestBProperty.Id.Value, result5);

                var result6 = context.ADbSet.Where(i => i.Id2 >= 0).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(_testEntity.TestBProperty.Id.Value, result6);

                var result7 = context.ADbSet.Where(i => i.List.Count >= 2).Count();
                Assert.AreEqual(1, result7);

                var result8 = context.ADbSet.Where(i => i[10] <= 10).Select(i => i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(1, result8);

                var result9 = context.ADbSet.Where(i => TestA.StaticProperty >= 0).Select(i => i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(1, result9);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result10 = context.ADbSet.Where(i => i.Id2 >= 0).Select(i => i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(0, result10);

            }
        }

        [Test]
        public void TestParameterExpression()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Where(i => i == null).Count();
                Assert.AreEqual(0, result);
            }
        }

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

                var c = new TestB(0) {Array = new[] {new TestC {Id = 5}}};
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
                var result2 = context.ADbSet.Where(i => i.C<testc).Count();
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
                    () => context.ADbSet.Where(i => checked (i.TestBProperty.Id.Value + int.MaxValue) == 9).Count());
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
        public void TestConvertExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => (int)i.D.Value > 30).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.D.Value  < i.TestBProperty.Id).Count();
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
                Assert.DoesNotThrow(() => context.ADbSet.Where(i => unchecked (i.Id3 > (int) tuple2.Item1)).Count());

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
                var result1 = context.ADbSet.Where(i => checked((int) i.D.Value) > 30).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => checked((short)i) == 5).Count();
                Assert.AreEqual(0, result2);

                var tuple = new Tuple<long>(long.MaxValue);

                Assert.Throws<OverflowException>(() => context.ADbSet.Where(i => checked (i.Id3 > (int)tuple.Item1)).Count());
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

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Array[i.TestBProperty.Id.Value-7].Id == 3).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.TestBProperty.Array6[0][1].Id == i.TestBProperty.Id.Value).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array6[1][0].Id == i.TestBProperty.Array6[0][1].Id).Count();
                Assert.AreEqual(0, result4);

                var result5 = context.ADbSet.Where(i => i.TestBProperty.Array7[0]).Count();
                Assert.AreEqual(1, result5);

                var c = new TestB(0) {Array = new[] {new TestC {Id = 5}}};
                var result6 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id == c.Array[0].Id).Count();
                Assert.AreEqual(0, result6);
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

        [Test]
        public void TestCallExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Array.Count(it => it.Id == 3) > 0).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => i.List.Where(it => it!=null).FirstOrDefault(it=>it.Array.Length > 0)!=null).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => i.L1[1] == 23).Count();
                Assert.AreEqual(1, result3);

                var array = new[] {3};

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array.Count(it => it.Id==array[0])>0).Count();
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

                var result9 = context.ADbSet.Where(i => i.List.Select(q=>q.ToString().Count(r=>r!='2')>0).Count()>0).Count();
                Assert.AreEqual(1, result9);

                var list = new List<int> {3};
                var result10 = context.ADbSet.Where(i => list[0] == 3).Select(i => i.C).Count();
                Assert.AreEqual(1, result10);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => i.List.All(item => item.Array.Length > 0)).Count());
            }
        }

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

        [Test]
        public void TestInvokeExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.Func(i.Id) == i.Id*2).Count();
                Assert.AreEqual(1, result1);

                Func<Predicate<int>, int, bool> a = (pred, i) => pred(i);
                var result2 = context.ADbSet.Where(i => a((n) => n + 2 == i.TestBProperty.Id.Value + 2, i.TestBProperty.Id.Value)).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => a((n) => n == 3, i.TestBProperty.Id.Value)).Count();
                Assert.AreEqual(0, result3);
            }
        }

        [Test]
        public void TestNegateExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => unchecked (-i) != 0).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => unchecked (-i.TestBProperty.Id) < 0).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => unchecked(-i.TestBProperty).Id == 4).Count();
                Assert.AreEqual(1, result3);

                var value = UInt16.MaxValue;
                Assert.DoesNotThrow(() => context.ADbSet.Where(i => unchecked ((ushort) (-value)) <= 0).Count());
            }
        }

        [Test]
        public void TestNegateCheckedExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => checked (-i) != 0).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Where(i => checked (-i.TestBProperty.Id) < 0).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => checked(-i.TestBProperty).Id == 4).Count();
                Assert.AreEqual(1, result3);

                var value = UInt16.MaxValue;
                Assert.Throws<OverflowException>(() => context.ADbSet.Where(i => checked ((ushort)(-value)) <= 0).Count());

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

        [Test]
        public void TestNewExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => new TestA(0)!=null).Count();
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
                var result1 = context.ADbSet.Where(i => new[] {2, 3, 4}.Length == 3).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Where(i => new[] {2, 3, 4}[1] == 3).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Where(i => new[] {new int[2, 3]} != null).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => new[] {2, 3, i.Id}[2] == i.Id).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => new Func<int>[] {() => 5, () => 6}[1]() == 6).Count();
                Assert.AreEqual(1, result5);

                context.ADbSet.InsertOnSubmit(new TestA(1) {C = new TestC() {Id = 8}});
                context.SubmitChanges();

                var result6 = context.ADbSet.Where(i => new[] {2, i.C.Value.Id}[1] == 8).Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.Where(i => new[] {2, i.C.Value.Id}[1] == i.C.Value.Id).Count();
                Assert.AreEqual(2, result7);
            }
        }
    }
}
