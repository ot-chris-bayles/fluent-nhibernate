using System;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Utils;

namespace FluentNHibernate.MappingModel
{
    [Serializable]
    public class AttributeStore
    {
        readonly AttributeLayeredValues layeredValues;

        public AttributeStore()
        {
            layeredValues = new AttributeLayeredValues();
        }

        public object Get(string property)
        {
            var values = layeredValues[property];

            if (!values.Any())
                return null;

            var topLayer = values.Max(x => x.Key);

            return values[topLayer];
        }

        public void Set(string attribute, int layer, object value)
        {
            layeredValues[attribute][layer] = value;
        }

        public bool IsSpecified(string attribute)
        {
            return layeredValues.ContainsKey(attribute);
        }

        public bool HasValue(string attribute)
        {
            return layeredValues.ContainsKey(attribute) && layeredValues[attribute].Any();
        }

        public void CopyTo(AttributeStore theirStore)
        {
            layeredValues.CopyTo(theirStore.layeredValues);
        }

        public AttributeStore Clone()
        {
            var clonedStore = new AttributeStore();

            CopyTo(clonedStore);

            return clonedStore;
        }

        public bool Equals(AttributeStore other)
        {
            if (other == null) return false;

            return other.layeredValues.ContentEquals(layeredValues);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AttributeStore)) return false;
            return Equals((AttributeStore)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((layeredValues != null ? layeredValues.GetHashCode() : 0) * 397);
            }
        }
    }

    [Serializable]
    public class AttributeStore<T>
    {
        private readonly AttributeStore store;

        public AttributeStore()
            : this(new AttributeStore())
        {

        }

        public AttributeStore(AttributeStore store)
        {
            this.store = store;
        }

        public TResult Get<TResult>(Expression<Func<T, TResult>> exp)
        {
            var value = store.Get(GetAttribute(exp));

            if (value == null)
                return default(TResult);

            return (TResult)value;
        }

        public void Set<TResult>(Expression<Func<T, TResult>> exp, int layer, TResult value)
        {
            store.Set(GetAttribute(exp), layer, value);
        }

        /// <summary>
        /// Returns whether the user has set a value for a property.
        /// </summary>
        public bool IsSpecified<TResult>(Expression<Func<T, TResult>> exp)
        {
            return store.IsSpecified(GetAttribute(exp));
        }

        /// <summary>
        /// Returns whether the user has set a value for a property.
        /// </summary>
        public bool IsSpecified(string property)
        {
            return store.IsSpecified(property);
        }

        /// <summary>
        /// Returns whether a property has any value, default or user specified.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool HasValue<TResult>(Expression<Func<T, TResult>> exp)
        {
            return store.HasValue(GetAttribute(exp));
        }

        public bool HasValue(string property)
        {
            return store.HasValue(property);
        }

        public void CopyTo(AttributeStore<T> target)
        {
            store.CopyTo(target.store);
        }

        private static string GetAttribute<TResult>(Expression<Func<T, TResult>> expression)
        {
            var member = expression.ToMember();
            return member.Name;
        }

        public AttributeStore<T> Clone()
        {
            var clonedStore = new AttributeStore<T>();

            store.CopyTo(clonedStore.store);

            return clonedStore;
        }

        public AttributeStore CloneInner()
        {
            var clonedStore = new AttributeStore();

            store.CopyTo(clonedStore);

            return clonedStore;
        }

        public bool Equals(AttributeStore<T> other)
        {
            return Equals(other.store, store);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(AttributeStore<T>)) return false;
            return Equals((AttributeStore<T>)obj);
        }

        public override int GetHashCode()
        {
            return (store != null ? store.GetHashCode() : 0);
        }
    }
}