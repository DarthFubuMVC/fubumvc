using FubuCore.Conversion;
using FubuMVC.Core.Json;

namespace FubuMVC.Tests.Json
{
	public class ComplexTypeConverter : FubuJsonConverter<ComplexType>
	{
		public ComplexTypeConverter(IObjectConverter converter) 
			: base(converter)
		{
		}

		protected override string toJsonValue(ComplexType value)
		{
			return value.ToString();
		}
	}
}