using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.ClassBased;
using Machine.Specifications;

namespace FluentNHibernate.Specs.PersistenceModel
{
    public class when_pairing_subclasses_of_interfaces_with_direct_implementations : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new FooMap());
            model.InjectMapping(new StringFooMap());
            model.InjectMapping(new StandAloneMap());
        };

        Because of = () =>
            mapping = model.BuildMappings()
                .Classes.Single();

        // The Parent is the IFoo interface the desired results 
        // of this test is the inclusion of the Foo<T> through the
        // GenericFooMap<T> subclass mapping.
        It should_add_subclass_that_implements_the_parent_interface = () =>
        {
            mapping.Subclasses.Count().ShouldEqual(1);
            mapping.Subclasses.Select(x => x.Type).ShouldContain(typeof(Foo<string>));
        };

        // The Parent is the IFoo interface the desired results 
        // of this test is the exclusion of the StandAlone class 
        // since it does not implement the interface.
        It should_not_add_subclass_that_doesnt_implement_the_parent_interface = () =>
            mapping.Subclasses.Select(x => x.Type).ShouldNotContain(typeof(StandAlone));

        static ClassMapping mapping;
    }

    public class when_pairing_subclasses_of_interfaces_with_abstract_base : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new FooMap());
            model.InjectMapping(new StringFooMap());
            model.InjectMapping(new StandAloneMap());
            model.InjectMapping(new BaseImplMap());
        };

        Because of = () =>
            mapping = model.BuildMappings()
                .Classes.Single();

        // The Parent is the IFoo interface the desired results 
        // of this test is the exclusion of the StandAlone class 
        // since it does not implement the interface.
        It should_not_add_subclass_that_doesnt_implement_the_parent_interface = () =>
            mapping.Subclasses.Select(x => x.Type).ShouldNotContain(typeof(StandAlone));

        // The Parent is the IFoo interface the desired results 
        // of this test is the inclusion of the BaseImpl class and 
        // the exclusion of the Foo<T> class since it implements 
        // the BaseImpl class which already implements FooBase.
        It Should_not_add_subclassmap_that_implements_a_subclass_of_the_parent_interface = () =>
        {
            mapping.Subclasses.Count().ShouldEqual(1);
            mapping.Subclasses.Where(x => x.Type.Equals(typeof(BaseImpl))).Count().ShouldEqual(1);
        };

        static ClassMapping mapping;
    }

    public class when_pairing_subclasses_with_explicit_extends : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new ExtendsParentMap());
            model.InjectMapping(new ExtendsChildMap());
        };

        Because of = () =>
            mapping = model.BuildMappings()
                .Classes.Single();

        It should_add_explicit_extend_subclasses_to_their_parent = () =>
        {
            mapping.Subclasses.Count().ShouldEqual(1);
            mapping.Subclasses.Select(x => x.Type).ShouldContain(typeof(ExtendsChild));
        };
        
        static ClassMapping mapping;
    }

    public class when_pairing_subclasses_with_a_abstract_base : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new StringFooMap());
            model.InjectMapping(new BaseMap());
            model.InjectMapping(new StandAloneMap());
        };

        Because of = () =>
            mapping = model.BuildMappings().Classes.Single();

        // The Parent is the FooBase class the desired results 
        // of this test is the inclusion of the Foo<T> through the
        // GenericFooMap<T> subclass mapping.
        It should_add_subclass_that_implements_the_parent_base = () =>
        {
            mapping.Subclasses.Count().ShouldEqual(1);
            mapping.Subclasses.Select(x => x.Type).ShouldContain(typeof(Foo<string>));
        };

        // The Parent is the FooBase class the desired results 
        // of this test is the exclusion of the StandAlone class 
        // since it does not implement the interface.
        It should_not_add_subclassmap_that_does_not_implement_parent_base = () =>
            mapping.Subclasses.Select(x => x.Type).ShouldNotContain(typeof(StandAlone));

        static ClassMapping mapping;
    }
    
    public class when_pairing_subclasses_with_an_intermediary_abstract_base : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new BaseMap());
            model.InjectMapping(new StandAloneMap());
            model.InjectMapping(new BaseImplMap());
        };

        Because of = () =>
            mapping = model.BuildMappings().Classes.Single();

        // The Parent is the FooBase class the desired results 
        // of this test is the exclusion of the StandAlone class 
        // since it does not implement the interface.
        It should_not_add_subclassmap_that_does_not_implement_parent_base = () =>
            mapping.Subclasses.Select(x => x.Type).ShouldNotContain(typeof(StandAlone));

        // The Parent is the FooBase class the desired results 
        // of this test is the inclusion of the BaseImpl class and 
        // the exclusion of the Foo<T> class since it implements 
        // the BaseImpl class which already implements FooBase.
        It should_not_add_subclassmap_that_implements_a_subclass_of_the_parent_base = () =>
        {
            mapping.Subclasses.Count().ShouldEqual(1);
            mapping.Subclasses.Select(x => x.Type).ShouldContain(typeof(BaseImpl));
        };

        static ClassMapping mapping;
    }

    public class when_pairing_subclasses_with_unioning_enabled : SubclassPairingSpecBase
    {
        Establish context = () =>
        {
            model = new FluentNHibernate.PersistenceModel();
            model.InjectMapping(new StringFooMap());
            model.InjectMapping(new UnionBaseMap());
        };

        Because of = () =>
            mapping = model.BuildMappings().Classes.Single();

        It Should_choose_UnionSubclass_when_the_class_mapping_IsUnionSubclass_is_true = () =>
            mapping.Subclasses.All(x => x.SubclassType == SubclassType.UnionSubclass).ShouldBeTrue();

        static ClassMapping mapping;
    }

    public abstract class SubclassPairingSpecBase
    {
        protected static FluentNHibernate.PersistenceModel model;
        protected static IEnumerable<ClassMapping> classes;

        protected interface IFoo
        {
            int Id { get; set; }
        }

        protected class Base : IFoo
        {
            public int Id { get; set; }
        }

        protected abstract class BaseImpl : Base
        { }

        protected class Foo<T> : BaseImpl, IFoo
        { }

        protected class FooMap : ClassMap<IFoo>
        {
            public FooMap()
            {
                Id(x => x.Id);
            }
        }

        protected class BaseMap : ClassMap<Base>
        {
            public BaseMap()
            {
                Id(x => x.Id);
            }
        }

        protected class UnionBaseMap : ClassMap<Base>
        {
            public UnionBaseMap()
            {
                Id(x => x.Id);
                UseUnionSubclassForInheritanceMapping();
            }
        }

        protected class BaseImplMap : SubclassMap<BaseImpl>
        { }

        protected abstract class GenericFooMap<T> : SubclassMap<Foo<T>>
        { }

        protected class StringFooMap : GenericFooMap<string>
        { }


        private interface IStand
        { }

        protected class StandAlone : IStand
        { }

        protected class StandAloneMap : SubclassMap<StandAlone>
        { }

        protected class ExtendsParent
        {
            public int Id { get; set; }
        }

        protected class ExtendsChild
        { }

        protected class ExtendsParentMap : ClassMap<ExtendsParent>
        {
            public ExtendsParentMap()
            {
                Id(x => x.Id);
            }
        }

        protected class ExtendsChildMap : SubclassMap<ExtendsChild>
        {
            public ExtendsChildMap()
            {
                Extends<ExtendsParent>();
            }
        }
    }
}
