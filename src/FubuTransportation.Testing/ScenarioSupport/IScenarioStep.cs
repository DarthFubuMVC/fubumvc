namespace FubuTransportation.Testing.ScenarioSupport
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