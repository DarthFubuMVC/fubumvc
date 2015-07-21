using FubuMVC.Core.ServiceBus;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public interface IScenarioStep
    {
        void PreviewAct(IScenarioWriter writer);
        void PreviewAssert(IScenarioWriter writer);

        void Act(IScenarioWriter writer);
        void Assert(IScenarioWriter writer);

        bool MatchesSentMessage(Message processed);
    }
}