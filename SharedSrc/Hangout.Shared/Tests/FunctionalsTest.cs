using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class FunctionalsTest
    {
        [Test]
        public void MapTest()
        {
            List<string> origList = new List<string>();
            origList.Add("ONE");
            origList.Add("TWO");
            origList.Add("THREE");
            origList.Add("FOUR");

            IEnumerable<string> res = Functionals.Map<string, string>(delegate(string s) { return s.ToLower(); }, origList);

            List<string> resCopy = new List<string>();

            int i = 0;
            foreach(string str in res)
            {
                Assert.IsTrue(str == origList[i].ToLower());
                resCopy.Add(str);
                ++i;
            }

            Assert.IsTrue(origList.Count == resCopy.Count);
        }

        [Test]
        public void MapImmediateTest()
        {
            List<string> origList = new List<string>();
            origList.Add("ONE");
            origList.Add("TWO");
            origList.Add("THREE");
            origList.Add("FOUR");

            IList<string> res = Functionals.MapImmediate<string, string>(delegate(string s) { return s.ToLower(); }, origList);

            int i = 0;
            foreach (string str in res)
            {
                Assert.IsTrue(str == origList[i].ToLower());
                ++i;
            }

            Assert.IsTrue(origList.Count == res.Count);
        }

        [Test]
        public void FilterTest()
        {
            List<string> origList = new List<string>();
            origList.Add("ONE");
            origList.Add("TWO");
            origList.Add("THREE");
            origList.Add("FOUR");

            IEnumerable<string> res = Functionals.Filter<string>(delegate(string s) { return s.Length > 3; }, origList);
            
            List<string> resCopy = new List<string>();

            foreach (string str in res)
            {
                Assert.IsTrue(str.Length > 3);
                resCopy.Add(str);
            }

            Assert.IsTrue(resCopy.Count == 2);
        }

        [Test]
        public void FilterImmediateTest()
        {
            List<string> origList = new List<string>();
            origList.Add("ONE");
            origList.Add("TWO");
            origList.Add("THREE");
            origList.Add("FOUR");

            IList<string> res = Functionals.FilterImmediate<string>(delegate(string s) { return s.Length > 3; }, origList);

            foreach (string str in res)
            {
                Assert.IsTrue(str.Length > 3);
            }

            Assert.IsTrue(res.Count == 2);
        }

		[Test]
		public void BuildDefaultTest()
		{
			object aListOfInts = Functionals.BuildDefault<List<int>>();
			object anInt = Functionals.BuildDefault<int>();
			object binaryReaderDefault = Functionals.BuildDefault<BinaryReader>(); // Has no default constructor

			Assert.AreEqual(0, ((List<int>)aListOfInts).Count);
			Assert.AreEqual(0, (int)anInt);
			Assert.IsNull(binaryReaderDefault);
		}

        [Test]
        public void ReduceTest()
        {
            // Reduce<T>
            List<string> origList = new List<string>();
            origList.Add("ONE");
            origList.Add("TWO");
            origList.Add("THREE");
            origList.Add("FOUR");

            string res = Functionals.Reduce<string, string>(String.Concat, origList);

            Assert.AreEqual("ONETWOTHREEFOUR", res);

            List<int> intList = new List<int>();
            intList.Add(1);
            intList.Add(2);
            intList.Add(3);
            intList.Add(4);

            int sum = Functionals.Reduce<int, int>(delegate(int x, int y) { return x + y; }, intList, 5);

            Assert.AreEqual(15, sum);

            // Reduce<T, U>
            List<int> lenList = Functionals.Reduce<string, List<int>>(delegate(List<int> lengths, string s) { lengths.Add(s.Length); return lengths; }, origList);

			Assert.AreEqual(lenList.Count, origList.Count);

            for (int i = 0; i < origList.Count; ++i)
            {
                Assert.IsTrue(origList[i].Length == lenList[i]);
            }
        }
    }
}
