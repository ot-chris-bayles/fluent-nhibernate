using FluentNHibernate.Cfg.Db;
using FluentNHibernate.MappingModel;
using NHibernate.Cfg;

namespace FluentNHibernate.Infrastructure
{
    /// <summary>
    /// Container for changes that need to be applied to a configuration
    /// </summary>
    public class CfgModificationSet
    {
        readonly IPersistenceInstructions instructions;

        public HibernateMapping Mappings { get; private set; }

        public IDatabaseConfiguration Database
        {
            get { return instructions.Database; }
        }

        public IExporter Exporter
        {
            get { return instructions.Exporter; }
        }

        public CfgModificationSet(HibernateMapping mappings, IPersistenceInstructions instructions)
        {
            this.instructions = instructions;
            Mappings = mappings;
        }

        public void PreConfigure(Configuration cfg)
        {
            if (instructions.PreConfigure != null)
                instructions.PreConfigure(cfg);
        }

        public void PostConfigure(Configuration cfg)
        {
            if (instructions.PostConfigure != null)
                instructions.PostConfigure(cfg);
        }
    }
}