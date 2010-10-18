namespace FluentNHibernate.Testing.Fixtures
{
    public class TestPersistenceModel : PersistenceModel
    {
        public TestPersistenceModel()
        {
            Scan
                .TheCallingAssembly()
                .ForMappings();
        }
    }
}