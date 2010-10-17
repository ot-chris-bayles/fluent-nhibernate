namespace FluentNHibernate.Infrastructure
{
    /// <summary>
    /// Persistence instructions derived from another set of instructions
    /// </summary>
    public class DerivedPersistenceInstructions : CombinatorPersistenceInstructions
    {
        public DerivedPersistenceInstructions(IPersistenceInstructions baseInstructions, IPersistenceInstructions instructions)
            : base(baseInstructions, instructions)
        { }
    }
}