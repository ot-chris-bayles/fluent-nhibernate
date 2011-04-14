using System;
using System.Linq.Expressions;
using FluentNHibernate.Utils;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class PropertyMapping : ColumnBasedMappingBase
    {
        public PropertyMapping()
            : this(new AttributeStore())
        {}

        public PropertyMapping(AttributeStore underlyingStore)
            : base(underlyingStore)
        {}

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessProperty(this);

            foreach (var column in Columns)
                visitor.Visit(column);
        }

        public Type ContainingEntityType { get; set; }

        public string Name
        {
            get { return (string)attributes.Get("Name"); }
        }

        public string Access
        {
            get { return (string)attributes.Get("Access"); }
        }

        public bool Insert
        {
            get { return (bool)attributes.Get("Insert"); }
        }

        public bool Update
        {
            get { return (bool)attributes.Get("Update"); }
        }

        public string Formula
        {
            get { return (string)attributes.Get("Formula"); }
        }

        public bool Lazy
        {
            get { return (bool)attributes.Get("Lazy"); }
        }

        public bool OptimisticLock
        {
            get { return (bool)attributes.Get("OptimisticLock"); }
        }

        public string Generated
        {
            get { return (string)attributes.Get("Generated"); }
        }

        public TypeReference Type
        {
            get { return (TypeReference)attributes.Get("Type"); }
        }

        public Member Member { get; set; }

        public bool Equals(PropertyMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                Equals(other.ContainingEntityType, ContainingEntityType) &&
                Equals(other.Member, Member);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(PropertyMapping)) return false;
            return Equals((PropertyMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0) * 397) ^ (Member != null ? Member.GetHashCode() : 0);
            }
        }

        public void Set(Expression<Func<PropertyMapping, object>> expression, int layer, object value)
        {
            attributes.Set(expression.ToMember().Name, layer, value);
        }
    }
}