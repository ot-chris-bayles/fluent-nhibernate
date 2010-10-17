using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Inspections;
using NUnit.Framework;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class DefaultAccessStrategyClassMapTester
    {
        [Test]
        public void AccessAsProperty_SetsAccessStrategyToProperty()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.Property()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "property");
        }

        [Test]
        public void AccessAsField_SetsAccessStrategyToField()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.Field()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field");
        }

        [Test]
        public void AccessAsCamelCaseField_SetsAccessStrategyToField_and_SetsNamingStrategyToCamelCase()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.CamelCaseField()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.camelcase");
        }

        [Test]
        public void AccessAsCamelCaseFieldWithUnderscorePrefix_SetsAccessStrategyToField_and_SetsNamingStrategyToCamelCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.CamelCaseField(CamelCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.camelcase-underscore");
        }

        [Test]
        public void AccessAsLowerCaseField_SetsAccessStrategyToField_and_SetsNamingStrategyToLowerCase()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.LowerCaseField()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.lowercase");
        }

        [Test]
        public void AccessAsLowerCaseFieldWithUnderscorePrefix_SetsAccessStrategyToField_and_SetsNamingStrategyToLowerCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.LowerCaseField(LowerCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.lowercase-underscore");
        }

        [Test]
        public void AccessAsPascalCaseFieldWithUnderscorePrefix_SetsAccessStrategyToField_and_SetsNamingStrategyToPascalCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.PascalCaseField(PascalCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.pascalcase-underscore");
        }

        [Test]
        public void AccessAsPascalCaseFieldWithMPrefix_SetsAccessStrategyToField_and_SetsNamingStrategyToLowerCaseM()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.PascalCaseField(PascalCasePrefix.M)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.pascalcase-m");
        }

        [Test]
        public void AccessAsPascalCaseFieldWithMUnderscorePrefix_SetsAccessStrategyToField_and_SetsNamingStrategyToLowerCaseMUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.PascalCaseField(PascalCasePrefix.MUnderscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "field.pascalcase-m-underscore");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughCamelCaseField_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToCamelCase()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughCamelCaseField()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.camelcase");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughCamelCaseFieldWithUnderscorePrefix_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToCamelCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughCamelCaseField(CamelCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.camelcase-underscore");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughLowerCaseField_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToLowerCase()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughLowerCaseField()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.lowercase");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughLowerCaseFieldWithUnderscorePrefix_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToLowerCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughLowerCaseField(LowerCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.lowercase-underscore");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughPascalCaseFieldWithUnderscorePrefix_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToPascalCaseUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughPascalCaseField(PascalCasePrefix.Underscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.pascalcase-underscore");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughPascalCaseFieldWithMPrefix_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToPascalCaseM()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughPascalCaseField(PascalCasePrefix.M)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.pascalcase-m");
        }

        [Test]
        public void AccessAsReadOnlyPropertyThroughPascalCaseFieldWithMUnderscorePrefix_SetsAccessStrategyToNoSetter_and_SetsNamingStrategyToPascalCaseMUnderscore()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyPropertyThroughPascalCaseField(PascalCasePrefix.MUnderscore)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter.pascalcase-m-underscore");
        }

        [Test]
        public void AccessAsReadOnlySetsAccessStrategyToReadOnly()
        {
            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.ReadOnlyProperty()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", "nosetter");
        }

        [Test]
        public void AccessUsingClassName_SetsAccessAttributeToClassName()
        {
            var className = typeof(FakePropertyAccessor).AssemblyQualifiedName;

            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.Using(className)))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", className);
        }

        [Test]
        public void AccessUsingClassType_SetsAccessAttributeToAssemblyQualifiedName()
        {
            var className = typeof(FakePropertyAccessor).AssemblyQualifiedName;

            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.Using(typeof(FakePropertyAccessor))))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", className);
        }

        [Test]
        public void AccessUsingClassGenericParameter_SetsAccessAttributeToAssemblyQualifiedName()
        {
            var className = typeof(FakePropertyAccessor).AssemblyQualifiedName;

            new MappingTester<PropertyTarget>()
                .Conventions(c => c.Add(DefaultAccess.Using<FakePropertyAccessor>()))
                .ForMapping(c => c.Id(x => x.Id))
                .RootElement.HasAttribute("default-access", className);
        }
    }
}