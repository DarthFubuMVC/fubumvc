using System;
using System.Collections.Generic;

namespace Bottles.Deployment
{
    public interface IHostManifest
    {
        T GetSettings<T>();
    }

    public class HostManifest : IHostManifest
    {
        private readonly IList<BottleReference> _bottles = new List<BottleReference>();
        

        public HostManifest(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
    
        public void RegisterBottle(BottleReference reference)
        {
            _bottles.Add(reference);
        }

        public T GetSettings<T>()
        {
            throw new NotImplementedException();
        }
    }
}