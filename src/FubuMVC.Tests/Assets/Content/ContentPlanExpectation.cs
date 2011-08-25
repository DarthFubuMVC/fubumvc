using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.Assets.Content;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Content
{
    public class ContentPlanExpectation
    {
        private readonly IList<string> _expectations = new List<string>();

        public ContentPlanExpectation(string expectation)
        {
            var reader = new StringReader(expectation);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _expectations.Add(line.Trim());
            }
        }

        public void Assert(ContentPlan plan)
        {
            var previewer = new ContentPlanPreviewer();
            plan.AcceptVisitor(previewer);

            try
            {
                previewer.Descriptions.ShouldHaveTheSameElementsAs(_expectations);
            }
            catch (Exception)
            {
                new ContentExpectationWriter(_expectations, previewer.Descriptions.ToList());

                throw;
            }
        }


    }
}