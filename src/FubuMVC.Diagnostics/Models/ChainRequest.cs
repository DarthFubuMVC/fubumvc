using System;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Models
{
	public class ChainRequest
	{
        [RouteInput]
		public Guid Id { get; set; }
	}
}