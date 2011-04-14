using System;
using System.Linq.Expressions;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel.ClassBased
{
    [Serializable]
    public class SubclassMapping : ClassMappingBase
    {
        public SubclassType SubclassType { get; private set; }
        private AttributeStore<SubclassMapping> attributes;

        public SubclassMapping(SubclassType subclassType)
            : this(subclassType, new AttributeStore())
        {}

        public SubclassMapping(SubclassType subclassType, AttributeStore underlyingStore)
        {
            SubclassType = subclassType;
            attributes = new AttributeStore<SubclassMapping>(underlyingStore);
        }

        /// <summary>
        /// Set which type this subclass extends.
        /// Note: This doesn't actually get output into the XML, it's
        /// instead used as a marker for the <see cref="SeparateSubclassVisitor"/>
        /// to pair things up.
        /// </summary>
        public Type Extends
        {
            get { return attributes.Get(x => x.Extends); }
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessSubclass(this);

            if (SubclassType == SubclassType.JoinedSubclass && Key != null)
                visitor.Visit(Key);

            base.AcceptVisitor(visitor);
        }

        public override string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public override Type Type
        {
            get { return attributes.Get(x => x.Type); }
        }

        public object DiscriminatorValue
        {
            get { return attributes.Get(x => x.DiscriminatorValue); }
        }

        public bool Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public string Proxy
        {
            get { return attributes.Get(x => x.Proxy); }
        }

        public bool DynamicUpdate
        {
            get { return attributes.Get(x => x.DynamicUpdate); }
        }

        public bool DynamicInsert
        {
            get { return attributes.Get(x => x.DynamicInsert); }
        }

        public bool SelectBeforeUpdate
        {
            get { return attributes.Get(x => x.SelectBeforeUpdate); }
        }

        public bool Abstract
        {
            get { return attributes.Get(x => x.Abstract); }
        }

        public string EntityName
        {
            get { return attributes.Get(x => x.EntityName); }
        }

        public string TableName
        {
            get { return attributes.Get(x => x.TableName); }
        }

        public KeyMapping Key
        {
            get { return attributes.Get(x => x.Key); }
        }

        public string Check
        {
            get { return attributes.Get(x => x.Check); }
        }

        public string Schema
        {
            get { return attributes.Get(x => x.Schema); }
        }

        public string Subselect
        {
            get { return attributes.Get(x => x.Subselect); }
        }

        public TypeReference Persister
        {
            get { return attributes.Get(x => x.Persister); }
        }

        public int BatchSize
        {
            get { return attributes.Get(x => x.BatchSize); }
        }

        public bool HasValue<TResult>(Expression<Func<SubclassMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public void OverrideAttributes(AttributeStore store)
        {
            attributes = new AttributeStore<SubclassMapping>(store);
        }

        public bool Equals(SubclassMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.attributes, attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubclassMapping);
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

        public override string ToString()
        {
            return "Subclass(" + Type.Name + ")";
        }
    }
}