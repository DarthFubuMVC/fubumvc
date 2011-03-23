using System;

namespace FubuMVC.Diagnostics
{
	public class UnknownChainException : Exception
	{
		public UnknownChainException(Guid chainId)
		{
			ChainId = chainId;
		}

		public Guid ChainId { get; private set; }
	}
}