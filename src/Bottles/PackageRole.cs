namespace Bottles
{
    public static class PackageRole
    {
        /// <summary>
        /// The package should just contain dlls. Useful for getting 3rd party dlls
        /// into the right spot during deployment.
        /// </summary>
        public static readonly string Binaries = "binaries";

        /// <summary>
        /// Packages that extend 'application' with new functionality. Another
        /// way to express this is 'plugin'
        /// </summary>
        public static readonly string Module = "module";

        /// <summary>
        /// This package should just contain config files.
        /// </summary>
        public static readonly string Config = "config";

        /// <summary>
        /// This represents an application like a console or website project
        /// </summary>
        public static readonly string Application = "application";
    }
}