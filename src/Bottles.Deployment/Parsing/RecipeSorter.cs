using System.Collections.Generic;

namespace Bottles.Deployment.Parsing
{
    public class RecipeSorter : IRecipeSorter
    {
        // TODO -- make this do something real with dependency graphs
        public IEnumerable<Recipe> Order(IEnumerable<Recipe> recipes)
        {
            return recipes;
        }
    }
}