using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Specs.Automapping.Fixtures;
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
                .BuildMappings();
        }

        public static void Add(this FluentNHibernate.PersistenceModel model, Type type)
        {
            model.AddMappingsFromSource(new StubTypeSource(type));
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

        public static void OutputXmlToConsole(this XmlDocument document)
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            document.WriteContentTo(xmlWriter);

            Console.WriteLine(string.Empty);
            Console.WriteLine(stringWriter.ToString());
            Console.WriteLine(string.Empty);
        }
    }
}
