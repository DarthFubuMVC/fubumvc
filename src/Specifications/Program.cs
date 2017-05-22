using StoryTeller;

namespace Specifications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StorytellerAgent.Run(args, new TestSystem());
        }

        public static void FixProblems()
        {
//            using (var runner = new StorytellerRunner(new TestSystem()))
//            {
//                runner
//            }
        }
    }
}