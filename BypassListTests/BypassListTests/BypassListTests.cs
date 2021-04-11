using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int threadsCount = 100;

        /// <summary>
        /// Tests for the BypassList (one threaded tests).
        /// </summary>
        [TestCaseSource(nameof(UnconditionalRuleSimpleTestCases))]
        [TestCaseSource(nameof(UnconditionalRuleComplexTestCases))]
        [TestCaseSource(nameof(MixedRulesSimpleTestCases))]
        public void ValidFormatOneThreadedTest(Dictionary<int, IRule> departments, int departmentIndex,
                (bool, List<HashSet<int>>?) seals)
        {
            var bypass = BypassList.BypassList.BypassListCreator(departments);
            Assert.NotNull(bypass);

            var sealsResult = bypass.UncrossedSeals(departmentIndex);

            Assert.IsTrue(CheckPairsAreTheSame(seals, sealsResult));
        }

        /// <summary>
        /// Tests in which the bypass list has not been created (one threaded tests).
        /// </summary>
        [TestCaseSource(nameof(InvalidFormatBypassListNotCreatedTestCases))]
        public void InvalidFormatBypassNotCreatedOneThreadedTest(Dictionary<int, IRule> departments)
        {
            var bypass = BypassList.BypassList.BypassListCreator(departments);
            Assert.IsNull(bypass);
        }

        /// <summary>
        /// Tests for the BypassList (multithreaded tests).
        /// </summary>
        [TestCaseSource(nameof(UnconditionalRuleSimpleTestCases))]
        [TestCaseSource(nameof(UnconditionalRuleComplexTestCases))]
        [TestCaseSource(nameof(MixedRulesSimpleTestCases))]
        public void ValidFormatMultithreadedTest(Dictionary<int, IRule> departments, int departmentIndex,
                (bool, List<HashSet<int>>?) seals)
        {
            var threads = new List<Thread>();
            var bypass = BypassList.BypassList.BypassListCreator(departments);
            Assert.NotNull(bypass);

            for (var i = 0; i < threadsCount; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    var sealsResult = bypass.UncrossedSeals(departmentIndex);
                    Assert.IsTrue(CheckPairsAreTheSame(seals, sealsResult));
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

        private static bool CheckPairsAreTheSame((bool?, List<HashSet<int>>?) expected, (bool?, List<HashSet<int>>?) actual)
        {
            var (containsLoopExp, sealsExp) = expected;
            var (containsLoopAct, sealsAct) = actual;

            if (containsLoopExp != containsLoopAct || sealsExp == null && sealsAct != null || 
                sealsAct == null && sealsExp != null)
            {
                return false;
            }

            if (sealsExp == null)
            {
                return true;
            }

            if (sealsExp.Count != sealsAct.Count)
            {
                return false;
            }

            return sealsExp.All(seals => sealsAct.Find(x => x.SetEquals(seals)) != null);
        }

        /// <summary>
        /// Test cases for the invalid format when the bypass list has not been created.
        /// </summary>
        private static readonly object[] InvalidFormatBypassListNotCreatedTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [3] = new UnconditionalRule(2, 2, 1)
                }
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [0] = new ConditionalRule(1, 2, 3, 2, 2, 1, 1),
                    [1] = new UnconditionalRule(2, 2, 1)
                }
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [-1] = new ConditionalRule(1, 2, 3, 2, 2, 1, 1),
                    [1] = new UnconditionalRule(2, 2, 1),
                    [0] = new UnconditionalRule(2, 2, 1)
                }
            }
        };

        /// <summary>
        /// Simple test cases for the unconditional rule.
        /// </summary>
        private static readonly object[] UnconditionalRuleSimpleTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                1,
                (false, new List<HashSet<int>> { new() { 1 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                2,
                (false, new List<HashSet<int>> { new() { 1 } })
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
                (true, new List<HashSet<int>> { new() { 1 } })
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
                (true, new List<HashSet<int>> { new () })
            }
            
        };

        /// <summary>
        /// Complex test cases for the unconditional rule.
        /// </summary>
        private static readonly object[] UnconditionalRuleComplexTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 5),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                4,
                (false, new List<HashSet<int>> { new() { 2, 4, 6 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 5),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                5,
                (false, new List<HashSet<int>> { new () { 1, 4, 6} })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 1),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                1,
                (true, new List<HashSet<int>> { new () {1}, new () {1, 4, 6} })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 1),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                2,
                (true, new List<HashSet<int>> { new () {6, 2, 1, 4}, new () {1, 4, 2} })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 1),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                4,
                (true, new List<HashSet<int>> { new () {6, 2, 4} })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 3),
                    [2] = new UnconditionalRule(4, 5, 4),
                    [3] = new UnconditionalRule(2, 3, 2),
                    [4] = new UnconditionalRule(6, 1, 1),
                    [5] = new UnconditionalRule(1, 2, 1)
                },
                5,
                (true, (List<HashSet<int>>?)null)
            }
        };

        /// <summary>
        /// Simple test cases for the mixed rules.
        /// </summary>
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
                (false, new List<HashSet<int>> { new() { 4 } })
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
                (false, new List<HashSet<int>> { new() { 5 } })
            }
        };

        /// <summary>
        /// Complex test cases for the mixed rules.
        /// </summary>
        private static readonly object[] MixedRulesComplexTestCases =
        {

        }
    }
}