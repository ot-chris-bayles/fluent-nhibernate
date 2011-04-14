using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel.Identity
{
    [Serializable]
    public class CompositeIdMapping : MappingBase, IIdentityMapping
    {
        private readonly AttributeStore<CompositeIdMapping> attributes;
        private readonly IList<ICompositeIdKeyMapping> keys = new List<ICompositeIdKeyMapping>();

        public CompositeIdMapping()
            : this(new AttributeStore())
        {}

        public CompositeIdMapping(AttributeStore underlyingStore)
        {
            attributes = new AttributeStore<CompositeIdMapping>(underlyingStore);
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessCompositeId(this);

            foreach (var key in keys)
            {
                if (key is KeyPropertyMapping)
                    visitor.Visit((KeyPropertyMapping)key);
                if (key is KeyManyToOneMapping)
                    visitor.Visit((KeyManyToOneMapping)key);
            }
        }

        public string Name
        {
            get { return attributes.Get(x => x.Name); }
                // TODO: Fix this
				//Mapped = !string.IsNullOrEmpty(value);
        }

        public string Access
        {
            get { return attributes.Get(x => x.Access); }
        }

        public bool Mapped
        {
            get { return attributes.Get(x => x.Mapped); }
        }

        public TypeReference Class
        {
            get { return attributes.Get(x => x.Class); }
        }

        public string UnsavedValue
        {
            get { return attributes.Get(x => x.UnsavedValue); }
        }

        public IEnumerable<ICompositeIdKeyMapping> Keys
        {
            get { return keys; }
        }

        public Type ContainingEntityType { get; set; }

        public void AddKey(ICompositeIdKeyMapping mapping)
        {
            keys.Add(mapping);
        }

        public bool HasValue<TResult>(Expression<Func<CompositeIdMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(CompositeIdMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.attributes, attributes) &&
                other.keys.ContentEquals(keys) &&
                Equals(other.ContainingEntityType, ContainingEntityType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CompositeIdMapping)) return false;
            return Equals((CompositeIdMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (attributes != null ? attributes.GetHashCode() : 0);
                result = (result * 397) ^ (keys != null ? keys.GetHashCode() : 0);
                result = (result * 397) ^ (ContainingEntityType != null ? ContainingEntityType.GetHashCode() : 0);
                return result;
            }
        }
    }
}