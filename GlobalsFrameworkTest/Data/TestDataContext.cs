using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GlobalsFramework;
using GlobalsFramework.Attributes;
using GlobalsFramework.Linq;

namespace GlobalsFrameworkTest.Data
{
    class TestDataContext : DataContext
    {
        private static readonly string Namespc;
        private static readonly string User;
        private static readonly string Password;

        static TestDataContext()
        {
            Namespc = ConfigurationManager.AppSettings["Namespc"];
            User = ConfigurationManager.AppSettings["User"];
            Password = ConfigurationManager.AppSettings["Password"];
        }

        public TestDataContext() : base(Namespc, User, Password) { }

        public DbSet<TestA> ADbSet { get; set; }

        public DbSet<TestB> BDbSet;
        public DbSet<TestD> DDbSet { get; set; }
        public DbSet<TestE> EDbSet { get; set; }
        public DbSet<TestF> FDbSet { get; set; }
    }

    #region Test classes

    [DbSet(Name = "TestTable")]
    public class TestA
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        public int Id2 { get; set; }

        public byte Id3 { get; set; }

        [Column(Name = "L")]
        public List<int> L1 { get; set; }

        [Column(Name = "ComplexList")]
        public List<TestB> List { get; set; }

        [Column]
        public TestC? C { get; set; }

        [Column]
        public TestC? C2 { get; set; }

        [Column]
        public Dictionary<string, string> Dict { get; set; }

        [Column]
        public double? D { get; set; }

        [Column]
        public TestB TestBProperty { get; set; }

        [Column]
        public TestEnum E { get; set; }

        public TestA(int i) { }

        public static implicit operator int(TestA a)
        {
            return 0;
        }

        public static implicit operator TestB(TestA a)
        {
            return new TestB(0);
        }
        public static int operator +(TestA a, TestA a2)
        {
            return 0;
        }

        public static int StaticProperty
        {
            get { return 0; }
        }

        public int this[int index]
        {
            get { return index; }
        }

        public Func<int, int> Func = (i) => i*2;
    }

    public class TestA2 : TestA
    {
        public TestA2() : base(0) { }

        public TestA2(Predicate<int> predicate, TestA a) : base(0) { }
    }

    public class TestInit : IEquatable<TestInit>
    {
        public int Id;
        public List<int> List = new List<int>();
        public TestInit2 Member = new TestInit2();
        public TestInit3 StructMember;
        public TestInit RecursiveReference;
        public TestInit NullMember;

        public TestInit()
        {
            RecursiveReference = this;
        }
        public TestInit(int i) : this()
        {
            Id = i;
        }

        public bool Equals(TestInit other)
        {
            if (Id != other.Id)
                return false;
            if (!List.SequenceEqual(other.List))
                return false;

            if (Member.Id != other.Member.Id)
                return false;
            if(!Member.List.SequenceEqual(other.Member.List))
                return false;

            if (StructMember.Id != other.StructMember.Id)
                return false;

            return true;
        }
    }

    public class TestInit2 : IEnumerable
    {
        public List<Tuple<int, int>> List = new List<Tuple<int, int>>();
        public int Id;

        public void Add(int a, int b)
        {
            List.Add(new Tuple<int, int>(a, b));
        }

        public IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }
    }

    public struct TestInit3
    {
        public int Id;
    }

    public class TestB : IEquatable<TestB>
    {
        [Column(Name = "Key", IsPrimaryKey = true, IsDbGenerated = true)]
        public int? Id { get; set; }

        [Column]
        public DateTimeOffset Date { get; set; }

        [Column()]
        public TestC[] Array { get; set; }

        [Column]
        public TestC[] Array2 { get; set; }

        [Column]
        public TestC[,] Array3 { get; set; }

        [Column]
        public TestC[][,] Array4 { get; set; }

        [Column]
        public TestC[,,] Array5 { get; set;}

        [Column]
        public TestC[][] Array6 { get; set; }

        [Column]
        public bool[] Array7 { get; set; }

        public TestB(int i) { }

        public static TestB operator -(TestB value)
        {
            return new TestB(4) {Id = 4};
        }

        public bool Equals(TestB other)
        {
            return true;
        }
    }

    public struct TestC
    {
        [Column]
        public int Id { get; set; }

        public string Value { get; set; }

        public static int operator +(TestC c1, TestC c2)
        {
            return 0;
        }

        public static TestC operator &(TestC c1, TestC c2)
        {
            return new TestC();
        }

        public static TestC operator |(TestC c1, TestC c2)
        {
            return new TestC();
        }

        public static bool operator true(TestC c)
        {
            return true;
        }

        public static bool operator false(TestC c)
        {
            return false;
        }

        public static bool operator >(TestC c1, TestC c2)
        {
            return true;
        }

        public static bool operator < (TestC c1, TestC c2)
        {
            return false;
        }

        public static bool operator !(TestC c1)
        {
            return true;
        }

        public static int operator +(TestC c)
        {
            return 0;
        }

        public static int operator ~(TestC c)
        {
            return 0;
        }

        public bool TestFunct(List<int> list)
        {
            return true;
        }
    }

    public class TestD
    {
        [Column(IsPrimaryKey = true)]
        public TestC? Id { get; set; }

        [Column()]
        public int Value { get; set; }

        public TestD(int i) { }
    }

    public class TestE
    {
        [Column(IsPrimaryKey = true)]
        public List<TestC> Id { get; set; }
    }

    public class TestF
    {
        [Column(IsPrimaryKey = true)]
        public List<TestC> C { get; set; }

        [Column]
        public TestC Value { get; set; }
    }

    public enum TestEnum
    {
        One,
        Two
    }

    public class TestComparer : IEqualityComparer<TestC>
    {
        public bool Equals(TestC x, TestC y)
        {
            return true;
        }

        public int GetHashCode(TestC obj)
        {
            return 0;
        }
    }

    #endregion
}
