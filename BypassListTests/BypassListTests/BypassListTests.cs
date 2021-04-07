using NUnit.Framework;
using System.Collections.Generic;
using BypassList;

namespace BypassListTests
{
    [TestFixture]
    public class BypassListTests
    {
        private static readonly object[] UnconditionalRulesTestCases =
        {
            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                1,
                (false, new List<HashSet<int>>() { new HashSet<int> { 1 } })
            },

            new object[]
            {
                new Dictionary<int, IRule>
                {
                    [1] = new UnconditionalRule(1, 2, 2),
                    [2] = new UnconditionalRule(2, 2, 1)
                },
                2,
                (false, new List<HashSet<int>>() { new HashSet<int> { 1 } })
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
                (true, new List<HashSet<int>>() { new HashSet<int> { 1 } })
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
                (true, new List<HashSet<int>>() { new HashSet<int>() })
            },
        };

        [TestCaseSource("UnconditionalRulesTestCases")]
        public void UnconditionalRulesTest(Dictionary<int, IRule> departments, int departmentIndex, 
                (bool, List<HashSet<int>>?) seales)
        {
            var bypass = BypassList.BypassList.BypassListCreator(departments);
            var sealesResult = bypass.UncrossedSeals(departmentIndex);

            Assert.IsTrue(CheckPairsAreTheSame(seales, sealesResult));

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
    }
}