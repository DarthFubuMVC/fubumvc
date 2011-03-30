using System;
using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class EditEntityModelTester
    {
        [Test]
        public void TypeName_is_the_true_type_name_of_its_target()
        {
            var model = new EditEntityModel(new Case());
            model.TypeName.ShouldEqual(typeof(Case).Name);
        }

        [Test]
        public void EntityType_is_the_true_type_of_its_target()
        {
            var model = new EditEntityModel(new Case());
            model.EntityType.ShouldEqual(typeof(Case));
        }

        [Test]
        public void Id_is_the_id_of_its_target()
        {
            var theId = Guid.NewGuid();
            new EditEntityModel(new Case
                                {
                                    Id = theId
                                }).Id.ShouldEqual(theId);
        }
    }
}