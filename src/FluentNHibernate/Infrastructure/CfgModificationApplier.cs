using FluentNHibernate.Cfg.Db;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Output;
using NHibernate.Cfg;

namespace FluentNHibernate.Infrastructure
{
    public class CfgModificationApplier
    {
        readonly CfgModificationSet alterations;

        public CfgModificationApplier(CfgModificationSet alterations)
        {
            this.alterations = alterations;
        }

        public void ApplyModifications(Configuration cfg)
        {
            alterations.PreConfigure(cfg);

            UpdateSettings(cfg, alterations.Database);
            InjectMappings(cfg, alterations.Mappings);

            alterations.PostConfigure(cfg);
        }

        void UpdateSettings(Configuration cfg, IDatabaseConfiguration database)
        {
            if (database != null)
                database.Configure(cfg);
        }

        void InjectMappings(Configuration cfg, HibernateMapping mappings)
        {
            var serializer = new MappingXmlSerializer();
            var document = serializer.Serialize(mappings);

            if (alterations.Exporter != null)
                alterations.Exporter.Export(document);

            cfg.AddDocument(document);
        }
    }
}