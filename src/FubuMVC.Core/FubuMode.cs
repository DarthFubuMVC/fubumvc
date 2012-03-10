using System;

namespace FubuMVC.Core
{
    public static class Fake
    {
        public static void DoSomething()
        {
            FubuMode.ForMode("Development").Do(() =>
            {
                // do stuff
            });

        }
    }

    public static class FubuMode
    {
        public static readonly string Development = "Development";

        static FubuMode()
        {
            Reset();
        }

        private static Lazy<string> _determineMode;
        public static bool InDevelopment()
        {
            return Mode().Equals(Development, StringComparison.OrdinalIgnoreCase);
        }

        public static string Mode()
        {
            return _determineMode.Value;
        }

        public static void Reset()
        {
            _determineMode = new Lazy<string>(() => Environment.GetEnvironmentVariable("FubuMode"));
        }

        public static void Mode(string mode)
        {
            _determineMode = new Lazy<string>(() => mode);
        }

        public static DoExpression ForMode(string mode)
        {
            return new DoExpression(Mode() == mode);
        }


         
        /// <summary>
        /// Use any possible logic you'd like.  Do it by testing for the existence of a 
        /// file, a *gulp* App.config flag, whatever
        /// </summary>
        /// <param name="devTest"></param>
        public static void Mode(Func<string> devTest)
        {
            _determineMode = new Lazy<string>(devTest);
        }
    }

        public class DoExpression
        {
            private readonly bool _shouldDo;

            internal DoExpression(bool shouldDo)
            {
                _shouldDo = shouldDo;
            }

            public void Do(Action action)
            {
                if (_shouldDo)
                {
                    action();
                }
            }
        }
}