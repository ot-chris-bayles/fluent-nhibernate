using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Output;
using NHibernate.Cfg;

namespace FluentNHibernate
{
    public static class ConfigurationHelper
    {
        public static Configuration ConfigureWith<T>(this Configuration cfg)
            where T : PersistenceModel, new()
        {
            return cfg.ConfigureWith(new T());
        }

        public static Configuration ConfigureWith(this Configuration cfg, PersistenceModel model)
        {
            return cfg.ConfigureWith((IPersistenceInstructionGatherer)model);
        }

        public static Configuration ConfigureWith(this Configuration cfg, IPersistenceInstructionGatherer gatherer)
        {
            // TODO: move this out of an extension method
            var instructions = gatherer.GetInstructions();

            return cfg.ConfigureWith(instructions);
        }

        public static Configuration ConfigureWith(this Configuration cfg, IPersistenceInstructions instructions)
        {
            // TODO: move this out of an extension method
            //var automapper = new AutomapperV2(new ConventionFinder(instructions.Conventions));
            var compiler = new MappingCompiler(null, instructions);
            var mappings = compiler.BuildMappings();
            var alterations = new ConfigurationAlterations(mappings, instructions);
            var injector = new ConfigurationModifier(alterations);

            injector.Inject(cfg);

            return cfg;
        }
    }

    public class ConfigurationModifier
    {
        readonly ConfigurationAlterations alterations;

        public ConfigurationModifier(ConfigurationAlterations alterations)
        {
            this.alterations = alterations;
        }

        public void Inject(Configuration cfg)
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

    /// <summary>
    /// Container for changes that need to be applied to a configuration
    /// </summary>
    public class ConfigurationAlterations
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

        public ConfigurationAlterations(HibernateMapping mappings, IPersistenceInstructions instructions)
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