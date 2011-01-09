using System;
using FubuMVC.Core;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using FubuCore;
using FubuMVC.Core.UI;
using System.Linq;
using System.Collections.Generic;

namespace FubuTestApplication
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
            var document = new HtmlDocument();
            document.Title = "Script Manager Tester";

            request.Mandatories.Split(',').Select(x => x.Trim()).Each(x => this.Script(x));
            if (request.Optionals != null)
                request.Optionals.Split(',').Select(x => x.Trim()).Each(x => this.OptionalScript(x));

            this.WriteScriptTags().AllTags().Each(tag => document.Add(tag));

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

        IUrlRegistry IFubuPage.Urls
        {
            get { throw new NotImplementedException(); }
        }
    }
}