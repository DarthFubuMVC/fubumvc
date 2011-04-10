using System.Web;
using System.Web.Routing;
using Bottles;
using FubuCore;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuTestApplication.ConnegActions;
using FubuTestApplication.Domain;
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
            Route("case/{Id}").Calls<CaseController>(x => x.Show(null));
            Route("viewcase/{Identifier}").Calls<CaseController>(x => x.Case(null));
            Route("loadcases").Calls<CaseController>(x => x.LoadCases(null));
            Route("person/{Id}").Calls<CaseController>(x => x.Person(null));

            Route("conneg/mirror").Calls<MirrorAction>(x => x.Return(null));
            Route("conneg/buckrogers").Calls<MirrorAction>(x => x.BuckRogers());

            Media.ApplyContentNegotiationToActions(call => call.HandlerType == typeof (MirrorAction));

            this.ApplySmartGridConventions(x => { x.ToThisAssembly(); });
        }
    }
}