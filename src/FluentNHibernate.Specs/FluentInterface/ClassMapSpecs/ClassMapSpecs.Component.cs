using System.Collections;
using System.Linq;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Specs.FluentInterface.Fixtures;
using Machine.Specifications;

namespace FluentNHibernate.Specs.FluentInterface.ClassMapSpecs
{
    public class when_class_map_is_told_to_map_a_component : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithComponent>(m =>
            {
                m.Id(x => x.Id);
                m.Component(x => x.Component, c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_component_from_a_field : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithFieldComponent>(m =>
            {
                m.Id(x => x.Id);
                m.Component(x => x.Component, c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_component_using_a_provider : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithComponent>(m =>
            {
                m.Id(x => x.Id);
                m.Component(new ComponentMappingProviderStub());
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;        

        private class ComponentMappingProviderStub : IComponentMappingProvider
        {
            public IComponentMapping GetComponentMapping()
            {
                return new ComponentMapping(ComponentType.Component);
            }
        }
    }

    public class when_class_map_is_told_to_map_a_component_using_reveal : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithComponent>(m =>
            {
                m.Id(x => x.Id);
                m.Component(Reveal.Member<EntityWithComponent>("Component"), c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_dynamic_component : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithComponent>(m =>
            {
                m.Id(x => x.Id);
                m.DynamicComponent(x => x.DynamicComponent, c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_dynamic_component_from_a_field : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithFieldComponent>(m =>
            {
                m.Id(x => x.Id);
                m.DynamicComponent(x => x.DynamicComponent, c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_dynamic_component_using_reveal : ProviderSpec
    {
        Because of = () =>
            mapping = map_as_class<EntityWithComponent>(m =>
            {
                m.Id(x => x.Id);
                m.DynamicComponent(Reveal.Member<EntityWithComponent, IDictionary>("DynamicComponent"), c => { });
            });

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_reference_component : ProviderSpec
    {
        Because of = () =>
        {
            var mappings = map_model(mod =>
            {
                mod.map_component<ComponentTarget>();
                mod.map_class<EntityWithComponent>(m =>
                {
                    m.Id(x => x.Id);
                    m.Component(x => x.Component);
                });
            });
            mapping = mappings.Classes.Single(x => x.Type == typeof(EntityWithComponent));
        };

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_reference_component_from_a_field : ProviderSpec
    {
        Because of = () =>
        {
            var mappings = map_model(mod =>
            {
                mod.map_component<ComponentTarget>();
                mod.map_class<EntityWithFieldComponent>(m =>
                {
                    m.Id(x => x.Id);
                    m.Component(x => x.Component);
                });
            });
            mapping = mappings.Classes.Single(x => x.Type == typeof(EntityWithFieldComponent));
        };

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }

    public class when_class_map_is_told_to_map_a_reference_component_using_reveal : ProviderSpec
    {
        Because of = () =>
        {
            var mappings = map_model(mod =>
            {
                mod.map_component<ComponentTarget>();
                mod.map_class<EntityWithComponent>(m =>
                {
                    m.Id(x => x.Id);
                    m.Component(Reveal.Member<EntityWithComponent, ComponentTarget>("Component"));
                });
            });
            mapping = mappings.Classes.Single(x => x.Type == typeof(EntityWithComponent));
        };

        Behaves_like<ClasslikeComponentBehaviour> a_component_in_a_classlike;

        protected static ClassMapping mapping;
    }
}