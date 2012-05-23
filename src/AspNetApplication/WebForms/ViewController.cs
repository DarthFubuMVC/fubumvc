namespace AspNetApplication.WebForms
{
    public class WebFormInput
    {
        public string Name { get; set; }
    }

    public class WebFormOutput
    {
        public string Text { get; set; }
    }

    public class ViewController
    {
        public WebFormOutput get_webforms_simple_Name(WebFormInput input)
        {
            return new WebFormOutput{
                Text = "My name is " + input.Name
            };
        }
    }
}