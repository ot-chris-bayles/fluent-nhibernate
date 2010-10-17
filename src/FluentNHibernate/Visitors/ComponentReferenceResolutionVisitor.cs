using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Visitors
{
    public class ComponentReferenceResolutionVisitor : DefaultMappingModelVisitor
    {
        private IEnumerable<ExternalComponentMapping> components = new ExternalComponentMapping[] {};

        public override void Visit(MappingModel.MappingBucket bucket)
        {
            components = bucket.Components;

            base.Visit(bucket);
        }

        public override void ProcessComponent(ReferenceComponentMapping mapping)
        {
            var providers = components.Where(x => x.Type == mapping.Type);

            if (!providers.Any())
                throw new MissingExternalComponentException(mapping.Type, mapping.ContainingEntityType, mapping.Member);
            if (providers.Count() > 1)
                throw new AmbiguousComponentReferenceException(mapping.Type, mapping.ContainingEntityType, mapping.Member);

            mapping.AssociateExternalMapping(providers.Single());

            mapping.MergedModel.AcceptVisitor(this);
        }
    }
}
