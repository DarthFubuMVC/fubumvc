using System.Linq;
using System.Web;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class SimpleSessionStateTester
    {
        private HttpContextBase theHttpContext;
        private StubSession theHttpSession;
        private SimpleSessionState theSessionState;

        [SetUp]
        public void SetUp()
        {
            theHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            theHttpSession = new StubSession();
            theHttpContext.Stub(x => x.Session).Return(theHttpSession);

            theSessionState = new SimpleSessionState(theHttpContext);
        }

        [Test]
        public void gets_and_sets_an_object_by_type_name()
        {
            var target = new SimpleSessionTarget {Name = "Test"};
            theSessionState.Set(target);
            theSessionState.Get<SimpleSessionTarget>().ShouldEqual(target);
        }

        [Test]
        public void gets_and_sets_an_object_by_explicit_key()
        {
            var target = new SimpleSessionTarget { Name = "Test" };
            var key = "the key";
            theSessionState.Set(key, target);
            theSessionState.Get<SimpleSessionTarget>(key).ShouldEqual(target);
        }

        public class StubSession : HttpSessionStateBase
        {
            public readonly Cache<string,object> Objects = new Cache<string, object>(); 

            public override object this[string name]
            {
                get { return Objects[name]; }
                set
                {
                    Objects[name] = value;
                }
            }
        }

        public class SimpleSessionTarget
        {
            public string Name { get; set; }

            protected bool Equals(SimpleSessionTarget other)
            {
                return string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SimpleSessionTarget) obj);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
    }
}