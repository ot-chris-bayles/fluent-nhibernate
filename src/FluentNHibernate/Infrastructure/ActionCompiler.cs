using System;
using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace FluentNHibernate.Infrastructure
{
    public interface IActionCompiler
    {
        IEnumerable<ITopMapping> Compile(IMappingAction action);
    }

    public class ActionCompiler : IActionCompiler
    {
        readonly IAutomapper automapper;
        readonly IPersistenceInstructions instructions;

        public ActionCompiler(IAutomapper automapper, IPersistenceInstructions instructions)
        {
            this.automapper = automapper;
            this.instructions = instructions;
        }

        public IEnumerable<ITopMapping> Compile(IMappingAction action)
        {
            if (action is ManualAction)
                return ManualMap((ManualAction)action);
            if (action is AutomapAction)
                return AutoMap((AutomapAction)action);

            throw new InvalidOperationException(string.Format("Unrecognised action '{0}'", action.GetType().FullName));
        }

        IEnumerable<ITopMapping> AutoMap(AutomapAction action)
        {
            var mainInstructions = instructions.AutomappingInstructions;
            var targets = action.GetMappingTargets(mainInstructions);

            return automapper.Map(targets);
        }

        static IEnumerable<ITopMapping> ManualMap(ManualAction action)
        {
            return new[] { action.GetMapping() };
        }
    }
}