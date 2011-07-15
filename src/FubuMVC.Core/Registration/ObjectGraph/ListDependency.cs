using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public class ListDependency : IDependency
    {
        public ListDependency(Type dependencyType)
        {
            _items = new List<ObjectDef>();
            DependencyType = dependencyType;
        }

        public Type DependencyType { get; private set; }

        private readonly IList<ObjectDef> _items;
        public IEnumerable<ObjectDef> Items
        {
            get { return _items; }
        }

        public Type ElementType
        {
            get
            {
                if (DependencyType.IsArray) return DependencyType.GetElementType();


                return DependencyType.GetGenericArguments()[0];
            }
        }

        public void AddValue(object value)
        {
            _items.Add(new ObjectDef(){
                Value = value
            });
        }

        /// <summary>
        /// Adds a concrete type to the list or enumeration
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ObjectDef AddType(Type type)
        {
            var objectDef = new ObjectDef(type);
            
            objectDef.ValidatePluggabilityTo(ElementType);
            
            _items.Add(objectDef);



            return objectDef;
        }

        /// <summary>
        /// Add a separately configured ObjectDef to the list or enumeration
        /// </summary>
        /// <param name="objectDef"></param>
        public void Add(ObjectDef objectDef)
        {
            objectDef.ValidatePluggabilityTo(ElementType);
            _items.Add(objectDef);
        }


        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.List(this);
        }

        public void AssertValid()
        {
            _items.Each(x => x.ValidatePluggabilityTo(ElementType));
        }

        public void AddRange(IEnumerable<ObjectDef> items)
        {
            items.Each(Add);
        }
    }
}