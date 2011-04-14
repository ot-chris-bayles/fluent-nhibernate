using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class OneToOneMapping : MappingBase
    {
        private readonly AttributeStore<OneToOneMapping> attributes;

        public OneToOneMapping()
            : this(new AttributeStore())
        {}

        public OneToOneMapping(AttributeStore underlyingStore)
        {
            attributes = new AttributeStore<OneToOneMapping>(underlyingStore);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessOneToOne(this);
        }

        public string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public string Access
        {
            get { return attributes.Get(x => x.Access); }
        }

        public TypeReference Class
        {
            get { return attributes.Get(x => x.Class); }
        }

        public string Cascade
        {
            get { return attributes.Get(x => x.Cascade); }
        }
        public bool Constrained
        {
            get { return attributes.Get(x => x.Constrained); }
        }

        public string Fetch
        {
            get { return attributes.Get(x => x.Fetch); }
        }

        public string ForeignKey
        {
            get { return attributes.Get(x => x.ForeignKey); }
        }

        public string PropertyRef
        {
            get { return attributes.Get(x => x.PropertyRef); }
        }

        public string Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public string EntityName
        {
            get { return attributes.Get(x => x.EntityName); }
        }

        public Type ContainingEntityType { get; set; }

        public bool HasValue<TResult>(Expression<Func<OneToOneMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(OneToOneMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.attributes, attributes) && Equals(other.ContainingEntityType, ContainingEntityType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(OneToOneMapping)) return false;
            return Equals((OneToOneMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((attributes != null ? attributes.GetHashCode() : 0) * 397) ^ (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
            }
        }
    }
}