using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace FluentNHibernate.MappingModel.ClassBased
{
    /// <summary>
    /// A component that is declared external to a class mapping.
    /// </summary>
    [Serializable]
    public class ExternalComponentMapping : ComponentMapping, ITopMapping
    {
        public ExternalComponentMapping(ComponentType componentType)
            : this(componentType, new AttributeStore())
        {}

        public ExternalComponentMapping(ComponentType componentType, AttributeStore underlyingStore)
            : base(componentType, underlyingStore)
        {}

        public void AddTo(MappingBucket bucket)
        {
            bucket.Components.Add(this);
        }

        public IEnumerable<Member> GetUsedMembers()
        {
            throw new NotImplementedException();
        }

        public void AddMappedMember(IMemberMapping mapping)
        {
            throw new NotImplementedException();
        }
    }
}