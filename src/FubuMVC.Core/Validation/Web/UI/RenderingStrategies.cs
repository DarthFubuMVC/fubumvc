using System;
using FubuCore;
using FubuMVC.Core.UI.Forms;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class RenderingStrategies : IRenderingStrategy
	{
		public static readonly RenderingStrategies Summary = new RenderingStrategies("Summary", x => x.CurrentTag.Data("validation-summary", true));
		public static readonly RenderingStrategies Highlight = new RenderingStrategies("Highlight", x => x.CurrentTag.Data("validation-highlight", true));
		public static readonly RenderingStrategies Inline = new RenderingStrategies("Inline", x => x.CurrentTag.Data("validation-inline", true));

		private readonly string _name;
		private readonly Action<FormRequest> _modify;

		public RenderingStrategies(string name, Action<FormRequest> modify)
		{
			_name = name;
			_modify = modify;
		}

		public string Name { get { return _name; } }

		public void Modify(FormRequest request)
		{
			_modify(request);
		}

		public override string ToString()
		{
			return "Rendering Strategy: {0}".ToFormat(_name);
		}
	}
}