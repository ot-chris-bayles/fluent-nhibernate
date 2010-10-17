using System;
using System.Collections;

namespace FluentNHibernate.Specs.FluentInterface.Fixtures
{
    class EntityWithComponent
    {
        public ComponentTarget Component { get; set; }
        public IDictionary DynamicComponent { get; set; }
        public int Id { get; set; }
    }

    class EntityWithFieldComponent
    {
        public ComponentTarget Component;
        public IDictionary DynamicComponent;
        public int Id { get; set; }
    }

    class ComponentTarget
    {
    }
}