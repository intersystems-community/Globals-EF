using System.Collections.Generic;
using System.Linq;
using GlobalsFramework.Attributes;
using NUnit.Framework;

namespace GlobalsFramework.Utils.PrimaryKeyCalculation
{
    [TestFixture]
    public class PrimaryKeyCalculatorTest
    {
        private static IEnumerable<TestA> InitializeUniqueTestData()
        {
            return new List<TestA>
            {
                new TestA
                {
                    B = new TestB
                    {
                        A = 3,
                        C = new List<TestC>
                        {
                            new TestC
                            {
                                D = null,
                                S = "qwerty"
                            },
                            new TestC
                            {
                                D = 3.4,
                                S = "qwerty2"
                            }
                        },
                        E = TestE.One
                    },
                    C = new[]
                    {
                        new TestC
                        {
                            D = 4.5,
                            S = "string"
                        },
                        new TestC
                        {
                            D = 2.2,
                            S = "string3"
                        }
                    }
                },

                new TestA
                {
                    B = new TestB
                    {
                        A = 2,
                        C = new List<TestC>
                        {
                            new TestC
                            {
                                D = 4.1,
                                S = "qwerty2"
                            },
                            new TestC
                            {
                                D = 3.42,
                                S = "qwerty3"
                            }
                        },
                        E = TestE.Two
                    },
                    C = new[]
                    {
                        new TestC
                        {
                            D = 1.5,
                            S = "string4"
                        },
                        new TestC
                        {
                            D = 1.2,
                            S = "string4"
                        }
                    }
                },

                new TestA
                {
                    B = new TestB
                    {
                        A = 31,
                        C = new List<TestC>
                        {
                            new TestC
                            {
                                D = null,
                                S = "qwerty2"
                            },
                            new TestC
                            {
                                D = 3.434,
                                S = "qwerty22"
                            }
                        },
                        E = null
                    },
                    C = new[]
                    {
                        new TestC
                        {
                            D = 0.5,
                            S = "string2"
                        },
                        new TestC
                        {
                            D = null,
                            S = "string3"
                        }
                    }
                },

                new TestA
                {
                    B = new TestB
                    {
                        A = 88,
                        C = new List<TestC>
                        {
                            new TestC
                            {
                                D = 88,
                                S = "qwerty8"
                            },
                            new TestC
                            {
                                D = 83.4,
                                S = "qwerty28"
                            }
                        },
                        E = TestE.Three
                    },
                    C = new[]
                    {
                        new TestC
                        {
                            D = 66.5,
                            S = "string"
                        },
                        new TestC
                        {
                            D = 66.2,
                            S = "string36"
                        }
                    }
                }
            };
        }

        private static TestA InitializeTestData()
        {
            return new TestA
            {
                B = new TestB
                {
                    A = 3,
                    C = new List<TestC>
                    {
                        new TestC
                        {
                            D = null,
                            S = "qwerty"
                        },
                        new TestC
                        {
                            D = 3.4,
                            S = "qwerty2"
                        }
                    },
                    E = TestE.One
                },
                C = new[]
                {
                    new TestC
                    {
                        D = 4.5,
                        S = "string"
                    },
                    new TestC
                    {
                        D = 2.2,
                        S = "string3"
                    }
                }
            };
        }

        [Test]
        public void TestPrimaryKey_Unique_NotEqual()
        {
            var data = InitializeUniqueTestData();

            var hashes = data.Select(PrimaryKeyCalculator.GetPrimaryKey);

            var nonUniqueHashes = hashes
                .GroupBy(g => g).Where(g => g.Count() > 1)
                .SelectMany(g => g.Select(i => i))
                .Distinct()
                .ToList();

            Assert.AreEqual(0, nonUniqueHashes.Count);
        }

        [Test]
        public void TestPrimaryKey_NonColumnDifference_Equal()
        {
            var a1 = InitializeTestData();
            var a2 = InitializeTestData();

            a1.B.C = new List<TestC> {new TestC {S = "qwert"}};
            a2.B.C = new List<TestC> {new TestC {S = "qwerty2"}};

            Assert.AreEqual(PrimaryKeyCalculator.GetPrimaryKey(a1), PrimaryKeyCalculator.GetPrimaryKey(a2));
        }

        [Test]
        public void TestPrimaryKey_ListDifference_NotEqual()
        {
            var a1 = InitializeTestData();
            var a2 = InitializeTestData();

            a1.B.C = new List<TestC> {new TestC {D = 5.5}, new TestC {D = null}};
            a2.B.C = new List<TestC> {new TestC {D = 5.5}, new TestC {D = 4.5}};

            Assert.AreNotEqual(PrimaryKeyCalculator.GetPrimaryKey(a1), PrimaryKeyCalculator.GetPrimaryKey(a2));
        }

        [Test]
        public void TestPrimaryKey_ListDifferenceNull_NotEqual()
        {
            var a1 = InitializeTestData();
            var a2 = InitializeTestData();

            a1.B.C = new List<TestC> { new TestC { D = 5.5 }, new TestC { D = null } };
            a2.B.C = null;

            Assert.AreNotEqual(PrimaryKeyCalculator.GetPrimaryKey(a1), PrimaryKeyCalculator.GetPrimaryKey(a2));

            a1 = InitializeTestData();
            a2 = InitializeTestData();

            a1.C = new TestC[0];
            a2.C = null;

            Assert.AreNotEqual(PrimaryKeyCalculator.GetPrimaryKey(a1), PrimaryKeyCalculator.GetPrimaryKey(a2));
        }

        [Test]
        public void TestPrimaryKey_EnumDifference_NotEqual()
        {
            var a1 = InitializeTestData();
            var a2 = InitializeTestData();

            a1.B.E = TestE.One;
            a2.B.E = null;

            Assert.AreNotEqual(PrimaryKeyCalculator.GetPrimaryKey(a1), PrimaryKeyCalculator.GetPrimaryKey(a2));
        }

        [Test]
        public void TestPrimaryKey_NonUnique_Equal()
        {
            var data = new[] {InitializeTestData(), InitializeTestData()};
            var hashes = data.Select(PrimaryKeyCalculator.GetPrimaryKey).ToList();

            Assert.AreEqual(hashes[0], hashes[1]);
        }
    }

    #region Test classes

    internal class TestA
    {
        [Column(IsPrimaryKey = true)]
        public TestB B { get; set; }

        [Column(IsPrimaryKey = true)]
        public TestC[] C { get; set; }
    }

    internal class TestB
    {
        [Column]
        public int A { get; set; }

        [Column]
        public List<TestC> C { get; set; }
 
        [Column]
        public TestE? E { get; set; }

    }

    internal struct TestC
    {
        [Column]
        public double? D { get; set; }

        public string S { get; set; }
    }

    internal enum TestE
    {
        One, Two, Three
    }

    #endregion
}
