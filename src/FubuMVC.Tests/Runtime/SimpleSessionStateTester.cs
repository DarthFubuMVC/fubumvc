﻿using System.Linq;
using System.Web;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    
    public class SimpleSessionStateTester
    {
        private HttpContextBase theHttpContext;
        private StubSession theHttpSession;
        private SimpleSessionState theSessionState;

        public SimpleSessionStateTester()
        {
            theHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            theHttpSession = new StubSession();
            theHttpContext.Stub(x => x.Session).Return(theHttpSession);

            theSessionState = new SimpleSessionState(theHttpContext);
        }

        [Fact]
        public void gets_and_sets_an_object_by_type_name()
        {
            var target = new SimpleSessionTarget {Name = "Test"};
            theSessionState.Set(target);
            theSessionState.Get<SimpleSessionTarget>().ShouldBe(target);
        }

        [Fact]
        public void gets_and_sets_an_object_by_explicit_key()
        {
            var target = new SimpleSessionTarget { Name = "Test" };
            var key = "the key";
            theSessionState.Set(key, target);
            theSessionState.Get<SimpleSessionTarget>(key).ShouldBe(target);
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