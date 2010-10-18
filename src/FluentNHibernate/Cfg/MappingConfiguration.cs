using System;
using FluentNHibernate.Diagnostics;
using NHibernate.Cfg;

namespace FluentNHibernate.Cfg
{
    /// <summary>
    /// Fluent mapping configuration
    /// </summary>
    public class MappingConfiguration
    {
        readonly IDiagnosticLogger logger;

        public MappingConfiguration(IDiagnosticLogger logger)
        {
            this.logger = logger;

            FluentMappings = new FluentMappingsContainer(logger);
            AutoMappings = new AutoMappingsContainer();
            HbmMappings = new HbmMappingsContainer();
        }

        /// <summary>
        /// Fluent mappings
        /// </summary>
        public FluentMappingsContainer FluentMappings { get; private set; }

        /// <summary>
        /// Automatic mapping configurations
        /// </summary>
        public AutoMappingsContainer AutoMappings { get; private set; }

        /// <summary>
        /// Hbm mappings
        /// </summary>
        public HbmMappingsContainer HbmMappings { get; private set; }

        /// <summary>
        /// Get whether any mappings of any kind were added
        /// </summary>
        public bool WasUsed
        {
            get
            {
                return FluentMappings.WasUsed ||
                       AutoMappings.WasUsed ||
                       HbmMappings.WasUsed;
            }
        }

        /// <summary>
        /// Applies any mappings to the NHibernate Configuration
        /// </summary>
        /// <param name="cfg">NHibernate Configuration instance</param>
        public void Apply(Configuration cfg)
        {
            foreach (var autoMapping in AutoMappings)
                autoMapping.SetLogger(logger);

            HbmMappings.Apply(cfg);
            FluentMappings.Apply(cfg);
            AutoMappings.Apply(cfg);
        }
    }
}