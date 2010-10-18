using System;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NUnit.Framework;
using NHibernate.Cfg;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class OneToManyIntegrationTester
    {
        private class OneToManyPersistenceModel : PersistenceModel
        {
            public void Configure(NHibernate.Cfg.Configuration configuration)
            {
                InjectMapping(new ChildObjectMap());
                InjectMapping(new OneToManyTargetMap());
            }

            private class ChildObjectMap : ClassMap<ChildObject>
            {
                public ChildObjectMap()
                {
                    Id(x => x.Id);
                }
            }

            private class OneToManyTargetMap : ClassMap<OneToManyTarget>
            {
                public OneToManyTargetMap()
                {
                    Id(x => x.Id);
                    HasMany(x => x.ListOfChildren).AsList();
                    HasMany(x => x.BagOfChildren).AsBag();
                    HasMany(x => x.SetOfChildren).AsSet();
                    HasMany(x => x.MapOfChildren);
                    HasMany(x => x.ArrayOfChildren).AsArray(x => x.Position);
                }
            }
        }

        [Test]
        public void NHibernateCanLoadOneToManyTargetMapping()
        {
            var cfg = new SQLiteConfiguration()
                .InMemory()
                .ConfigureProperties(new Configuration());

            var model = new OneToManyPersistenceModel();
            model.Configure(cfg);

            cfg.BuildSessionFactory();
        }
    }
}
