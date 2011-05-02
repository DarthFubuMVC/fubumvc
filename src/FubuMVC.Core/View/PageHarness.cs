namespace FubuMVC.Core.View
{
    /// <summary>
    /// Used strictly for supporting automated testing scenarios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageHarness<T> where T : class
    {
        public PageHarness(T model, SimpleFubuPage<T> page)
        {
            Model = model;
            Page = page;
        }

        public T Model { get; set; }
        public SimpleFubuPage<T> Page { get; set; }
    }
}