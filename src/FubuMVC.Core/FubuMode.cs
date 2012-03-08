using System;

namespace FubuMVC.Core
{
    public static class FubuMode
    {
        static FubuMode()
        {
            DevModeReset();
        }

        private static Lazy<bool> _isInDevMode;
        public static bool DevMode()
        {
            return _isInDevMode.Value;
        }

        public static void DevModeReset()
        {
            _isInDevMode = _isInDevMode = new Lazy<bool>(() => Environment.GetEnvironmentVariable("FubuMode").Equals("Development", StringComparison.OrdinalIgnoreCase));
        }

        public static void DevMode(bool isDevMode)
        {
            _isInDevMode = new Lazy<bool>(() => isDevMode);
        }
         
        /// <summary>
        /// Use any possible logic you'd like.  Do it by testing for the existence of a 
        /// file, a *gulp* App.config flag, whatever
        /// </summary>
        /// <param name="devTest"></param>
        public static void DevMode(Func<bool> devTest)
        {
            _isInDevMode = new Lazy<bool>(devTest);
        }
    }
}