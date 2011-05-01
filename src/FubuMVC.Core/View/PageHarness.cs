using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Core.View
{
    /// <summary>
    /// Used strictly for supporting automated testing scenarios
    /// </summary>
    /// <typeparam name="T"></typeparam>
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