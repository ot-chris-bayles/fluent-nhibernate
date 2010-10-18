using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Diagnostics;

namespace FluentNHibernate
{
    public class CollectionTypeSource : ITypeSource
    {
        readonly IEnumerable<Type> types;

        public CollectionTypeSource(IEnumerable<Type> types)
        {
            this.types = types.ToArray();
        }

        public IEnumerable<Type> GetTypes()
        {
            return types;
        }

        public void LogSource(IDiagnosticLogger logger)
        {
            logger.LoadedFluentMappingsFromSource(this);
        }

        public string GetIdentifier()
        {
            var names = string.Join(", ", types.OrderBy(x => x.Name).Select(x => x.Name).ToArray());

            return "Collection[" + names + "]";
        }
    }
}