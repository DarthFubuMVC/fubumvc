using System;

namespace FubuLocalization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HeaderTextAttribute : Attribute
    {
        public HeaderTextAttribute(string text)
        {
            Text = text;
        }

        private string _culture = "en-US";

        public string Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }

        public string Text { get; private set; }
        public string DefaultText { get; set; }
    }
}