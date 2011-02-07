using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap.Bootstrap;
using FubuMVC.StructureMap;
using FubuFastPack.StructureMap;
using FubuTestApplication.Grids;

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
                    var databaseFile = FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), @"..\..\test.db").ToFullPath();
                    var container = DatabaseDriver.BootstrapContainer(databaseFile, false);

                    container.Configure(x =>
                    {
                        x.Activate<ISchemaWriter>("Building the schema", writer =>
                        {
                            writer.BuildSchema();
                        });
                    });

                    return new TransactionalStructureMapContainerFacility(container);
                })
                .Bootstrap(RouteTable.Routes);
        }
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Import<SmartGridRegistry>("data");

            Actions.IncludeType<ScriptsHandler>();

            Route("cases").Calls<CaseController>(x => x.AllCases());
            Route("case/{Id}").Calls<CaseController>(x => x.Show(null));
            Route("viewcase/{Identifier}").Calls<CaseController>(x => x.Case(null));
            Route("loadcases").Calls<CaseController>(x => x.LoadCases(null));
        }
    }
}