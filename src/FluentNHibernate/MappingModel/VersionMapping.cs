using System;
using System.Linq.Expressions;
using FluentNHibernate.Utils;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class VersionMapping : ColumnBasedMappingBase
    {
        public VersionMapping()
            : this(new AttributeStore())
        {}

        public VersionMapping(AttributeStore underlyingStore)
            : base(underlyingStore)
        {}

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessVersion(this);

            Columns.Each(visitor.Visit);
        }

        public string Name
        {
            get { return (string)attributes.Get("Name"); }
        }

        public string Access
        {
            get { return (string)attributes.Get("Access"); }
        }

        public TypeReference Type
        {
            get { return (TypeReference)attributes.Get("Type"); }
        }

        public string UnsavedValue
        {
            get { return (string)attributes.Get("UnsavedValue"); }
        }

        public string Generated
        {
            get { return (string)attributes.Get("Generated"); }
        }

        public Type ContainingEntityType { get; set; }

        public bool Equals(VersionMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.ContainingEntityType, ContainingEntityType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as VersionMapping);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                {
                    return (base.GetHashCode() * 397) ^ (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
                }
            }
        }

        public void Set(Expression<Func<VersionMapping, object>> expression, int layer, object value)
        {
            attributes.Set(expression.ToMember().Name, layer, value);
        }
    }
}