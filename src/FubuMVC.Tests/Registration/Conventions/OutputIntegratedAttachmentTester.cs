using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Tests.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;
using OutputNode = FubuMVC.Core.Resources.Conneg.OutputNode;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class OutputIntegratedAttachmentTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<JsonOutputAttachmentTesterController>();
            });
        }

        #endregion

        private BehaviorGraph graph;


        private BehaviorChain chainFor(Expression<Action<JsonOutputAttachmentTesterController>> expression)
        {
            return graph.BehaviorFor(expression);
        }

        private BehaviorChain chainFor(Expression<Func<JsonOutputAttachmentTesterController, object>> expression)
        {
            return graph.BehaviorFor(expression);
        }

        [Test]
        public void automatically_output_methds_that_are_decorated_with_JsonEndpoint_to_json()
        {
            var behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.Decorated()).Calls.First().Next;
            behavior.ShouldBeOfType<OutputNode>().ResourceType.ShouldEqual(typeof (ViewModel1));
        }

        [Test]
        public void automatically_output_methods_that_return_string_as_text_if_there_is_not_output()
        {
            var behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.Stringify()).Calls.First().Next;

            behavior.ShouldBeOfType<OutputNode>().Writes(MimeType.Text).ShouldBeTrue();
        }

        [Test]
        public void methods_that_do_not_take_in_a_json_message_should_not_have_a_json_deserialization_behavior()
        {
            chainFor(x => x.NotJson1(null)).Top.Any(x => x is InputNode).ShouldBeFalse();
            chainFor(x => x.NotJson2(null)).Top.Any(x => x is InputNode).ShouldBeFalse();
            chainFor(x => x.NotJson3(null)).Top.Any(x => x is InputNode).ShouldBeFalse();
        }

        [Test]
        public void methods_that_return_a_json_message_should_output_json()
        {
            var chain = chainFor(x => x.OutputJson1());
            chain.Output.Writes(MimeType.Json);

            chainFor(x => x.OutputJson2()).Top.Any(x => x.GetType() == typeof (OutputNode)).ShouldBeTrue();
            chainFor(x => x.OutputJson3()).Top.Any(x => x.GetType() == typeof (OutputNode)).ShouldBeTrue();
        }

    }

    public class CrudReport
    {
    }

    public class SpecialCrudReport : CrudReport
    {
    }

    public class ContinuationClass
    {
    }

    public class JsonOutputAttachmentTesterController
    {
        public CrudReport Report()
        {
            return null;
        }

        public SpecialCrudReport Report2()
        {
            return null;
        }

        public string Stringify()
        {
            return null;
        }

        public string StringifyHtml()
        {
            return null;
        }

        public ContinuationClass WhatNext()
        {
            return null;
        }

        public Output Output()
        {
            return null;
        }

        public HtmlTagOutput GetFake()
        {
            return null;
        }

        public ViewModel1 Decorated()
        {
            return null;
        }

        public void JsonInput1(Json1 json)
        {
        }

        public void JsonInput2(Json2 json)
        {
        }

        public void JsonInput3(Json3 json)
        {
        }

        public void NotJson1(NotJson1 message)
        {
        }

        public void NotJson2(NotJson2 message)
        {
        }

        public void NotJson3(NotJson3 message)
        {
        }

        public Json1 OutputJson1()
        {
            return new Json1();
        }

        public Json2 OutputJson2()
        {
            return new Json2();
        }

        public Json3 OutputJson3()
        {
            return new Json3();
        }

        public NotJson1 NotOutputJson1()
        {
            return new NotJson1();
        }

        public NotJson2 NotOutputJson2()
        {
            return new NotJson2();
        }

        public NotJson3 NotOutputJson3()
        {
            return new NotJson3();
        }
    }

    public class ViewModel1
    {
        
    }

    public class Json1
    {
    }

    public class Json2
    {
    }

    public class Json3
    {
    }

    public class NotJson1
    {
    }

    public class NotJson2
    {
    }

    public class NotJson3
    {
    }

    public class HtmlTagOutput
    {
    }
}