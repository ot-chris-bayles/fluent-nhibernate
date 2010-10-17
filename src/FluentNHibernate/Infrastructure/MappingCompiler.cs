using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Infrastructure
{
    public interface IMappingCompiler
    {
        HibernateMapping Compile();
    }

    public class MappingCompiler : IMappingCompiler
    {
        readonly IPersistenceInstructions instructions;
        readonly IActionCompiler actionCompiler;

        public MappingCompiler(IAutomapper automapper, IPersistenceInstructions instructions)
        {
            actionCompiler = new ActionCompiler(automapper, this.instructions);
            this.instructions = instructions;
        }

        public HibernateMapping Compile()
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
            var mappings = actions.SelectMany(x => actionCompiler.Compile(x));
            var bucket = new MappingBucket();

            mappings.Each(x => x.AddTo(bucket));

            return bucket;
        }
    }
}