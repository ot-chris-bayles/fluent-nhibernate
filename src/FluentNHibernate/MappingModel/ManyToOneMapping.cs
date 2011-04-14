using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class ManyToOneMapping : MappingBase, IHasColumnMappings, IRelationship
    {
        private readonly AttributeStore<ManyToOneMapping> attributes;
        private readonly IDefaultableList<ColumnMapping> columns = new DefaultableList<ColumnMapping>();

        public ManyToOneMapping()
            : this(new AttributeStore())
        {}

        public ManyToOneMapping(AttributeStore underlyingStore)
        {
            attributes = new AttributeStore<ManyToOneMapping>(underlyingStore);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessManyToOne(this);

            foreach (var column in columns)
                visitor.Visit(column);
        }

        public Type ContainingEntityType { get; set; }
        public Member Member { get; set; }

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

        public string Fetch
        {
            get { return attributes.Get(x => x.Fetch); }
        }

        public bool Update
        {
            get { return attributes.Get(x => x.Update); }
        }

        public bool Insert
        {
            get { return attributes.Get(x => x.Insert); }
        }
        
        public string Formula
        {
            get { return attributes.Get(x => x.Formula); }
        }

        public string ForeignKey
        {
            get { return attributes.Get(x => x.ForeignKey); }
        }

        public string PropertyRef
        {
            get { return attributes.Get(x => x.PropertyRef); }
        }

        public string NotFound
        {
            get { return attributes.Get(x => x.NotFound); }
        }

        public string Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public string EntityName
        {
            get { return attributes.Get(x => x.EntityName); }
        }

        public bool OptimisticLock
        {
            get { return attributes.Get(x => x.OptimisticLock); }
        }

        public IDefaultableEnumerable<ColumnMapping> Columns
        {
            get { return columns; }
        }

        public void AddColumn(ColumnMapping column)
        {
            columns.Add(column);
        }

        public void AddDefaultColumn(ColumnMapping column)
        {
            columns.AddDefault(column);
        }

        public void ClearColumns()
        {
            columns.ClearAll();
        }

        public bool HasValue<TResult>(Expression<Func<ManyToOneMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public void Set<T>(Expression<Func<ManyToOneMapping, T>> expression, int layer, T value)
        {
            attributes.Set(expression, layer, value);
        }

        public bool Equals(ManyToOneMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.attributes, attributes) &&
                other.columns.ContentEquals(columns) &&
                Equals(other.ContainingEntityType, ContainingEntityType) &&
                Equals(other.Member, Member);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ManyToOneMapping)) return false;
            return Equals((ManyToOneMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (attributes != null ? attributes.GetHashCode() : 0);
                result = (result * 397) ^ (columns != null ? columns.GetHashCode() : 0);
                result = (result * 397) ^ (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
                result = (result * 397) ^ (Member != null ? Member.GetHashCode() : 0);
                return result;
            }
        }

        public IRelationship OtherSide { get; set; }
    }
}