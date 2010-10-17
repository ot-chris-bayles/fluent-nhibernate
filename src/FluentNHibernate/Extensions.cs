using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Infrastructure;
using NHibernate.Cfg;

namespace FluentNHibernate
{
    public static class ConfigurationHelper
    {
        public static Configuration ConfigureWith<T>(this Configuration cfg)
            where T : PersistenceModel, new()
        {
            return cfg.ConfigureWith(new T());
        }

        public static Configuration ConfigureWith(this Configuration cfg, PersistenceModel model)
        {
            var injector = new PersistenceModelInjector();

            injector.Inject(model, cfg);

            return cfg;
        }

        public static Configuration ConfigureWith(this Configuration cfg, IPersistenceInstructionGatherer gatherer)
        {
            var injector = new PersistenceModelInjector();

            injector.Inject(gatherer, cfg);

            return cfg;
        }

        public static Configuration ConfigureWith(this Configuration cfg, IPersistenceInstructions instructions)
        {
            var injector = new PersistenceModelInjector();

            injector.Inject(instructions, cfg);

            return cfg;
        }
    }
}