using System;
using System.Collections.Generic;
using FubuMVC.Core.UI.Scripts;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Engine;
using FluentNHibernate.Utils;
using System.Linq;

namespace IntegrationTesting.Fixtures.Scripts
{
    public class ScriptGraphFixture : Fixture
    {
        private ScriptGraph _graph;
        private IEnumerable<IScript> _scripts;

        public ScriptGraphFixture()
        {
            _graph = new ScriptGraph(new StubScriptFinder());
        }

        public IGrammar SetupScriptGraph()
        {
            return Embed<ScriptGraphSetupFixture>("If the script graph is configured as")
                .Before((step, context) =>
                {
                    _graph = new ScriptGraph(new StubScriptFinder());
                    context.Store(_graph);
                });
        }




        [FormatAs("Scenario:  {comment}")]
        public void Scenario(string comment)
        {
            // do nothing
        }

        [Hidden]
        [FormatAs("Fetch the scripts for {names}")]
        public void FetchList(string[] names)
        {
            _scripts = _graph.GetScripts(names);
        }

        [Hidden]
        [FormatAs("All the scripts in order should be {expected}")]
        public bool CheckList(string[] expected)
        {
            var correct = true;

            var names = _scripts.Select(x => x.Name).ToArray();
            if (names.Length != expected.Length)
            {
                correct = false;
            }
            else
            {
                for (int i = 0; i < names.Length; i++)
                {
                    string actual = names[i];
                    correct = correct && actual == expected[i];
                }
            }

            if (!correct)
            {
                StoryTellerAssert.Fail("Actual:  " + names.Join(", "));
            }

            return correct;
        }


        public IGrammar Query()
        {
            return InlineParagraph("Query the ScriptGraph", x =>
            {
                x += this["Scenario"];
                x += this["FetchList"];
                x += this["CheckList"];
            });
        }
    }

    public class StubScriptFinder : IScriptFinder
    {
        public IScript Find(string name)
        {
            return new Script(){
                Name = name
            };
        }
    }

    public class ScriptGraphSetupFixture : Fixture
    {
        private ScriptGraph _graph;

        public override void SetUp(ITestContext context)
        {
            _graph = context.Retrieve<ScriptGraph>();
        }

        [FormatAs("{name} extends {baseName}")]
        public void Extends(string name, string baseName)
        {
            _graph.Extension(name, baseName);
        }

        [FormatAs("{name} requires {reqs}")]
        public void Requires(string name, string[] reqs)
        {
            CollectionExtensions.Each(reqs, x => _graph.Dependency(name, x));
        }

        [FormatAs("{setName} includes {scripts}")]
        public void Includes(string setName, string[] scripts)
        {
            CollectionExtensions.Each(scripts, s => _graph.AddToSet(setName, s));
        }

        [FormatAs("alias {name} as {alias}")]
        public void Alias(string name, string alias)
        {
            _graph.Alias(name, alias);
        }
    }
}