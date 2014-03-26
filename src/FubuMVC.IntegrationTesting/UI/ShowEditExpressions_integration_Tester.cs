using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Security;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class ShowEditExpressions_integration_Tester
    {
        // TODO -- change all this to in memory host and scenarios
        public class TestRegistry : FubuRegistry
        {
            public TestRegistry()
            {
                Services(x => x.AddService<IFieldAccessRule, ShowEditFakePolicy>());
                Actions.IncludeType<ShowEditEndpoints>();
                Import<HtmlConventionRegistry>(
                    x => { x.Profile("table", profile => profile.FieldChrome<TableRowFieldChrome>()); });
            }
        }

        [Test]
        public void edit_downgrades_to_show_when_the_access_rights_make_it_so()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.ReadOnly});

                server.Endpoints.GetByInput(new EditModel {Name = "Jeremy"})
                      .ToString()
                      .ShouldEqual("<dt><label for=\"Name\">Name</label></dt>\n<dd><span id=\"Name\">Jeremy</span></dd>");
            }
        }

        [Test]
        public void edit_shows_nothing_with_no_access_rights()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.None});

                server.Endpoints.GetByInput(new EditModel {Name = "Jeremy"}).ReadAsText().ShouldBeEmpty();
            }
        }

        [Test]
        public void field_chrome_is_served_up_by_profile()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.All});

                server.Endpoints.GetByInput(new ProfileModel {Name = "Jeremy"})
                      .ToString()
                      .ShouldEqual(
                          "<tr><td><label for=\"Name\">Name</label></td><td><span id=\"Name\">Jeremy</span></td></tr>");
            }
        }

        [Test]
        public void security_rule_does_use_the_request_model()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy
                {
                    Logic = r => r.Model.As<ShowModel>().Level < 10 ? AccessRight.All : AccessRight.None
                });

                server.Endpoints.GetByInput(new EditModel {Name = "Jeremy", Level = 15}).ReadAsText().ShouldBeEmpty();
                server.Endpoints.GetByInput(new EditModel {Name = "Jeremy", Level = 5}).ReadAsText().ShouldNotBeEmpty();
            }
        }

        [Test]
        public void show_downgrades_to_none_if_the_security_ixnays_it()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.None});

                server.Endpoints.GetByInput(new ShowModel {Name = "Jeremy"}).ReadAsText().ShouldBeEmpty();
            }
        }

        [Test]
        public void show_is_still_visible_if_access_is_read_only()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.ReadOnly});

                server.Endpoints.GetByInput(new ShowModel {Name = "Jeremy"})
                      .ToString()
                      .ShouldEqual("<dt><label for=\"Name\">Name</label></dt>\n<dd><span id=\"Name\">Jeremy</span></dd>");
            }
        }

        [Test]
        public void simplest_example_of_edit()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.All});

                server.Endpoints.GetByInput(new EditModel {Name = "Jeremy"})
                      .ToString()
                      .ShouldEqual(
                          "<dt><label for=\"Name\">Name</label></dt>\n<dd><input type=\"text\" value=\"Jeremy\" name=\"Name\" /></dd>");
            }
        }

        [Test]
        public void simplest_example_of_show()
        {
            var container = new Container();
            using (
                EmbeddedFubuMvcServer server = FubuApplication.For<TestRegistry>().StructureMap(container).RunEmbeddedWithAutoPort()
                )
            {
                container.Inject<IFieldAccessRule>(new ShowEditFakePolicy {Logic = r => AccessRight.All});

                server.Endpoints.GetByInput(new ShowModel {Name = "Jeremy"})
                      .ToString()
                      .ShouldEqual("<dt><label for=\"Name\">Name</label></dt>\n<dd><span id=\"Name\">Jeremy</span></dd>");
            }
        }
    }

    public class ShowEditEndpoints
    {
        private readonly FubuHtmlDocument<ShowModel> _document;
        private readonly ActiveProfile _profile;

        public ShowEditEndpoints(FubuHtmlDocument<ShowModel> document, ActiveProfile profile)
        {
            _document = document;
            _profile = profile;
        }

        public string get_show_Name(ShowModel model)
        {
            _document.Model = model;

            return _document.Show(x => x.Name).ToString();
        }

        public string get_profiled_Name(ProfileModel model)
        {
            _profile.Push("table");

            _document.Model = model;

            return _document.Show(x => x.Name).ToString();
        }

        public string get_edit_Name_Level(EditModel model)
        {
            _document.Model = model;

            return _document.Edit(x => x.Name).ToString();
        }
    }

    public class ProfileModel : ShowModel
    {
    }

    public class ShowEditFakePolicy : IFieldAccessRule
    {
        public Func<ElementRequest, AccessRight> Logic = r => AccessRight.All;

        public AccessRight RightsFor(ElementRequest request)
        {
            return Logic(request);
        }

        public bool Matches(Accessor accessor)
        {
            return accessor.OwnerType.CanBeCastTo(typeof (ShowModel));
        }

        public FieldAccessCategory Category { get; private set; }
    }


    public class ShowModel
    {
        public string Name { get; set; }

        public int Level { get; set; }
    }

    public class EditModel : ShowModel
    {
    }
}