using System;

namespace Bottles
{
    public class BottleLoadingRecord
    {
        public BottleLoadingRecord()
        {
            Started = DateTime.Now;
        }

        public DateTime Started { get; private set; }
        public DateTime Finished { get; set; }

        public override string ToString()
        {
            return string.Format("Bottling Process finished on {0} at {1}", Finished.ToShortDateString(), Finished.ToLongTimeString());
        }
    }
}