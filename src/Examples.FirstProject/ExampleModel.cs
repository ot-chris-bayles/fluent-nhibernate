using System.IO;
using FluentNHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Examples.FirstProject
{
    class ExampleModel : PersistenceModel
    {
        ExampleModel()
        {
            Scan
                .AssemblyContaining<Program>()
                .ForMappings();

            Database(
                SQLiteConfiguration.Standard
                    .UsingFile(DbFile));

            PostConfigure(BuildSchema);
        }

        static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(DbFile))
                File.Delete(DbFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }

        const string DbFile = "firstProgram.db";
    }
}