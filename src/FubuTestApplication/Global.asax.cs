using System.Web;
using System.Web.Routing;
using Bottles;
using FubuCore;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;

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
}