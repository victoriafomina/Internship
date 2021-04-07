using NUnit.Framework;
using System.Collections.Generic;
using BypassList;
using System.Threading;

namespace BypassListTests
{
    /// <summary>
    /// Class is to test BypassList class.
    /// </summary>
    [TestFixture]
    public class BypassListTests
    {
        /// <summary>
        /// Tests BypassList (one threaded test)..
        /// </summary>
        [TestCaseSource("UnconditionalRulesSimpleTestCases")]
        [TestCaseSource("MixedRulesSimpleTestCases")]
        public void OneThreadedTest(Dictionary<int, IRule> departments, int departmentIndex,
                (bool, List<HashSet<int>>?) seales)
        {
            var bypass = BypassList.BypassList.BypassListCreator(departments);
            var sealesResult = bypass.UncrossedSeals(departmentIndex);

            Assert.IsTrue(CheckPairsAreTheSame(seales, sealesResult));

        }

        /// <summary>
        /// Tests BypassList (multithreaded test).
        /// </summary>
        [TestCaseSource("UnconditionalRulesSimpleTestCases")]
        [TestCaseSource("MixedRulesSimpleTestCases")]
        public void MultithreadedTest(Dictionary<int, IRule> departments, int departmentIndex,
                (bool, List<HashSet<int>>?) seales)
        {
            var threads = new List<Thread>();
            var bypass = BypassList.BypassList.BypassListCreator(departments);

            for (var i = 0; i < 100; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    var sealesResult = bypass.UncrossedSeals(departmentIndex);
                    Assert.IsTrue(CheckPairsAreTheSame(seales, sealesResult));
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private static bool CheckPairsAreTheSame((bool, List<HashSet<int>>?) expected, (bool, List<HashSet<int>>?) actual)
        {
            if (expected.Item1 != actual.Item1 || expected.Item2 == null && actual.Item2 != null || 
                    actual.Item2 == null && expected.Item2 != null)
            {
                return false;
            }

            if (expected.Item2 == null && actual.Item2 == null)
            {
                return true;
            }

            if (expected.Item2.Count != actual.Item2.Count)
            {
                return false;
            }

            foreach (var seales in expected.Item2)
            {
                if (actual.Item2.Find(x => x.SetEquals(seales)) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static readonly object[] UnconditionalRulesSimpleTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                1,
                (false, new List<HashSet<int>> { new HashSet<int> { 1 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                2,
                (false, new List<HashSet<int>> { new HashSet<int> { 1 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1),
                    [3] = new UnconditionalRule(1, 2, 1)
                },
                2,
                (true, new List<HashSet<int>> { new HashSet<int> { 1 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1),
                    [3] = new UnconditionalRule(1, 2, 1)
                },
                3,
                (true, (List<HashSet<int>>?)null)
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 1, 2),
                    [2] = new UnconditionalRule(2, 2, 1),
                    [3] = new UnconditionalRule(1, 2, 1)
                },
                1,
                (true, new List<HashSet<int>> { new HashSet<int>() })
            },
            
        };

        private static readonly object[] MixedRulesSimpleTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new ConditionalRule(3, 1, 2, 2, 4, 7, 3),
                    [2] = new UnconditionalRule(1, 2, 1),
                    [3] = new ConditionalRule(2, 2, 3, 1, 4, 5, 1)
                },
                1,
                (false, new List<HashSet<int>> { new HashSet<int> { 4 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new ConditionalRule(3, 1, 2, 2, 4, 7, 3),
                    [2] = new UnconditionalRule(1, 2, 1),
                    [3] = new ConditionalRule(2, 2, 3, 1, 4, 5, 1)
                },
                2,
                (false, (List<HashSet<int>>?)null)
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new ConditionalRule(3, 1, 2, 2, 4, 7, 3),
                    [2] = new UnconditionalRule(1, 2, 1),
                    [3] = new ConditionalRule(2, 2, 3, 1, 5, 4, 1)
                },
                3,
                (false, new List<HashSet<int>> { new HashSet<int> { 5 } })
            },
        };
    }
}