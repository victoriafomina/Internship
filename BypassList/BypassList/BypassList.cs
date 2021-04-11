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
        /// The dictionary should contain more than 2 departments indexed from 1 to dictionary's size.
        /// Seals in the rules could not be less than 1.
        /// The departments should refer to the departments with the correct indexes.
        /// </summary>
        public static BypassList? BypassListCreator(Dictionary<int, IRule> departments) =>
                !CheckDepartmentsAreValid(departments) ? null : new BypassList(departments);

        /// <summary>
        /// Returns the uncrossed seals after the user leaves the corresponding department.
        /// </summary>
        /// <returns>Returns pair.
        /// If the first element of the pair is true then the bypass contains a loop.
        /// The second element is seals if the department has been visited or null otherwise.
        /// If both of elements are nulls then the department does not exist in the bypass list. </returns>
        public (bool?, List<HashSet<int>>?) UncrossedSeals(int departmentToExit)
        {
            if (departmentToExit < 1 || departmentToExit > departments.Count)
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
        /// Runs the departments' bypass.
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

            var nextDepartment = department!.Next(seals);

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
        /// Checks if the departments are valid.
        /// </summary>
        private static bool CheckDepartmentsAreValid(Dictionary<int, IRule> departments)
        {
            if (departments.Count < 2)
            {
                return false;
            }

            var indexes = departments.Keys;

            foreach (var (departmentIndex, rule) in departments)
            {
                switch (rule)
                {
                    case UnconditionalRule unconditionalRule when departmentIndex != indexes.Count &&
                                                                  (unconditionalRule.NextDepartment < 1 || unconditionalRule.NextDepartment > indexes.Count):
                        return false;

                    case UnconditionalRule unconditionalRule when unconditionalRule.ToSeal < 1 || unconditionalRule.ToCrossOut < 1:
                        return false;

                    case ConditionalRule conditionalRule:
                    {
                        var invalidNextDepartment = conditionalRule.NextDepartmentIfCheckedContains < 1 ||
                                                    conditionalRule.NextDepartmentIfCheckedContains > indexes.Count ||
                                                    conditionalRule.NextDepartmentIfCheckedDoesNotContain < 1 ||
                                                    conditionalRule.NextDepartmentIfCheckedDoesNotContain > indexes.Count;

                        var invalidSeal = conditionalRule.ToSealIfCheckedContains < 1 ||
                                          conditionalRule.ToCrossOutIfCheckedDoesNotContain < 1 ||
                                          conditionalRule.ToCrossOutIfCheckedContains < 1 ||
                                          conditionalRule.ToCrossOutIfCheckedDoesNotContain < 1;

                        if (departmentIndex != indexes.Count && invalidNextDepartment)
                        {
                            return false;
                        }

                        if (invalidSeal)
                        {
                            return false;
                        }

                        break;
                    }

                    default:
                        throw new InvalidOperationException("Unexpected IRule implementation!");
                }
            }

            return indexes.All(index => index >= 1 && index <= indexes.Count);
        }
    }
}
