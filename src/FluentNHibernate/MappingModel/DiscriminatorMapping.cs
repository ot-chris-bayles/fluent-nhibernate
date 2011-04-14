using System;
using System.Linq.Expressions;
using FluentNHibernate.Utils;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class DiscriminatorMapping : ColumnBasedMappingBase
    {
        public DiscriminatorMapping()
            : this(new AttributeStore())
        {}

        public DiscriminatorMapping(AttributeStore underlyingStore)
            : base(underlyingStore)
        {}

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessDiscriminator(this);

            Columns.Each(visitor.Visit);
        }

        public bool Force
        {
            get { return (bool)attributes.Get("Force"); }
        }

        public bool Insert
        {
            get { return (bool)attributes.Get("Insert"); }
        }

        public string Formula
        {
            get { return (string)attributes.Get("Formula"); }
        }

        public TypeReference Type
        {
            get { return (TypeReference)attributes.Get("Type"); }
        }

        public Type ContainingEntityType { get; set; }

        public bool Equals(DiscriminatorMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ContainingEntityType, ContainingEntityType) &&
                other.Columns.ContentEquals(Columns) &&
                Equals(other.attributes, attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(DiscriminatorMapping)) return false;
            return Equals((DiscriminatorMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0) * 397) ^ ((Columns != null ? Columns.GetHashCode() : 0) * 397) ^ (attributes != null ? attributes.GetHashCode() : 0);
            }
        }

        public void Set(Expression<Func<DiscriminatorMapping, object>> expression, int layer, object value)
        {
            attributes.Set(expression.ToMember().Name, layer, value);
        }
    }
}
