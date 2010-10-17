using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Output;
using FluentNHibernate.Utils;
using FluentNHibernate.Visitors;
using NHibernate.Cfg;

namespace FluentNHibernate.Automapping
{
    public class AutoPersistenceModel
    {
        readonly IList<IProvider> classProviders = new List<IProvider>();
        readonly IList<IFilterDefinition> filterDefinitions = new List<IFilterDefinition>();
        readonly IList<IIndeterminateSubclassMappingProvider> subclassProviders = new List<IIndeterminateSubclassMappingProvider>();
        readonly IList<IExternalComponentMappingProvider> componentProviders = new List<IExternalComponentMappingProvider>();
        readonly IList<IMappingModelVisitor> visitors = new List<IMappingModelVisitor>();
        readonly ConventionsCollection conventions = new ConventionsCollection();
        IEnumerable<HibernateMapping> compiledMappings;
        ValidationVisitor validationVisitor;
        PairBiDirectionalManyToManySidesDelegate BiDirectionalManyToManyPairer { get; set; }
        IDiagnosticMessageDespatcher diagnosticDespatcher = new DefaultDiagnosticMessageDespatcher();
        IDiagnosticLogger log = new NullDiagnosticsLogger();
        
        public AutoPersistenceModel()
        {
            expressions = new AutoMappingExpressions();
            cfg = new ExpressionBasedAutomappingConfiguration(expressions);
            autoMapper = new AutoMapper(cfg, new ConventionFinder(conventions), inlineOverrides);

            InitialiseVisitors();
        }

        public AutoPersistenceModel(IAutomappingConfiguration cfg)
        {
            this.cfg = cfg;
            autoMapper = new AutoMapper(cfg, new ConventionFinder(conventions), inlineOverrides);

            InitialiseVisitors();
        }

        private void InitialiseVisitors()
        {
            BiDirectionalManyToManyPairer = (c, o, w) => { };

            visitors.Add(new SeparateSubclassVisitor());
            visitors.Add(new ComponentReferenceResolutionVisitor());
            visitors.Add(new ComponentColumnPrefixVisitor());
            visitors.Add(new RelationshipPairingVisitor(BiDirectionalManyToManyPairer));
            visitors.Add(new ManyToManyTableNameVisitor());
            visitors.Add(new ConventionVisitor(new ConventionFinder(conventions)));
            visitors.Add(new RelationshipKeyPairingVisitor());
            visitors.Add((validationVisitor = new ValidationVisitor()));
        }

        public void SetLogger(IDiagnosticLogger logger)
        {
            log = logger;
        }

        protected void AddMappingsFromThisAssembly()
        {
            var assembly = FindTheCallingAssembly();
            AddMappingsFromAssembly(assembly);
        }

        public void AddMappingsFromAssembly(Assembly assembly)
        {
            AddMappingsFromSource(new AssemblyTypeSource(assembly));
        }

        public void AddMappingsFromSource(ITypeSource source)
        {
            source.GetTypes()
                .Where(x => IsMappingOf<IProvider>(x) ||
                            IsMappingOf<IIndeterminateSubclassMappingProvider>(x) ||
                            IsMappingOf<IExternalComponentMappingProvider>(x) ||
                            IsMappingOf<IFilterDefinition>(x))
                .Each(Add);

            log.LoadedFluentMappingsFromSource(source);
        }

        private static Assembly FindTheCallingAssembly()
        {
            StackTrace trace = new StackTrace(Thread.CurrentThread, false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public void Add(IProvider provider)
        {
            classProviders.Add(provider);
        }

        public void Add(IIndeterminateSubclassMappingProvider provider)
        {
            subclassProviders.Add(provider);
        }

        public void Add(IFilterDefinition definition)
        {
            filterDefinitions.Add(definition);
        }

        public void Add(IExternalComponentMappingProvider provider)
        {
            componentProviders.Add(provider);
        }

        public void Add(Type type)
        {
            var mapping = type.InstantiateUsingParameterlessConstructor();

            if (mapping is IProvider)
            {
                log.FluentMappingDiscovered(type);
                Add((IProvider)mapping);
            }
            else if (mapping is IFilterDefinition)
                Add((IFilterDefinition)mapping);
            else
                throw new InvalidOperationException("Unsupported mapping type '" + type.FullName + "'");
        }

        private bool IsMappingOf<T>(Type type)
        {
            return !type.IsGenericType && typeof(T).IsAssignableFrom(type);
        }

        public IEnumerable<HibernateMapping> BuildMappings()
        {
            CompileMappings();

            var bucket = new MappingBucket();

            AddMappingsToBucket(bucket);
            ApplyVisitors(bucket);

            log.Flush();

            if (MergeMappings)
            {
                var hbm = new HibernateMapping();

                bucket.Classes
                    .Each(hbm.AddClass);

                return new[] {hbm};
            }
            
            return bucket.Classes
                .Select(x =>
                {
                    var hbm = new HibernateMapping();

                    hbm.AddClass(x);

                    return hbm;
                })
                .ToArray();
        }

        public bool MergeMappings { get; set; }

        private void AddMappingsToBucket(MappingBucket bucket)
        {
            foreach (var classMap in classProviders)
            {
                var manualAction = classMap.GetAction() as ManualAction;

                if (manualAction == null)
                    throw new InvalidOperationException("Unable to process classMap");

                bucket.Classes.Add((ClassMapping)manualAction.GetMapping());
            }

            foreach (var filterDefinition in filterDefinitions)
            {
                bucket.Filters.Add(filterDefinition.GetFilterMapping());
            }
        }

        private void ApplyVisitors(MappingBucket bucket)
        {
            foreach (var visitor in visitors)
                visitor.Visit(bucket);
        }

        private void EnsureMappingsBuilt()
        {
            if (compiledMappings != null) return;

            compiledMappings = BuildMappings();
        }

        private string DetermineMappingFileName(HibernateMapping mapping)
        {
            if (mapping.Classes.Count() > 0)
                return GetMappingFileName();

            return "filter-def." + mapping.Filters.First().Name + ".hbm.xml";
        }

        public void WriteMappingsTo(string folder)
        {
            WriteMappingsTo(mapping => new XmlTextWriter(Path.Combine(folder, DetermineMappingFileName(mapping)), Encoding.Default), true);
        }

        public void WriteMappingsTo(TextWriter writer)
        {
            WriteMappingsTo(_ => new XmlTextWriter(writer), false);
        }

        private void WriteMappingsTo(Func<HibernateMapping, XmlTextWriter> writerBuilder, bool shouldDispose)
        {
            EnsureMappingsBuilt();

            foreach (HibernateMapping mapping in compiledMappings)
            {
                var serializer = new MappingXmlSerializer();
                var document = serializer.Serialize(mapping);

                XmlTextWriter xmlWriter = null;

                try
                {
                    xmlWriter = writerBuilder(mapping);
                    xmlWriter.Formatting = Formatting.Indented;
                    document.WriteTo(xmlWriter);
                }
                finally
                {
                    if (shouldDispose && xmlWriter != null)
                        xmlWriter.Close();
                }
            }
        }

        public void Configure(Configuration cfg)
        {
            CompileMappings();
            
            EnsureMappingsBuilt();

            foreach (var mapping in compiledMappings.Where(m => m.Classes.Count() == 0))
            {
                var serializer = new MappingXmlSerializer();
                XmlDocument document = serializer.Serialize(mapping);
                cfg.AddDocument(document);
            }

            foreach (var mapping in compiledMappings.Where(m => m.Classes.Count() > 0))
            {
                var serializer = new MappingXmlSerializer();
                XmlDocument document = serializer.Serialize(mapping);

                if (cfg.GetClassMapping(mapping.Classes.First().Type) == null)
                    cfg.AddDocument(document);
            }
        }

        public bool ContainsMapping(Type type)
        {
            return classProviders.Any(x => x.GetType() == type) ||
                filterDefinitions.Any(x => x.GetType() == type) ||
                subclassProviders.Any(x => x.GetType() == type) ||
                componentProviders.Any(x => x.GetType() == type);
        }

        /// <summary>
        /// Gets or sets whether validation of mappings is performed. 
        /// </summary>
        public bool ValidationEnabled
        {
            get { return validationVisitor.Enabled; }
            set { validationVisitor.Enabled = value; }
        }
    
        readonly IAutomappingConfiguration cfg;
        readonly AutoMappingExpressions expressions;
        readonly AutoMapper autoMapper;
        readonly List<ITypeSource> sources = new List<ITypeSource>();
        Func<Type, bool> whereClause;
        readonly List<AutoMapType> mappingTypes = new List<AutoMapType>();
        bool autoMappingsCreated;
        readonly AutoMappingAlterationCollection alterations = new AutoMappingAlterationCollection();
        readonly List<InlineOverride> inlineOverrides = new List<InlineOverride>();
        readonly List<Type> ignoredTypes = new List<Type>();
        readonly List<Type> includedTypes = new List<Type>();

        /// <summary>
        /// Specify alterations to be used with this AutoPersisteceModel
        /// </summary>
        /// <param name="alterationDelegate">Lambda to declare alterations</param>
        /// <returns>AutoPersistenceModel</returns>
        public AutoPersistenceModel Alterations(Action<AutoMappingAlterationCollection> alterationDelegate)
        {
            alterationDelegate(alterations);
            return this;
        }

        /// <summary>
        /// Use auto mapping overrides defined in the assembly of T.
        /// </summary>
        /// <typeparam name="T">Type to get assembly from</typeparam>
        /// <returns>AutoPersistenceModel</returns>
        public AutoPersistenceModel UseOverridesFromAssemblyOf<T>()
        {
            return UseOverridesFromAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Use auto mapping overrides defined in the assembly of T.
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <returns>AutoPersistenceModel</returns>
        public AutoPersistenceModel UseOverridesFromAssembly(Assembly assembly)
        {
            alterations.Add(new AutoMappingOverrideAlteration(assembly));
            return this;
        }

        /// <summary>
        /// Alter convention discovery
        /// </summary>
        public SetupConventionContainer<AutoPersistenceModel> Conventions
        {
            get { return new SetupConventionContainer<AutoPersistenceModel>(this, new ConventionContainer(conventions, log)); }
        }

        /// <summary>
        /// Alter some of the configuration options that control how the automapper works.
        /// Depreciated in favour of supplying your own IAutomappingConfiguration instance to AutoMap: <see cref="AutoMap.AssemblyOf{T}(FluentNHibernate.Automapping.IAutomappingConfiguration)"/>.
        /// Cannot be used in combination with a user-defined configuration.
        /// </summary>
        [Obsolete("Depreciated in favour of supplying your own IAutomappingConfiguration instance to AutoMap: AutoMap.AssemblyOf<T>(your_configuration_instance)")]
        public AutoPersistenceModel Setup(Action<AutoMappingExpressions> expressionsAction)
        {
            if (HasUserDefinedConfiguration)
                throw new InvalidOperationException("Cannot use Setup method when using a user-defined IAutomappingConfiguration instance.");

            expressionsAction(expressions);
            return this;
        }

        /// <summary>
        /// Supply a criteria for which types will be mapped.
        /// Cannot be used in combination with a user-defined configuration.
        /// </summary>
        /// <param name="where">Where clause</param>
        public AutoPersistenceModel Where(Func<Type, bool> where)
        {
            if (HasUserDefinedConfiguration)
                throw new InvalidOperationException("Cannot use Where method when using a user-defined IAutomappingConfiguration instance.");

            whereClause = where;
            return this;
        }

        private void CompileMappings()
        {
            if (autoMappingsCreated)
                return;

            alterations.Apply(this);

            var types = sources
                .SelectMany(x => x.GetTypes())
                .OrderBy(x => InheritanceHierarchyDepth(x));

            foreach (var type in types)
            {
                // skipped by user-defined configuration criteria
                if (!cfg.ShouldMap(type))
                {
                    log.AutomappingSkippedType(type, "Skipped by result of IAutomappingConfiguration.ShouldMap(Type)");
                    continue;
                }
                // skipped by inline where clause
                if (whereClause != null && !whereClause(type))
                {
                    log.AutomappingSkippedType(type, "Skipped by Where clause");
                    continue;
                }
                // skipped because either already mapped elsewhere, or not valid for mapping            
                if (!ShouldMap(type))
                    continue;

                mappingTypes.Add(new AutoMapType(type));
            }

            log.AutomappingCandidateTypes(mappingTypes.Select(x => x.Type));

            foreach (var type in mappingTypes)
            {
                if (type.IsMapped) continue;

                AddMapping(type.Type);
            }

            autoMappingsCreated = true;
        }

        private int InheritanceHierarchyDepth(Type type)
        {
            var depth = 0;
            var parent = type;

            while (parent != null && parent != typeof(object))
            {
                parent = parent.BaseType;
                depth++;
            }

            return depth;
        }

        private void AddMapping(Type type)
        {
            log.BeginAutomappingType(type);

            Type typeToMap = GetTypeToMap(type);
            var mapping = autoMapper.Map(typeToMap, mappingTypes);

            Add(new PassThroughMappingProvider(mapping));
        }

        private Type GetTypeToMap(Type type)
        {
            while (ShouldMapParent(type))
			{
				type = type.BaseType;
			}

			return type;
        }

        private bool ShouldMapParent(Type type)
        {
            return ShouldMap(type.BaseType) && !cfg.IsConcreteBaseType(type.BaseType);
        }

        private bool ShouldMap(Type type)
        {
            if (includedTypes.Contains(type))
                return true; // inclusions take precedence over everything
            if (ignoredTypes.Contains(type))
            {
                log.AutomappingSkippedType(type, "Skipped by IgnoreBase");
                return false; // excluded
            }
            if (type.IsGenericType && ignoredTypes.Contains(type.GetGenericTypeDefinition()))
            {
                log.AutomappingSkippedType(type, "Skipped by IgnoreBase");
                return false; // generic definition is excluded
            }
            if (type.IsAbstract && cfg.AbstractClassIsLayerSupertype(type))
            {
                log.AutomappingSkippedType(type, "Skipped by IAutomappingConfiguration.AbstractClassIsLayerSupertype(Type)");
                return false; // is abstract and a layer supertype
            }
            if (cfg.IsComponent(type))
            {
                log.AutomappingSkippedType(type, "Skipped by IAutomappingConfiguration.IsComponent(Type)");
                return false; // skipped because we don't want to map components as entities
            }
            if (type == typeof(object))
                return false; // object!

            return true;
        }

        /// <summary>
        /// Adds all entities from a specific assembly.
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        public AutoPersistenceModel AddEntityAssembly(Assembly assembly)
        {
            return AddTypeSource(new AssemblyTypeSource(assembly));
        }

        /// <summary>
        /// Adds all entities from the <see cref="ITypeSource"/>.
        /// </summary>
        /// <param name="source"><see cref="ITypeSource"/> to load from</param>
        public AutoPersistenceModel AddTypeSource(ITypeSource source)
        {
            sources.Add(source);
            return this;
        }

        internal void AddOverride(Type type, Action<object> action)
        {
            inlineOverrides.Add(new InlineOverride(type, action));
        }

        /// <summary>
        /// Override the mapping of a specific entity.
        /// </summary>
        /// <remarks>This may affect subclasses, depending on the alterations you do.</remarks>
        /// <typeparam name="T">Entity who's mapping to override</typeparam>
        /// <param name="populateMap">Lambda performing alterations</param>
        public AutoPersistenceModel Override<T>(Action<AutoMapping<T>> populateMap)
        {
            inlineOverrides.Add(new InlineOverride(typeof(T), x =>
            {
                if (x is AutoMapping<T>)
                    populateMap((AutoMapping<T>)x);
            }));

            return this;
        }

        /// <summary>
        /// Override all mappings.
        /// </summary>
        /// <remarks>Currently only supports ignoring properties on all entities.</remarks>
        /// <param name="alteration">Lambda performing alterations</param>
        public AutoPersistenceModel OverrideAll(Action<IPropertyIgnorer> alteration)
        {
            inlineOverrides.Add(new InlineOverride(typeof(object), x =>
            {
                if (x is IPropertyIgnorer)
                    alteration((IPropertyIgnorer)x);
            }));

            return this;
        }

        /// <summary>
        /// Ignore a base type. This removes it from any mapped inheritance hierarchies, good for non-abstract layer
        /// supertypes.
        /// </summary>
        /// <typeparam name="T">Type to ignore</typeparam>
        public AutoPersistenceModel IgnoreBase<T>()
        {
            return IgnoreBase(typeof(T));
        }

        /// <summary>
        /// Ignore a base type. This removes it from any mapped inheritance hierarchies, good for non-abstract layer
        /// supertypes.
        /// </summary>
        /// <param name="baseType">Type to ignore</param>
        public AutoPersistenceModel IgnoreBase(Type baseType)
        {
            ignoredTypes.Add(baseType);
            return this;
        }

        /// <summary>
        /// Explicitly includes a type to be used as part of a mapped inheritance hierarchy.
        /// </summary>
        /// <remarks>
        /// Abstract classes are probably what you'll be using this method with. Fluent NHibernate considers abstract
        /// classes to be layer supertypes, so doesn't automatically map them as part of an inheritance hierarchy. You
        /// can use this method to override that behavior for a specific type; otherwise you should consider using the
        /// <see cref="IAutomappingConfiguration.AbstractClassIsLayerSupertype"/> setting.
        /// </remarks>
        /// <typeparam name="T">Type to include</typeparam>
        public AutoPersistenceModel IncludeBase<T>()
        {
            return IncludeBase(typeof(T));
        }

        /// <summary>
        /// Explicitly includes a type to be used as part of a mapped inheritance hierarchy.
        /// </summary>
        /// <remarks>
        /// Abstract classes are probably what you'll be using this method with. Fluent NHibernate considers abstract
        /// classes to be layer supertypes, so doesn't automatically map them as part of an inheritance hierarchy. You
        /// can use this method to override that behavior for a specific type; otherwise you should consider using the
        /// <see cref="AutoMappingExpressions.AbstractClassIsLayerSupertype"/> setting.
        /// </remarks>
        /// <param name="baseType">Type to include</param>
        public AutoPersistenceModel IncludeBase(Type baseType)
        {
            includedTypes.Add(baseType);
            return this;
        }

        protected string GetMappingFileName()
        {
            return "AutoMappings.hbm.xml";
        }

        bool HasUserDefinedConfiguration
        {
            get { return !(cfg is ExpressionBasedAutomappingConfiguration); }
        }
    }
}
