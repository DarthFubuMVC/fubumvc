using System;
using FubuCore;

namespace FubuMVC.Core
{
    public interface IModeDetector
    {
        string GetMode();
        void SetMode(string mode);
    }

    public class EnvironmentVariableDetector : IModeDetector
    {
        public const string EnvironmentVariable = "FubuMode";

        public string GetMode()
        {
            return System.Environment.GetEnvironmentVariable(EnvironmentVariable, EnvironmentVariableTarget.Machine) ??
                   "";
        }

        public void SetMode(string mode)
        {
            System.Environment.SetEnvironmentVariable(EnvironmentVariable, mode, EnvironmentVariableTarget.Machine);
        }
    }

    /// <summary>
    /// Detects the machine or environment "mode" of a FubuMVC application.  Generally used
    /// to latch development features
    /// </summary>
    public static class FubuMode
    {
        public static readonly string Development = "Development";
        public static readonly string Testing = "Testing";

        /// <summary>
        /// Change the mechanism used to detect the FubuMode
        /// </summary>
        public static IModeDetector Detector
        {
            get { return _detector; }
            set
            {
                _detector = value;
                _determineMode = new Lazy<string>(_detector.GetMode);
            }
        }

        static FubuMode()
        {
            Reset();
        }

        /// <summary>
        /// Sets the detection strategy back to the default for the runtime,
        /// environment variable detection for Windows, and the .fubumode file
        /// while running in Mono
        /// </summary>
        public static void Reset()
        {

                Detector = new EnvironmentVariableDetector();
        }

        private static Lazy<string> _determineMode;
        private static IModeDetector _detector;

        /// <summary>
        /// Is the "Development" mode detected?
        /// </summary>
        /// <returns></returns>
        public static bool InDevelopment()
        {
            return Mode().InDevelopment();
        }

        /// <summary>
        /// Is the "Development" mode detected?
        /// </summary>
        /// <returns></returns>
        public static bool InDevelopment(this string mode)
        {
            if (mode.IsEmpty()) return false;

            return mode.Equals(Development, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The name of the FubuMode detected
        /// </summary>
        /// <returns></returns>
        public static string Mode()
        {
            return _determineMode.Value ?? string.Empty;
        }

        /// <summary>
        /// Programmatically set the FubuMode.  Generally only used for testing scenarios
        /// </summary>
        /// <param name="mode"></param>
        public static void Mode(string mode)
        {
            _determineMode = new Lazy<string>(() => mode);
        }

        public static bool InTestingMode()
        {
            var returnValue = false;
            bool.TryParse(FubuRuntime.Properties[Testing], out returnValue);

            return returnValue;
        }

        public static void SetupForTestingMode()
        {
            FubuRuntime.Properties[Testing] = true.ToString();
        }

        public static void RemoveTestingMode()
        {
            FubuRuntime.Properties.Remove(Testing);
        }

        /// <summary>
        /// Conditionally execute code if the current mode matches the given node
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
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

        public static void SetUpForDevelopmentMode()
        {
            Mode(Development);
        }
    }

    public class DoExpression
    {
        private readonly bool _shouldDo;

        internal DoExpression(bool shouldDo)
        {
            _shouldDo = shouldDo;
        }

        /// <summary>
        /// Perform an action if the mode matches
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action action)
        {
            if (_shouldDo)
            {
                action();
            }
        }
    }
}