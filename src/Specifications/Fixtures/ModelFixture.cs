using System;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using FubuCore.Dates;
using FubuCore.Reflection;
using FubuMVC.Authentication;
using FubuMVC.PersistedMembership;
using FubuPersistence;
using FubuPersistence.Reset;
using StoryTeller;
using StoryTeller.Grammars.ObjectBuilding;
using StoryTeller.Grammars.Tables;

namespace Specifications.Fixtures
{
    public class ModelFixture : Fixture
    {
        private ICompleteReset _reset;
        private IUnitOfWork _unitOfWork;
        private IEntityRepository _repository;

        public ModelFixture()
        {
            Title = "The system state";
        }

        public override void SetUp()
        {
            var clock = (Clock)Context.Service<IClock>();
            clock.Live();

            _reset = Context.Service<ICompleteReset>();
            _reset.ResetState();

            _unitOfWork = Context.Service<IUnitOfWork>();
            _repository = _unitOfWork.Start();
        }


        public override void TearDown()
        {
            _unitOfWork.Commit();
            _reset.CommitChanges(); // doesn't do anything now, but might later when we go to IIS
        }

        private IGrammar setter(Expression<Func<AuthenticationSettings, object>> property)
        {
            var accessor = property.ToAccessor();
            var grammar = new SetPropertyGrammar(accessor.InnerProperty);
            grammar.CellModifications.DefaultValue(accessor.GetValue(new AuthenticationSettings()).ToString());

            return grammar;
        }

        public IGrammar SetAuthenticationSettings()
        {
            return Paragraph("The Authentication Settings are", x => {
                x += () => Context.State.CurrentObject = Context.Service<AuthenticationSettings>();
                x += setter(o => o.ExpireInMinutes);
                x += setter(o => o.SlidingExpiration);
                x += setter(o => o.MaximumNumberOfFailedAttempts);
                x += setter(o => o.CooloffPeriodInMinutes);
            });
        }

        [ExposeAsTable("The users are")]
        public void UsersAre(string UserName, string Password)
        {
            var user = new User
            {
                UserName = UserName,
                Password = Context.Service<IPasswordHash>().CreateHash(Password)
            };

            _repository.Update(user);
        }
    }
}