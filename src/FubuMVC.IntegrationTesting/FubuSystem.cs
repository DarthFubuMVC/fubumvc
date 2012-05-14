using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestApplication;
using StructureMap;
using TestPackage1;

namespace FubuMVC.IntegrationTesting
{
    public class FubuTestApplication : IApplicationSource
    {
        public string Name
        {
            get { return "FubuTestApplication"; }
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .For<FubuTestApplicationRegistry>()
                .StructureMap(new Container())
                .Packages(x => x.Assembly(typeof (JsonSerializedMessage).Assembly));
        }
    }
}