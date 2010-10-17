using System;
using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Diagnostics;

namespace FluentNHibernate.Cfg
{
    public class SetupConventionContainer<TReturn> : IConventionContainer
    {
        private readonly TReturn parent;
        private readonly IConventionContainer conventionContainer;

        public SetupConventionContainer(TReturn container, IConventionContainer conventionContainer)
        {
            parent = container;
            this.conventionContainer = conventionContainer;
        }

        public TReturn AddSource(ITypeSource source)
        {
            conventionContainer.AddSource(source);
            return parent;
        }

        void IConventionContainer.AddSource(ITypeSource source)
        {
            AddSource(source);
        }

        public TReturn AddAssembly(Assembly assembly)
        {
            conventionContainer.AddAssembly(assembly);
            return parent;
        }

        public TReturn AddFromAssemblyOf<T>()
        {
            conventionContainer.AddFromAssemblyOf<T>();
            return parent;
        }

        void IConventionContainer.AddFromAssemblyOf<T>()
        {
            AddFromAssemblyOf<T>();
        }

        void IConventionContainer.AddAssembly(Assembly assembly)
        {
            AddAssembly(assembly);
        }

        public TReturn Add<T>() where T : IConvention
        {
            conventionContainer.Add<T>();
            return parent;
        }

        void IConventionContainer.Add<T>()
        {
            Add<T>();
        }

        public void Add(Type type, object instance)
        {
            conventionContainer.Add(type, instance);
        }

        public TReturn Add<T>(T instance) where T : IConvention
        {
            conventionContainer.Add(instance);
            return parent;
        }

        void IConventionContainer.Add(Type type)
        {
            Add(type);
        }

        public TReturn Add(Type type)
        {
            conventionContainer.Add(type);
            return parent;
        }

        void IConventionContainer.Add<T>(T instance)
        {
            Add(instance);
        }

        public TReturn Add(params IConvention[] instances)
        {
            foreach (var instance in instances)
            {
                conventionContainer.Add(instance.GetType(), instance);
            }

            return parent;
        }

        public TReturn Setup(Action<IConventionContainer> setupAction)
        {
            setupAction(this);
            return parent;
        }
    }
}