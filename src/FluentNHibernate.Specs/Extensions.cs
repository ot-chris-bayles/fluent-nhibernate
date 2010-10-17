using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using Machine.Specifications;
using NHibernate.Cfg;

namespace FluentNHibernate.Specs
{
    public static class Extensions
    {
        public static HibernateMapping BuildMappings(this FluentNHibernate.PersistenceModel model)
        {
            return model.As<IPersistenceInstructionGatherer>()
                .GetInstructions()
                .BuildMappings();
        }

        public static HibernateMapping BuildMappings(this IPersistenceInstructions instructions)
        {
            return new MappingCompiler(null, instructions)
                .Compile();
        }

        public static T As<T>(this object instance)
        {
            return (T)instance;
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            collection.Any(predicate).ShouldBeTrue();
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            collection.Any(predicate).ShouldBeFalse();
        }

        public static ClassMapping BuildMappingFor<T>(this FluentNHibernate.PersistenceModel model)
        {
            return model.BuildMappings()
                .Classes
                .FirstOrDefault(x => x.Type == typeof(T));
        }
    }
}
