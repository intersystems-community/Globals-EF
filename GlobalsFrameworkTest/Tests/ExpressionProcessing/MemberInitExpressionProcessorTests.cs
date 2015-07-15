using System;
using System.Linq;
using GlobalsFrameworkTest.Data;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests.ExpressionProcessing
{
    [TestFixture]
    public class MemberInitExpressionProcessorTests : ExpressionProcessorTests
    {
        [Test]
        public void TestMemberInitExpression()
        {
            using (var context = new TestDataContext())
            {
                var instance = new TestInit
                {
                    Id = 2,
                    List = { 3, 5 },
                    Member = new TestInit2 { Id = 3, List = { new Tuple<int, int>(4, 5) } },
                    StructMember = { Id = 3 }
                };

                var result1 = context.ADbSet.Where(i => new TestInit
                {
                    Id = 2,
                    List = { 3, 5 },
                    Member = new TestInit2 { Id = 3, List = { new Tuple<int, int>(4, 5) } },
                    StructMember = { Id = 3 }
                }.Equals(instance)).Count();

                Assert.AreEqual(1, result1);


                instance = new TestInit { Member = { { 3, 4 }, { 5, 6 } } };
                var result2 = context.ADbSet.Where(i => new TestInit { Member = { { 3, 4 }, { 5, 6 } } }.Equals(instance)).Count();
                Assert.AreEqual(1, result2);

                context.ADbSet.InsertOnSubmit(TestEntity);
                context.SubmitChanges();

                var id = context.ADbSet.Select(i => i.Id).Last();

                var result3 = context.ADbSet
                    .Where(i => new TestInit { Member = { { 3, 4 }, { 5, i.Id } } }.Member.List[1].Item2 == id)
                    .Count();

                Assert.AreEqual(1, result3);

                var result4 = context.ADbSet.Where(i => new TestInit(i.Id) { Id = i.Id, }.Id == id).Count();
                Assert.AreEqual(1, result4);

                var result5 = context.ADbSet.Where(i => new TestInit(i.Id) { Id = -5 }.Id == -5).Count();
                Assert.AreEqual(2, result5);

                var result6 = context.ADbSet.Where(i => new TestInit3 { Id = i.Id }.Id == i.Id).Count();
                Assert.AreEqual(2, result6);

                var result7 = context.ADbSet.Where(i => new TestInit { Member = { Id = i.Id } }.Member.Id == id).Count();
                Assert.AreEqual(1, result7);

                context.ADbSet.DeleteAllOnSubmit(context.ADbSet);
                context.SubmitChanges();

                var result8 = context.ADbSet.Where(i => new TestInit { Id = i.Id }.Id == i.Id).Count();
                Assert.AreEqual(0, result8);

                Assert.Throws<NullReferenceException>(() => context.ADbSet.Where(i => new TestInit { NullMember = { Id = 3 } } != null).Count());
            }
        }
    }
}
