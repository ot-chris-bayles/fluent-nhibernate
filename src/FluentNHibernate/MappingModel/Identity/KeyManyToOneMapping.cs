using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel.Identity
{
    [Serializable]
    public class KeyManyToOneMapping : MappingBase, ICompositeIdKeyMapping
    {
        private readonly AttributeStore<KeyManyToOneMapping> attributes = new AttributeStore<KeyManyToOneMapping>();
        private readonly IList<ColumnMapping> columns = new List<ColumnMapping>();

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessKeyManyToOne(this);

            foreach (var column in columns)
                visitor.Visit(column);
        }

        public string Access
        {
            get { return attributes.Get(x => x.Access); }
        }

        public string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public TypeReference Class
        {
            get { return attributes.Get(x => x.Class); }
        }

        public string ForeignKey
        {
            get { return attributes.Get(x => x.ForeignKey); }
        }

        public bool Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public string NotFound
        {
            get { return attributes.Get(x => x.NotFound); }
        }

        public string EntityName
        {
            get { return attributes.Get(x => x.EntityName); }
        }

        public IEnumerable<ColumnMapping> Columns
        {
            get
            {
                return columns;
            }
        }

        public Type ContainingEntityType { get; set; }

        public void AddColumn(ColumnMapping mapping)
        {
            columns.Add(mapping);
        }

        public bool HasValue<TResult>(Expression<Func<KeyManyToOneMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(KeyManyToOneMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.attributes, attributes) &&
                other.columns.ContentEquals(columns) &&
                Equals(other.ContainingEntityType, ContainingEntityType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(KeyManyToOneMapping)) return false;
            return Equals((KeyManyToOneMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (attributes != null ? attributes.GetHashCode() : 0);
                result = (result * 397) ^ (columns != null ? columns.GetHashCode() : 0);
                result = (result * 397) ^ (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
                return result;
            }
        }
    }
}