using System;

namespace FluentNHibernate.Specs.FluentInterface.Fixtures
{
    class EntityWithReferences
    {
        public ReferenceTarget Reference { get; set; }
        public int Id { get; set; }
    }

    class ReferenceTarget
    {}
}