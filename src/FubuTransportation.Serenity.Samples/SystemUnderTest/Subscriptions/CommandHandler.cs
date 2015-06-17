namespace FubuTransportation.Serenity.Samples.SystemUnderTest.Subscriptions
{
    public class CommandHandler
    {
        public PublishedEvent Handle(SomeCommand message)
        {
            return new PublishedEvent();
        }
    }
}