using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Diagnostics;

namespace FluentNHibernate.Conventions
{
    /// <summary>
    /// Convention finder - used to search through assemblies for types that implement a specific convention interface.
    /// </summary>
    public interface IConventionFinder
    {
        /// <summary>
        /// Find any conventions implementing T.
        /// </summary>
        /// <typeparam name="T">Convention interface type</typeparam>
        /// <returns>IEnumerable of T</returns>
        IEnumerable<T> Find<T>() where T : IConvention;
    }

    /// <summary>
    /// Default convention finder - doesn't do anything special.
    /// </summary>
    public class ConventionFinder : IConventionFinder
    {
        readonly ConventionsCollection conventions;

        public ConventionFinder(ConventionsCollection conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Find any conventions implementing T.
        /// </summary>
        /// <typeparam name="T">Convention interface type</typeparam>
        /// <returns>IEnumerable of T</returns>
        public IEnumerable<T> Find<T>() where T : IConvention
        {
            foreach (var type in conventions.Where(x => typeof(T).IsAssignableFrom(x)))
            {
                foreach (var instance in conventions[type])
                {
                    yield return (T)instance;
                }
            }
        }
    }
}