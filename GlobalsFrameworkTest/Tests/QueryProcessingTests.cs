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
        public void TestSingle()
        {
            using (var context = new TestDataContext())
            {
                var data = context.ADbSet.Where(a => a.L1.Count == 4).Single();
                var data2 = context.ADbSet.Single();
                Assert.AreEqual(data.Id, data2.Id);
            }
        }

        [Test]
        public void TestSingleOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var data = context.ADbSet.Where(a => a.Id == -100).SingleOrDefault();
                Assert.AreEqual(data, null);
            }
        }

        [Test]
        public void TestLast()
        {
            using (var context = new TestDataContext())
            {
                var data = context.BDbSet.Select(b => b.Array).Last();
                Assert.AreEqual(data[0].Id, 3);
            }
        }

        [Test]
        public void TestLastOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var data = context.ADbSet.LastOrDefault(a => a.Id == -100);
                Assert.AreEqual(data, null);
            }
        }

        [Test]
        public void TestElementAt()
        {
            using (var context = new TestDataContext())
            {
                var data1 = context.ADbSet.ElementAt(0);
                var data2 = context.ADbSet.First();

                Assert.AreEqual(data1.Id, data2.Id);
            }
        }

        [Test]
        public void TestElementAtOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var data1 = context.ADbSet.ElementAtOrDefault(-100);

                Assert.AreEqual(data1, null);
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
