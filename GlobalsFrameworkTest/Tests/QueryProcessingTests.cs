using System;
using System.Collections.Generic;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests
{
    [TestFixture]
    public class QueryProcessingTests
    {
        private readonly TestA _testEntity = TestDataFactory.GetTestData();

        [SetUp]
        public void Init()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(_testEntity);
                context.BDbSet.InsertOnSubmit(_testEntity.TestBProperty);

                context.SubmitChanges();
            }
        }

        [TearDown]
        public void ClearData()
        {
            using (var context = new TestDataContext())
            {
                var aData3 = context.ADbSet.Select(a => a);
                var bData3 = context.BDbSet.Select(b => b);
                var dData3 = context.DDbSet.Select(d => d);
                var eData3 = context.EDbSet.Select(e => e);
                var eData4 = context.FDbSet.Select(f => f);

                context.ADbSet.DeleteAllOnSubmit(aData3);
                context.BDbSet.DeleteAllOnSubmit(bData3);
                context.DDbSet.DeleteAllOnSubmit(dData3);
                context.EDbSet.DeleteAllOnSubmit(eData3);
                context.FDbSet.DeleteAllOnSubmit(eData4);

                context.SubmitChanges();
            }
        }

        [Test]
        public void TestSelect()
        {
            using (var context = new TestDataContext())
            {
                var aData1 = context.ADbSet.Select(a => a.L1.First()).Single();
                Assert.AreEqual(1, aData1);

                var aData2 = context.ADbSet.Select(a => a.C.Value.Id).Single();
                Assert.AreEqual(5, aData2);

                var aData3 = context.ADbSet.Select(a => a.L1.Count).Single();
                Assert.AreEqual(4, aData3);

                var aData4 = context.ADbSet.Select(a => a.List[0].Array[0]).Single();
                Assert.AreEqual(3, aData4.Id);

                var aData5 = context.ADbSet.Select(a => a.List[1].Array2[0]).First();
                Assert.AreEqual(8, aData5.Id);

                var aData6 = context.ADbSet.Select(a => a.C2).Single();
                Assert.AreEqual(null, aData6);

                var aData7 = context.ADbSet.Select(a => a.TestBProperty.Array3).Single();
                Assert.AreEqual(5, aData7[0, 1].Id);

                var aData8 = context.ADbSet.Select(a => a).Select(a => a.TestBProperty).Select(b => b.Array3).Select(ar => ar[0,1]).Single();
                Assert.AreEqual(5, aData8.Id);

                var bData1 = context.BDbSet.Select(b => b.Array[0].Id).Single();
                Assert.AreEqual(3, bData1);

                var bData2 = context.BDbSet.Select(b => b.Array.Length).Single();
                Assert.AreEqual(1, bData2);

                var bData3 = context.BDbSet.Select(b => b.Array4[1][0, 0].Id).Single();
                Assert.AreEqual(3, bData3);

                var bData4 = context.BDbSet.Select(b => b.Array4[0].Length).Single();
                Assert.AreEqual(6, bData4);

                var bData5 = context.BDbSet.Select(b => b.Array4.Length).Single();
                Assert.AreEqual(3, bData5);

                var bData6 = context.BDbSet.Select(b => b.Array5[0, 1, 1].Id).Single();
                Assert.AreEqual(7, bData6);

                var bData7 = context.BDbSet.Select((element, index) => index).ToList();
                Assert.AreEqual(0, bData7[0]);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var bData8 = context.ADbSet.Select(b => 5).ToList();
                Assert.AreEqual(5, bData8[1]);
            }
        }

        [Test]
        public void TestWhere()
        {
            using (var context = new TestDataContext())
            {
                var aData1 = context.ADbSet.Where(a => a.TestBProperty.Array[0].Id == 3).Select(a => a.L1.First()).Single();
                Assert.AreEqual(1, aData1);
            }
        }

        [Test]
        public void TestCount()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Count();
                Assert.AreEqual(1, result);

                var result2 = context.ADbSet.Count(a => a.TestBProperty.Id > 0);
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.OrderBy(a => a.Id).ThenBy(a => a.E).Count(a => a.Id >= 0);
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.OrderBy(a => a.Id).ThenBy(a => a.E).Where(a => a.Id >= 0).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Count(a => a.Id < 0);
                Assert.AreEqual(0, result5);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result6 = context.ADbSet.Count();
                Assert.AreEqual(2, result6);
            }
        }

        [Test]
        public void TestLongCount()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.LongCount();
                Assert.AreEqual(1, result);

                var result2 = context.ADbSet.LongCount(a => a.TestBProperty.Id > 0);
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.OrderBy(a => a.Id).ThenBy(a => a.E).LongCount(a => a.Id >= 0);
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.OrderBy(a => a.Id).ThenBy(a => a.E).Where(a => a.Id >= 0).LongCount();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.LongCount(a => a.Id < 0);
                Assert.AreEqual(0, result5);
            }
        }

        [Test]
        public void TestAny()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Any();
                Assert.AreEqual(true, result);

                var result2 = context.ADbSet.Any(a => a.Id == -100);
                Assert.AreEqual(false, result2);

                var result3 = context.ADbSet.Any(a => a.TestBProperty.Id > 0);
                Assert.AreEqual(true, result3);

                var result4 = context.ADbSet.Any(a => a.Id < 0);
                Assert.AreEqual(false, result4);
            }
        }

        [Test]
        public void TestFirst()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).First();
                var result2 = context.ADbSet.First();
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.First(a => a.E == TestEnum.One);
                Assert.AreEqual(result1.Id, result3.Id);

                Assert.Throws<InvalidOperationException>(() => result1 = context.ADbSet.First(a => a.Id < 0));
            }
        }

        [Test]
        public void TestFirstOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).FirstOrDefault();
                var result2 = context.ADbSet.FirstOrDefault();

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.FirstOrDefault(a => a.E == TestEnum.One);
                Assert.NotNull(result3);
                Assert.AreEqual(result1.Id, result3.Id);

                var result4 = context.ADbSet.FirstOrDefault(a => a.Id < 0);
                Assert.Null(result4);

                var result5 = context.ADbSet.Select(a => a.C.Value).FirstOrDefault(c => c.Id < 0);
                Assert.AreEqual(default(TestC), result5);
            }
        }

        [Test]
        public void TestElementAt()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.ElementAt(0);
                var result2 = context.ADbSet.First();

                Assert.AreEqual(result1.Id, result2.Id);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result3 = context.ADbSet.ElementAt(1);
                var result4 = context.ADbSet.ToList();
                Assert.AreEqual(result4[1].Id, result3.Id);

                Assert.Throws<ArgumentOutOfRangeException>(() => result3 = context.ADbSet.ElementAt(3));

                var result5 = context.ADbSet.Select(a => a.C).ElementAt(1);

                Assert.True(result5.HasValue);
                Assert.AreEqual(5, result5.Value.Id);
            }
        }

        [Test]
        public void TestElementAtOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.ElementAtOrDefault(0);
                var result2 = context.ADbSet.First();

                Assert.NotNull(result1);
                Assert.AreEqual(result1.Id, result2.Id);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result3 = context.ADbSet.ElementAtOrDefault(1);
                var result4 = context.ADbSet.ToList();

                Assert.NotNull(result3);
                Assert.AreEqual(result4[1].Id, result3.Id);

                Assert.Null(context.ADbSet.ElementAtOrDefault(4));

                var result5 = context.ADbSet.Select(i => i.C).ElementAtOrDefault(3);
                Assert.Null(result5);
            }
        }

        [Test]
        public void TestSingle()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).Single();
                var result2 = context.ADbSet.Single();
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.Single(a => a.E == TestEnum.One);
                Assert.AreEqual(result1.Id, result3.Id);

                Assert.Throws<InvalidOperationException>(() => result1 = context.ADbSet.Single(a => a.Id < 0));

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                Assert.Throws<InvalidOperationException>(() => result1 = context.ADbSet.Single(a => a.Id >= 0));
            }
        }

        [Test]
        public void TestSingleOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).SingleOrDefault();
                var result2 = context.ADbSet.SingleOrDefault();

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.SingleOrDefault(a => a.E == TestEnum.One);
                Assert.NotNull(result3);
                Assert.AreEqual(result1.Id, result3.Id);

                var result4 = context.ADbSet.SingleOrDefault(a => a.Id < 0);
                Assert.Null(result4);

                var result5 = context.ADbSet.Select(a => a.C.Value).SingleOrDefault(c => c.Id < 0);
                Assert.AreEqual(default(TestC), result5);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                Assert.Throws<InvalidOperationException>(() => result1 = context.ADbSet.SingleOrDefault(a => a.Id >= 0));
            }
        }

        [Test]
        public void TestLast()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).Last();
                var result2 = context.ADbSet.Last();
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.Last(a => a.E == TestEnum.One);
                Assert.AreEqual(result1.Id, result3.Id);

                Assert.Throws<InvalidOperationException>(() => result1 = context.ADbSet.Last(a => a.Id < 0));

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();
            }
        }

        [Test]
        public void TestLastOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(a => a.E == TestEnum.One).LastOrDefault();
                var result2 = context.ADbSet.LastOrDefault();

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.AreEqual(result1.Id, result2.Id);

                var result3 = context.ADbSet.LastOrDefault(a => a.E == TestEnum.One);
                Assert.NotNull(result3);
                Assert.AreEqual(result1.Id, result3.Id);

                var result4 = context.ADbSet.LastOrDefault(a => a.Id < 0);
                Assert.Null(result4);

                var result5 = context.ADbSet.Select(a => a.C.Value).LastOrDefault(c => c.Id < 0);
                Assert.AreEqual(default(TestC), result5);
            }
        }

        [Test]
        public void TestAll()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.All(a => a.E == TestEnum.One);
                Assert.AreEqual(true, result1);

                var resul2 = context.ADbSet.All(a => a.Id < 0);
                Assert.AreEqual(false, resul2);

                var result3 = context.ADbSet.OrderBy(a => a.Id).All(a => a.Id >= 0);
                Assert.AreEqual(true, result3);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var firstId = context.ADbSet.Select(a => a.Id).First();
                var result4 = context.ADbSet.All(a => a.Id <= firstId);
                Assert.AreEqual(false, result4);

            }
        }

        [Test]
        public void TestAverage()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Average(a => a);
                Assert.AreEqual(0, result1);

                var firstElement = context.ADbSet.Select(a => a).First();
                var result2 = context.ADbSet.Average(a => a.Id);
                Assert.AreEqual(firstElement.Id, result2);

                var result3 = context.ADbSet.Average(a => a.TestBProperty.Id);
                Assert.AreEqual(firstElement.TestBProperty.Id, result3);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result4 = context.ADbSet.Average(a => a.Id);
                var average = ((firstElement.Id*2) + 1)/2.0;
                Assert.AreEqual(average, result4);

                var result5 = context.ADbSet.Average(a => new TestA(0));
                Assert.AreEqual(0, result5);

                var result6 = context.ADbSet.Average(a => 4);
                Assert.AreEqual(4, result6);

                var result7 = context.ADbSet.Select(a => a.TestBProperty.Id).Average();
                var average2 = ((firstElement.TestBProperty.Id * 2) + 1) / 2.0;
                Assert.AreEqual(average2, result7);

                var result8 = context.ADbSet.Select(a => a.Id).Average();
                Assert.AreEqual(average, result8);

                var result9 = context.ADbSet.Select(a => a.Id).OrderBy(id => id).Average();
                Assert.AreEqual(average, result9);

                var result10 = context.ADbSet.OrderBy(a => a.Id).Average(a => a.TestBProperty.Id);
                Assert.AreEqual(average2, result10);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                Assert.Throws<InvalidOperationException>(() => result2 = context.ADbSet.Average(a => a.Id));
            }
        }

        [Test]
        public void TestConvertQueries()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Cast<TestA>().Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Select(a=>a.Id).Cast<int>().Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Select(a => a.Id).OrderBy(i => i).Cast<int>().Count();
                Assert.AreEqual(1, result3);

                Assert.Throws<InvalidCastException>(() => result3 = context.ADbSet.Cast<int>().Count());

                var result4 = context.ADbSet.OfType<TestA>().Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Select(a => a.Id).OfType<int>().Count();
                Assert.AreEqual(1, result5);

                var result6 = context.ADbSet.Select(a => a.Id).OrderBy(i => i).OfType<int>().Count();
                Assert.AreEqual(1, result6);

                var result7 = context.ADbSet.OfType<int>().Count();
                Assert.AreEqual(0, result7);
            }
        }

        [Test]
        public void TestConcat()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Concat(context.ADbSet).Count();
                Assert.AreEqual(2, result1);

                var result2 = context.ADbSet.OrderBy(a => a.Id).Concat(context.ADbSet).Count();
                Assert.AreEqual(2, result2);

                var result3 = context.ADbSet.Select(a => a.Id).Concat(new List<int> {2, 3}).Count();
                Assert.AreEqual(3, result3);

                var result4 = context.ADbSet.Select(a => a.Id).Concat(new List<int>()).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Select(a => a.Id).Concat(context.ADbSet.Select(a => a.Id)).Count();
                Assert.AreEqual(2, result5);

                Assert.Throws<ArgumentNullException>(() => result4 = context.ADbSet.Concat(null).Count());
            }
        }

        [Test]
        public void TestContains()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Contains(new TestA(0));
                Assert.AreEqual(false, result1);

                var result2 = context.ADbSet.Select(a => a.C.Value).OrderBy(a => a.Id).Contains(default(TestC));
                Assert.AreEqual(false, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).OrderBy(c => c.Id).Contains(default(TestC),new TestComparer());
                Assert.AreEqual(true, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value.Id).Contains(5);
                Assert.AreEqual(true, result4);

                var result5 = context.ADbSet.Select(a => a.TestBProperty).Contains(new TestB(0));
                Assert.AreEqual(true, result5);
            }
        }

        [Test]
        public void TestDefaultIfEmpty()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.DefaultIfEmpty(new TestA(0) {Id = -3}).First();
                Assert.GreaterOrEqual(result1.Id, 0);

                var result2 = context.ADbSet.Cast<TestA>().DefaultIfEmpty(new TestA(0)).First();
                Assert.GreaterOrEqual(result2.Id, 0);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result3 = context.ADbSet.DefaultIfEmpty(new TestA(0)).First();
                Assert.AreEqual(0, result3.Id);

                var result4 = context.ADbSet.Cast<TestA>().DefaultIfEmpty(new TestA(0) {Id = -3}).First();
                Assert.AreEqual(-3, result4.Id);

                var result5 = context.ADbSet.Cast<TestA>().DefaultIfEmpty().First();
                Assert.AreEqual(null, result5);

                var result6 = context.ADbSet.DefaultIfEmpty().First();
                Assert.AreEqual(null, result6);

                var result7 = context.ADbSet.Select(a=>a.C.Value).DefaultIfEmpty().First();
                Assert.AreEqual(0, result7.Id);
            }
        }

        [Test]
        public void TestOrderBy()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var ids = context.ADbSet.Select(a => a.Id).ToList();

                var result1 = context.ADbSet.OrderBy(a => a.Id).Select(a => a.Id).ToList();
                Assert.IsTrue(result1.SequenceEqual(ids));

                var result2 = context.ADbSet.OrderByDescending(a => a.Id).Select(a => a.Id).ToList();
                Assert.IsTrue(result2.SequenceEqual(ids.OrderByDescending(i => i)));

                var result3 = context.ADbSet.OrderBy(a => 5).Select(a => a.Id).ToList();
                Assert.IsTrue(result3.SequenceEqual(ids));

                var result4 = context.ADbSet.Join(new List<int> {ids[0], ids[1]}, a => a.Id, b => b, (a, b) => a)
                    .OrderBy(a => 5)
                    .Select(a => a.Id)
                    .ToList();

                Assert.IsTrue(result4.SequenceEqual(ids));

                var result5 = context.ADbSet.Join(new List<int> { ids[0], ids[1] }, a => a.Id, b => b, (a, b) => a)
                    .OrderByDescending(a => a.Id)
                    .Select(a => a.Id)
                    .ToList();

                Assert.IsTrue(result5.SequenceEqual(ids.OrderByDescending(i => i)));

                var result6 = context.ADbSet.OrderBy(a => a.Id).OrderByDescending(a=>a.Id).Select(a => a.Id).ToList();
                Assert.IsTrue(result6.SequenceEqual(ids.OrderByDescending(i => i)));

                var result7 = context.ADbSet.OrderBy(a => a.Id, new TestIntComparer()).Select(a => a.Id).ToList();
                Assert.IsTrue(result7.SequenceEqual(ids.OrderByDescending(i => i)));

                var result8 = context.ADbSet.OrderBy(a => a.TestBProperty).Select(a => a.Id).ToList();
                Assert.IsTrue(result8.SequenceEqual(ids));

                Assert.Throws<InvalidOperationException>(() => result8 = context.ADbSet.OrderBy(a => a).Select(a => a.Id).ToList());
            }
        }

        [Test]
        public void TestThenBy()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                context.ADbSet.InsertOnSubmit(new TestA(0) {Id2 = 0, Id3 = 1, L1 = new List<int>()});
                context.ADbSet.InsertOnSubmit(new TestA(0) {Id2 = 0, Id3 = 1, L1 = new List<int>()});
                context.ADbSet.InsertOnSubmit(new TestA(0) {Id2 = 1, Id3 = 2, L1 = new List<int>()});

                context.SubmitChanges();

                var ids = context.ADbSet.Select(a => a.Id).ToList();

                var result1 = context.ADbSet.OrderBy(a => a.Id2).ThenByDescending(a=>a.Id3).Select(a => a.Id).ToList();
                Assert.IsTrue(result1.SequenceEqual(ids));

                var result2 = context.ADbSet.OrderBy(a => a.Id2).ThenBy(a => a.Id3).ThenByDescending(a=>a.Id).Select(a => a.Id).ToList();
                Assert.IsTrue(result2.SequenceEqual(new List<int> { ids[1], ids[0], ids[2] }));

                var result3 = context.ADbSet.OrderBy(a => a.Id2).ThenByDescending(a => a.Id, new TestIntComparer()).Select(a => a.Id).ToList();
                Assert.IsTrue(result3.SequenceEqual(ids));

                var result4 = context.ADbSet.OrderBy(a => a.Id2).ThenByDescending(a => 5).Select(a => a.Id).ToList();
                Assert.IsTrue(result4.SequenceEqual(ids));

                //order by result is not deferred
                var result5 = context.ADbSet
                    .Select((a,index) => a)
                    .OrderBy(a => a.Id)
                    .ThenBy(a => a.Id2)
                    .ThenByDescending(a => a.Id)
                    .Select(a => a.Id)
                    .ToList();
                Assert.IsTrue(result5.SequenceEqual(ids));

                //default processing of then by, order by is not deferred
                var result6 = context.ADbSet
                    .OrderBy(a => a.L1.FirstOrDefault(i => i == a.Id))
                    .ThenBy(a => a.L1.FirstOrDefault(i => i == a.Id))
                    .Select(a => a.Id)
                    .ToList();
                Assert.IsTrue(result6.SequenceEqual(ids));

                //default processing of then by, order by is deferred
                var result7 = context.ADbSet
                    .OrderBy(a => a.Id2)
                    .ThenByDescending(a=>a.Id)
                    .ThenBy(a => a.L1.FirstOrDefault(i => i == a.Id))
                    .Select(a => a.Id)
                    .ToList();
                Assert.IsTrue(result7.SequenceEqual(new List<int> {ids[1], ids[0], ids[2]}));

                //order by result is processed by default
                var result8 = context.ADbSet
                    .OrderBy(a => a.L1.FirstOrDefault(i => i == a.Id))
                    .ThenBy(a => a.Id2)
                    .ThenByDescending(a => a.Id)
                    .Select(a => a.Id)
                    .ToList();
                Assert.IsTrue(result8.SequenceEqual(new List<int> { ids[1], ids[0], ids[2] }));
            }
        }

        [Test]
        public void TestDistinct()
        {
            using (var context = new TestDataContext())
            {
                context.ADbSet.InsertOnSubmit(new TestA(0) { TestBProperty = _testEntity.TestBProperty, C = new TestC {Id = 9}});
                context.SubmitChanges();

                var result1 = context.ADbSet.Distinct().Count();
                Assert.AreEqual(2, result1);

                var result2 = context.ADbSet.Select(a=>a.TestBProperty).Distinct().Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).Distinct(new TestComparer()).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value).Distinct().Count();
                Assert.AreEqual(2, result4);
            }
        }

        [Test]
        public void TestExcept()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Except(new List<TestA> {new TestA(0)}).Count();
                Assert.AreEqual(1, result1);

                var result2 = context.ADbSet.Select(a => a.TestBProperty).Except(new List<TestB> {new TestB(0)}).Count();
                Assert.AreEqual(0, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).Except(context.ADbSet.Select(a => new TestC())).Count();
                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value).Except(new List<TestC> { new TestC()}, new TestComparer()).Count();
                Assert.AreEqual(0, result4);
            }
        }

        [Test]
        public void TestIntersect()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Intersect(new List<TestA> {new TestA(0)}).Count();
                Assert.AreEqual(0, result1);

                var result2 = context.ADbSet.Select(a => a.TestBProperty).Intersect(new List<TestB> {new TestB(0)}).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).Intersect(context.ADbSet.Select(a => new TestC())).Count();
                Assert.AreEqual(0, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value).Intersect(new List<TestC> {new TestC()}, new TestComparer()).Count();
                Assert.AreEqual(1, result4);
            }
        }

        [Test]
        public void TestUnion()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Union(new List<TestA> {new TestA(0)}).Count();
                Assert.AreEqual(2, result1);

                var result2 = context.ADbSet.Select(a => a.TestBProperty).Union(new List<TestB> {new TestB(0)}).Count();
                Assert.AreEqual(1, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).Union(context.ADbSet.Select(a => new TestC())).Count();
                Assert.AreEqual(2, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value).Union(new List<TestC> {new TestC()}, new TestComparer()).Count();
                Assert.AreEqual(1, result4);
            }
        }

        [Test]
        public void TestSequenceEqual()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.SequenceEqual(new List<TestA> { new TestA(0) });
                Assert.AreEqual(false, result1);

                var result2 = context.ADbSet.Select(a => a.TestBProperty).SequenceEqual(new List<TestB> {new TestB(0)});
                Assert.AreEqual(true, result2);

                var result3 = context.ADbSet.Select(a => a.C.Value).SequenceEqual(context.ADbSet.Select(a => new TestC()));
                Assert.AreEqual(false, result3);

                var result4 = context.ADbSet.Select(a => a.C.Value).SequenceEqual(new List<TestC> {new TestC()}, new TestComparer());
                Assert.AreEqual(true, result4);
            }
        }

        [Test]
        public void TestMax()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Max(a => a);
                Assert.AreEqual(0, result1);

                var firstElement = context.ADbSet.Select(a => a).First();
                var result2 = context.ADbSet.Max(a => a.Id);
                Assert.AreEqual(firstElement.Id, result2);

                var result3 = context.ADbSet.Max(a => a.TestBProperty.Id);
                Assert.AreEqual(firstElement.TestBProperty.Id, result3);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result4 = context.ADbSet.Max(a => a.Id);
                var max = firstElement.Id + 1;
                Assert.AreEqual(max, result4);

                var result5 = context.ADbSet.Max(a => new TestA(0));
                Assert.AreEqual(0, result5);

                var result6 = context.ADbSet.Max(a => 4);
                Assert.AreEqual(4, result6);

                var result7 = context.ADbSet.Select(a => a.TestBProperty.Id).Max();
                var max2 = firstElement.TestBProperty.Id + 1;
                Assert.AreEqual(max2, result7);

                var result8 = context.ADbSet.Select(a => a.Id).Max();
                Assert.AreEqual(max, result8);

                var result9 = context.ADbSet.Select(a => a.Id).OrderBy(id => id).Max();
                Assert.AreEqual(max, result9);

                var result10 = context.ADbSet.OrderBy(a => a.Id).Max(a => a.TestBProperty.Id);
                Assert.AreEqual(max2, result10);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                Assert.Throws<InvalidOperationException>(() => result2 = context.ADbSet.Max(a => a.Id));
            }
        }

        [Test]
        public void TestMin()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Min(a => a);
                Assert.AreEqual(0, result1);

                var firstElement = context.ADbSet.Select(a => a).First();
                var result2 = context.ADbSet.Min(a => a.Id);
                Assert.AreEqual(firstElement.Id, result2);

                var result3 = context.ADbSet.Min(a => a.TestBProperty.Id);
                Assert.AreEqual(firstElement.TestBProperty.Id, result3);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result4 = context.ADbSet.Min(a => a.Id);
                var min = firstElement.Id;
                Assert.AreEqual(min, result4);

                var result5 = context.ADbSet.Min(a => new TestA(0));
                Assert.AreEqual(0, result5);

                var result6 = context.ADbSet.Min(a => 4);
                Assert.AreEqual(4, result6);

                var result7 = context.ADbSet.Select(a => a.TestBProperty.Id).Min();
                var min2 = firstElement.TestBProperty.Id;
                Assert.AreEqual(min2, result7);

                var result8 = context.ADbSet.Select(a => a.Id).Min();
                Assert.AreEqual(min, result8);

                var result9 = context.ADbSet.Select(a => a.Id).OrderBy(id => id).Min();
                Assert.AreEqual(min, result9);

                var result10 = context.ADbSet.OrderBy(a => a.Id).Min(a => a.TestBProperty.Id);
                Assert.AreEqual(min2, result10);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                Assert.Throws<InvalidOperationException>(() => result2 = context.ADbSet.Min(a => a.Id));
            }
        }

        [Test]
        public void TestSum()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Sum(a => a);
                Assert.AreEqual(0, result1);

                var firstElement = context.ADbSet.Select(a => a).First();
                var result2 = context.ADbSet.Sum(a => a.Id);
                Assert.AreEqual(firstElement.Id, result2);

                var result3 = context.ADbSet.Sum(a => a.TestBProperty.Id);
                Assert.AreEqual(firstElement.TestBProperty.Id, result3);

                context.ADbSet.InsertOnSubmit(_testEntity);
                context.SubmitChanges();

                var result4 = context.ADbSet.Sum(a => a.Id);
                var sum = (firstElement.Id*2) + 1;
                Assert.AreEqual(sum, result4);

                var result5 = context.ADbSet.Sum(a => new TestA(0));
                Assert.AreEqual(0, result5);

                var result6 = context.ADbSet.Sum(a => 4);
                Assert.AreEqual(4, result6);

                var result7 = context.ADbSet.Select(a => a.TestBProperty.Id).Sum();
                var sum2 = (firstElement.TestBProperty.Id*2) + 1;
                Assert.AreEqual(sum2, result7);

                var result8 = context.ADbSet.Select(a => a.Id).Sum();
                Assert.AreEqual(sum, result8);

                var result9 = context.ADbSet.Select(a => a.Id).OrderBy(id => id).Sum();
                Assert.AreEqual(sum, result9);

                var result10 = context.ADbSet.OrderBy(a => a.Id).Sum(a => a.TestBProperty.Id);
                Assert.AreEqual(sum2, result10);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result11 = context.ADbSet.Sum(a => a.Id);
                Assert.AreEqual(0, result11);
            }
        }

        [Test]
        public void TestTake()
        {
            using (var context = new TestDataContext())
            {
                var data1 = new List<TestA> { _testEntity }.Concat(context.ADbSet).Take(1).Single();
                var data2 = context.ADbSet.First();

                Assert.AreEqual(data1.Id, data2.Id);
            }
        }

        [Test]
        public void TestSkip()
        {
            using (var context = new TestDataContext())
            {
                var data1 = context.ADbSet.Concat(new List<TestA> { _testEntity }).Skip(1).Single();
                var data2 = context.ADbSet.First();

                Assert.AreEqual(data1.Id, data2.Id);
            }
        }
    }
}
