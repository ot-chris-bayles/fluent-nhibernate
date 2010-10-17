using NHibernate.Cfg;

namespace FluentNHibernate.Cfg.Db
{
    public class PreconfiguredDatabaseConfiguration : IDatabaseConfiguration
    {
        readonly IPersistenceConfigurer db;

        public PreconfiguredDatabaseConfiguration(IPersistenceConfigurer db)
        {
            this.db = db;
        }

        public IPersistenceConfigurer Configurer
        {
            get { return db; }
        }

        public void Configure(Configuration cfg)
        {
            if (db != null)
                db.ConfigureProperties(cfg);
        }
    }
}