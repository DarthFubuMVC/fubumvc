namespace FubuMVC.Core.Registration.Conventions
{
    [ConfigurationType(ConfigurationType.Explicit)]
    public class PartialOnlyConvention : Policy
    {
        public const string Partial = "Partial";

        public PartialOnlyConvention()
        {
            Where.AnyActionMatches(call => call.Method.Name.EndsWith("Partial"));
            ModifyBy(chain => chain.IsPartialOnly = true, configurationType: ConfigurationType.Explicit);
        }
    }
}