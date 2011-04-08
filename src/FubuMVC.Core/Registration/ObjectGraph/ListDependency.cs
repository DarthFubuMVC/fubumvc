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

        // TODO -- defensive programming check
        public ObjectDef AddType(Type type)
        {
            var objectDef = new ObjectDef(type);
            _items.Add(objectDef);

            return objectDef;
        }

        public void Add(ObjectDef objectDef)
        {
            // TODO -- defensive programming check
            _items.Add(objectDef);
        }


        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.List(this);
        }

        public void AddRange(IEnumerable<ObjectDef> items)
        {
            _items.AddRange(items);
        }
    }
}