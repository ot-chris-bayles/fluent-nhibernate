using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.Testing.DomainModel.Mapping;
using NUnit.Framework;

namespace FluentNHibernate.Testing.FluentInterfaceTests
{
    [TestFixture]
    public class ClassMapMutablePropertyModelGenerationTests : BaseModelFixture
    {
        [Test]
        public void BatchSizeSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.BatchSize(10);
                })
                .ModelShouldMatch(x => x.BatchSize.ShouldEqual(10));
        }

        [Test]
        public void CheckSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.CheckConstraint("constraint");
                })
                .ModelShouldMatch(x => x.Check.ShouldEqual("constraint"));
        }

        [Test]
        public void OptimisticLockSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.OptimisticLock.All();
                })
                .ModelShouldMatch(x => x.OptimisticLock.ShouldEqual("all"));
        }

        [Test]
        public void PersisterSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Persister<CustomPersister>();
                })
                .ModelShouldMatch(x => x.Persister.ShouldEqual(typeof(CustomPersister).AssemblyQualifiedName));
        }

        [Test]
        public void PolymorphismSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Polymorphism.Implicit();
                })
                .ModelShouldMatch(x => x.Polymorphism.ShouldEqual("implicit"));
        }

        [Test]
        public void ProxySetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Proxy<FakeProxy>();
                })
                .ModelShouldMatch(x => x.Proxy.ShouldEqual(typeof(FakeProxy).AssemblyQualifiedName));
        }

        [Test]
        public void SelectBeforeUpdateSetsPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.SelectBeforeUpdate();
                })
                .ModelShouldMatch(x => x.SelectBeforeUpdate.ShouldBeTrue());
        }

        [Test]
        public void LazyLoadSetsModelPropertyToTrue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.LazyLoad();
                })
                .ModelShouldMatch(x => x.Lazy.ShouldEqual(true));
        }

        [Test]
        public void NotLazyLoadSetsModelPropertyToFalse()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Not.LazyLoad();
                })
                .ModelShouldMatch(x => x.Lazy.ShouldEqual(false));
        }

        [Test]
        public void WithTableShouldSetModelPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Table("table");
                })
                .ModelShouldMatch(x => x.TableName.ShouldEqual("table"));
        }

        [Test]
        public void DynamicInsertShouldSetModelPropertyToTrue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.DynamicInsert();
                })
                .ModelShouldMatch(x => x.DynamicInsert.ShouldBeTrue());
        }

        [Test]
        public void NotDynamicInsertShouldSetModelPropertyToFalse()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Not.DynamicInsert();
                })
                .ModelShouldMatch(x => x.DynamicInsert.ShouldBeFalse());
        }

        [Test]
        public void DynamicUpdateShouldSetModelPropertyToTrue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.DynamicUpdate();
                })
                .ModelShouldMatch(x => x.DynamicUpdate.ShouldBeTrue());
        }

        [Test]
        public void NotDynamicUpdateShouldSetModelPropertyToFalse()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Not.DynamicUpdate();
                })
                .ModelShouldMatch(x => x.DynamicUpdate.ShouldBeFalse());
        }

        [Test]
        public void ReadOnlyShouldSetModelMutablePropertyToFalse()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.ReadOnly();
                })
                .ModelShouldMatch(x => x.Mutable.ShouldBeFalse());
        }

        [Test]
        public void NotReadOnlyShouldSetModelMutablePropertyToTrue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Not.ReadOnly();
                })
                .ModelShouldMatch(x => x.Mutable.ShouldBeTrue());
        }

        [Test]
        public void SchemaIsShouldSetModelPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Schema("schema");
                })
                .ModelShouldMatch(x => x.Schema.ShouldEqual("schema"));
        }

        [Test]
        public void SubselectShouldSetModelPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Subselect("sql query");
                })
                .ModelShouldMatch(x => x.Subselect.ShouldEqual("sql query"));
        }

        [Test]
        public void SchemaActionShouldSetModelPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.SchemaAction.None();
                })
                .ModelShouldMatch(x => x.SchemaAction.ShouldEqual("none"));
        }

        [Test]
        public void EntityNameShouldSetModelPropertyToValue()
        {
            ClassMap<PropertyTarget>()
                .Mapping(m =>
                {
                    m.Id(x => x.Id);
                    m.EntityName("entity1");
                })
                .ModelShouldMatch(x => x.EntityName.ShouldEqual("entity1"));
        }

        public class FakeProxy
        { }
    }
}
