using System;
using System.Linq.Expressions;
using FluentNHibernate.MappingModel.Identity;
using FluentNHibernate.Visitors;

namespace FluentNHibernate.MappingModel.ClassBased
{
    [Serializable]
    public class ClassMapping : ClassMappingBase
    {
        private readonly AttributeStore<ClassMapping> attributes;

        public ClassMapping()
            : this(new AttributeStore())
        {}

        public ClassMapping(AttributeStore store)
        {
            attributes = new AttributeStore<ClassMapping>(store);
        }

        public IIdentityMapping Id
        {
            get { return attributes.Get(x => x.Id); }
        }

        public NaturalIdMapping NaturalId
        {
            get { return attributes.Get(x => x.NaturalId); }
        }

        public override string Name
        {
            get { return attributes.Get(x => x.Name); }
        }

        public override Type Type
        {
            get { return attributes.Get(x => x.Type); }
        }

        public CacheMapping Cache
        {
            get { return attributes.Get(x => x.Cache); }
        }

        public VersionMapping Version
        {
            get { return attributes.Get(x => x.Version); }
        }

        public DiscriminatorMapping Discriminator
        {
            get { return attributes.Get(x => x.Discriminator); }
        }

        public bool IsUnionSubclass
        {
            get { return attributes.Get(x => x.IsUnionSubclass); }
        }

        public TuplizerMapping Tuplizer
        {
            get { return attributes.Get(x => x.Tuplizer); }
        }

        public override void AcceptVisitor(IMappingModelVisitor visitor)
        {
            visitor.ProcessClass(this);            

            if (Id != null)
                visitor.Visit(Id);

            if (NaturalId != null)
                visitor.Visit(NaturalId);

            if (Discriminator != null)
                visitor.Visit(Discriminator);

            if (Cache != null)
                visitor.Visit(Cache);

            if (Version != null)
                visitor.Visit(Version);

            if (Tuplizer != null)
                visitor.Visit(Tuplizer);

            base.AcceptVisitor(visitor);
        }

        public string TableName
        {
            get { return attributes.Get(x => x.TableName); }
        }

        public int BatchSize
        {
            get { return attributes.Get(x => x.BatchSize); }
        }

        public object DiscriminatorValue
        {
            get { return attributes.Get(x => x.DiscriminatorValue); }
        }

        public string Schema
        {
            get { return attributes.Get(x => x.Schema); }
        }

        public bool Lazy
        {
            get { return attributes.Get(x => x.Lazy); }
        }

        public bool Mutable
        {
            get { return attributes.Get(x => x.Mutable); }
        }

        public bool DynamicUpdate
        {
            get { return attributes.Get(x => x.DynamicUpdate); }
        }

        public bool DynamicInsert
        {
            get { return attributes.Get(x => x.DynamicInsert); }
        }

        public string OptimisticLock
        {
            get { return attributes.Get(x => x.OptimisticLock); }
        }

        public string Polymorphism
        {
            get { return attributes.Get(x => x.Polymorphism); }
        }

        public string Persister
        {
            get { return attributes.Get(x => x.Persister); }
        }

        public string Where
        {
            get { return attributes.Get(x => x.Where); }
        }

        public string Check
        {
            get { return attributes.Get(x => x.Check); }
        }

        public string Proxy
        {
            get { return attributes.Get(x => x.Proxy); }
        }

        public bool SelectBeforeUpdate
        {
            get { return attributes.Get(x => x.SelectBeforeUpdate); }
        }

        public bool Abstract
        {
            get { return attributes.Get(x => x.Abstract); }
        }

        public string Subselect
        {
            get { return attributes.Get(x => x.Subselect); }
        }

        public string SchemaAction
        {
            get { return attributes.Get(x => x.SchemaAction); }
        }

        public string EntityName
        {
            get { return attributes.Get(x => x.EntityName); }
        }       

        public bool HasValue<TResult>(Expression<Func<ClassMapping, TResult>> property)
        {
            return attributes.HasValue(property);
        }

        public bool Equals(ClassMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                Equals(other.attributes, attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ClassMapping)) return false;
            return Equals((ClassMapping)obj);
        }

        public override int GetHashCode()
        {
            return (attributes != null ? attributes.GetHashCode() : 0);
        }
    }
}