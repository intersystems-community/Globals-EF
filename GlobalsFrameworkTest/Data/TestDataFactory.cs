using System;
using System.Collections.Generic;

namespace GlobalsFrameworkTest.Data
{
    internal class TestDataFactory
    {
        internal static TestA GetTestData()
        {
            return new TestA(0)
            {
                TestBProperty = new TestB(0)
                {
                    Id = 7,
                    Date = DateTime.Now,
                    Array = new[] {new TestC {Id = 3}},
                    Array3 = new[,]
                    {
                        {new TestC(), new TestC {Id = 5}},
                        {new TestC(), new TestC()},
                        {new TestC(), new TestC()}
                    },
                    Array4 = new[]
                    {
                        new TestC[2,3], 
                        new[,]
                        {
                            {new TestC{Id = 3}},
                            {new TestC()},
                            {new TestC{Id = 2}}
                        }, 
                        new TestC[4,2],   
                    },
                    Array5 = new[,,]
                    {
                        {
                            {new TestC(), new TestC()},
                            {new TestC(), new TestC(){Id = 7} }
                        },
                        {
                            {new TestC(), new TestC()},
                            {new TestC(), new TestC(){Id = 5} }
                        }
                    },
                    Array6 = new []
                    {
                        new []
                        {
                            new TestC(), new TestC{Id = 7}
                        },
                        new []
                        {
                            new TestC{Id = 5}
                        }
                    },
                    Array7 = new []{true, false}
                },

                Id = 5,
                Id2 = 7,
                D = 34.67,
                L1 = new List<int> {1, 23, 4, 5},
                C = new TestC {Id = 5},
                Dict = new Dictionary<string, string> {{"qwerty", "12345"}},
                List = new List<TestB>
                {
                    new TestB(0)
                    {
                        Array = new[]
                        {
                            new TestC {Id = 3},
                            new TestC {Id = 7}
                        }
                    },
                    new TestB(0)
                    {
                        Array2 = new[]
                        {
                            new TestC
                            {
                                Id = 8
                            }
                        }
                    }
                }
            };
        }
    }
}
