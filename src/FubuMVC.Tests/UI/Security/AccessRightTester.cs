using FubuMVC.UI.Security;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Security
{
    [TestFixture]
    public class AccessRightTester
    {
        [Test]
        public void all_should_have_both_read_and_write()
        {
            AccessRight.All.Read.ShouldBeTrue();
            AccessRight.All.Write.ShouldBeTrue();
        }

        [Test]
        public void read_only_should_have_read_but_not_write()
        {
            AccessRight.ReadOnly.Read.ShouldBeTrue();
            AccessRight.ReadOnly.Write.ShouldBeFalse();
        }

        [Test]
        public void none_should_have_neither_read_nor_write()
        {
            AccessRight.None.Read.ShouldBeFalse();
            AccessRight.None.Write.ShouldBeFalse();
        }

        [Test]
        public void plus_operator_always_returns_the_highest_level_of_rights()
        {
            (AccessRight.All + AccessRight.None).ShouldEqual(AccessRight.All);
            (AccessRight.All + AccessRight.All).ShouldEqual(AccessRight.All);
            (AccessRight.All + AccessRight.ReadOnly).ShouldEqual(AccessRight.All);

            (AccessRight.ReadOnly + AccessRight.None).ShouldEqual(AccessRight.ReadOnly);
            (AccessRight.ReadOnly + AccessRight.ReadOnly).ShouldEqual(AccessRight.ReadOnly);
            (AccessRight.ReadOnly + AccessRight.All).ShouldEqual(AccessRight.All);

            (AccessRight.None + AccessRight.None).ShouldEqual(AccessRight.None);
            (AccessRight.None + AccessRight.ReadOnly).ShouldEqual(AccessRight.ReadOnly);
            (AccessRight.None + AccessRight.All).ShouldEqual(AccessRight.All);
        }


        [Test]
        public void the_most()
        {
            AccessRight.Most(AccessRight.None, AccessRight.All, AccessRight.ReadOnly).ShouldEqual(AccessRight.All);
            AccessRight.Most(AccessRight.None, AccessRight.None, AccessRight.ReadOnly).ShouldEqual(AccessRight.ReadOnly);
            AccessRight.Most(AccessRight.None, AccessRight.None).ShouldEqual(AccessRight.None);
        }

        [Test]
        public void the_least()
        {
            AccessRight.Least(AccessRight.None, AccessRight.All, AccessRight.ReadOnly).ShouldEqual(AccessRight.None);
            AccessRight.Least(AccessRight.All, AccessRight.All, AccessRight.ReadOnly).ShouldEqual(AccessRight.ReadOnly);
            AccessRight.Least(AccessRight.All, AccessRight.All).ShouldEqual(AccessRight.All);
        }

    }


}