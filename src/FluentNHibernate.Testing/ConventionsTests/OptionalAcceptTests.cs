using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Visitors;
using NUnit.Framework;
using Is=FluentNHibernate.Conventions.AcceptanceCriteria.Is;

namespace FluentNHibernate.Testing.ConventionsTests
{
    [TestFixture]
    public class OptionalAcceptTests
    {
        ConventionFinder conventions;
        ConventionVisitor visitor;
        ConventionContainer container;

        [SetUp]
        public void CreateVisitor()
        {
            var collection = new ConventionsCollection();
            container = new ConventionContainer(collection, new NullDiagnosticsLogger());
            conventions = new ConventionFinder(collection);
            visitor = new ConventionVisitor(conventions);
        }

        [Test]
        public void ShouldNotApplyConventionWithFailingAccept()
        {
            container.Add<ConventionWithFailingAccept>();

            var mapping = new ClassMapping();
            
            visitor.ProcessClass(mapping);

            mapping.IsSpecified(x => x.TableName).ShouldBeFalse();
        }

        [Test]
        public void ShouldApplyConventionWithSuccessfulAccept()
        {
            container.Add<ConventionWithSuccessfulAccept>();

            var mapping = new ClassMapping();

            visitor.ProcessClass(mapping);

            mapping.IsSpecified(x => x.TableName).ShouldBeTrue();
        }

        [Test]
        public void ShouldApplyConventionWithNoAccept()
        {
            container.Add<ConventionWithNoAccept>();

            var mapping = new ClassMapping();

            visitor.ProcessClass(mapping);

            mapping.IsSpecified(x => x.TableName).ShouldBeTrue();
        }

        private class ConventionWithFailingAccept : IClassConvention, IConventionAcceptance<IClassInspector>
        {
            public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
            {
                criteria.Expect(x => x.TableName == "test"); // never true
            }

            public void Apply(IClassInstance instance)
            {
                instance.Table("xxx");
            }
        }

        private class ConventionWithSuccessfulAccept : IClassConvention, IConventionAcceptance<IClassInspector>
        {
            public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
            {
                criteria.Expect(x => x.TableName, Is.Not.Set); // always true
            }

            public void Apply(IClassInstance instance)
            {
                instance.Table("xxx");
            }
        }

        private class ConventionWithNoAccept : IClassConvention
        {
            public void Apply(IClassInstance instance)
            {
                instance.Table("xxx");
            }
        }
    }
}