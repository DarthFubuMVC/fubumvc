using System;

namespace FubuMVC.Core.Registration
{
    public static class AssemblyScanningExtensions
    {
        public static IAssemblyScanner AddAllTypesOf<T>(this IAssemblyScanner scanner)
        {
            return scanner.AddAllTypesOf(typeof (T));
        }

        public static IAssemblyScanner AddAllTypesOf(this IAssemblyScanner scanner, Type type)
        {
            return scanner.ApplyConvention(new AddImplementationsServiceRegistrationConvention(type));
        }

        public static IAssemblyScanner ConnectImplementationsToTypesClosing(this IAssemblyScanner scanner, Type openType)
        {
            return scanner.ApplyConvention(new ConnectImplementationsServiceRegistrationConvention(openType));
        }
    }
}