using System;
using System.Linq;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Specs.FluentInterface.Fixtures;

namespace FluentNHibernate.Specs.FluentInterface
{
    public abstract class ProviderSpec
    {
        public static ClassMapping map_as_class<T>(Action<ClassMap<T>> setup)
        {
            return map_model(mod => mod.map_class(setup))
                .Classes.FirstOrDefault(x => x.Type == typeof(T));
        }

        public static HibernateMapping map_model(Action<FluentNHibernate.PersistenceModel> setup)
        {
            var model = new FluentNHibernate.PersistenceModel();

            setup(model);

            return model.BuildMappings();
        }

        public static SubclassMapping map_as_subclass<T>(Action<SubclassMap<T>> setup)
        {
            return map_model(mod =>
            {
                mod.map_class<SuperTarget>(m => m.Id(x => x.Id));
                mod.map_subclass<T>(m =>
                {
                    m.Extends<SuperTarget>();
                    setup(m);
                });
            }).Classes.Single().Subclasses.Single();
        }
    }

    public static class ProviderExtensions
    {
        public static void map_class<T>(this FluentNHibernate.PersistenceModel model, Action<ClassMap<T>> setup)
        {
            var provider = new ClassMap<T>();

            setup(provider);

            model.InjectMapping(provider);
        }

        public static void map_subclass<T>(this FluentNHibernate.PersistenceModel model, Action<SubclassMap<T>> setup)
        {
            var provider = new SubclassMap<T>();

            setup(provider);

            model.InjectMapping(provider);
        }

        public static void map_component<T>(this FluentNHibernate.PersistenceModel model)
        {
            var provider = new ComponentMap<T>();

            model.InjectMapping(provider);
        }
    }
}