namespace FubuMVC.Docs
{
    public class FubuMvcMainTopicTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuMvcMainTopicTopicRegistry()
        {
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Ajax.WorkingWithAjaxEndpoints>();

            For<FubuMVC.Docs.Ajax.WorkingWithAjaxEndpoints>().Append<FubuMVC.Docs.Ajax.DetectingAnAjaxRequest>();

        }
    }
}
