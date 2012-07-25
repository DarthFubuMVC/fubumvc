using System;

namespace FubuMVC.Core
{
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
            _determineMode = new Lazy<string>(() => Environment.GetEnvironmentVariable("FubuMode") ?? "");
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
        /// Provides a lambda to allow Mode to be set through complex logic using delayed execution 
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