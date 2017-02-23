using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class ModelBindingErrorsTester : InteractionContext<ModelBindingErrors>
    {
        [Fact]
        public void should_place_problems_into_the_notification()
        {
            MockFor<IFubuRequest>().Stub(x => x.ProblemsFor<ValidationTarget>()).Return(problems());

            var notification = new Notification();

            ClassUnderTest.AddAnyErrors<ValidationTarget>(notification);

            notification.MessagesFor<ValidationTarget>(x => x.Number).Single().StringToken.ShouldBe(ValidationKeys.InvalidFormat);
            notification.MessagesFor<ValidationTarget>(x => x.Time).Single().StringToken.ShouldBe(ValidationKeys.InvalidFormat);
            
            notification.MessagesFor<ValidationTarget>(x => x.Other)
                .Any().ShouldBeFalse();
        
        }

        private IEnumerable<ConvertProblem> problems()
        {
            yield return new ConvertProblem{Property = ReflectionHelper.GetProperty<ValidationTarget>(x => x.Number)};
            yield return new ConvertProblem{Property = ReflectionHelper.GetProperty<ValidationTarget>(x => x.Time)};
        }

        public class ValidationTarget
        {
            public int Number { get; set; }
            public DateTime Time { get; set; }
            public int Other { get; set; }
        }
    }
}