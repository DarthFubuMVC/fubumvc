namespace Fubu.Packages
{
    public class CreatePackageInput
    {
        public string PackageFolder { get; set; }
        public string ZipFile { get; set; }
        public bool PdbFlag { get; set; }
        public bool ForceFlag { get; set; }
    }
}