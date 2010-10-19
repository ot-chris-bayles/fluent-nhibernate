using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Mapping;
using Machine.Specifications;
using Rhino.Mocks;

namespace FluentNHibernate.Specs.PersistenceModel
{
    public class when_scanning_without_a_purpose : ScannerSpec
    {
        Because of = () =>
            scanner.Collection(new[] {typeof(object)});

        It should_not_yield_any_types = () =>
            gatherer.AssertWasNotCalled(x => x.AddSource(Arg<ITypeSource>.Is.Anything));
    }

    public class when_scanning_for_mappings : ScannerSpec
    {
        Because of = () =>
            scanner
                .Collection(new[] {typeof(object)})
                .Collection(new[] {typeof(object)})
                .ForMappings();

        It should_add_sources_to_gatherer = () =>
            gatherer.AssertWasCalled(x => x.AddSource(Arg<ITypeSource>.Is.NotNull),
                opt => opt.Repeat.Twice());
    }

    public class when_scanning_for_imports : ScannerSpec
    {
        Because of = () =>
            scanner
                .Collection(new[] {typeof(object)})
                .ForImporting();

        It should_add_sources_to_gatherer = () =>
            gatherer.AssertWasCalled(x => x.AddProviderInstance(Arg<ImportPart>.Is.NotNull));
    }

    public class when_scanning_for_conventions : ScannerSpec
    {
        Because of = () =>
            scanner
                .Collection(new[] { typeof(Convention) })
                .ForConventions();

        It should_add_sources_to_gatherer = () =>
            gatherer.Conventions.Contains(typeof(Convention)).ShouldBeTrue();

        class Convention : IConvention
        {}
    }

    [Subject(typeof(SourceScanner))]
    public class ScannerSpec
    {
        Establish context = () =>
        {
            despatcher = new DefaultDiagnosticMessageDespatcher();
            log = new DefaultDiagnosticLogger(despatcher);
            gatherer = MockRepository.GenerateStub<PersistenceInstructionGatherer>();
            scanner = new SourceScanner(gatherer, log);
        };

        protected static SourceScanner scanner;
        protected static PersistenceInstructionGatherer gatherer;
        static IDiagnosticLogger log;
        static IDiagnosticMessageDespatcher despatcher;
    }
}
