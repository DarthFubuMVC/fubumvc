using System;
using System.Reflection;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class when_loading_a_single_package : InteractionContext<AssemblyLoader>
    {
        protected override void beforeEach()
        {
            
        }

        [Test]
        public void uses_double_dispatch_to_let_a_package_use_itself_to_load_assemblies()
        {
            ClassUnderTest.LoadAssembliesFromPackage(MockFor<IPackageInfo>());
            MockFor<IPackageInfo>().AssertWasCalled(x => x.LoadAssemblies(ClassUnderTest));
        }

        [Test]
        public void log_successful_registration_of_an_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var package = new StubPackage("something"){
                LoadingAssemblies = x => x.Use(assembly)
            };

            ClassUnderTest.LoadAssembliesFromPackage(package);

            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogAssembly(package, assembly, AssemblyLoader.DIRECTLY_REGISTERED_MESSAGE));

        
            ClassUnderTest.Assemblies.Contains(assembly).ShouldBeTrue();
        }

        [Test]
        public void log_duplicate_registration_of_an_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var package1 = new StubPackage("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };

            var package2 = new StubPackage("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };

            ClassUnderTest.LoadAssembliesFromPackage(package1);
            ClassUnderTest.LoadAssembliesFromPackage(package2);

            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogDuplicateAssembly(package2, assembly.FullName));
        }

        [Test]
        public void load_successfully_from_file_for_a_new_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var package = new StubPackage("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", "AssemblyName")
            };

            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Return(assembly);

            ClassUnderTest.LoadAssembliesFromPackage(package);

            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogAssembly(package, assembly, "Loaded from filename.dll"));
            assemblyFileLoader.VerifyAllExpectations();

            ClassUnderTest.Assemblies.Contains(assembly).ShouldBeTrue();
        }



        [Test]
        public void load_unsuccessfully_from_file_for_a_new_assembly()
        {
            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var package = new StubPackage("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", "AssemblyName")
            };

            var theExceptionFromAssemblyLoading = new ApplicationException("You shall not pass!");
            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Throw(theExceptionFromAssemblyLoading);

            ClassUnderTest.LoadAssembliesFromPackage(package);

            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogAssemblyFailure(package, "filename.dll", theExceptionFromAssemblyLoading));

        }


        [Test]
        public void load_duplicate_assembly_attempt_from_file_for_a_new_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var package1 = new StubPackage("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };
            ClassUnderTest.LoadAssembliesFromPackage(package1);

            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var theAssemblyName = assembly.GetName().Name;
            var package = new StubPackage("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", theAssemblyName)
            };

            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Return(assembly);

            ClassUnderTest.LoadAssembliesFromPackage(package);

            MockFor<IPackagingDiagnostics>().AssertWasCalled(x => x.LogDuplicateAssembly(package, theAssemblyName));


            ClassUnderTest.Assemblies.Count.ShouldEqual(1);
        }
    }

    
}