using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuTestApplication.Domain;
using FubuTestApplication.Grids;
using FubuMVC.Core.UI;

namespace FubuTestApplication
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            // TODO -- add smart grid controllers
            FubuApplication.For<FubuTestApplicationRegistry>()
                .ContainerFacility(() =>
                {
                    var databaseFile =
                        FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), @"..\..\test.db").ToFullPath();
                    var container = DatabaseDriver.BootstrapContainer(databaseFile, false);

                    container.Configure(
                        x => { x.Activate<ISchemaWriter>("Building the schema", writer => { writer.BuildSchema(); }); });

                    return new TransactionalStructureMapContainerFacility(container);
                })

                // TODO -- convenience method here?
                .Packages(x => x.Assembly(typeof (IGridColumn).Assembly))
                .Bootstrap(RouteTable.Routes);

            PackageRegistry.AssertNoFailures();
        }
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Actions.IncludeType<ScriptsHandler>();

            Route("cases").Calls<CaseController>(x => x.AllCases());
            Route<Case>("case/{Id}").Calls<CaseController>(x => x.Show(null));
            Route<CaseRequest>("viewcase/{Identifier}").Calls<CaseController>(x => x.Case(null));
            Route("loadcases").Calls<CaseController>(x => x.LoadCases(null));
            Route<Person>("person/{Id}").Calls<CaseController>(x => x.Person(null));

            this.ApplySmartGridConventions(x => { x.ToThisAssembly(); });
            this.CombineScriptAndCssFiles(); // only here to test the combining - remove this line if its troublesome
        }
    }
}