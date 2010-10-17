using FluentNHibernate.Infrastructure;
using FluentNHibernate.MappingModel;

namespace FluentNHibernate.Mapping
{
    public interface IFilterDefinition : IProvider
    {
        string Name { get; }
        FilterDefinitionMapping GetFilterMapping();
        HibernateMapping GetHibernateMapping();
    }
}
