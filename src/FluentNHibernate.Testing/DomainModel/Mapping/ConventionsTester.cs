using System;
using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NUnit.Framework;

namespace FluentNHibernate.Testing.DomainModel.Mapping
{
    [TestFixture]
    public class ConventionsTester
    {
        [Test]
        public void AddPropertyConventionForTypeOfAttribute()
        {
            new MappingTester<Site>()
                .Conventions(conventions => conventions.Add<MyAttributePropertyConvention>())
                .ForMapping(m =>
                {
                    m.Id(x => x.Id);
                    m.Map(x => x.Name);
                })
                .Element("class/property[@name='Name']")
                    .HasAttribute("access", "field");
        }

        [Test]
        public void AddCollectionConventionForTypeOfAttribute() 
        {
            new MappingTester<Site>()
                .Conventions(conventions => conventions.Add<MyAttributeCollectionConvention>())
                .ForMapping(m =>
                {
                    m.Id(x => x.Id);
                    m.HasMany(x => x.Prior);
                })
                .Element("class/*[@name='Prior']")
                    .HasAttribute("cascade", "all-delete-orphan");
        }
    }

    public class Invoice{}

    public class Site
    {
        public int Id { get; set; }
        [My]
        public string Name { get; set; }

        public string LastName { get; set; }

        public Address Primary { get; set; }
        public Address Secondary { get; set; }

        [My]
        public IList<Address> Prior { get; set; }
    }
    public class Address{}

    public class MyAttribute : Attribute
    {
        
    }

    public class MyAttributePropertyConvention : AttributePropertyConvention<MyAttribute>
    {
        protected override void Apply(MyAttribute attribute, IPropertyInstance instance)
        {
            instance.Access.Field();
        }
    }

    public class MyAttributeCollectionConvention : AttributeCollectionConvention<MyAttribute> 
    {
        protected override void Apply(MyAttribute attribute, ICollectionInstance instance) 
        {
            instance.Cascade.AllDeleteOrphan();
        }
    }
}
