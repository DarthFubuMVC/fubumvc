using Bottles.Deployment;
using Bottles.Deployment.Parsing;
using NUnit.Framework;
using System.Collections.Generic;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class RecipeSorterTester
    {
        [Test]
        public void NestedGraphTest()
        {
            /*      a
             *     / \
             *    b   c
             *   /   / \
             *  d   e   f
             *  
             * d,e,f - b,c - a
             * 
             */

            var ar = new Recipe("a");
            ar.RegisterDependency("b");
            ar.RegisterDependency("c");

            var br = new Recipe("b");
            br.RegisterDependency("d");

            var cr = new Recipe("c");
            cr.RegisterDependency("e");
            cr.RegisterDependency("f");

            var dr = new Recipe("d");
            var er = new Recipe("e");
            var fr = new Recipe("f");

            var r = new List<Recipe> {ar, br, cr, dr, er, fr};

            var sorter = new RecipeSorter();

            var output = sorter.Order(r);
            output.ShouldHaveTheSameElementsAs(dr,er,fr,br,cr,ar);
        }


        [Test]
        public void NestedGraphTest2()
        {
            /*      a
             *     / \
             *    b   c
             *   /   / \
             *  d <-e   f
             *  
             * e, f, c, d, b, a
             * 
             */

            var ar = new Recipe("a");
            ar.RegisterDependency("b");
            ar.RegisterDependency("c");

            var br = new Recipe("b");
            br.RegisterDependency("d");

            var cr = new Recipe("c");
            cr.RegisterDependency("e");
            cr.RegisterDependency("f");

            var dr = new Recipe("d");
            dr.RegisterDependency("e");
            
            var er = new Recipe("e");

            var fr = new Recipe("f");

            var r = new List<Recipe> { ar, br, cr, dr, er, fr };

            var sorter = new RecipeSorter();

            var output = sorter.Order(r);
            output.ShouldHaveTheSameElementsAs(er, fr, cr, dr, br, ar);
        }
    }
}