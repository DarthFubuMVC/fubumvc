using Bottles.Diagnostics;

namespace Bottles.Deployment
{

    public class BottleExplosionRequest
    {
        public BottleExplosionRequest(IPackageLog log)
        {
            Log = log;
        }

        public string BottleName { get; set; }

        /// <summary>
        /// This is the directory within the bottle
        /// </summary>
        public string BottleDirectory { get; set; }
        public string DestinationDirectory { get; set;}

        public IPackageLog Log { get; private set; }

        public bool Equals(BottleExplosionRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.BottleName, BottleName) && Equals(other.BottleDirectory, BottleDirectory) && Equals(other.DestinationDirectory, DestinationDirectory);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BottleExplosionRequest)) return false;
            return Equals((BottleExplosionRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (BottleName != null ? BottleName.GetHashCode() : 0);
                result = (result*397) ^ (BottleDirectory != null ? BottleDirectory.GetHashCode() : 0);
                result = (result*397) ^ (DestinationDirectory != null ? DestinationDirectory.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("BottleName: {0}, BottleDirectory: {1}, DestinationDirectory: {2}", BottleName, BottleDirectory, DestinationDirectory);
        }
    }

    public interface IBottleRepository
    {
        void CopyTo(string bottleName, string destination);
        void ExplodeTo(string bottleName, string destination);

        void ExplodeFiles(BottleExplosionRequest request);
    }
}