using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NUnit.Framework;
using NHibernate.Cfg;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class ManyToManyIntegrationTester
    {
        private class ManyToManyPersistenceModel : PersistenceModel
        {
            public void Configure(Configuration configuration)
            {
                InjectMapping(new ChildObjectMap());
                InjectMapping(new ManyToManyTargetMap());
            }

            private class ChildObjectMap : ClassMap<ChildObject>
            {
                public ChildObjectMap()
                {
                    Id(x => x.Id);
                }
            }

            private class ManyToManyTargetMap : ClassMap<ManyToManyTarget>
            {
                public ManyToManyTargetMap()
                {
                    Id(x => x.Id);
                    HasManyToMany(x => x.GetOtherChildren()).AsBag().Access.CamelCaseField();
                }
            }
        }

        [Test]
        public void NHibernateCanLoadOneToManyTargetMapping()
        {
            var cfg = new SQLiteConfiguration()
                .InMemory()
                .ConfigureProperties(new Configuration());

            var model = new ManyToManyPersistenceModel();
            model.Configure(cfg);

            cfg.BuildSessionFactory();
        }
    }
}
