using System;

namespace FubuMVC.Diagnostics
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