using System;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using FluentNHibernate.Visitors;
using Machine.Specifications;
using NHibernate.Cfg;

namespace FluentNHibernate.Specs.PersistenceModel
{
    public class when_the_persistence_model_is_told_to_build_the_mappings_for_a_class_with_a_join
    {
        Establish context = () =>
        {
            cfg = new Configuration();

            SQLiteConfiguration.Standard.InMemory()
                .ConfigureProperties(cfg);

            model = new FluentNHibernate.PersistenceModel();
            
            var class_map = new ClassMap<Target>();
            class_map.Id(x => x.Id);
            class_map.Join("other", m => m.Map(x => x.Property));
            
            model.InjectMapping(class_map);
        };

        Because of = () =>
            cfg.ConfigureWith(model);

        It shouldnt_duplicate_join_mapping = () =>
            cfg.ClassMappings.First()
                .JoinClosureIterator.Count().ShouldEqual(1);
        
        static FluentNHibernate.PersistenceModel model;
        static Configuration cfg;

        class Target
        {
            public int Id { get; set; }
            public string Property { get; set; }
        }
    }
}