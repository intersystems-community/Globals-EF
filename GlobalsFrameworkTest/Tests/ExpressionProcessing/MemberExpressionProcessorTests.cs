using System.Globalization;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class MemberExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestMemberExpression()
        {
            using (var context = new TestDataContext())
            {
                var result1 = context.ADbSet.Where(i => i.TestBProperty.Id.Value == 7).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(TestEntity.TestBProperty.Id.Value, result1);

                var result2 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Id == 7).Select(i => i).ToList();
                Assert.AreEqual(0, result2.Count);

                var result3 = context.ADbSet.Where(i => i.L1[0].ToString(CultureInfo.InvariantCulture) == "1").Select(i => i.L1[0]).Single();
                Assert.AreEqual(TestEntity.L1[0], result3);

                var result4 = context.ADbSet.Where(i => i.TestBProperty.Array[0].Value == null).Select(i => i.E).Single();
                Assert.AreEqual(TestEntity.E, result4);

                var result5 = context.ADbSet.Where(i => i.TestBProperty.Id != null).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(TestEntity.TestBProperty.Id.Value, result5);

                var result6 = context.ADbSet.Where(i => i.Id2 >= 0).Select(i => i.TestBProperty.Id.Value).Single();
                Assert.AreEqual(TestEntity.TestBProperty.Id.Value, result6);

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
    }
}
