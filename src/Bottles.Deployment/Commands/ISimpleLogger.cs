using System;

namespace Bottles.Deployment.Commands
{
    public interface ISimpleLogger
    {
        void Log(string text, params object[] parameters);
    }

    public class SimpleLogger : ISimpleLogger
    {
        public void Log(string text, params object[] parameters)
        {
            Console.WriteLine(text, parameters);
        }
    }
}