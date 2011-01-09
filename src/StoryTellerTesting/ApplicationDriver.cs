using System;
using System.Diagnostics;
using System.Net;
using System.Web.Script.Serialization;
using WatiN.Core;

namespace IntegrationTesting
{
    public class ApplicationDriver
    {
        private WatiN.Core.IE _browser;
    
        public void NavigateTo(string url)
        {
            if (_browser == null)
            {
                _browser = new IE();
            }

            _browser.GoTo(url);
        }
    
    
        public void Teardown()
        {
            if (_browser != null)
            {
                try
                {
                    _browser.Close();
                    _browser.Dispose();
                }
                catch (Exception)
                {
                }
            }

            try
            {
                var ieProcessList = Process.GetProcessesByName("iexplore");
                foreach (var process in ieProcessList)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            _browser = null;
        }

        public T InvokeJson<T>(string url, object message)
        {
            var client = new WebClient();
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(message);


            var response = client.UploadString(url, json);

            return serializer.Deserialize<T>(response);
        }

        public string InvokeString(string url)
        {
            var client = new WebClient();
            return client.DownloadString(url);
        }

        public IE Browser
        {
            get { return _browser; }
        }
    }
}