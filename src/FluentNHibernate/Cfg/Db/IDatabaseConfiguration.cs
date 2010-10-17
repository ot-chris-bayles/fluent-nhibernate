using NHibernate.Cfg;

namespace FluentNHibernate.Cfg.Db
{
    public interface IDatabaseConfiguration
    {
        void Configure(Configuration cfg);
    }
}