using System;

namespace FluentNHibernate.Specs.FluentInterface.Fixtures
{
    class EntityWithProperties
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    class EntityWithPrivateProperties
    {
        private string Name { get; set; }
        private int Id { get; set; }
        string name;
    }
}
