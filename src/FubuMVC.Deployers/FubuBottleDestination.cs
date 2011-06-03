using System.Collections.Generic;
using Bottles;
using Bottles.Deployers.Iis;
using Bottles.Deployment.Runtime.Content;
using FubuCore;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Deployers
{
    public class FubuBottleDestination : WebsiteBottleDestination, IBottleDestination
    {
        private readonly string _physicalPath;

        public FubuBottleDestination(string physicalPath) : base(physicalPath)
        {
            _physicalPath = physicalPath;
        }


        public override IEnumerable<BottleExplosionRequest> DetermineExplosionRequests(PackageManifest manifest)
        {
            var baseRequests = base.DetermineExplosionRequests(manifest);
            switch (manifest.Role)
            {
                    
//                case BottleRoles.Binaries:
//                    yield return new BottleExplosionRequest
//                    {
//                        BottleDirectory = BottleFiles.BinaryFolder, 
//                        BottleName = manifest.Name, 
//                        DestinationDirectory = FileSystem.Combine(_physicalPath, BottleFiles.BinaryFolder)
//                    };
//                    break;
//
//                case BottleRoles.Config:
//                    yield return new BottleExplosionRequest()
//                                 {
//                                     BottleDirectory = BottleFiles.ConfigFolder,
//                                     BottleName = manifest.Name,
//                                     DestinationDirectory = FileSystem.Combine(_physicalPath, BottleFiles.ConfigFolder)
//                                 };
//                    break;

                case BottleRoles.Module:
                    yield return new BottleExplosionRequest()
                                 {
                                     BottleDirectory = null,
                                     BottleName = manifest.Name,
                                     DestinationDirectory = _physicalPath.AppendPath(BottleFiles.BinaryFolder, FubuMvcPackageFacility.FubuPackagesFolder)
                                 };

                    
                    foreach(var request in baseRequests)
                    {
                        yield return request;
                    }
                    break;

//                case BottleRoles.Application:
//                    yield return new BottleExplosionRequest{
//                        BottleName = manifest.Name,
//                        BottleDirectory = BottleFiles.BinaryFolder,
//                        DestinationDirectory = FileSystem.Combine(_physicalPath, BottleFiles.BinaryFolder)
//                    };
//
//                    yield return new BottleExplosionRequest{
//                        BottleName = manifest.Name,
//                        BottleDirectory = BottleFiles.WebContentFolder,
//                        DestinationDirectory = _physicalPath
//                    };
//                    
//                    break;

                default:
                    foreach(var request in baseRequests)
                    {
                        yield return request;
                    }
                    break;
            }
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
            if (obj.GetType() != typeof(FubuBottleDestination)) return false;
            return Equals((FubuBottleDestination)obj);
        }

        public override int GetHashCode()
        {
            return (_physicalPath != null ? _physicalPath.GetHashCode() : 0);
        }


        public override string ToString()
        {
            return string.Format("PhysicalPath: {0}", _physicalPath);
        }
    }
}