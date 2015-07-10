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
        public void TestNotKeys()
        {
            Assert.Throws(typeof (EntityValidationException),
                () => EntityValidator.ValidateDefinitionAndThrow(typeof (TestA)));
        }

        [Test]
        public void TestIdentityColumnTypes()
        {
            Assert.Throws(typeof(EntityValidationException),
                () => EntityValidator.ValidateDefinitionAndThrow(typeof(TestB)));
        }

        [Test]
        public void TestIdentityAndCustomKeys()
        {
            Assert.Throws(typeof(EntityValidationException),
                () => EntityValidator.ValidateDefinitionAndThrow(typeof(TestC)));
        }

        [Test]
        public void TestSingleIdentityKey()
        {
            Assert.Throws(typeof(EntityValidationException),
                () => EntityValidator.ValidateDefinitionAndThrow(typeof(TestD)));
        }

        [Test]
        public void TestInternalClass()
        {
            Assert.Throws(typeof(EntityValidationException),
                () => EntityValidator.ValidateDefinitionAndThrow(typeof(TestE)));
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
