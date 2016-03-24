using System;
using FubuMVC.Core.Ajax;

namespace FubuMVC.Tests.Validation.Web
{
    public class SampleInputModel
    {
        public string Field { get; set; }

        public SampleInputModel Test()
        {
            return this;
        }

        public SampleInputModel Test(string input)
        {
            return this;
        }

        public SampleInputModel Test(int input)
        {
            return this;
        }
    }

    public class SampleAjaxModel
    {
        public string Name { get; set; }

        public AjaxContinuation post_model(SampleAjaxModel input)
        {
            throw new NotImplementedException();
        }
    }
}