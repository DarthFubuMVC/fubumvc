using Bottles.Deployment;

namespace Bottles.Tests.Deployment.Runtime
{
    public class FakeDirective : IDirective
    {
        public int Hits;

        public void HitIt()
        {
            Hits++;
        }
    }
}