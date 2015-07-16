namespace Bottles.Configuration
{
    public interface IBottleConfigurationRule
    {
        void Evaluate(BottleConfiguration configuration);
    }
}