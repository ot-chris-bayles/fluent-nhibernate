using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Mapping;
using FluentNHibernate.Utils;
using FluentNHibernate.Utils.Reflection;

namespace FluentNHibernate.Infrastructure
{
    public class SourceScanner
    {
        readonly PersistenceInstructionGatherer gatherer;
        readonly IDiagnosticLogger log;
        readonly List<ITypeSource> sources = new List<ITypeSource>();

        public SourceScanner(PersistenceInstructionGatherer gatherer, IDiagnosticLogger log)
        {
            this.gatherer = gatherer;
            this.log = log;
        }

        public SourceScanner AssemblyContaining<T>()
        {
            return Assembly(typeof(T).Assembly);
        }

        public SourceScanner Assembly(Assembly assembly)
        {
            return Source(new AssemblyTypeSource(assembly));
        }

        public SourceScanner Source(ITypeSource source)
        {
            sources.Add(source);
            return this;
        }

        public SourceScanner TheCallingAssembly()
        {
            return Assembly(ReflectionHelper.FindTheCallingAssembly());
        }

        public SourceScanner Collection(IEnumerable<Type> collection)
        {
            return Source(new CollectionTypeSource(collection));
        }

        public void ForMappings()
        {
            sources.Each(gatherer.AddSource);
        }

        public void ForImporting()
        {
            ForImporting(opt => {});
        }

        public void ForImporting(Action<ImportOptions> opts)
        {
            var renames = new Dictionary<Type, string>();

            opts(new ImportOptions(renames));

            sources
                .SelectMany(x => x.GetTypes())
                .Each(x =>
                {
                    var builder = new ImportPart(x);
                    string newName;

                    if (renames.TryGetValue(x, out newName))
                        builder.As(newName);

                    gatherer.AddProviderInstance(builder);
                });
        }

        public void ForConventions()
        {
            var conventions = new ConventionContainer(gatherer.Conventions, log);

            sources.Each(conventions.AddSource);
        }
    }
}