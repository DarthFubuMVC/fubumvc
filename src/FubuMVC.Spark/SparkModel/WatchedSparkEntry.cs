using System;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class WatchedSparkEntry
    {
        private readonly Func<ISparkViewEntry> _source;
        private ISparkViewEntry _entry;

        public WatchedSparkEntry(Func<ISparkViewEntry> source)
        {
            _source = source;
        }

        public void Precompile()
        {
            if (_entry == null)
            {
                _entry = _source();
            }
        }

        public ISparkViewEntry Value
        {
            get
            {
                if (_entry == null || !_entry.IsCurrent())
                {
                    _entry = _source();
                }

                return _entry;
            }
        }
    }
}