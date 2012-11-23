namespace AspNetApplication
{
    public class FormBindingEndpoint
    {
        public string post_form_values(FormInput input)
        {
            return input.ToString();
        }
    }

    public class FormInput
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public override string ToString()
        {
            return string.Format("Color: {0}, Direction: {1}", Color, Direction);
        }
    }
}