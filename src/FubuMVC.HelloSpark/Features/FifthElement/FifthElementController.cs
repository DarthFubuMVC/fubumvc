namespace FubuMVC.HelloSpark.Features.FifthElement
{
    public class FifthElementController
    {
        public FifthElementViewModel AnotherDimension()
        {
            return new FifthElementViewModel();
        }

        public FubuJourneyViewModel FubuJourney(FubuJourneyInputModel journey)
        {
            return new FubuJourneyViewModel
                       {
                           Message = string.Format("Attempt {0}: Entering {1}", journey.Attempt, journey.WhereTo),
                           Region = journey.WhereTo,
                           Attempt = journey.Attempt,
                           Trajectory = journey.Trajectory
                       };
        }

        public FubuJourneyViewModel FubuProceedToEventHorizon(FubuEventHorizonInputModel horizon)
        {
            return new FubuJourneyViewModel
            {
                Message = string.Format("Attempt {0}: Proceeding to Event Horizon. Trajectory: {1}", horizon.Attempt, horizon.Trajectory),
                Trajectory = horizon.Trajectory,
                Attempt = horizon.Attempt,
            };
        }
    }

    public class FifthElementViewModel
    {
    }
}