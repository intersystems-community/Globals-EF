using GlobalsFramework.Attributes;
using GlobalsFramework.Exceptions;
using GlobalsFramework.Validation;
using NUnit.Framework;

namespace GlobalsFrameworkTest.Tests
{
    [TestFixture]
    internal class EntityValidatorTest
    {
        [Test]
        public void TestWithoutKeys_Throws()
        {
            Assert.Throws<EntityValidationException>(() => EntityValidator.ValidateDefinitionAndThrow(typeof (TestA)));
        }

        [Test]
        public void TestIdentityColumnTypes_Throws()
        {
            Assert.Throws<EntityValidationException>(() => EntityValidator.ValidateDefinitionAndThrow(typeof (TestB)));
        }

        [Test]
        public void TestIdentityAndCustomKeys_Throws()
        {
            Assert.Throws<EntityValidationException>(() => EntityValidator.ValidateDefinitionAndThrow(typeof (TestC)));
        }

        [Test]
        public void TestSingleIdentityKey_Throws()
        {
            Assert.Throws<EntityValidationException>(() => EntityValidator.ValidateDefinitionAndThrow(typeof (TestD)));
        }

        [Test]
        public void TestInternalClass_Throws()
        {
            Assert.Throws<EntityValidationException>(() => EntityValidator.ValidateDefinitionAndThrow(typeof(TestE)));
        }

        [Test]
        public void TestInternalClass_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EntityValidator.ValidateDefinitionAndThrow(typeof(Data.TestA)));
        }
        
        private class TestA
        {
            public int Id { get; set; }
        }

        private class TestB
        {
            [Column(IsPrimaryKey = true,IsDbGenerated = true)]
            public string Id { get; set; }
        }

        private class TestC
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column(IsPrimaryKey = true)]
            public string Id2 { get; set; }
        }

        private class TestD
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id2 { get; set; }
        }

        private class TestE
        {
            [Column(IsPrimaryKey = true)]
            public int Id { get; set; }

            [Column(IsPrimaryKey = true)]
            public string Id2 { get; set; }
        }
    }
}
