using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;

namespace FubuMVC.Core.Assets
{
    public static class AssetGraphScenario
    {
        public static readonly string ScenarioHeader = "If the asset configuration is";
        public static readonly string Requesting = "requesting";
        public static readonly string ShouldReturn = "should return";


        public static AssetGraph For(string name, string text)
        {
            var reader = new Reader();

            text.ReadLines(reader.ReadLine);
            reader.ReadLine(string.Empty);  // Depends on a blank line to tell it to "go" in some cases

            return reader.Graph;
        }

        #region Nested type: Display

        public class Display
        {
            private readonly IList<string> _actual;
            private readonly IList<string> _expected;

            public Display(IEnumerable<string> expected, IEnumerable<string> actual)
            {
                _expected = expected.ToList();
                _actual = actual.ToList();

                while (_expected.Count > _actual.Count)
                {
                    _actual.Add(string.Empty);
                }

                while (_actual.Count > _expected.Count)
                {
                    _expected.Add(string.Empty);
                }
            }

            public void Write(TextWriter writer)
            {
                var length1 = _expected.Max(x => x.Length) + 4;
                var totalLength = _actual.Max(x => x.Length) + length1 + 4;

                writer.WriteLine("".PadRight(totalLength, '='));
                writer.WriteLine("     Expected".PadRight(length1) + "     Actual");
                writer.WriteLine("".PadRight(totalLength, '='));

                for (var i = 0; i < _expected.Count; i++)
                {
                    writer.WriteLine();
                    writer.Write((i + 1).ToString().PadLeft(3) + ". ");
                    writer.Write(_expected[i].PadRight(length1));
                    writer.Write(_actual[i]);
                }
            }

            public override string ToString()
            {
                var writer = new StringWriter();
                Write(writer);

                return writer.ToString();
            }
        }

        #endregion

        #region Nested type: Reader

        public class Reader
        {
            private readonly AssetDslReader _dslReader;
            private readonly AssetGraph _graph;
            private Action<string> _action;
            private Request _request;
            private readonly Lazy<AssetDependencyFinderCache> _cache;

            public Reader()
            {
                _graph = new AssetGraph();
                _cache = new Lazy<AssetDependencyFinderCache>(() =>
                {
                    _graph.CompileDependencies(new PackageLog());
                    return new AssetDependencyFinderCache(_graph);
                });

                _dslReader = new AssetDslReader(_graph);

                _action = text =>
                {
                    if (text.StartsWith("if", StringComparison.OrdinalIgnoreCase))
                    {
                        _action = _dslReader.ReadLine;
                    }
                };

                
            }

            public AssetGraph Graph
            {
                get { return _graph; }
            }

            public void ReadLine(string text)
            {
                text = text.TrimStart();

                if (text.StartsWith("#")) return;

                if (text.StartsWith(Requesting, StringComparison.OrdinalIgnoreCase))
                {
                    _request = new Request();

                    var leftoverText = text.Substring(Requesting.Length);
                    _request.Requested(leftoverText);

                    _action = _request.Requested;

                    return;
                }

                if (text.StartsWith(ShouldReturn, StringComparison.OrdinalIgnoreCase))
                {
                    var leftoverText = text.Substring(ShouldReturn.Length);
                    _request.Expected(leftoverText);

                    _action = x =>
                    {
                        if (x.Trim().IsEmpty())
                        {
                            _request.Assert(_cache.Value);
                        }
                        else
                        {
                            _request.Expected(x);
                        }

                        
                    };

                    return;
                }

                _action(text);
            }
        }

        #endregion

        #region Nested type: Request

        public class Request
        {
            private readonly IList<string> _expected = new List<string>();
            private readonly IList<string> _requests = new List<string>();
            private IList<string> _actual = new List<string>();

            public void Requested(string text)
            {
                _requests.AddRange(text.ToDelimitedArray());
            }

            public void Expected(string text)
            {
                _expected.AddRange(text.ToDelimitedArray());
            }

            public void Assert(AssetDependencyFinderCache cache)
            {
                _actual = cache.CompileDependenciesAndOrder(_requests).ToList();

                if (_expected.SequenceEqual(_actual)) return;

                var display = new Display(_expected, _actual);

                throw new ApplicationException("Request for " + _requests.Join(", ") + " was not correct\n\n" + display);
            }
        }

        #endregion
    }
}