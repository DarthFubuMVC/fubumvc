using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using FubuCore;
using FubuFastPack.NHibernate;
using FubuFastPack.Persistence;
using FubuFastPack.StructureMap;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestApplication;
using FubuTestApplication.Domain;
using FubuTestApplication.Grids;
using HtmlTags;
using NHibernate;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Engine;
using StructureMap;
using System.Linq;

namespace IntegrationTesting.Fixtures.FubuFastPack
{
    public class SmartGridFixture : Fixture
    {

        private IList<Case> _cases = new List<Case>();
        public IGrammar CasesAre()
        {
            return CreateNewObject<Case>(x =>
            {
                x.CreateNew<Case>();
                x.SetProperty(c => c.Identifier, "001");
                x.SetProperty(c => c.CaseType, "Question");
                x.SetProperty(c => c.Condition, "Open");
                x.SetProperty(c => c.Priority, "Urgent");
                x.SetProperty(c => c.Status, "Working");
                x.SetProperty(c => c.Title, "Important Case");
                x.SetProperty(c => c.Number, "100");

                x.Do = c => _cases.Add(c);
            }).AsTable("If the cases are").Before(() => _cases = new List<Case>()).After(postCases);
        }

        private void postCases()
        {
            var request = new CaseDataRequest(){
                Cases = _cases.ToArray()
            };

            var client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            var bytes = Encoding.Default.GetBytes(JsonUtil.ToJson(request));
            var response = client.UploadData("http://localhost/fubu-testing/loadcases", "POST", bytes);
            var message = JsonUtil.Get<CaseDataResponse>(response);
            StoryTellerAssert.Fail(!message.Success, message.Message);
        }
    }
}