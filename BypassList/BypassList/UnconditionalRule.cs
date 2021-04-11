using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Describes an unconditional rule.
    /// </summary>
    public class UnconditionalRule : IRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnconditionalRule"/> class.
        /// </summary>
        public UnconditionalRule(int toSeal, int toCrossOut, int nextDepartment)
        {
            ToSeal = toSeal;
            ToCrossOut = toCrossOut;
            NextDepartment = nextDepartment;
        }

        /// <summary>
        /// Gets the index of the seal that is being sealed by the rule.
        /// </summary>
        public int ToSeal { get; }

        /// <summary>
        /// Gets the index of the seal that is being crossed out by the rule.
        /// </summary>
        public int ToCrossOut { get; }

        /// <summary>
        /// Gets the index of the next department.
        /// </summary>
        public int NextDepartment { get; }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(HashSet<int> seals)
        {
            seals.Add(ToSeal);
            seals.Remove(ToCrossOut);

            return NextDepartment;
        }
    }
}
