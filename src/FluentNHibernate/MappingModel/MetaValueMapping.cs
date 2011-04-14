using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class MetaValueMapping : MappingBase
    {
        private readonly AttributeStore<MetaValueMapping> attributes;

        public MetaValueMapping()
            : this(new AttributeStore())
        {}

        protected MetaValueMapping(AttributeStore underlyingStore)
        {
            attributes = new AttributeStore<MetaValueMapping>(underlyingStore);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessMetaValue(this);
        }

        public string Value
        {
            get { return attributes.Get(x => x.Value); }
        }

        public TypeReference Class
        {
            get { return attributes.Get(x => x.Class); }
        }

        public Type ContainingEntityType { get; set; }

        public bool HasValue<TResult>(Expression<Func<MetaValueMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(MetaValueMapping other)
        {
            return Equals(other.attributes, attributes) && Equals(other.ContainingEntityType, ContainingEntityType);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(MetaValueMapping)) return false;
            return Equals((MetaValueMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((attributes != null ? attributes.GetHashCode() : 0) * 397) ^
                    (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
            }
        }

        public void Set<T>(Expression<Func<MetaValueMapping, T>> expression, int layer, T value)
        {
            attributes.Set(expression, layer, value);
        }
    }
}