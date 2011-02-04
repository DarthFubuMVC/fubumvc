using System;

namespace FubuMVC.Core.Security.AntiForgery
{
	public class AntiForgeryData
	{
		public string Salt { get; set; }
		public string Value { get; set; }
		public string Username { get; set; }
		public DateTime CreationDate { get; set; }
	}
}