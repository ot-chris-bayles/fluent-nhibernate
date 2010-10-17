using FluentNHibernate.Conventions.Helpers;
using NUnit.Framework;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class HbmAttributeTests
    {
        [Test]
        public void Can_specify_default_cascade()
        {
            new MappingTester<MappedObject>()
                .Conventions(c => c.Add(DefaultAccess.CamelCaseField()))
                .ForMapping(m => m.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.camelcase");
        }

        [Test]
        public void Can_specify_auto_import_as_true()
        {
            new MappingTester<MappedObject>()
                .Conventions(c => c.Add(AutoImport.Always()))
                .ForMapping(m => m.Id(x => x.Id))
                .RootElement.HasAttribute("auto-import", "true");
        }

        [Test]
        public void Can_specify_auto_import_as_false()
        {
            new MappingTester<MappedObject>()
                .Conventions(c => c.Add(AutoImport.Never()))
                .ForMapping(m => m.Id(x => x.Id))
                .RootElement.HasAttribute("auto-import", "false");
        }
    }
}