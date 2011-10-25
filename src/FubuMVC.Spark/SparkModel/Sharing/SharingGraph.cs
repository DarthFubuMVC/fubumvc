using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SharingGraph : ISharingRegistration
    {
        private readonly List<GlobalDependency> _globals = new List<GlobalDependency>();
        private readonly List<DependencyRule> _dependencyRules = new List<DependencyRule>(); 

        public void Global(string global)
        {
            _globals.Fill(new GlobalDependency
            {
                Dependent = global                                          
            });
        }

        public void Dependency(string dependent, string dependency)
        {
            _dependencyRules.Fill(new DependencyRule
            {
                Dependent = dependent,
                Dependency = dependency                                          
            });
        }

        public class GlobalDependency
        {            
            public string Dependent { get; set; }
            public int Rank { get; set; }
        }

        public class DependencyRule
        {
            public string Dependency { get; set; }
            public string Dependent { get; set; }
        }
    }
}