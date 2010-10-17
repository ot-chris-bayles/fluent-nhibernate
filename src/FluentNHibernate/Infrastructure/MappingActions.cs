using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Infrastructure
{
    /// <summary>
    /// Describes how to map a set of classes. A fully-manual ClassMap would return
    /// a <see cref="ManualAction"/>; an automapped set would return a <see cref="AutomapAction"/>;
    /// and a partially automapped ClassMap would return a <see cref="PartialAutomapAction"/>.
    /// </summary>
    public interface IMappingAction
    { }

    public class PartialAutomapAction : IMappingAction
    {
        readonly ClassMapping mapping;
        readonly AutomappingEntitySetup setup;

        public PartialAutomapAction(Type type, AutomappingEntitySetup setup)
            : this(new ClassMapping { Type = type }, setup)
        { }

        public PartialAutomapAction(ClassMapping mapping, AutomappingEntitySetup setup)
        {
            this.mapping = mapping;
            this.setup = setup;
        }

        public AutomappingTarget CreateTarget(IAutomappingInstructions mainInstructions)
        {
            var instructions = new EntityAutomappingInstructions(mainInstructions, setup);

            return new AutomappingTarget(mapping.Type, mapping, instructions);
        }
    }

    public class ManualAction : IMappingAction
    {
        readonly ITopMapping mapping;

        public ManualAction(ITopMapping mapping)
        {
            this.mapping = mapping;
        }

        public ITopMapping GetMapping()
        {
            return mapping;
        }

        public override string ToString()
        {
            return "{ ManualAction: " + mapping.GetType().Name + "<" + mapping.Type.Name + "> }";
        }
    }

    public class AutomapAction : IMappingAction
    {
        readonly IEnumerable<PartialAutomapAction> actions;

        public AutomapAction(IEnumerable<PartialAutomapAction> actions)
        {
            this.actions = actions;
        }

        public static AutomapAction ComposeFrom(IEnumerable<IMappingAction> partials)
        {
            if (partials.Any(x => !(x is PartialAutomapAction)))
                throw new ArgumentException("partials must all be of type PartialAutomapAction");

            return new AutomapAction(partials.Cast<PartialAutomapAction>());
        }

        public IEnumerable<AutomappingTarget> GetMappingTargets(IAutomappingInstructions mainInstructions)
        {
            return actions.Select(x => x.CreateTarget(mainInstructions));
        }
    }
}