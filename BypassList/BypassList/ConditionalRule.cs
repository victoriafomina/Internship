using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Describes a conditional rule.
    /// </summary>
    public class ConditionalRule : IRule
    {
        private readonly int toCheck;
        private readonly int toSealIfCheckedContains;
        private readonly int toCrossOutIfCheckedContains;
        private readonly int nextDepartmentIfCheckedContains;
        private readonly int toSealIfCheckedDoesNotContain;
        private readonly int toCrossOutIfCheckedDoesNotContain;
        private readonly int nextDepartmentIfCheckedDoesNotContain;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalRule"/> class.
        /// </summary>
        public ConditionalRule(int toCheck, int toSealIfCheckedContains, int toCrossOutIfCheckedContains, int nextDepartmentIfCheckedContains,
                int toSealIfCheckedDoesNotContain, int toCrossOutIfCheckedDoesNotContain, int nextDepartmentIfCheckedDoesNotContain)
        {
            this.toCheck = toCheck;
            this.toSealIfCheckedContains = toSealIfCheckedContains;
            this.toCrossOutIfCheckedContains = toCrossOutIfCheckedContains;
            this.toSealIfCheckedDoesNotContain = toSealIfCheckedDoesNotContain;
            this.nextDepartmentIfCheckedContains = nextDepartmentIfCheckedContains;
            this.toCrossOutIfCheckedDoesNotContain = toCrossOutIfCheckedDoesNotContain;
            this.nextDepartmentIfCheckedDoesNotContain = nextDepartmentIfCheckedDoesNotContain;
        }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(HashSet<int> seals)
        {
            if (seals.Contains(toCheck))
            {
                seals.Add(toSealIfCheckedContains);
                seals.Remove(toCrossOutIfCheckedContains);

                return nextDepartmentIfCheckedContains;
            }

            seals.Add(toSealIfCheckedDoesNotContain);
            seals.Remove(toCrossOutIfCheckedDoesNotContain);

            return nextDepartmentIfCheckedDoesNotContain;
        }
    }
}
