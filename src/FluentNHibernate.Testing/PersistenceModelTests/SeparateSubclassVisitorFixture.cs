using System.Linq;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.ClassBased;
using NUnit.Framework;

namespace FluentNHibernate.Testing.PersistenceModelTests
{
    [TestFixture]
    public class SeparateSubclassVisitorFixture
    {
        PersistenceModel model;

        [SetUp]
        public void SetUp()
        {
            model = new PersistenceModel();
        }

        [Test]
        public void Should_add_subclass_that_implements_the_parent_interface()
        {
            /* The Parent is the IFoo interface the desired results 
             * of this test is the inclusion of the Foo<T> through the
             * GenericFooMap<T> subclass mapping.
             */

            model.Add(new FooMap());
            model.Add(new StringFooMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(1, classMapping.Subclasses.Count());
            Assert.AreEqual(1, classMapping.Subclasses.Where(sub => sub.Type.Equals(typeof(Foo<string>))).Count());
        }

        [Test]
        public void Should_add_subclass_that_implements_the_parent_base()
        {
            /* The Parent is the FooBase class the desired results 
             * of this test is the inclusion of the Foo<T> through the
             * GenericFooMap<T> subclass mapping.
             */

            model.Add(new BaseMap());
            model.Add(new StringFooMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(1, classMapping.Subclasses.Count());
            Assert.AreEqual(1, classMapping.Subclasses.Where(sub => sub.Type.Equals(typeof(Foo<string>))).Count());
        }

        [Test]
        public void Should_not_add_subclassmap_that_does_not_implement_parent_interface()
        {
            /* The Parent is the IFoo interface the desired results 
             * of this test is the exclusion of the StandAlone class 
             * since it does not implement the interface.
             */

            model.Add(new FooMap());
            model.Add(new StandAloneMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(0, classMapping.Subclasses.Count());
        }

        [Test]
        public void Should_not_add_subclassmap_that_does_not_implement_parent_base()
        {
            /* The Parent is the FooBase class the desired results 
             * of this test is the exclusion of the StandAlone class 
             * since it does not implement the interface.
             */

            model.Add(new BaseMap());
            model.Add(new StandAloneMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(0, classMapping.Subclasses.Count());
        }

        [Test]
        public void Should_not_add_subclassmap_that_implements_a_subclass_of_the_parent_interface()
        {
            /* The Parent is the IFoo interface the desired results 
             * of this test is the inclusion of the BaseImpl class and 
             * the exclusion of the Foo<T> class since it implements 
             * the BaseImpl class which already implements FooBase.
             */

            model.Add(new FooMap());
            model.Add(new BaseImplMap());
            model.Add(new StringFooMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(1, classMapping.Subclasses.Count());
            Assert.AreEqual(1, classMapping.Subclasses.Where(sub => sub.Type.Equals(typeof(BaseImpl))).Count());
        }

        [Test]
        public void Should_not_add_subclassmap_that_implements_a_subclass_of_the_parent_base()
        {
            /* The Parent is the FooBase class the desired results 
             * of this test is the inclusion of the BaseImpl class and 
             * the exclusion of the Foo<T> class since it implements 
             * the BaseImpl class which already implements FooBase.
             */

            model.Add(new BaseMap());
            model.Add(new BaseImplMap());
            model.Add(new StringFooMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(1, classMapping.Subclasses.Count());
            Assert.AreEqual(1, classMapping.Subclasses.Where(sub => sub.Type.Equals(typeof(BaseImpl))).Count());
        }

        [Test]
        public void Should_add_explicit_extend_subclasses_to_their_parent()
        {
            model.Add(new ExtendsParentMap());
            model.Add(new ExtendsChildMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            Assert.AreEqual(1, classMapping.Subclasses.Count());
            Assert.AreEqual(1, classMapping.Subclasses.Where(sub => sub.Type.Equals(typeof(ExtendsChild))).Count());
        }

        [Test]
        public void Should_choose_UnionSubclass_when_the_class_mapping_IsUnionSubclass_is_true()
        {
            var map = new BaseMap();
            map.UseUnionSubclassForInheritanceMapping();

            model.Add(map);
            model.Add(new StringFooMap());

            var classMapping = model.BuildMappings()
                .Classes.Single();

            classMapping.Subclasses.First().SubclassType.ShouldEqual(SubclassType.UnionSubclass);
        }

        private interface IFoo
        {
            int Id { get; set; }
        }

        private class Base : IFoo
        {
            public int Id { get; set; }
        }

        private abstract class BaseImpl : Base
        { }

        private class Foo<T> : BaseImpl, IFoo
        { }

        private class FooMap : ClassMap<IFoo>
        {
            public FooMap()
            {
                Id(x => x.Id);
            }
        }

        private class BaseMap : ClassMap<Base>
        {
            public BaseMap()
            {
                Id(x => x.Id);
            }
        }

        private class BaseImplMap : SubclassMap<BaseImpl>
        { }

        private abstract class GenericFooMap<T> : SubclassMap<Foo<T>>
        { }

        private class StringFooMap : GenericFooMap<string>
        { }


        private interface IStand
        { }

        private class StandAlone : IStand
        { }

        private class StandAloneMap : SubclassMap<StandAlone>
        { }

        class ExtendsParent
        {
            public int Id { get; set; }
        }

        class ExtendsChild
        {}

        class ExtendsParentMap : ClassMap<ExtendsParent>
        {
            public ExtendsParentMap()
            {
                Id(x => x.Id);
            }
        }

        class ExtendsChildMap : SubclassMap<ExtendsChild>
        {
            public ExtendsChildMap()
            {
                Extends<ExtendsParent>();
            }
        }
    }
}
