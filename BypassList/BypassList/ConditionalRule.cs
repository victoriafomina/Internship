using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Describes a conditional rule.
    /// </summary>
    public class ConditionalRule : IRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalRule"/> class.
        /// </summary>
        public ConditionalRule(int toCheck, int toSealIfCheckedContains, int toCrossOutIfCheckedContains, int nextDepartmentIfCheckedContains,
                int toSealIfCheckedDoesNotContain, int toCrossOutIfCheckedDoesNotContain, int nextDepartmentIfCheckedDoesNotContain)
        {
            ToCheck = toCheck;
            ToSealIfCheckedContains = toSealIfCheckedContains;
            ToCrossOutIfCheckedContains = toCrossOutIfCheckedContains;
            ToSealIfCheckedDoesNotContain = toSealIfCheckedDoesNotContain;
            NextDepartmentIfCheckedContains = nextDepartmentIfCheckedContains;
            ToCrossOutIfCheckedDoesNotContain = toCrossOutIfCheckedDoesNotContain;
            NextDepartmentIfCheckedDoesNotContain = nextDepartmentIfCheckedDoesNotContain;
        }

        /// <summary>
        /// Gets the index of the seal which presence in the list is being checked according to the rule.
        /// </summary>
        public int ToCheck { get; }

        /// <summary>
        /// Gets the index of the seal which is being sealed if the checked one is presented.
        /// </summary>
        public int ToSealIfCheckedContains { get; }

        /// <summary>
        /// Gets the index of the seal which is being crossed out if the checked one is presented.
        /// </summary>
        public int ToCrossOutIfCheckedContains { get; }

        /// <summary>
        /// Gets the index of the next department if the checked seal is presented.
        /// </summary>
        public int NextDepartmentIfCheckedContains { get; }

        /// <summary>
        /// Gets the index of the seal which is being sealed if the checked one is presented.
        /// </summary>
        public int ToSealIfCheckedDoesNotContain { get; }

        /// <summary>
        /// Gets the index of the seal which is being crossed out if the checked one is presented.
        /// </summary>
        public int ToCrossOutIfCheckedDoesNotContain { get; }

        /// <summary>
        /// Gets the index of the next department if the checked seal is presented.
        /// </summary>
        public int NextDepartmentIfCheckedDoesNotContain { get; }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(HashSet<int> seals)
        {
            if (seals.Contains(ToCheck))
            {
                seals.Add(ToSealIfCheckedContains);
                seals.Remove(ToCrossOutIfCheckedContains);

                return NextDepartmentIfCheckedContains;
            }

            seals.Add(ToSealIfCheckedDoesNotContain);
            seals.Remove(ToCrossOutIfCheckedDoesNotContain);

            return NextDepartmentIfCheckedDoesNotContain;
        }
    }
}
