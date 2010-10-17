using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.Utils;
using FluentNHibernate.Utils.Reflection;
using NHibernate.Cfg;

namespace FluentNHibernate
{
    public interface IMappingCompiler
    {
        IEnumerable<ITopMapping> Compile(IMappingAction action);
    }

    public class MappingCompiler : IMappingCompiler
    {
        readonly IAutomapper automapper;
        readonly IPersistenceInstructions instructions;

        public MappingCompiler(IAutomapper automapper, IPersistenceInstructions instructions)
        {
            this.automapper = automapper;
            this.instructions = instructions;
        }

        public HibernateMapping BuildMappings()
        {
            var actions = instructions.GetActions();
            var bucket = CompileActions(actions);

            instructions.Visitors
                .Each(x => x.Visit(bucket));

            var hbm = new HibernateMapping();

            instructions.Visitors
                .Each(x => x.Visit(hbm));

            bucket.Classes
                .Each(hbm.AddClass);
            bucket.Filters
                .Each(hbm.AddFilter);
            bucket.Imports
                .Each(hbm.AddImport);

            return hbm;
        }

        MappingBucket CompileActions(IEnumerable<IMappingAction> actions)
        {
            var mappings = actions.SelectMany(x => Compile(x));
            var bucket = new MappingBucket();

            mappings.Each(x => x.AddTo(bucket));

            return bucket;
        }

        public virtual IEnumerable<ITopMapping> AutoMap(AutomapAction action)
        {
            var mainInstructions = instructions.AutomappingInstructions;
            var targets = action.GetMappingTargets(mainInstructions);

            return automapper.Map(targets);
        }

        public virtual IEnumerable<ITopMapping> ManualMap(ManualAction action)
        {
            return new[] { action.GetMapping() };
        }

        public IEnumerable<ITopMapping> Compile(IMappingAction action)
        {
            if (action is ManualAction)
                return ManualMap((ManualAction)action);
            if (action is AutomapAction)
                return AutoMap((AutomapAction)action);

            throw new InvalidOperationException(string.Format("Unrecognised action '{0}'", action.GetType().FullName));
        }
    }
    public interface IPersistenceModel : IPersistenceInstructionGatherer
    {}

    public class PersistenceModel : IPersistenceModel
    {
        readonly PersistenceInstructionGatherer gatherer = new PersistenceInstructionGatherer();
        protected IDiagnosticLogger log = new NullDiagnosticsLogger();

        public void SetLogger(IDiagnosticLogger logger)
        {
            log = logger;
        }

        public AutomappingBuilder AutoMap
        {
            get { return new AutomappingBuilder(gatherer.Automapping); }
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
            get { return new ConventionContainer(gatherer.Conventions); }
        }

        protected void AddMappingsFromThisAssembly()
        {
            var assembly = ReflectionHelper.FindTheCallingAssembly();
            AddMappingsFromAssembly(assembly);
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            AddMappingsFromSource(new AssemblyTypeSource(assembly));
        }

        public void Add(IProvider provider)
        {
            gatherer.AddProviderInstance(provider);
        }

        public void AddMappingsFromSource(ITypeSource source)
        {
            gatherer.AddSource(source);

            log.LoadedFluentMappingsFromSource(source);
        }

        public void AddMappings(params IProvider[] providers)
        {
            providers.Each(gatherer.AddProviderInstance);
        }

        IPersistenceInstructions IPersistenceInstructionGatherer.GetInstructions()
        {
            return gatherer.GetInstructions();
        }

        public void WriteMappingsTo(string exportPath)
        {
            gatherer.UseExporter(new PathExporter(Path.Combine(exportPath, GetType().Name + ".hbm.xml")));
        }

        public void WriteMappingsTo(TextWriter textWriter)
        {
            gatherer.UseExporter(new TextWriterExporter(textWriter));
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
}