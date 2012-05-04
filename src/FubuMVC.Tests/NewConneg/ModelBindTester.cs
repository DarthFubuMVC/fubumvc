using System;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class ModelBindTester
    {
        [Test]
        public void argument_null_if_input_type_is_null()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() =>
            {
                new ModelBind(null);
            });
        }

        [Test]
        public void input_type()
        {
            new ModelBind(typeof (Address))
                .InputType.ShouldEqual(typeof (Address));
        }

        [Test]
        public void build_object_def()
        {
            new ModelBind(typeof (Address))
                .As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)
                .Type
                .ShouldEqual(typeof (ModelBindingReader<Address>));
        }

        [Test]
        public void mime_types()
        {
            new ModelBind(typeof(Address)).Mimetypes.ShouldHaveTheSameElementsAs(MimeType.HttpFormMimetype, MimeType.MultipartMimetype);
        }
    }
}