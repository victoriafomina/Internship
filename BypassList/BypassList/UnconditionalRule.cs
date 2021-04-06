using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Describes an unconditional rule.
    /// </summary>
    public class UnconditionalRule : IRule
    {
        private readonly int sealToSeal;
        private readonly int sealToCrossOut;
        private readonly int nextDepartment;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnconditionalRule"/> class.
        /// </summary>
        public UnconditionalRule(int sealToSeal, int sealToCrossOut, int nextDepartment)
        {
            this.sealToSeal = sealToSeal;
            this.sealToCrossOut = sealToCrossOut;
            this.nextDepartment = nextDepartment;
        }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(ref HashSet<int> seals)
        {
            seals.Add(sealToSeal);
            seals.Remove(sealToCrossOut);

            return nextDepartment;
        }
    }
}
