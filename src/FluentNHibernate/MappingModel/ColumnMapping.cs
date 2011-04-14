using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class ColumnMapping : MappingBase
    {
        private readonly AttributeStore<ColumnMapping> attributes;

        public ColumnMapping()
            : this(new AttributeStore())
        {}

        public ColumnMapping(string defaultColumnName)
            : this()
        {
            Set(x => x.Name, Layer.Defaults, defaultColumnName);
        }

        public ColumnMapping(AttributeStore underlyingStore)
        {
            attributes = new AttributeStore<ColumnMapping>(underlyingStore);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessColumn(this);
        }

        public Member Member { get; set; }

        public string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public int Length
        {
            get { return attributes.Get(x => x.Length); }
        }

        public bool NotNull
        {
            get { return attributes.Get(x => x.NotNull); }
        }

        public bool Unique
        {
            get { return attributes.Get(x => x.Unique); }
        }

        public string UniqueKey
        {
            get { return attributes.Get(x => x.UniqueKey); }
        }

        public string SqlType
        {
            get { return attributes.Get(x => x.SqlType); }
        }

        public string Index
        {
            get { return attributes.Get(x => x.Index); }
        }

        public string Check
        {
            get { return attributes.Get(x => x.Check); }
        }

        public int Precision
        {
            get { return attributes.Get(x => x.Precision); }
        }

        public int Scale
        {
            get { return attributes.Get(x => x.Scale); }
        }

        public string Default
        {
            get { return attributes.Get(x => x.Default); }
        }

        public bool HasValue<TResult>(Expression<Func<ColumnMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public ColumnMapping Clone()
        {
            return new ColumnMapping(attributes.CloneInner());
        }

        public bool Equals(ColumnMapping other)
        {
            return Equals(other.attributes, attributes) && Equals(other.Member, Member);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ColumnMapping)) return false;
            return Equals((ColumnMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((attributes != null ? attributes.GetHashCode() : 0) * 397) ^ (Member != null ? Member.GetHashCode() : 0);
            }
        }

        public void Set<T>(Expression<Func<ColumnMapping, T>> expression, int layer, T value)
        {
            attributes.Set(expression, layer, value);
        }
    }
}