namespace FubuMVC.Core.Ajax
{
    /// <summary>
    /// DTO class to standardize the transmission of validation errors
    /// to a client page from the server.
    /// </summary>
    public class AjaxError
    {
        /// <summary>
        /// error/warning/etc.
        /// </summary>
        public string category { get; set; }

        public string field { get; set; }
		public string label { get; set; }
        public string message { get; set; }
    }
}