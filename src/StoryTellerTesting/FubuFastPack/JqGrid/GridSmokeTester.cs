using System.Diagnostics;
using FubuFastPack.JqGrid;
using FubuTestApplication;
using FubuTestApplication.Grids;
using NUnit.Framework;
using FubuFastPack.StructureMap;

namespace IntegrationTesting.FubuFastPack.JqGrid
{
    [TestFixture]
    public class GridSmokeTester
    {
        [Test]
        public void try_to_build_out_the_test_harness_case_grid()
        {
            var container = DatabaseDriver.GetFullFastPackContainer();
            container.ExecuteInTransaction<CaseController>(x =>
            {
                Debug.WriteLine(x.AllCases().ToString());
            });

            
        }
    }
}