using System;
using System.Collections.Generic;

namespace FluentNHibernate.Infrastructure
{
    public class ImportOptions
    {
        readonly IDictionary<Type, string> renames;

        public ImportOptions(IDictionary<Type, string> renames)
        {
            this.renames = renames;
        }

        public void Rename<TImported>(string newName)
        {
            renames[typeof(TImported)] = newName;
        }
    }
}