using System.Collections.Generic;
using System.Threading;

namespace BypassList
{
    /// <summary>
    /// Implements logic describing Vasya's trip with bypass list.
    /// </summary>
    public class BypassList
    {
        private readonly object locker = new ();
        private readonly Dictionary<int, IRule> departments;
        private readonly Dictionary<int, List<HashSet<int>>> departmentsStates;
        private readonly HashSet<int> seals;
        private bool containsLoop;
        private bool bypassIsDone;

        /// <summary>
        /// Initializes a new instance of the <see cref="BypassList"/> class.
        /// Dictionary should include all the departments indexed from 1 to the Dictionary's size.
        /// </summary>
        private BypassList(Dictionary<int, IRule> departments)
        {
            this.departments = departments;
            departmentsStates = new Dictionary<int, List<HashSet<int>>>();
            seals = new HashSet<int>();
        }

        /// <summary>
        /// Creates an instance of the <see cref="BypassList"/> class.
        /// </summary>
        public static BypassList? BypassListCreator(Dictionary<int, IRule> departments)
        {
            if (!CheckDepartmentsIndexesAreCorrect(departments))
            {
                return null;
            }

            return new BypassList(departments);
        }

        /// <summary>
        /// Returns the uncrossed seals after the user leaves the corresponding department.
        /// </summary>
        /// <returns>Returns pair. If the first element of the pair is true then the bypass contains a loop.
        /// The second element is seals if department was visited or null otherwise.</returns>
        public (bool, List<HashSet<int>>?) UncrossedSeals(int departmentToExit)
        {
            if (!Volatile.Read(ref bypassIsDone))
            {
                lock (locker)
                {
                    if (!Volatile.Read(ref bypassIsDone))
                    {
                        RunBypass();
                        Volatile.Write(ref bypassIsDone, true);
                    }
                }
            }

            return (containsLoop, departmentsStates.ContainsKey(departmentToExit)
                ? departmentsStates[departmentToExit]
                : null);
        }

        /// <summary>
        /// Runs the bypass of the departments.
        /// </summary>
        private void RunBypass()
        {
            int departmentIndex = 1;

            while (departmentIndex != departments.Count)
            {
                SetDepartmentState(departmentIndex);

                if (containsLoop == true)
                {
                    return;
                }

                departmentIndex = departments[departmentIndex].Next(seals);
            }

            SetDepartmentState(departmentIndex);
            containsLoop = false;
        }

        private void SetDepartmentState(int departmentIndex)
        {
            var department = departments[departmentIndex];
            department.Next(seals);

            if (departmentsStates.ContainsKey(departmentIndex))
            {
                if (departmentsStates[departmentIndex].Contains(seals))
                {
                    containsLoop = true;
                }
                else
                {
                    departmentsStates[departmentIndex].Add(seals);
                }
            }
            else
            {
                var departmentState = new List<HashSet<int>>
                {
                    seals,
                };

                departmentsStates.Add(departmentIndex, departmentState);
            }
        }

        private static bool CheckDepartmentsIndexesAreCorrect(Dictionary<int, IRule> departments)
        {
            if (departments.Count < 2)
            {
                return false;
            }

            var indexes = departments.Keys;

            foreach (var index in indexes)
            {
                if (index < 1 || index > indexes.Count)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
