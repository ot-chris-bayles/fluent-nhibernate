using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Mapping;
using FluentNHibernate.Utils;
using NHibernate.Cfg;

namespace FluentNHibernate.Infrastructure
{
    public interface IPersistenceInstructionGatherer
    {
        IPersistenceInstructions GetInstructions();
    }

    public class PersistenceInstructionGatherer : IPersistenceInstructionGatherer
    {
        readonly List<ITypeSource> sources = new List<ITypeSource>();
        readonly List<IProvider> instances = new List<IProvider>();
        readonly ConventionsCollection conventions = new ConventionsCollection();
        readonly IAutomappingInstructions automapping = new AutomappingInstructions();
        IPersistenceInstructionGatherer baseModel;
        IPersistenceInstructionGatherer extendedModel;
        Action<Configuration> preConfigure;
        Action<Configuration> postConfigure;
        IDatabaseConfiguration databaseConfiguration;
        IExporter exporter;
        IDiagnosticLogger log = new NullDiagnosticsLogger();

        public void SetLogger(IDiagnosticLogger logger)
        {
            log = logger;
        }

        public IPersistenceInstructions GetInstructions()
        {
            var instructions = CreateInstructions();

            if (baseModel != null)
                instructions = new DerivedPersistenceInstructions(baseModel.GetInstructions(), instructions);

            if (extendedModel != null)
                instructions = new ExtendedPersistenceInstructions(extendedModel.GetInstructions(), instructions);

            log.Flush();

            return instructions;
        }

        public ConventionsCollection Conventions
        {
            get { return conventions; }
        }

        public IAutomappingInstructions Automapping
        {
            get { return automapping; }
        }

        public virtual void AddProviderInstance(IProvider provider)
        {
            instances.Add(provider);
        }

        public virtual void AddSource(ITypeSource source)
        {
            sources.Add(source);
        }

        IPersistenceInstructions CreateInstructions()
        {
            var instructions = new PersistenceInstructions();
            var actions = GetActions();

            instructions.AddActions(actions);
            instructions.UseConventions(conventions);

            if (databaseConfiguration != null)
                instructions.UseDatabaseConfiguration(databaseConfiguration);

            if (preConfigure != null)
                instructions.UsePreConfigureAction(preConfigure);

            if (postConfigure != null)
                instructions.UsePostConfigureAction(postConfigure);

            if (automapping != null)
                instructions.UseAutomappingInstructions(automapping);

            if (exporter != null)
                instructions.UseExporter(exporter);

            return instructions;
        }

        public void UseBaseModel(IPersistenceInstructionGatherer instrucionGather)
        {
            baseModel = instrucionGather;
        }

        public void UseExtendModel(IPersistenceInstructionGatherer instructionGatherer)
        {
            extendedModel = instructionGatherer;
        }

        public void UseExporter(IExporter mappingExporter)
        {
            exporter = mappingExporter;
        }

        public void UsePreConfigure(Action<Configuration> preConfigureAction)
        {
            preConfigure = preConfigureAction;
        }

        public void UsePostConfigure(Action<Configuration> postConfigureAction)
        {
            postConfigure = postConfigureAction;
        }

        public void UseDatabaseConfiguration(IDatabaseConfiguration dbCfg)
        {
            databaseConfiguration = dbCfg;
        }

        IEnumerable<IMappingAction> GetActions()
        {
            var actionsFromInstances = instances.Select(x => x.GetAction());
            var actionsFromProviders = GetProvidersFromSources().Select(x => x.GetAction());

            instances.Each(x => log.FluentMappingDiscovered(x.GetType()));

            // all pre-instantiated providers);)
            foreach (var action in actionsFromInstances)
                yield return action;

            // all providers found by scanning
            foreach (var action in actionsFromProviders)
                yield return action;

            // all types for mapping by the automapper
            if (automapping != null)
            {
                var actionsForAutomapping = automapping.GetTypesToMap().Select(x => new PartialAutomapAction(x, new AutomappingEntitySetup()));

                foreach (var action in actionsForAutomapping)
                    yield return action;
            }
        }

        IEnumerable<IProvider> GetProvidersFromSources()
        {
            // TODO: Add user-defined filtering in here
            return sources
                .SelectMany(x =>
                {
                    log.LoadedFluentMappingsFromSource(x);

                    return x.GetTypes();
                })
                .Where(x => x.HasInterface<IProvider>())
                .Select(x =>
                {
                    log.FluentMappingDiscovered(x);

                    return x.InstantiateUsingParameterlessConstructor();
                })
                .Cast<IProvider>();
        }
    }

    public interface IExporter
    {
        void Export(XmlDocument hbm);
    }

    public interface IAutomappingInstructions
    {
        IAutomappingConfiguration Configuration { get; }
        void UseConfiguration(IAutomappingConfiguration cfg);
        void AddSource(ITypeSource source);
        IEnumerable<Type> GetTypesToMap();
    }

    public interface IEntityAutomappingConfiguration
    {
        /// <summary>
        /// Determines whether a member of a type should be auto-mapped.
        /// Override to restrict which members are considered in automapping.
        /// </summary>
        /// <remarks>
        /// You normally want to override this method to restrict which members will be
        /// used for mapping. This method will be called for every property, field, and method
        /// on your types.
        /// </remarks>
        /// <example>
        /// // all writable public properties:
        /// return member.IsProperty &amp;&amp; member.IsPublic &amp;&amp; member.CanWrite;
        /// </example>
        /// <param name="member">Member to map</param>
        /// <returns>Should map member</returns>
        bool ShouldMap(Member member);
    }

    public interface IEntityAutomappingInstructions
    {
        IEntityAutomappingConfiguration Configuration { get; }
    }

    // TODO: Use this class to combine the main instructions with individual
    // entity instructions. Currently just delegates everything to the main instructions
    public class EntityAutomappingInstructions : IEntityAutomappingInstructions
    {
        readonly IAutomappingInstructions mainInstructions;
        readonly AutomappingEntitySetup setup;

        public EntityAutomappingInstructions(IAutomappingInstructions mainInstructions, AutomappingEntitySetup setup)
        {
            this.mainInstructions = mainInstructions;
            this.setup = setup;
        }

        public IEntityAutomappingConfiguration Configuration
        {
            get { return GetConfiguration(); }
        }

        IEntityAutomappingConfiguration GetConfiguration()
        {
            var innerCfg = setup.Configuration ?? mainInstructions.Configuration ?? new DefaultAutomappingConfiguration();

            if (!setup.Exclusions.Any())
                return innerCfg;

            return new ExclusionWrappedConfiguration(innerCfg, setup.Exclusions);
        }

        public class ExclusionWrappedConfiguration : IEntityAutomappingConfiguration
        {
            readonly IAutomappingConfiguration innerCfg;
            readonly IEnumerable<Predicate<Member>> exclusions;

            public ExclusionWrappedConfiguration(IAutomappingConfiguration innerCfg, IEnumerable<Predicate<Member>> exclusions)
            {
                this.innerCfg = innerCfg;
                this.exclusions = exclusions;
            }

            public bool ShouldMap(Member member)
            {
                return innerCfg.ShouldMap(member) && !exclusions.Any(x => x(member));
            }

            public IEnumerable<IAutomappingStep> GetMappingSteps(AutoMapper mapper, IConventionFinder conventionFinder)
            {
                return innerCfg.GetMappingSteps(mapper, conventionFinder);
            }
        }
    }


    public class AutomappingInstructions : IAutomappingInstructions
    {
        readonly List<ITypeSource> sources = new List<ITypeSource>();
        public IAutomappingConfiguration Configuration { get; private set; }

        public AutomappingInstructions()
        {
            Configuration = new DefaultAutomappingConfiguration();
        }

        public void UseConfiguration(IAutomappingConfiguration cfg)
        {
            Configuration = cfg;
        }

        public void AddSource(ITypeSource source)
        {
            sources.Add(source);
        }

        public IEnumerable<Type> GetTypesToMap()
        {
            return sources
                .SelectMany(x => x.GetTypes())
                .Where(x => Configuration.ShouldMap(x))
                .ToArray();
        }
    }

    public class NullAutomappingInstructions : IAutomappingInstructions
    {
        public IAutomappingConfiguration Configuration
        {
            get { return null; }
        }

        public void UseConfiguration(IAutomappingConfiguration cfg)
        {
            throw new NotSupportedException("Cannot set configuration in null instructions");
        }

        public void AddSource(ITypeSource source)
        {
            throw new NotSupportedException("Cannot add source in null instructions");
        }

        public IEnumerable<Type> GetTypesToMap()
        {
            return new Type[0];
        }
    }
}