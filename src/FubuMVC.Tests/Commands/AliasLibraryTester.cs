using Fubu;
using FubuCore;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class AliasLibraryTester
    {
        private IFileSystem fileSystem;
        private AliasRegistry theRegistry;
        private AliasCommand theCommand;

        [SetUp]
        public void SetUp()
        {
            fileSystem = MockRepository.GenerateMock<IFileSystem>();
            theRegistry = new AliasRegistry();

            fileSystem.Stub(x => x.LoadFromFile<AliasRegistry>(AliasRegistry.ALIAS_FILE))
                .Return(theRegistry);

            theCommand = new AliasCommand();
        }

        [Test]
        public void add_alias_when_it_does_not_exist()
        {
            theCommand.Execute(new AliasInput(){
                Name = "agent",
                Folder = "/something"
            }, fileSystem);

            fileSystem.AssertWasCalled(x => x.WriteObjectToFile(AliasRegistry.ALIAS_FILE, theRegistry));

            theRegistry.AliasFor("agent").Folder.ShouldEqual("/something");
        }

        [Test]
        public void add_alias_when_it_does_exist()
        {
            theRegistry.CreateAlias("agent", "/different");

            theCommand.Execute(new AliasInput()
            {
                Name = "agent",
                Folder = "/something"
            }, fileSystem);

            fileSystem.AssertWasCalled(x => x.WriteObjectToFile(AliasRegistry.ALIAS_FILE, theRegistry));

            theRegistry.AliasFor("agent").Folder.ShouldEqual("/something");
        }

        [Test]
        public void display_smoke_test()
        {
            theRegistry.CreateAlias("agent", "/different");

            theCommand.Execute(new AliasInput()
            {

            }, fileSystem);
        }

        [Test]
        public void remove_an_alias()
        {
            theRegistry.CreateAlias("agent", "/different");
            theRegistry.CreateAlias("agent2", "/different");
            theRegistry.CreateAlias("agent3", "/different");

            theCommand.Execute(new AliasInput()
            {
                Name = "agent",
                RemoveFlag = true
            }, fileSystem);

            fileSystem.AssertWasCalled(x => x.WriteObjectToFile(AliasRegistry.ALIAS_FILE, theRegistry));

            theRegistry.Aliases.ShouldHaveCount(2);
            theRegistry.AliasFor("agent").ShouldBeNull();
        }

    }
}