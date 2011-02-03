using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuFastPack.NHibernate;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap.Bootstrap;
using FubuMVC.StructureMap;
using FubuFastPack.StructureMap;

namespace FubuTestApplication
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            // TODO -- add smart grid controllers
            FubuApplication.For<FubuTestApplicationRegistry>()
                .StructureMap(() =>
                {
                    var databaseFile = FileSystem.Combine(FubuMvcPackageFacility.GetApplicationPath(), "../../test.db");
                    var container = DatabaseDriver.BootstrapContainer(databaseFile, true);

                    container.Configure(x =>
                    {
                        x.Activate<ISchemaWriter>("Building the schema", writer =>
                        {
                            writer.BuildSchema();
                        });
                    });

                    return container;
                })
                .Bootstrap(RouteTable.Routes);
        }
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Actions.IncludeType<ScriptsHandler>();
        }
    }
}