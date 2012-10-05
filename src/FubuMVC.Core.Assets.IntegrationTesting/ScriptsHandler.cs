using System;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using FubuCore;
using FubuMVC.Core.UI;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    public class ScriptRequest
    {

        public string Mandatories { get; set; }

        public string Optionals { get; set; }
    }

    public class ScriptsHandler : IFubuPage
    {
        private readonly IServiceLocator _locator;

        public ScriptsHandler(IServiceLocator locator)
        {
            _locator = locator;
        }

        [UrlPattern("scriptloading/{Mandatories}")]
        public HtmlDocument LinkScripts(ScriptRequest request)
        {
            var document = new HtmlDocument{
                Title = "Script Manager Tester"
            };



            

            request.Mandatories.UrlDecode().Split(',').Select(x => x.Trim()).Each(x => this.Asset(x));
            if (request.Optionals != null)
                request.Optionals.Split(',').Select(x => x.Trim()).Each(x => this.OptionalScript(x));

            this.WriteAssetTags().AllTags().Each(tag => document.Head.Append(tag));

            return document;
        }

        string IFubuPage.ElementPrefix
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        IServiceLocator IFubuPage.ServiceLocator
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        T IFubuPage.Get<T>()
        {
            return _locator.GetInstance<T>();
        }

        T IFubuPage.GetNew<T>()
        {
            throw new NotImplementedException();
        }

        public void Write(object content)
        {
            throw new NotImplementedException();
        }

        IUrlRegistry IFubuPage.Urls
        {
            get { throw new NotImplementedException(); }
        }
    }
}