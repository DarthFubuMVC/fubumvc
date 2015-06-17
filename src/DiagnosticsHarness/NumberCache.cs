using System.Collections.Generic;

namespace DiagnosticsHarness
{
    public class NumberCache : INumberCache
    {
        private readonly IList<int> _numbers = new List<int>();

        public void Register(int number)
        {
            _numbers.Add(number);
        }

        public IEnumerable<int> Captured
        {
            get { return _numbers; }
        }
    }
}