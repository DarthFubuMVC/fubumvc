using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class CrudTypeExtensionsTester
    {
        [Test]
        public void is_crud_controller_positive()
        {
            typeof(CasesController).IsCrudController().ShouldBeTrue();
            typeof(SitesController).IsCrudController().ShouldBeTrue();
        }

        [Test]
        public void is_crud_controller_negative()
        {
            GetType().IsCrudController().ShouldBeFalse();
        }

        [Test]
        public void get_entity_type()
        {
            typeof(CasesController).GetEntityType().ShouldEqual(typeof(Case));
            typeof(SitesController).GetEntityType().ShouldEqual(typeof(Site));
        }

        [Test]
        public void get_entity_model_type()
        {
            typeof(CasesController).GetEditEntityModelType().ShouldEqual(typeof(EditCaseViewModel));
            typeof(SitesController).GetEditEntityModelType().ShouldEqual(typeof(EditSiteViewModel));
        }
    }


}