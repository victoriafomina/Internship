using System;

namespace BypassList
{
    /// <summary>
    /// Describes the department.
    /// </summary>
    public class Department
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Department"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <param name="index"/> is less than 1.</exception>
        public Department(int index, IRule rule)
        {
            if (index < 1)
            {
                throw new ArgumentException($"Index can not be less than 1. Current index: {index}");
            }

            Index = index;
            Rule = rule;
        }

        /// <summary>
        /// Gets the index of the department.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the rule of the department.
        /// </summary>
        public IRule Rule { get; private set; }
    }
}
