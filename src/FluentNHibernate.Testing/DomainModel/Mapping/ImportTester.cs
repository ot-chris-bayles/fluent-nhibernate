using NUnit.Framework;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class ImportTester
    {
        [Test]
        public void ShouldAddImportElementsBeforeClass()
        {
            var model = new PersistenceModel();

            model.Import<SecondMappedObject>();

            new MappingTester<MappedObject>(model)
                .ForMapping(m => m.Id(x => x.Id))
                .Element("import")
                .Exists()
                .HasAttribute("class", typeof(SecondMappedObject).AssemblyQualifiedName);
        }

        [Test]
        public void ShouldntAddImportElementsInsideClass()
        {
            var model = new PersistenceModel();

            model.Import<SecondMappedObject>();

            new MappingTester<MappedObject>(model)
                .ForMapping(m => m.Id(x => x.Id))
                .Element("class/import").DoesntExist();
        }

        [Test]
        public void ShouldAddRenameAttributeWhenDifferentNameSpecified()
        {
            var model = new PersistenceModel();

            model.Import<SecondMappedObject>().As("MappedObject");

            new MappingTester<MappedObject>(model)
                .ForMapping(m => m.Id(x => x.Id))
                .Element("import").HasAttribute("rename", "MappedObject");
        }
    }
}