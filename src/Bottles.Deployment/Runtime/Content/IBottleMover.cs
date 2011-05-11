using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using System.Linq;

namespace Bottles.Deployment.Runtime.Content
{
    public interface IBottleMover
    {
        void Move(IBottleDestination destination, IEnumerable<BottleReference> references);
    }


    public class BottleMover : IBottleMover
    {
        private readonly IBottleRepository _repository;

        public BottleMover(IBottleRepository repository)
        {
            _repository = repository;
        }

        public void Move(IBottleDestination destination, IEnumerable<BottleReference> references)
        {
            var manifests = references.Select(r => _repository.ReadManifest(r.Name));
            var explosionRequests = manifests.SelectMany(destination.DetermineExplosionRequests);

            explosionRequests.Each(x => _repository.ExplodeFiles(x));
        }
    }


    // Let's say it already "knows" the physical destination
    public interface IBottleDestination
    {
        IEnumerable<BottleExplosionRequest> DetermineExplosionRequests(PackageManifest manifest);
    }

    // Have this thing used by FubuWebsite
    public class FubuBottleDestination : IBottleDestination
    {
        private readonly string _physicalPath;

        public FubuBottleDestination(string physicalPath)
        {
            _physicalPath = physicalPath;
        }



        public bool Equals(FubuBottleDestination other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._physicalPath, _physicalPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FubuBottleDestination)) return false;
            return Equals((FubuBottleDestination) obj);
        }

        public override int GetHashCode()
        {
            return (_physicalPath != null ? _physicalPath.GetHashCode() : 0);
        }

        public IEnumerable<BottleExplosionRequest> DetermineExplosionRequests(PackageManifest manifest)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("PhysicalPath: {0}", _physicalPath);
        }
    }

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

        public string DestinationDirectory { get; set; }

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
            if (obj.GetType() != typeof(BottleExplosionRequest)) return false;
            return Equals((BottleExplosionRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (BottleName != null ? BottleName.GetHashCode() : 0);
                result = (result * 397) ^ (BottleDirectory != null ? BottleDirectory.GetHashCode() : 0);
                result = (result * 397) ^ (DestinationDirectory != null ? DestinationDirectory.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("BottleName: {0}, BottleDirectory: {1}, DestinationDirectory: {2}", BottleName, BottleDirectory, DestinationDirectory);
        }
    }

}