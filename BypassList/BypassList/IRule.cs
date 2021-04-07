using System.Collections.Generic;

namespace BypassList
{
    /// <summary>
    /// Helps to define rules for the departments.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Gets the seals.
        /// </summary>
        public HashSet<int>? Seals { get; }

        /// <summary>
        /// Seals and returns the index of the next department.
        /// </summary>
        public int Next(HashSet<int> seals);
    }
}
