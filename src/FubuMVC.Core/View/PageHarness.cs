namespace FubuMVC.Core.View
{
    public class PageHarness<T> where T : class
    {
        public PageHarness(T model, FubuPage<T> page)
        {
            Model = model;
            Page = page;
        }

        public T Model { get; set; }
        public FubuPage<T> Page { get; set; }
    }
}