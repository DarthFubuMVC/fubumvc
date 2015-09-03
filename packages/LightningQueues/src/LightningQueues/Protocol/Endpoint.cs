namespace LightningQueues.Protocol
{
    public class Endpoint
    {
        public Endpoint(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Host, Port);
        }

        public string Host { get; private set; }
        public int Port { get; private set; }

        public bool Equals(Endpoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Host, Host) && other.Port == Port;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Endpoint)) return false;
            return Equals((Endpoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Host != null ? Host.GetHashCode() : 0)*397) ^ Port;
            }
        }
    }
}