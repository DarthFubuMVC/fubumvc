using FubuMVC.Core;
using Serenity;
using System;

namespace %NAMESPACE%
{
	public class ReplaceWithYourApplicationSource : IApplicationSource
	{
        public FubuApplication BuildApplication()
        {
            throw new NotImplementedException();
        }
	}

	public class %SHORT_NAME%System : FubuMvcSystem<ReplaceWithYourApplicationSource>
	{

	}
}