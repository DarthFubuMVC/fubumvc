using System;

namespace Fubu.Applications
{
    [Serializable]
    public class RecycleResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}