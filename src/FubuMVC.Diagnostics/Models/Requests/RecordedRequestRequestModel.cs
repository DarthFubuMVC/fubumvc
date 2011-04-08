using System;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Models.Requests
{
	public class RecordedRequestRequestModel
	{
		[RouteInput]
		public Guid Id { get; set; }
	}
}