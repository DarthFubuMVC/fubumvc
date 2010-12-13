using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.Diagnostics.Querying
{
    public class RemoteBehaviorGraph
    {
        private readonly string _applicationUrl;
        private readonly string _getAllUrl;

        public RemoteBehaviorGraph(string applicationUrl)
        {
            _applicationUrl = applicationUrl.TrimEnd('/');
            _getAllUrl = _applicationUrl + "/_fubu/all";
        }

        public EndpointModel All()
        {
            var client = new WebClient();
            var jsonBytes = client.DownloadData(_getAllUrl);
            var json = Encoding.Default.GetString(jsonBytes);

            return JsonUtil.Get<EndpointModel>(json);
        }

        public string GetImageUrl(string name)
        {
            var client = new WebClient();
            var url = _applicationUrl + "/_fubu/imageurl/" + name.UrlEncode();

            Debug.WriteLine("Querying " + url);

            return client.DownloadString(url);
        }


    }
}