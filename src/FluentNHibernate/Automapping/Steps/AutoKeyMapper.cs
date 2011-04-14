using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;
using CollectionMapping = FluentNHibernate.MappingModel.Collections.CollectionMapping;

namespace FluentNHibernate.Automapping.Steps
{
    public class AutoKeyMapper
    {
        readonly IAutomappingConfiguration cfg;

        public AutoKeyMapper(IAutomappingConfiguration cfg)
        {
            this.cfg = cfg;
        }

        public void SetKey(Member property, ClassMappingBase classMap, CollectionMapping mapping)
        {
            var columnName = property.DeclaringType.Name + "_id";
            var key = new KeyMapping();

            key.ContainingEntityType = classMap.Type;
            var columnMapping = new ColumnMapping();
            columnMapping.Set(x => x.Name, Layer.Defaults, columnName);
            key.AddDefaultColumn(columnMapping);

            mapping.SetDefaultValue(x => x.Key, key);
        }
    }
}