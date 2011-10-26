namespace FubuMVC.Core.Ajax
{
    public class AjaxError
    {
        // error/warning/I don't know.
        public string category { get; set; }

        // Use this to attach the server side validation errors 
        public string field { get; set; }
        public string message { get; set; }
    }
}