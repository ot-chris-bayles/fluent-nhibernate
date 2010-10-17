using NHibernate.Cfg;

namespace FluentNHibernate.Infrastructure
{
    /// <summary>
    /// Injects a PersistenceModel and it's mappings/settings into a Configuration
    /// instance.
    /// </summary>
    public class PersistenceModelInjector
    {
        public void Inject(IPersistenceModel model, Configuration cfg)
        {
            Inject((IPersistenceInstructionGatherer)model, cfg);
        }

        public void Inject(IPersistenceInstructionGatherer instructionGatherer, Configuration cfg)
        {
            var instructions = instructionGatherer.GetInstructions();

            Inject(instructions, cfg);
        }

        public void Inject(IPersistenceInstructions instructions, Configuration cfg)
        {
            // TODO: move this out of an extension method
            //var automapper = new AutomapperV2(new ConventionFinder(instructions.Conventions));
            IMappingCompiler compiler = new MappingCompiler(null, instructions);
            var mappings = compiler.Compile();
            var modificationSet = new CfgModificationSet(mappings, instructions);
            var cfgModifier = new CfgModificationApplier(modificationSet);

            cfgModifier.ApplyModifications(cfg);
        }
    }
}