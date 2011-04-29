using Bottles.Commands;
using NUnit.Framework;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class ListCommandTester
    {
        [Test]
        public void Test()
        {
            var input = new ListInput()
                        {
                            PointFlag = @"..\..\..\.."
                        };

            var cmd = new ListCommand();

            cmd.Execute(input);

            //REVIEW: how do I test the console out put?
        }
    }
}