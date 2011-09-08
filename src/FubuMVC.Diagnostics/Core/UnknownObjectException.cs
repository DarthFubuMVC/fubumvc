using System;

namespace FubuMVC.Diagnostics.Core
{
	public class UnknownObjectException : Exception
	{
		public UnknownObjectException(Guid id)
		{
			Id = id;
		}

		public Guid Id { get; private set; }
	}
}