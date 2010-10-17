using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Visitors;
using NHibernate.Cfg;

namespace FluentNHibernate.Infrastructure
{
    public abstract class CombinatorPersistenceInstructions : IPersistenceInstructions
    {
        readonly IPersistenceInstructions baseInstructions;
        readonly IPersistenceInstructions instructions;

        public CombinatorPersistenceInstructions(IPersistenceInstructions baseInstructions, IPersistenceInstructions instructions)
        {
            this.baseInstructions = baseInstructions;
            this.instructions = instructions;
        }

        public IEnumerable<IMappingAction> GetActions()
        {
            var actions = instructions.GetActions();

            if (actions.Any())
                return actions;

            return baseInstructions.GetActions();
        }

        public ConventionsCollection Conventions
        {
            get
            {
                var conventions = instructions.Conventions;

                if (conventions.IsNotEmpty())
                    return conventions;

                return baseInstructions.Conventions;
            }
        }

        public IEnumerable<IMappingModelVisitor> Visitors
        {
            get
            {
                var visitors = instructions.Visitors;

                if (visitors.Any())
                    return visitors;

                return baseInstructions.Visitors;
            }
        }

        public IDatabaseConfiguration Database
        {
            get { return instructions.Database ?? baseInstructions.Database; }
        }

        public Action<Configuration> PreConfigure
        {
            get { return instructions.PreConfigure ?? baseInstructions.PreConfigure; }
        }

        public Action<Configuration> PostConfigure
        {
            get { return instructions.PostConfigure ?? baseInstructions.PostConfigure; }
        }

        public IAutomappingInstructions AutomappingInstructions
        {
            get
            {
                var automapping = instructions.AutomappingInstructions;

                if (automapping != null && !(automapping is NullAutomappingInstructions))
                    return automapping;

                return baseInstructions.AutomappingInstructions;
            }
        }

        public IExporter Exporter
        {
            get
            {
                var exporter = instructions.Exporter;
                
                if (exporter != null)
                    return exporter;

                return baseInstructions.Exporter;
            }
        }
    }
}