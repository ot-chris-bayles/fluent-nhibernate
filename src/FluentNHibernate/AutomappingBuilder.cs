using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Infrastructure;
using FluentNHibernate.Utils.Reflection;

namespace FluentNHibernate
{
    public class AutomappingBuilder
    {
        readonly IAutomappingInstructions instructions;

        public AutomappingBuilder(IAutomappingInstructions instructions)
        {
            this.instructions = instructions;
        }

        public AutomappingBuilder ThisAssembly()
        {
            var assembly = ReflectionHelper.FindTheCallingAssembly();
            return Assembly(assembly);
        }

        public AutomappingBuilder AssemblyOf<T>()
        {
            return Assembly(typeof(T).Assembly);
        }

        public AutomappingBuilder Assembly(Assembly assembly)
        {
            instructions.AddSource(new AssemblyTypeSource(assembly));
            return this;
        }

        public AutomappingBuilder UsingConfiguration<T>()
            where T : IAutomappingConfiguration, new()
        {
            return UsingConfiguration(new T());
        }

        public AutomappingBuilder UsingConfiguration(IAutomappingConfiguration cfg)
        {
            instructions.UseConfiguration(cfg);
            return this;
        }
    }
}