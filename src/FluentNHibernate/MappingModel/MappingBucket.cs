using System.Collections.Generic;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.MappingModel
{
    public class MappingBucket
    {
        public MappingBucket()
        {
            Classes = new List<ClassMapping>();
            Subclasses = new List<SubclassMapping>();
            Filters = new List<FilterDefinitionMapping>();
            Components = new List<ExternalComponentMapping>();
            Imports = new List<ImportMapping>();
        }

        public List<SubclassMapping> Subclasses { get; private set; }
        public List<ClassMapping> Classes { get; private set; }
        public List<FilterDefinitionMapping> Filters { get; private set; }
        public List<ExternalComponentMapping> Components { get; private set; }
        public List<ImportMapping> Imports { get; private set; }
    }
}