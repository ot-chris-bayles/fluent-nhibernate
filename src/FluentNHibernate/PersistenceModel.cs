using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using FluentNHibernate.Utils;
using FluentNHibernate.Utils.Reflection;
using NHibernate.Cfg;

namespace FluentNHibernate
{
    public interface IPersistenceModel : IPersistenceInstructionGatherer
    {}

    public class PersistenceModel : IPersistenceModel
    {
        readonly PersistenceInstructionGatherer gatherer = new PersistenceInstructionGatherer();
        IDiagnosticLogger log = new NullDiagnosticsLogger();

        public void SetLogger(IDiagnosticLogger logger)
        {
            log = logger;
            gatherer.SetLogger(log);
        }

        private AutomappingBuilder AutoMap
        {
            get { return new AutomappingBuilder(gatherer.Automapping); }
        }

        public SourceScanner Scan
        {
            get { return new SourceScanner(gatherer, log); }
        }

        public void InjectMapping(IProvider provider)
        {
            gatherer.AddProviderInstance(provider);
        }

        public ImportPart Import<T>()
        {
            var import = new ImportPart(typeof(T));

            gatherer.AddProviderInstance(import);

            return import;
        }

        /// <summary>
        /// Specify an action that is executed before any changes are made by Fluent NHibernate to
        /// the NHibernate <see cref="Configuration"/> instance. This method can only be called once,
        /// multiple calls will result in only the last call being used.
        /// </summary>
        /// <example>
        /// 1:
        /// <code>
        /// PreConfigure(cfg =>
        /// {
        /// });
        /// </code>
        /// 
        /// 2:
        /// <code>
        /// PreConfigure(someMethod);
        /// 
        /// private void someMethod(Configuration cfg)
        /// {
        /// }
        /// </code>
        /// </example>
        /// <param name="preConfigureAction">Action to execute</param>
        protected void PreConfigure(Action<Configuration> preConfigureAction)
        {
            gatherer.UsePreConfigure(preConfigureAction);
        }

        /// <summary>
        /// Specify an action that is executed after all other changes have been made by Fluent
        /// NHibernate to the NHibernate <see cref="Configuration"/> instance. This method can
        /// only be called once, multiple calls will result in only the last call being used.
        /// </summary>
        /// <example>
        /// See <see cref="PreConfigure"/> for examples.
        /// </example>
        /// <param name="postConfigureAction">Action to execute</param>
        protected void PostConfigure(Action<Configuration> postConfigureAction)
        {
            gatherer.UsePostConfigure(postConfigureAction);
        }

        /// <summary>
        /// Base this configuration on another <see cref="IPersistenceModel"/>'s setup.
        /// Use this method to "inherit" settings, that can be overwritten in your own
        /// model. This method can only be called once, multiple calls will result in only the last call
        /// being used.
        /// </summary>
        /// <param name="model">PersistenceModel to inherit settings from</param>
        protected void BaseConfigurationOn(IPersistenceModel model)
        {
            BaseConfigurationOn((IPersistenceInstructionGatherer)model);
        }

        /// <summary>
        /// Base this configuration on another <see cref="IPersistenceModel"/>'s setup.
        /// Use this method to "inherit" settings, that can be overwritten in your own
        /// model. This method can only be called once, multiple calls will result in only the last call
        /// being used.
        /// </summary>
        /// <param name="instrucionGather">PersistenceModel to inherit settings from</param>
        protected void BaseConfigurationOn(IPersistenceInstructionGatherer instrucionGather)
        {
            gatherer.UseBaseModel(instrucionGather);
        }

        /// <summary>
        /// Extend ththis configuration with another PersistenceModel's setup.
        /// Use this method to apply existing settings "on top" of your own settings. Good
        /// for if you want to pass in a "test" configuration that just alters minor settings but
        /// keeps everything else intact. This method can only be called once, multiple calls will
        /// result in only the last call being used.
        /// </summary>
        /// <param name="model">PersistenceModel to extend your own with</param>
        protected void ExtendConfigurationWith(IPersistenceModel model)
        {
            ExtendConfigurationWith((IPersistenceInstructionGatherer)model);
        }

        /// <summary>
        /// Extend ththis configuration with another PersistenceModel's setup.
        /// Use this method to apply existing settings "on top" of your own settings. Good
        /// for if you want to pass in a "test" configuration that just alters minor settings but
        /// keeps everything else intact. This method can only be called once, multiple calls will
        /// result in only the last call being used.
        /// </summary>
        /// <param name="instructionGatherer">PersistenceModel to extend your own with</param>
        protected void ExtendConfigurationWith(IPersistenceInstructionGatherer instructionGatherer)
        {
            gatherer.UseExtendModel(instructionGatherer);
        }

        /// <summary>
        /// Supply settings for the database used in the persistence of your entities.
        /// This method can only be called once, multiple calls will result in only
        /// the last call being used.
        /// </summary>
        /// <remarks>
        /// Where the instance comes from that you pass into this method
        /// is up to you. You can instantiate it yourself, or you could
        /// inject it into your <see cref="PersistenceModel"/> via a
        /// container and pass it into this method.
        /// </remarks>
        /// <example>
        /// Inline:
        /// <code>
        /// Database(new ProductionDatabaseConfiguration());
        /// </code>
        /// 
        /// Container:
        /// <code>
        /// public class MyPersistenceModel : PersistenceModel
        /// {
        ///   public MyPersistenceModel(IDatabaseConfiguration dbCfg)
        ///   {
        ///     Database(dbCfg);
        ///   }
        /// }
        /// </code>
        /// </example>
        /// <param name="dbCfg">Database configuration instance</param>
        protected void Database(IDatabaseConfiguration dbCfg)
        {
            gatherer.UseDatabaseConfiguration(dbCfg);
        }

        /// <summary>
        /// Supply settings for the database used in the persistence of your entities.
        /// This method can only be called once, multiple calls will result in only
        /// the last call being used.
        /// </summary>
        /// <example>
        /// See <see cref="Database(FluentNHibernate.Cfg.Db.IDatabaseConfiguration)"/> for examples.
        /// </example>
        /// <typeparam name="T">Type of database configuration</typeparam>
        protected void Database<T>()
            where T : IDatabaseConfiguration, new()
        {
            Database(new T());
        }

        /// <summary>
        /// Supply settings, in the form of an inline setup, for the database used in the persistence
        /// of your entities. This method can only be called once, multiple calls will result in only
        /// the last call being used.
        /// </summary>
        /// <remarks>
        /// The parameter to this method will be wrapped inside a <see cref="IDatabaseConfiguration"/>
        /// instance. This method is mainly useful for short inline database configuration.
        /// </remarks>
        /// <example>
        /// <code>
        /// Database(SQLiteConfiguration.StandardInMemory);
        /// </code>
        /// </example>
        /// <param name="db">Persistence configurer instance</param>
        protected void Database(IPersistenceConfigurer db)
        {
            gatherer.UseDatabaseConfiguration(new PreconfiguredDatabaseConfiguration(db));
        }

        public IConventionContainer Conventions
        {
            get { return new ConventionContainer(gatherer.Conventions, log); }
        }

        IPersistenceInstructions IPersistenceInstructionGatherer.GetInstructions()
        {
            return gatherer.GetInstructions();
        }

        public void WriteMappingsTo(string exportPath)
        {
            gatherer.UseExporter(new PathExporter(Path.Combine(exportPath, GetExportFileName())));
        }

        public void WriteMappingsTo(TextWriter textWriter)
        {
            gatherer.UseExporter(new TextWriterExporter(textWriter));
        }

        protected virtual string GetExportFileName()
        {
            return GetType().Name + ".hbm.xml";
        }

        class TextWriterExporter : IExporter
        {
            readonly TextWriter textWriter;

            public TextWriterExporter(TextWriter textWriter)
            {
                this.textWriter = textWriter;
            }

            public void Export(XmlDocument hbm)
            {
                textWriter.Write(hbm.InnerXml);
            }
        }

        class PathExporter : IExporter
        {
            readonly string exportPath;

            public PathExporter(string exportPath)
            {
                this.exportPath = exportPath;
            }

            public void Export(XmlDocument hbm)
            {
                File.WriteAllText(exportPath, hbm.InnerXml);
            }
        }
    }

    public class SourceScanner
    {
        readonly PersistenceInstructionGatherer gatherer;
        readonly IDiagnosticLogger log;
        readonly List<ITypeSource> sources = new List<ITypeSource>();

        public SourceScanner(PersistenceInstructionGatherer gatherer, IDiagnosticLogger log)
        {
            this.gatherer = gatherer;
            this.log = log;
        }

        public SourceScanner AssemblyContaining<T>()
        {
            return Assembly(typeof(T).Assembly);
        }

        public SourceScanner Assembly(Assembly assembly)
        {
            return Source(new AssemblyTypeSource(assembly));
        }

        public SourceScanner Source(ITypeSource source)
        {
            sources.Add(source);
            return this;
        }

        public SourceScanner TheCallingAssembly()
        {
            return Assembly(ReflectionHelper.FindTheCallingAssembly());
        }

        public SourceScanner Collection(IEnumerable<Type> collection)
        {
            return Source(new CollectionTypeSource(collection));
        }

        public void ForMappings()
        {
            sources.Each(gatherer.AddSource);
        }

        public void ForImporting()
        {}

        public void ForImporting(Action<ImportOptions> opts)
        {}

        public void ForConventions()
        {
            var conventions = new ConventionContainer(gatherer.Conventions, log);

            sources.Each(conventions.AddSource);
        }
    }

    public class ImportOptions
    {
        public void Rename<TImported>(string newName)
        {
        }
    }
}