using System;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Identity;
namespace FluentNHibernate.Mapping
{
    public class KeyPropertyPart
    {
        private readonly KeyPropertyMapping mapping;

        public KeyPropertyPart(KeyPropertyMapping mapping)
        {
            this.mapping = mapping;
            Access = new AccessStrategyBuilder<KeyPropertyPart>(this, value => mapping.Access = value);
        }

        public KeyPropertyPart ColumnName(string columnName)
        {
            var column = new ColumnMapping();
            column.Set(x => x.Name, Layer.UserSupplied, columnName);

            mapping.AddColumn(column);
            return this;
        }

        public KeyPropertyPart Type(Type type)
        {
            mapping.Type = new TypeReference(type);
            return this;
        }

        public KeyPropertyPart Type(string type)
        {
            mapping.Type = new TypeReference(type);
            return this;
        }

        public KeyPropertyPart Length(int length)
        {
            mapping.Length = length;
            return this;
        }

        public AccessStrategyBuilder<KeyPropertyPart> Access { get; private set; }
    }
}