using System.Linq;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionFinderTests
{
    [TestFixture]
    public class FindTests
    {
        ConventionFinder finder;
        ConventionContainer container;

        [SetUp]
        public void CreateConventionFinder()
        {
            var collection = new ConventionsCollection();
            container = new ConventionContainer(collection, new NullDiagnosticsLogger());
            finder = new ConventionFinder(collection);
        }

        [Test]
        public void ShouldFindNothingWhenNoAssembliesGiven()
        {
            finder.Find<IClassConvention>()
                .ShouldBeEmpty();
        }

        [Test]
        public void ShouldFindTypesFromAssembly()
        {
            container.AddAssembly(GetType().Assembly);
            finder.Find<IClassConvention>()
                .ShouldContain(c => c is DummyClassConvention);
        }

        [Test]
        public void ShouldFindTypesThatHaveConstructorNeedingFinder()
        {
            container.AddAssembly(GetType().Assembly);
            finder.Find<IClassConvention>()
                .ShouldContain(c => c is DummyClassAssemblyConvention);
        }

        [Test]
        public void ShouldntFindGenericTypes()
        {
            container.AddAssembly(GetType().Assembly);
            finder.Find<IClassConvention>()
                .ShouldNotContain(c => c.GetType() == typeof(OpenGenericClassConvention<>));
        }

        [Test]
        public void ShouldOnlyFindExplicitAdded()
        {
            container.Add<DummyClassConvention>();
            finder.Find<IClassConvention>()
                .ShouldHaveCount(1)
                .ShouldContain(c => c is DummyClassConvention);
        }

        [Test]
        public void ShouldOnlyAddInstanceOnceIfHasMultipleInterfaces()
        {
            container.Add<MultiPartConvention>();
            var ids = finder.Find<IIdConvention>();
            var properties = finder.Find<IPropertyConvention>();

            ids.ShouldHaveCount(1);
            properties.ShouldHaveCount(1);

            ids.First().ShouldEqual(properties.First());
        }

        [Test]
        public void ShouldOnlyAddInstanceOnce()
        {
            container.Add<DummyClassConvention>();
            container.Add<DummyClassConvention>();
            var conventions = finder.Find<IClassConvention>();

            conventions.ShouldHaveCount(1);
        }

        [Test]
        public void ShouldAllowAddingInstanceMultipleTimesIfHasMultipleAttribute()
        {
            container.Add<DummyClassWithMultipleAttributeConvention>();
            container.Add<DummyClassWithMultipleAttributeConvention>();
            var conventions = finder.Find<IClassConvention>();

            conventions.ShouldHaveCount(2);
        }

        [Test]
        public void ShouldOnlyAddInstanceOnceIfHasMultipleInterfacesWhenAddedByAssembly()
        {
            container.AddAssembly(typeof(MultiPartConvention).Assembly);
            var ids = finder.Find<IIdConvention>().Where(c => c.GetType() == typeof(MultiPartConvention));
            var properties = finder.Find<IPropertyConvention>().Where(c => c.GetType() == typeof(MultiPartConvention));

            ids.ShouldHaveCount(1);
            properties.ShouldHaveCount(1);

            ids.First().ShouldEqual(properties.First());
        }
    }

    public class OpenGenericClassConvention<T> : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
        }
    }

    public class DummyClassConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
        }
    }

    [Multiple]
    public class DummyClassWithMultipleAttributeConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
        }
    }

    public class DummyClassAssemblyConvention : IClassConvention
    {
        public DummyClassAssemblyConvention(IConventionFinder finder)
        {

        }

        public void Apply(IClassInstance instance)
        {
        }
    }

    public class MultiPartConvention : IIdConvention, IPropertyConvention
    {
        public void Apply(IIdentityInstance instance)
        {
        }

        public void Apply(IPropertyInstance instance)
        {
        }
    }
}
