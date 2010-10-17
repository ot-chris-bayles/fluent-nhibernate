using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.MappingModel;
using NUnit.Framework;

namespace FluentNHibernate.Testing.AutoMapping.Overrides
{
    [TestFixture]
    public class HibernateMappingOverrides
    {
        [Test, Ignore]
        public void CanOverrideDefaultLazy()
        {
            var model = AutoMap.Source(new StubTypeSource(new[] { typeof(Parent) }))
               .Conventions.Add(DefaultLazy.Never());

            HibernateMapping hibernateMapping = model.BuildMappings().First();

            hibernateMapping.DefaultLazy.ShouldBeFalse();
        }

    }
}
