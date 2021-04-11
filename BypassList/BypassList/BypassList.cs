using System;
using System.Collections.Generic;
using System.Linq;
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
        public static BypassList? BypassListCreator(Dictionary<int, IRule> departments) =>
                !CheckDepartmentsIndexesAreCorrect(departments) ? null : new BypassList(departments);

        /// <summary>
        /// Returns the uncrossed seals after the user leaves the corresponding department.
        /// </summary>
        /// <returns>Returns pair.
        /// If the first element of the pair is true then the bypass contains a loop.
        /// The second element is seals if the department has been visited or null otherwise.
        /// If both of elements are nulls then the department does not exist in the bypass list. </returns>
        public (bool?, List<HashSet<int>>?) UncrossedSeals(int departmentToExit)
        {
            if (departmentToExit > departments.Count)
            {
                return (null, null);
            }

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
            var nextDepartmentIndex = 1;
            int departmentIndex;

            do
            {
                departmentIndex = nextDepartmentIndex;

                nextDepartmentIndex = SetDepartmentState(departmentIndex);

                if (containsLoop)
                {
                    return;
                }
            }
            while (departmentIndex != departments.Count);

            containsLoop = false;
        }

        /// <summary>
        /// Adds the department state to the HashSet if it is new.
        /// </summary>
        private int SetDepartmentState(int departmentIndex)
        {
            var department = departments.GetValueOrDefault(departmentIndex);

            if (department == null)
            {
                throw new NullReferenceException($"Department at the {departmentIndex} position" +
                        "was null!");
            }

            var nextDepartment = department.Next(seals);

            if (departmentsStates.ContainsKey(departmentIndex))
            {
                if (departmentsStates[departmentIndex].Exists(x => x.SetEquals(seals)))
                {
                    containsLoop = true;
                }
                else
                {
                    departmentsStates[departmentIndex].Add(new HashSet<int>(seals));
                }
            }
            else
            {
                var departmentState = new List<HashSet<int>> { new (seals) };

                departmentsStates.Add(departmentIndex, departmentState);
            }

            return nextDepartment;
        }

        /// <summary>
        /// Checks if the departments' indexes are correct.
        /// </summary>
        private static bool CheckDepartmentsIndexesAreCorrect(Dictionary<int, IRule> departments)
        {
            if (departments.Count < 2)
            {
                return false;
            }

            var indexes = departments.Keys;

            return indexes.All(index => index >= 1 && index <= indexes.Count);
        }
    }
}
