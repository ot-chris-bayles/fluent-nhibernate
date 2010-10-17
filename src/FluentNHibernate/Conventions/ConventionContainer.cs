using System;
using System.Reflection;
using FluentNHibernate.Diagnostics;

namespace FluentNHibernate.Conventions
{
    public interface IConventionContainer
    {
        /// <summary>
        /// Add a source to be queried.
        /// </summary>
        /// <remarks>
        /// All convention types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <param name="source">Assembly instance to query</param>
        void AddSource(ITypeSource source);

        /// <summary>
        /// Add an assembly to be queried.
        /// </summary>
        /// <remarks>
        /// All convention types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <param name="assembly">Assembly instance to query</param>
        void AddAssembly(Assembly assembly);

        /// <summary>
        /// Adds all conventions found in the assembly that contains <typeparam name="T" />.
        /// </summary>
        /// <remarks>
        /// All convention types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        void AddFromAssemblyOf<T>();

        /// <summary>
        /// Add a single convention by type.
        /// </summary>
        /// <remarks>
        /// Type must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <typeparam name="T">Convention type</typeparam>
        void Add<T>() where T : IConvention;

        /// <summary>
        /// Add a single convention by type.
        /// </summary>
        /// <remarks>
        /// Types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <param name="type">Type of convention</param>
        void Add(Type type);

        /// <summary>
        /// Add a single convention by type.
        /// </summary>
        /// <remarks>
        /// Types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <param name="type">Type of convention</param>
        /// <param name="instance">Instance of convention</param>
        void Add(Type type, object instance);

        /// <summary>
        /// Add an instance of a convention.
        /// </summary>
        /// <remarks>
        /// Useful for supplying conventions that require extra constructor parameters.
        /// </remarks>
        /// <typeparam name="T">Convention type</typeparam>
        /// <param name="instance">Instance of convention</param>
        void Add<T>(T instance) where T : IConvention;
    }

    public class ConventionContainer : IConventionContainer
    {
        readonly ConventionsCollection collection;
        readonly IDiagnosticLogger log;

        public ConventionContainer(ConventionsCollection collection, IDiagnosticLogger log)
        {
            this.collection = collection;
            this.log = log;
        }

        public void AddSource(ITypeSource source)
        {
            foreach (var type in source.GetTypes())
            {
                if (type.IsAbstract || type.IsGenericType || !typeof(IConvention).IsAssignableFrom(type)) continue;

                Add(type, MissingConstructor.Ignore);
            }

            log.LoadedConventionsFromSource(source);
        }

        /// <summary>
        /// Add an assembly to be queried.
        /// </summary>
        /// <remarks>
        /// All convention types must have a parameterless constructor, or a single parameter of IConventionFinder.
        /// </remarks>
        /// <param name="assembly">Assembly instance to query</param>
        public void AddAssembly(Assembly assembly)
        {
            AddSource(new AssemblyTypeSource(assembly));
        }

        /// <summary>
        /// Adds all conventions found in the assembly that contains T.
        /// </summary>
        /// <remarks>
        /// All convention types must have a parameterless constructor, or a single parameter of IConventionFinder.
        /// </remarks>
        public void AddFromAssemblyOf<T>()
        {
            AddAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Add a single convention by type.
        /// </summary>
        /// <remarks>
        /// Type must have a parameterless constructor, or a single parameter of IConventionFinder.
        /// </remarks>
        /// <typeparam name="T">Convention type</typeparam>
        public void Add<T>() where T : IConvention
        {
            Add(typeof(T), MissingConstructor.Throw);
        }

        /// <summary>
        /// Add a single convention by type.
        /// </summary>
        /// <remarks>
        /// Types must have a parameterless constructor, or a single parameter of <see cref="IConventionFinder" />.
        /// </remarks>
        /// <param name="type">Type of convention</param>
        public void Add(Type type)
        {
            Add(type, MissingConstructor.Throw);
        }

        public void Add(Type type, object instance)
        {
            if (collection.Contains(type) && !AllowMultiplesOf(type)) return;

            collection.Add(type, instance);
        }

        /// <summary>
        /// Add an instance of a convention.
        /// </summary>
        /// <remarks>
        /// Useful for supplying conventions that require extra constructor parameters.
        /// </remarks>
        /// <typeparam name="T">Convention type</typeparam>
        /// <param name="instance">Instance of convention</param>
        public void Add<T>(T instance) where T : IConvention
        {
            if (collection.Contains(typeof(T)) && !AllowMultiplesOf(instance.GetType())) return;

            collection.Add(typeof(T), instance);
        }

        private void Add(Type type, MissingConstructor missingConstructor)
        {
            if (missingConstructor == MissingConstructor.Throw && !HasValidConstructor(type))
                throw new MissingConstructorException(type);
            if (missingConstructor == MissingConstructor.Ignore && !HasValidConstructor(type))
                return;

            if (collection.Contains(type) && !AllowMultiplesOf(type)) return;

            collection.Add(type, Instantiate(type));
            log.ConventionDiscovered(type);
        }

        bool AllowMultiplesOf(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(MultipleAttribute), true) != null;
        }

        object Instantiate(Type type)
        {
            object instance = null;

            // messy - find either ctor(IConventionFinder) or ctor()
            foreach (var constructor in type.GetConstructors())
            {
                if (IsFinderConstructor(constructor))
                    instance = constructor.Invoke(new[] { new ConventionFinder(collection) });
                else if (IsParameterlessConstructor(constructor))
                    instance = constructor.Invoke(new object[] { });
            }

            return instance;
        }

        static bool HasValidConstructor(Type type)
        {
            foreach (var constructor in type.GetConstructors())
                if (IsFinderConstructor(constructor) || IsParameterlessConstructor(constructor)) return true;

            return false;
        }

        static bool IsFinderConstructor(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            return parameters.Length == 1 && parameters[0].ParameterType == typeof(IConventionFinder);
        }

        static bool IsParameterlessConstructor(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            return parameters.Length == 0;
        }

        enum MissingConstructor
        {
            Throw,
            Ignore
        }
    }
}