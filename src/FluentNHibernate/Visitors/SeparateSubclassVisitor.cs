using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Visitors
{
    public class SeparateSubclassVisitor : DefaultMappingModelVisitor
    {
        public override void Visit(MappingModel.MappingBucket bucket)
        {
            base.Visit(bucket);

            foreach (var classMapping in bucket.Classes)
            {
                var subclasses = FindClosestSubclasses(classMapping.Type, bucket.Subclasses);

                foreach (var subclass in subclasses)
                    classMapping.AddSubclass(subclass);
            }

            foreach (var subclassMapping in bucket.Subclasses)
            {
                var subclasses = FindClosestSubclasses(subclassMapping.Type, bucket.Subclasses);

                foreach (var subclass in subclasses)
                    subclassMapping.AddSubclass(subclass);
            }

            foreach (var classMapping in bucket.Classes)
            {
                var subclassType = GetSubclassType(classMapping);

                SetSubclassType(classMapping.Subclasses, subclassType);
            }
        }

        static void SetSubclassType(IEnumerable<SubclassMapping> subclasses, SubclassType subclassType)
        {
            foreach (var subclassMapping in subclasses)
            {
                subclassMapping.SubclassType = subclassType;
                SetSubclassType(subclassMapping.Subclasses, subclassType);
            }
        }

        private IEnumerable<SubclassMapping> FindClosestSubclasses(Type type, IEnumerable<SubclassMapping> subclassMappings)
        {
            var extendsSubclasses = subclassMappings
                .Where(x => x.Extends == type);
            var subclasses = SortByDistanceFrom(type, subclassMappings.Except(extendsSubclasses));

            if (subclasses.Keys.Count == 0 && !extendsSubclasses.Any())
                return new SubclassMapping[0];
            if (subclasses.Keys.Count == 0)
                return extendsSubclasses;

            var lowestDistance = subclasses.Keys.Min();

            return subclasses[lowestDistance].Concat(extendsSubclasses);
        }

        private SubclassType GetSubclassType(ClassMapping mapping)
        {
            if (mapping.IsUnionSubclass)
            {
                return SubclassType.UnionSubclass;
            }

            if (mapping.Discriminator == null)
                return SubclassType.JoinedSubclass;

            return SubclassType.Subclass;
        }

        private bool IsMapped(Type type, IEnumerable<SubclassMapping> subclassMappings)
        {
            return subclassMappings.Any(x => x.Type == type);
        }

        /// <summary>
        /// Takes a type that represents the level in the class/subclass-hiearchy that we're starting from, the parent,
        /// this can be a class or subclass; also takes a list of subclass providers. The providers are then iterated
        /// and added to a dictionary key'd by the types "distance" from the parentType; distance being the number of levels
        /// between parentType and the subclass-type.
        /// 
        /// By default if the Parent type is an interface the level will always be zero. At this time there is no check for
        /// hierarchical interface inheritance.
        /// </summary>
        /// <param name="parentType">Starting point, parent type.</param>
        /// <param name="subclassMappings">List of subclasses</param>
        /// <returns>Dictionary key'd by the distance from the parentType.</returns>
        private IDictionary<int, IList<SubclassMapping>> SortByDistanceFrom(Type parentType, IEnumerable<SubclassMapping> subclassMappings)
        {
            var arranged = new Dictionary<int, IList<SubclassMapping>>();

            foreach (var subclass in subclassMappings)
            {
                var subclassType = subclass.Type;
                var level = 0;

                bool implOfParent = parentType.IsInterface
                    ? DistanceFromParentInterface(parentType, subclassType, subclassMappings, ref level)
                    : DistanceFromParentBase(parentType, subclassType.BaseType, subclassMappings, ref level);

                if (!implOfParent) continue;

                if (!arranged.ContainsKey(level))
                    arranged[level] = new List<SubclassMapping>();

                arranged[level].Add(subclass);
            }

            return arranged;
        }

        /// <summary>
        /// The evalType starts out as the original subclass. The class hiearchy is only
        /// walked if the subclass inherits from a class that is included in the subclassProviders.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="evalType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool DistanceFromParentInterface(Type parentType, Type evalType, IEnumerable<SubclassMapping> subclassMappings, ref int level)
        {
            if (!evalType.HasInterface(parentType)) return false;

            if (!(evalType == typeof(object)) &&
                IsMapped(evalType.BaseType, subclassMappings))
            {
                //Walk the tree if the subclasses base class is also in the subclassProviders
                level++;
                DistanceFromParentInterface(parentType, evalType.BaseType, subclassMappings, ref level);
            }

            return true;
        }

        /// <summary>
        /// The evalType is always one class higher in the hiearchy starting from the original subclass. The class 
        /// hiearchy is walked until the IsTopLevel (base class is Object) is met. The level is only incremented if 
        /// the subclass inherits from a class that is also in the subclassProviders.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="evalType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool DistanceFromParentBase(Type parentType, Type evalType, IEnumerable<SubclassMapping> subclassMappings, ref int level)
        {
            var evalImplementsParent = false;
            if (evalType == parentType)
                evalImplementsParent = true;

            if (!evalImplementsParent && !(evalType == typeof(object)))
            {
                //If the eval class does not inherit the parent but it is included
                //in the subclassprovides, then the original subclass can not inherit 
                //directly from the parent.
                if (IsMapped(evalType, subclassMappings))
                    level++;
                evalImplementsParent = DistanceFromParentBase(parentType, evalType.BaseType, subclassMappings, ref level);
            }

            return evalImplementsParent;
        }
    }
}