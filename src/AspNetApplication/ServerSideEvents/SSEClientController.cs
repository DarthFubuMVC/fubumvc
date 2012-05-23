using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using HtmlTags;
using HtmlTags.Extended.Attributes;

namespace AspNetApplication.ServerSideEvents
{
    public class SSEClientModel
    {
        public string Url { get; set; }
    }

    public class SSEClientController
    {
        private readonly FubuHtmlDocument<SSEClientModel> _document;
        private readonly IUrlRegistry _urls;

        public SSEClientController(FubuHtmlDocument<SSEClientModel> document, IUrlRegistry urls)
        {
            _document = document;
            _urls = urls;
        }

        public HtmlDocument get_events()
        {
            var model = new SSEClientModel{
                Url = _urls.UrlFor<SimpleFlowController>(x => x.get_events_simple(null))
            };

            _document.Model = model;

            _document.Title = "Server Sent Events Test Harness";

            _document.Add(x => x.WriteAssetTagsImmediately(MimeType.Javascript, "jquery", "sse/serverSideEventPage.js"));

            _document.Add("h1").Text("Server Sent Events");
            _document.Add(new HiddenTag().Id("url").Value(model.Url));

            var tag = new LiteralTag(@"
<table>
    <tr>
        <td>Last message</td>
        <td id='last-message'></td>
    </tr>
    <tr>
        <td>All Messages</td>
        <td><ul id='all-messages'></ul></td>
    </tr>

</table>


".Replace("'", "\""));

            _document.Add(tag);

            return _document;
        }
    }
}