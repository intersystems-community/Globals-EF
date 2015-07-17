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
                var aData2 = context.ADbSet.Select(a => a.C.Value.Id).Single();
                var aData3 = context.ADbSet.Select(a => a.L1.Count).Single();
                var aData4 = context.ADbSet.Select(a => a.List[0].Array[0]).Single();
                var aData5 = context.ADbSet.Select(a => a.List[1].Array2[0]).First();
                var aData6 = context.ADbSet.Select(a => a.C2).Single();
                var aData7 = context.ADbSet.Select(a => a.TestBProperty.Array3).Single();

                var bData1 = context.BDbSet.Select(b => b.Array[0].Id).Single();
                var bData2 = context.BDbSet.Select(b => b.Array.Length).Single();
                var bData3 = context.BDbSet.Select(b => b.Array4[1][0, 0].Id).Single();
                var bData4 = context.BDbSet.Select(b => b.Array4[0].Length).Single();
                var bData5 = context.BDbSet.Select(b => b.Array4.Length).Single();
                var bData6 = context.BDbSet.Select(b => b.Array5[0, 1, 1].Id).Single();

                Assert.AreEqual(aData1, 1);
                Assert.AreEqual(aData2, 5);
                Assert.AreEqual(aData3, 4);
                Assert.AreEqual(aData4.Id, 3);
                Assert.AreEqual(aData5.Id, 8);
                Assert.AreEqual(aData6, null);
                Assert.AreEqual(aData7[0, 1].Id, 5);

                Assert.AreEqual(bData1, 3);
                Assert.AreEqual(bData2, 1);
                Assert.AreEqual(bData3, 3);
                Assert.AreEqual(bData4, 6);
                Assert.AreEqual(bData5, 3);
                Assert.AreEqual(bData6, 7);
            }
        }

        [Test]
        public void TestFirst()
        {
            using (var context = new TestDataContext())
            {
                var data = context.ADbSet.Where(a => a.E == TestEnum.One).First();
                var data2 = context.ADbSet.First();
                Assert.AreEqual(data.Id, data2.Id);
            }
        }

        [Test]
        public void TestFirstOrDefault()
        {
            using (var context = new TestDataContext())
            {
                var data = context.ADbSet.Where(a => a.Id == -100).FirstOrDefault();
                Assert.AreEqual(data, null);
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
        public void TestAny()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Any();
                var result2 = context.ADbSet.Any(a => a.Id == -100);

                Assert.AreEqual(result, true);
                Assert.AreEqual(result2, false);
            }
        }

        [Test]
        public void TestCount()
        {
            using (var context = new TestDataContext())
            {
                var result = context.ADbSet.Count();
                var result2 = context.ADbSet.Count(a => a.TestBProperty.Id > 0);
                var result3 = context.ADbSet.LongCount();

                Assert.AreEqual(result, 1);
                Assert.AreEqual(result2, 1);
                Assert.AreEqual(result3, 1);
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
