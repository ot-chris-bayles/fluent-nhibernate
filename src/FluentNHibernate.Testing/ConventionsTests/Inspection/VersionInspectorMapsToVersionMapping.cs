using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.Utils.Reflection;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionsTests.Inspection
{
    [TestFixture, Category("Inspection DSL")]
    public class VersionInspectorMapsToVersionMapping
    {
        private VersionMapping mapping;
        private IVersionInspector inspector;

        [SetUp]
        public void CreateDsl()
        {
            mapping = new VersionMapping();
            inspector = new VersionInspector(mapping);
        }

        [Test]
        public void AccessMapped()
        {
            mapping.Access = "field";
            inspector.Access.ShouldEqual(Access.Field);
        }

        [Test]
        public void AccessIsSet()
        {
            mapping.Access = "field";
            inspector.IsSet(Prop(x => x.Access))
                .ShouldBeTrue();
        }

        [Test]
        public void AccessIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Access))
                .ShouldBeFalse();
        }

        [Test]
        public void ColumnCollectionHasSameCountAsMapping()
        {
            mapping.AddColumn(new ColumnMapping());
            inspector.Columns.Count().ShouldEqual(1);
        }

        [Test]
        public void ColumnsCollectionOfInspectors()
        {
            mapping.AddColumn(new ColumnMapping());
            inspector.Columns.First().ShouldBeOfType<IColumnInspector>();
        }

        [Test]
        public void ColumnsCollectionIsEmpty()
        {
            inspector.Columns.IsEmpty().ShouldBeTrue();
        }

        [Test]
        public void GeneratedMapped()
        {
            mapping.Generated = "insert";
            inspector.Generated.ShouldEqual(Generated.Insert);
        }

        [Test]
        public void GeneratedIsSet()
        {
            mapping.Generated = "insert";
            inspector.IsSet(Prop(x => x.Generated))
                .ShouldBeTrue();
        }

        [Test]
        public void GeneratedIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Generated))
                .ShouldBeFalse();
        }

        [Test]
        public void NameMapped()
        {
            mapping.Name = "name";
            inspector.Name.ShouldEqual("name");
        }

        [Test]
        public void NameIsSet()
        {
            mapping.Name = "name";
            inspector.IsSet(Prop(x => x.Name))
                .ShouldBeTrue();
        }

        [Test]
        public void NameIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Name))
                .ShouldBeFalse();
        }

        [Test]
        public void TypeMapped()
        {
            mapping.Type = new TypeReference(typeof(string));
            inspector.Type.ShouldEqual(new TypeReference(typeof(string)));
        }

        [Test]
        public void TypeIsSet()
        {
            mapping.Type = new TypeReference(typeof(string));
            inspector.IsSet(Prop(x => x.Type))
                .ShouldBeTrue();
        }

        [Test]
        public void TypeIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Type))
                .ShouldBeFalse();
        }

        [Test]
        public void UnsavedValueMapped()
        {
            mapping.UnsavedValue = "test";
            inspector.UnsavedValue.ShouldEqual("test");
        }

        [Test]
        public void UnsavedValueIsSet()
        {
            mapping.UnsavedValue = "test";
            inspector.IsSet(Prop(x => x.UnsavedValue))
                .ShouldBeTrue();
        }

        [Test]
        public void UnsavedValueIsNotSet()
        {
            inspector.IsSet(Prop(x => x.UnsavedValue))
                .ShouldBeFalse();
        }

        [Test]
        public void LengthMapped()
        {
            mapping.AddColumn(new ColumnMapping { Length = 100 });
            inspector.Length.ShouldEqual(100);
        }

        [Test]
        public void LengthIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Length = 100 });
            inspector.IsSet(Prop(x => x.Length))
                .ShouldBeTrue();
        }

        [Test]
        public void LengthIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Length))
                .ShouldBeFalse();
        }

        [Test]
        public void PrecisionIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Precision = 10 });
            inspector.IsSet(Prop(x => x.Precision))
                .ShouldBeTrue();
        }

        [Test]
        public void PrecisionIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Precision))
                .ShouldBeFalse();
        }

        [Test]
        public void ScaleMapped()
        {
            mapping.AddColumn(new ColumnMapping { Scale = 10 });
            inspector.Scale.ShouldEqual(10);
        }

        [Test]
        public void ScaleIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Scale = 10 });
            inspector.IsSet(Prop(x => x.Scale))
                .ShouldBeTrue();
        }

        [Test]
        public void ScaleIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Scale))
                .ShouldBeFalse();
        }

        [Test]
        public void NullableMapped()
        {
            mapping.AddColumn(new ColumnMapping { NotNull = false });
            inspector.Nullable.ShouldEqual(true);
        }

        [Test]
        public void NullableIsSet()
        {
            mapping.AddColumn(new ColumnMapping { NotNull = false });
            inspector.IsSet(Prop(x => x.Nullable))
                .ShouldBeTrue();
        }

        [Test]
        public void NullableIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Nullable))
                .ShouldBeFalse();
        }

        [Test]
        public void UniqueMapped()
        {
            mapping.AddColumn(new ColumnMapping { Unique = true });
            inspector.Unique.ShouldEqual(true);
        }

        [Test]
        public void UniqueIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Unique = true });
            inspector.IsSet(Prop(x => x.Unique))
                .ShouldBeTrue();
        }

        [Test]
        public void UniqueIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Unique))
                .ShouldBeFalse();
        }

        [Test]
        public void UniqueKeyMapped()
        {
            mapping.AddColumn(new ColumnMapping { UniqueKey = "key" });
            inspector.UniqueKey.ShouldEqual("key");
        }

        [Test]
        public void UniqueKeyIsSet()
        {
            mapping.AddColumn(new ColumnMapping { UniqueKey = "key" });
            inspector.IsSet(Prop(x => x.UniqueKey))
                .ShouldBeTrue();
        }

        [Test]
        public void UniqueKeyIsNotSet()
        {
            inspector.IsSet(Prop(x => x.UniqueKey))
                .ShouldBeFalse();
        }

        [Test]
        public void SqlTypeMapped()
        {
            mapping.AddColumn(new ColumnMapping { SqlType = "sql" });
            inspector.SqlType.ShouldEqual("sql");
        }

        [Test]
        public void SqlTypeIsSet()
        {
            mapping.AddColumn(new ColumnMapping { SqlType = "sql" });
            inspector.IsSet(Prop(x => x.SqlType))
                .ShouldBeTrue();
        }

        [Test]
        public void SqlTypeIsNotSet()
        {
            inspector.IsSet(Prop(x => x.SqlType))
                .ShouldBeFalse();
        }

        [Test]
        public void IndexMapped()
        {
            mapping.AddColumn(new ColumnMapping { Index = "index" });
            inspector.Index.ShouldEqual("index");
        }

        [Test]
        public void IndexIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Index = "index" });
            inspector.IsSet(Prop(x => x.Index))
                .ShouldBeTrue();
        }

        [Test]
        public void IndexIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Index))
                .ShouldBeFalse();
        }

        [Test]
        public void CheckMapped()
        {
            mapping.AddColumn(new ColumnMapping { Check = "key" });
            inspector.Check.ShouldEqual("key");
        }

        [Test]
        public void CheckIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Check = "key" });
            inspector.IsSet(Prop(x => x.Check))
                .ShouldBeTrue();
        }

        [Test]
        public void CheckIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Check))
                .ShouldBeFalse();
        }

        [Test]
        public void DefaultMapped()
        {
            mapping.AddColumn(new ColumnMapping { Default = "key" });
            inspector.Default.ShouldEqual("key");
        }

        [Test]
        public void DefaultIsSet()
        {
            mapping.AddColumn(new ColumnMapping { Default = "key" });
            inspector.IsSet(Prop(x => x.Default))
                .ShouldBeTrue();
        }

        [Test]
        public void DefaultIsNotSet()
        {
            inspector.IsSet(Prop(x => x.Default))
                .ShouldBeFalse();
        }

        #region Helpers

        private Member Prop(Expression<Func<IVersionInspector, object>> propertyExpression)
        {
            return ReflectionHelper.GetMember(propertyExpression);
        }

        #endregion
    }
}