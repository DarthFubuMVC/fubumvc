using System;
using Bottles.DependencyAnalysis;

namespace Bottles.Tests.Deployment
{
    public class TestCases
    {
        public void Bob()
        {
            var tree = new DependencyGraph<Bottle>(bot=>bot.Name, bot=>bot.Dependencies);
            

            var a = new Bottle("urn:bottle:a");
            a.AddDependency("urn:bottle:d");

            var b = new Bottle("urn:bottle:b");
            b.AddDependency("urn:bottle:a");
            b.AddDependency("urn:bottle:c");

            var c = new Bottle("urn:bottle:c");
            c.AddDependency("urn:bottle:a");

            var d = new Bottle("urn:bottle:d");
                        
            tree.RegisterItem(b);
            tree.RegisterItem(c);
            tree.RegisterItem(a);
            tree.RegisterItem(d);

            if(tree.HasCycles())
                Console.WriteLine("Found Cycles");

            if(tree.HasMissingDependencies())
            {
                
                Console.WriteLine("Missing deps");
                foreach (var missingDependency in tree.MissingDependencies())
                {
                    Console.WriteLine("Missing {0}", missingDependency);
                }
            }

            //should be d->a->c->b
            Console.WriteLine("Load Order");
            var index = 0;
            foreach (var name in tree.GetLoadOrder())
            {
                Console.WriteLine("{0}. {1}",index, name);
                index++;
            }
        }
    }
}