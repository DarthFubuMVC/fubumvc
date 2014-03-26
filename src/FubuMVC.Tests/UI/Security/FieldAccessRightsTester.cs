using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Security
{
    [TestFixture]
    public class FieldAccessRightsTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _securities = new IFieldAccessRule[0];
            _logics = new IFieldAccessRule[0];
            request = ElementRequest.For(new FieldAccessModel(), x => x.Name);
        }

        #endregion

        private ElementRequest request;
        private IEnumerable<IFieldAccessRule> _securities;
        private IEnumerable<IFieldAccessRule> _logics;

        private void securityRulesAre(params AccessRight[] rights)
        {
            _securities = rights.Select(r => {
                var rule = MockRepository.GenerateMock<IFieldAccessRule>();
                rule.Stub(x => x.RightsFor(request)).Return(r);

                return rule;
            });
        }

        private void logicRulesAre(params AccessRight[] rights)
        {
            _logics = rights.Select(r => {
                var rule = MockRepository.GenerateMock<IFieldAccessRule>();
                rule.Stub(x => x.RightsFor(request)).Return(r);

                return rule;
            });
        }

        private AccessRight theRights
        {
            get { return new FieldAccessRightsExecutor().RightsFor(request, _securities, _logics); }
        }

        [Test]
        public void if_no_rules_of_any_kind_then_return_all()
        {
            theRights.ShouldEqual(AccessRight.All);
        }

        [Test]
        public void with_both_security_and_logic_rules_1()
        {
            securityRulesAre(AccessRight.All, AccessRight.All);
            logicRulesAre(AccessRight.All, AccessRight.All);

            theRights.ShouldEqual(AccessRight.All);
        }

        [Test]
        public void with_both_security_and_logic_rules_2()
        {
            securityRulesAre(AccessRight.All, AccessRight.All);
            logicRulesAre(AccessRight.All, AccessRight.ReadOnly);

            theRights.ShouldEqual(AccessRight.ReadOnly);
        }


        [Test]
        public void with_both_security_and_logic_rules_3()
        {
            securityRulesAre(AccessRight.ReadOnly);
            logicRulesAre(AccessRight.All, AccessRight.All);

            theRights.ShouldEqual(AccessRight.ReadOnly);
        }


        [Test]
        public void with_both_security_and_logic_rules_4()
        {
            securityRulesAre(AccessRight.ReadOnly);
            logicRulesAre(AccessRight.All, AccessRight.None);

            theRights.ShouldEqual(AccessRight.None);
        }

        [Test]
        public void with_only_logic_rights_1()
        {
            logicRulesAre(AccessRight.All);
            theRights.ShouldEqual(AccessRight.All);
        }

        [Test]
        public void with_only_logic_rights_2()
        {
            logicRulesAre(AccessRight.All, AccessRight.ReadOnly);
            theRights.ShouldEqual(AccessRight.ReadOnly);
        }


        [Test]
        public void with_only_logic_rights_3()
        {
            logicRulesAre(AccessRight.All, AccessRight.ReadOnly, AccessRight.None);
            theRights.ShouldEqual(AccessRight.None);
        }

        [Test]
        public void with_only_security_rights_1()
        {
            securityRulesAre(AccessRight.All);
            theRights.ShouldEqual(AccessRight.All);

            securityRulesAre(AccessRight.All, AccessRight.All, AccessRight.All);
            theRights.ShouldEqual(AccessRight.All);
        }


        [Test]
        public void with_only_security_rights_2()
        {
            securityRulesAre(AccessRight.All, AccessRight.ReadOnly, AccessRight.ReadOnly);
            theRights.ShouldEqual(AccessRight.All);

            securityRulesAre(AccessRight.All, AccessRight.All, AccessRight.None);
            theRights.ShouldEqual(AccessRight.All);
        }


        [Test]
        public void with_only_security_rights_3()
        {
            securityRulesAre(AccessRight.ReadOnly, AccessRight.None);
            theRights.ShouldEqual(AccessRight.ReadOnly);
        }

        [Test]
        public void with_only_security_rights_4()
        {
            securityRulesAre(AccessRight.None);
            theRights.ShouldEqual(AccessRight.None);
        }
    }


    public class FieldAccessModel
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
    }
}