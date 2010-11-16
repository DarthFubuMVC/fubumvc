using System.Globalization;
using System.Threading;

namespace FubuLocalization
{
    public class CurrentCultureContext : ICurrentCultureContext
    {
        public CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
            set { Thread.CurrentThread.CurrentCulture = value; }
        }

        public CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }
    }
}