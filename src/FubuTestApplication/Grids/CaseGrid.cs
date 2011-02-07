using System;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuTestApplication.Domain;
using System.Collections.Generic;
using HtmlTags;

namespace FubuTestApplication.Grids
{
    public class CaseGrid : RepositoryGrid<Case>
    {
        public CaseGrid()
        {
            Show(x => x.Identifier);
            Show(x => x.Title);
            Show(x => x.Priority);
        }
    }

    public class CaseRequest
    {
        public string Identifier { get; set; }
    }

    public class CaseDataResponse : JsonMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class CaseDataRequest : JsonMessage
    {
        public Case[] Cases { get; set; }
    }

    public class CaseController
    {
        private readonly IRepository _repository;
        private readonly FubuHtmlDocument<Case> _document;
        private readonly ISqlRunner _runner;
        private readonly IUrlRegistry _urls;

        public CaseController(IRepository repository, FubuHtmlDocument<Case> document, ISqlRunner runner, IUrlRegistry urls)
        {
            _repository = repository;
            _document = document;
            _runner = runner;
            _urls = urls;
        }

        public HtmlDocument AllCases()
        {
            _document.Head.Child(_document.CSS("ui.jqgrid.css"));

            _document.Title = "All Cases";

            _document.Add("h1").Text("All Cases");
            _document.Add("hr");

            _document.Push("ul");
            _repository.GetAll<Case>().Each(x =>
            {
                _document.Push("li");
                _document.Add("a").Text(x.Identifier).Attr("href", _urls.UrlFor(x));
                _document.Pop();
            });

            _document.Pop();


            _document.Add("hr");
            _document.Add(x => x.SmartGridFor<CaseGrid>(null));

            _document.WriteScriptsToBody();

            return _document;
        }

        // I'm just being lazy here and using the Case as the input
        // model.  You might not in real life.
        public HtmlDocument Show(Case @first)
        {
            var @case = _repository.Find<Case>(@first.Id);
            return showCase(@case);
        }

        public HtmlDocument Case(CaseRequest request)
        {
            var @case = _repository.FindBy<Case>(c => c.Identifier == request.Identifier);
            return showCase(@case);
        }

        public CaseDataResponse LoadCases(CaseDataRequest request)
        {
            try
            {
                _runner.ExecuteCommand("delete from cases");

                request.Cases.Each(c => _repository.Save(c));

                return new CaseDataResponse(){
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new CaseDataResponse(){
                    Success = false,
                    Message = e.ToString()
                };
            }
        }

        private HtmlDocument showCase(Case @case)
        {
            _document.SetModel(@case);
            _document.Title = "Cases";

            _document.Title = "Case:  " + @case.Identifier;

            _document.Add("h2").Text("Case");
            _document.Push("dl");
            _document.ShowProp(x => x.Identifier);
            _document.ShowProp(x => x.Title);
            _document.ShowProp(x => x.Priority);
            _document.ShowProp(x => x.Status);
            _document.ShowProp(x => x.Number);

            return _document;
        }

        public FubuHtmlDocument Grid()
        {
            _document.Add("h2").Text("Case Grid");
            _document.Add("hr");
            _document.Add(_document.SmartGridFor<CaseGrid>(5));
            _document.WriteScriptsToBody();

            return _document;
        }
    }
}