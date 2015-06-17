using System.Collections.Generic;

namespace DiagnosticsHarness
{
    public interface INumberCache
    {
        void Register(int number);

        IEnumerable<int> Captured { get; }
    }
}