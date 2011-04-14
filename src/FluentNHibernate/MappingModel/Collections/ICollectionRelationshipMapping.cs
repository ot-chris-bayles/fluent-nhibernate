using System;

namespace FluentNHibernate.MappingModel.Collections
{
    public interface ICollectionRelationshipMapping : IMappingBase
    {
        Type ChildType { get; }
        TypeReference Class { get; }
        string NotFound { get; }
        string EntityName { get; }
    }
}