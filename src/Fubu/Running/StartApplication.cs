using FubuCore;

namespace Fubu.Running
{
    public class StartApplication
    {
        public string ApplicationName { get; set; }
        public int PortNumber { get; set; }
        public string PhysicalPath { get; set; }

        public bool UseProductionMode { get; set; }

        public override string ToString()
        {
            if (ApplicationName.IsNotEmpty())
            {
                return string.Format("ApplicationName: {0}, PortNumber: {1}, PhysicalPath: {2}", ApplicationName, PortNumber, PhysicalPath);
                
            }

            return "{0} with port number {1}".ToFormat(PhysicalPath, PortNumber);
        }
    }
}