using System;

namespace Serenity
{
	public class SerenityEnvironment : MarshalByRefObject
	{
		public SerenityEnvironment()
		{
			WorkingDir = AppDomain.CurrentDomain.BaseDirectory;
			Browser = BrowserType.Firefox;
		}

		public string WorkingDir { get; set; }
		public BrowserType Browser { get; set; }
	}
}