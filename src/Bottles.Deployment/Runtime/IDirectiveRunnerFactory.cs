namespace Bottles.Deployment.Runtime
{
    public interface IDirectiveRunnerFactory
    {
        IDirectiveRunner Build(IDirective directive);
    }
}