using System;
using System.Diagnostics;
using System.Linq;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;

namespace FluentNHibernate.Conventions.Instances
{
    public class ManyToOneInstance : ManyToOneInspector, IManyToOneInstance
    {
        private readonly ManyToOneMapping mapping;
        private bool nextBool = true;

        public ManyToOneInstance(ManyToOneMapping mapping)
            : base(mapping)
        {
            this.mapping = mapping;
        }

        public void Column(string columnName)
        {
            if (mapping.Columns.UserDefined.Count() > 0)
                return;

            var originalColumn = mapping.Columns.FirstOrDefault();
            var column = originalColumn == null ? new ColumnMapping() : originalColumn.Clone();

            column.Name = columnName;

            mapping.ClearColumns();
            mapping.AddColumn(column);
        }
        
        public new void Formula(string formula)
        {
            mapping.Formula = formula;
            // TODO: Fix this
            // mapping.ClearColumns();
        }

        public void CustomClass<T>()
        {
                mapping.Class = new TypeReference(typeof(T));
        }

        public void CustomClass(Type type)
        {
                mapping.Class = new TypeReference(type);
        }

        public new IAccessInstance Access
        {
            get
            {
                return new AccessInstance(value =>
                {
                        mapping.Access = value;
                });
            }
        }

        public new ICascadeInstance Cascade
        {
            get
            {
                return new CascadeInstance(value =>
                {
                        mapping.Cascade = value;
                });
            }
        }

        new public IFetchInstance Fetch
        {
            get
            {
                return new FetchInstance(value =>
                {
                        mapping.Fetch = value;
                });
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IManyToOneInstance Not
        {
            get
            {
                nextBool = !nextBool;
                return this;
            }
        }

        public new INotFoundInstance NotFound
        {
            get
            {
                return new NotFoundInstance(value =>
                {
                        mapping.NotFound = value;
                });
            }
        }

        public void Index(string index)
        {
            foreach (var column in mapping.Columns)
                column.Index = index;
        }

        public new void Insert()
        {
                mapping.Insert = nextBool;
            nextBool = true;
        }

        public new void OptimisticLock()
        {
                mapping.OptimisticLock = nextBool;
            nextBool = true;
        }

        public new void LazyLoad()
        {
                if (nextBool)
                    LazyLoad(Laziness.Proxy);
                else
                    LazyLoad(Laziness.False);
            nextBool = true;
        }

        public new void LazyLoad(Laziness laziness)
        {
            mapping.Lazy = laziness.ToString();
            nextBool = true;
        }

        public new void Nullable()
        {
                foreach (var column in mapping.Columns)
                    column.NotNull = !nextBool;

            nextBool = true;
        }

        public new void PropertyRef(string property)
        {
                mapping.PropertyRef = property;
        }

        public void ReadOnly()
        {
                mapping.Insert = !nextBool;
                mapping.Update = !nextBool;
            nextBool = true;
        }

        public void Unique()
        {
                foreach (var column in mapping.Columns)
                    column.Unique = nextBool;

            nextBool = true;
        }

        public void UniqueKey(string key)
        {
            foreach (var column in mapping.Columns)
                column.UniqueKey = key;
        }

        public new void Update()
        {
                mapping.Update = nextBool;
            nextBool = true;
        }

        public new void ForeignKey(string key)
        {
                mapping.ForeignKey = key;
        }

        public void OverrideInferredClass(Type type)
        {
            mapping.Class = new TypeReference(type);
        }
    }
}