using System;
using System.IO;
using Fubu.Packages;
using FubuCore.CommandLine;
using NUnit.Framework;

namespace IntegrationTesting
{
    [TestFixture]
    public class debugger
    {
        [Test]
        public void try_to_set_up_the_environment()
        {
            var system = new FubuSystem();
            system.SetupEnvironment();
        }

		[Test]
		public void try_to_create_a_package()
		{
			var factory = new CommandFactory();
			factory.RegisterCommands(typeof(IFubuCommand).Assembly);
			factory.RegisterCommands(typeof(CreatePackageCommand).Assembly);

			var atRoot = false;
			var directory = Environment.CurrentDirectory;
			while(!atRoot)
			{
				directory = new DirectoryInfo(directory).Parent.FullName;
				atRoot = Directory.Exists(Path.Combine(directory, "src"));
			}

			Environment.CurrentDirectory = directory;

			var executor = new CommandExecutor(factory);
			executor.Execute("init-pak src/TestPackage4 pak4");
			executor.Execute("create-pak pak4 pak4.zip -f");
		}
    }
}