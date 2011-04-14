using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel.ClassBased
{
    [Serializable]
    public class ComponentMapping : ComponentMappingBase, IComponentMapping
    {
        public ComponentType ComponentType { get; set; }
        private readonly AttributeStore<ComponentMapping> attributes = new AttributeStore<ComponentMapping>();

        public ComponentMapping(ComponentType componentType)
            : this(componentType, new AttributeStore())
        {}

        public ComponentMapping(ComponentType componentType, AttributeStore store)
            : base(store)
        {
            ComponentType = componentType;
            attributes = new AttributeStore<ComponentMapping>(store);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessComponent(this);

            base.AcceptVisitor(visitor);
        }

        public bool HasColumnPrefix
        {
            get { return !string.IsNullOrEmpty(ColumnPrefix); }
        }

        public string ColumnPrefix { get; set; }

        public override string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public override Type Type
        {
            get { return attributes.Get(x => x.Type); }
        }

        public TypeReference Class
        {
            get { return attributes.Get(x => x.Class); }
        }

        public bool Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public bool HasValue<TResult>(Expression<Func<ComponentMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public override bool HasValue(string property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(ComponentMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                Equals(other.attributes, attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ComponentMapping);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                {
                    return (base.GetHashCode() * 397) ^ (attributes != null ? attributes.GetHashCode() : 0);
                }
            }
        }
    }
}