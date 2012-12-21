using System;
using FubuCore;
using FubuMVC.Core.Packaging;

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
            return Environment.GetEnvironmentVariable(EnvironmentVariableDetector.EnvironmentVariable, EnvironmentVariableTarget.Machine) ?? "";
        }

        public void SetMode(string mode)
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableDetector.EnvironmentVariable, mode, EnvironmentVariableTarget.Machine);
        }
    }

    public class FubuModeFileDetector : IModeDetector
    {
        public static readonly string File = ".fubumode";

        private static string filename()
        {
            return FubuMvcPackageFacility.GetApplicationPath().AppendPath(File);
        }

        public string GetMode()
        {
            var system = new FileSystem();
            var file = filename();
            if (system.FileExists(file))
            {
                return (system.ReadStringFromFile(file) ?? "").Trim();
            }

            return string.Empty;
        }

        public void SetMode(string mode)
        {
            var fileSystem = new FileSystem();
            string file = filename();
            if (mode.IsEmpty())
            {
                fileSystem.DeleteFile(file);
            }
            else
            {
                fileSystem.WriteStringToFile(file, mode);
            }
        }

        public static void Clear()
        {
            var fileSystem = new FileSystem();
            string file = filename();

            fileSystem.DeleteFile(file);
        }
    }


    public static class FubuMode
    {
        public static readonly string Development = "Development";

        public static IModeDetector Detector
        {
            get { return _detector; }
            set
            {
                _detector = value;
                _determineMode = new Lazy<string>(_detector.GetMode);
            }
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        static FubuMode()
        {
            Reset();
        }

        public static void Reset()
        {
            if (IsRunningOnMono())
            {
                Detector = new FubuModeFileDetector();
            }
            else
            {
                Detector = new EnvironmentVariableDetector();
            }
        }

        private static Lazy<string> _determineMode;
        private static IModeDetector _detector;

        public static bool InDevelopment()
        {
            return Mode().Equals(Development, StringComparison.OrdinalIgnoreCase);
        }

        public static string Mode()
        {
            return _determineMode.Value;
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