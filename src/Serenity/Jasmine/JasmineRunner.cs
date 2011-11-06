using System.Threading;
using FubuMVC.OwinHost;

namespace Serenity.Jasmine
{
    public class JasmineRunner
    {
        private readonly InteractiveJasmineInput _input;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private readonly Thread _kayakLoop;
        private SerenityJasmineApplication _application;
        private ApplicationUnderTest _applicationUnderTest;
        private FubuOwinHost _host;


        public JasmineRunner(InteractiveJasmineInput input)
        {
            _input = input;
            var threadStart = new ThreadStart(run);
            _kayakLoop = new Thread(threadStart);
        }

        // TODO -- this will get changed to Run(file), or we'll bring file thru a ctor
        public void Run()
        {
            buildApplication();
            _kayakLoop.Start();

            // TODO -- make a helper method for this
            _applicationUnderTest.Driver.Navigate().GoToUrl(_applicationUnderTest.RootUrl);

            _reset.WaitOne();
            
        }

        private void run()
        {
            _host = new FubuOwinHost(_application);
            _host.RunApplication(_input.PortFlag);

            _reset.Set();
        }

        private void buildApplication()
        {
            _application = new SerenityJasmineApplication();
            var configLoader = new ConfigFileLoader(_input.SerenityFile, _application);
            configLoader.ReadFile();


            var applicationSettings = new ApplicationSettings(){
                RootUrl = "http://localhost:" + _input.PortFlag
            };

            var browserBuilder = _input.GetBrowserBuilder();

            _applicationUnderTest = new ApplicationUnderTest(_application, applicationSettings, browserBuilder);
        }
    }
}