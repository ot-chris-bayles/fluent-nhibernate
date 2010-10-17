namespace FluentNHibernate.Infrastructure
{
    /// <summary>
    /// Persistence instructions extended by another set of instructions
    /// </summary>
    public class ExtendedPersistenceInstructions : CombinatorPersistenceInstructions
    {
        public ExtendedPersistenceInstructions(IPersistenceInstructions instructions, IPersistenceInstructions baseInstructions)
            : base(baseInstructions, instructions)
        { }
    }
}