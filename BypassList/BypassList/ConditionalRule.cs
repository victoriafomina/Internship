using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Describes a conditional rule.
    /// </summary>
    public class ConditionalRule : IRule
    {
        private readonly int sealToCheck;
        private readonly int sealToSealIfCheckedContains;
        private readonly int sealToCrossOutIfCheckedContains;
        private readonly int nextDepartmentIfCheckedContains;
        private readonly int sealToSealIfCheckedDoesNotContain;
        private readonly int sealToCrossOutIfCheckedDoesNotContain;
        private readonly int nextDepartmentIfCheckedDoesNotContain;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalRule"/> class.
        /// </summary>
        public ConditionalRule(int sealToCheck, int sealToSealIfCheckedContains, int sealToCrossOutIfCheckedContains, int nextDepartmentIfCheckedContains,
                int sealToSealIfCheckedDoesNotContain, int sealToCrossOutIfCheckedDoesNotContain, int nextDepartmentIfCheckedDoesNotContain)
        {
            this.sealToCheck = sealToCheck;
            this.sealToSealIfCheckedContains = sealToSealIfCheckedContains;
            this.sealToCrossOutIfCheckedContains = sealToCrossOutIfCheckedContains;
            this.sealToSealIfCheckedDoesNotContain = sealToSealIfCheckedDoesNotContain;
            this.nextDepartmentIfCheckedContains = nextDepartmentIfCheckedContains;
            this.sealToCrossOutIfCheckedDoesNotContain = sealToCrossOutIfCheckedDoesNotContain;
            this.nextDepartmentIfCheckedDoesNotContain = nextDepartmentIfCheckedDoesNotContain;
        }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(HashSet<int> seals)
        {
            if (seals.Contains(sealToCheck))
            {
                seals.Add(sealToSealIfCheckedContains);
                seals.Remove(sealToCrossOutIfCheckedContains);

                return nextDepartmentIfCheckedContains;
            }
            else
            {
                seals.Add(sealToSealIfCheckedDoesNotContain);
                seals.Remove(sealToCrossOutIfCheckedDoesNotContain);

                return nextDepartmentIfCheckedDoesNotContain;
            }
        }
    }
}
