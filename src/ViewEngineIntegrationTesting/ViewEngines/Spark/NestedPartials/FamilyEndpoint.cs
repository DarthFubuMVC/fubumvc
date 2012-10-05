namespace ViewEngineIntegrationTesting.ViewEngines.Spark.NestedPartials
{
    public class FamilyEndpoint
    {
        public JackViewModel Jack(JackViewModel input)
        {
            return new JackViewModel();
        }
        public MarcusViewModel Marcus(MarcusViewModel input)
        {
            return new MarcusViewModel();
        }
        public GeorgeViewModel George(GeorgeViewModel input)
        {
            return new GeorgeViewModel();
        }
        public SilviaViewModel Silvia(SilviaViewModel input)
        {
            return new SilviaViewModel();
        }

    }

    public class JackViewModel {}
    public class MarcusViewModel {}
    public class GeorgeViewModel {}
    public class SilviaViewModel {}
}