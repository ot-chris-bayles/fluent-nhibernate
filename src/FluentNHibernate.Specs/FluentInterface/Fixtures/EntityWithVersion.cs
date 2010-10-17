using System;

namespace FluentNHibernate.Specs.FluentInterface.Fixtures
{
    class EntityWithVersion
    {
        public TimeSpan VersionNumber { get; set; }
        public int Id { get; set; }
    }
}