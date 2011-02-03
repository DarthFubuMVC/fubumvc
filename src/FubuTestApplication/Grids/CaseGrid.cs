using FubuFastPack.JqGrid;
using FubuTestApplication.Domain;

namespace FubuTestApplication.Grids
{
    public class CaseGrid : RepositoryGrid<Case>
    {
        public CaseGrid()
        {
            Show(x => x.Identifier);
            Show(x => x.Title);
            Show(x => x.Priority);
        }
    }

    

    public class CaseController
    {
        
    }
}