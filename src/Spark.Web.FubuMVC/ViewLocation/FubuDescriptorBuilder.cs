using System.Collections.Generic;
using System.Linq;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class FubuDescriptorBuilder : IDescriptorBuilder
    {
        private readonly ISparkViewEngine _engine;
        private readonly UseMasterGrammar _grammar = new UseMasterGrammar();

        public FubuDescriptorBuilder()
        {
            Filters = new List<IDescriptorFilter> {new AreaDescriptorFilter()};
        }

        public FubuDescriptorBuilder(ISparkViewEngine engine)
            : this()
        {
            _engine = engine;
        }

        public IList<IDescriptorFilter> Filters { get; set; }

        public ParseAction<string> ParseUseMaster
        {
            get { return _grammar.ParseUseMaster; }
        }

        #region IDescriptorBuilder Members

        public virtual IDictionary<string, object> GetExtraParameters(ActionContext actionContext)
        {
            var extra = new Dictionary<string, object>();
            foreach (IDescriptorFilter filter in Filters)
                filter.ExtraParameters(actionContext, extra);
            return extra;
        }

        public SparkViewDescriptor BuildDescriptor(BuildDescriptorParams buildDescriptorParams, ICollection<string> searchedLocations)
        {
            var descriptor = new SparkViewDescriptor
                                 {
                                     TargetNamespace = buildDescriptorParams.ActionNamespace
                                 };

            if (!LocatePotentialTemplate(
                     PotentialViewLocations(buildDescriptorParams.AcionName,
                                            buildDescriptorParams.ViewName,
                                            buildDescriptorParams.Extra),
                     descriptor.Templates,
                     searchedLocations))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(buildDescriptorParams.MasterName))
            {
                if (!LocatePotentialTemplate(
                         PotentialMasterLocations(buildDescriptorParams.MasterName,
                                                  buildDescriptorParams.Extra),
                         descriptor.Templates,
                         searchedLocations))
                {
                    return null;
                }
            }
            else if (buildDescriptorParams.FindDefaultMaster && string.IsNullOrEmpty(TrailingUseMasterName(descriptor)))
            {
                LocatePotentialTemplate(
                    PotentialDefaultMasterLocations(buildDescriptorParams.AcionName,
                                                    buildDescriptorParams.Extra),
                    descriptor.Templates,
                    null);
            }

            string trailingUseMaster = TrailingUseMasterName(descriptor);
            while (buildDescriptorParams.FindDefaultMaster && !string.IsNullOrEmpty(trailingUseMaster))
            {
                if (!LocatePotentialTemplate(
                         PotentialMasterLocations(trailingUseMaster,
                                                  buildDescriptorParams.Extra),
                         descriptor.Templates,
                         searchedLocations))
                {
                    return null;
                }
                trailingUseMaster = TrailingUseMasterName(descriptor);
            }

            return descriptor;
        }

        #endregion

        private string TrailingUseMasterName(SparkViewDescriptor descriptor)
        {
            string lastTemplate = descriptor.Templates.Last();
            SourceContext sourceContext = AbstractSyntaxProvider.CreateSourceContext(lastTemplate, _engine.ViewFolder);
            if (sourceContext == null)
                return null;
            ParseResult<string> result = ParseUseMaster(new Position(sourceContext));
            return result == null ? null : result.Value;
        }

        private bool LocatePotentialTemplate(IEnumerable<string> potentialTemplates, ICollection<string> descriptorTemplates, ICollection<string> searchedLocations)
        {
            string template = potentialTemplates.FirstOrDefault(t => _engine.ViewFolder.HasView(t));
            if (template != null)
            {
                descriptorTemplates.Add(template);
                return true;
            }
            if (searchedLocations != null)
            {
                foreach (string potentialTemplate in potentialTemplates)
                    searchedLocations.Add(potentialTemplate);
            }
            return false;
        }

        private IEnumerable<string> ApplyFilters(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            // apply all of the filters PotentialLocations in order
            return Filters.Aggregate(
                locations,
                (aggregate, filter) => filter.PotentialLocations(aggregate, extra));
        }

        protected virtual IEnumerable<string> PotentialViewLocations(string actionName, string viewName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        actionName + "\\" + viewName + ".spark",
                                        "Shared\\" + viewName + ".spark"
                                    }, extra);
        }

        protected virtual IEnumerable<string> PotentialMasterLocations(string masterName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        "Layouts\\" + masterName + ".spark",
                                        "Shared\\" + masterName + ".spark"
                                    }, extra);
        }

        protected virtual IEnumerable<string> PotentialDefaultMasterLocations(string actionName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        "Layouts\\" + actionName + ".spark",
                                        "Shared\\" + actionName + ".spark",
                                        "Layouts\\Application.spark",
                                        "Shared\\Application.spark"
                                    }, extra);
        }

        #region Nested type: UseMasterGrammar

        private class UseMasterGrammar : CharGrammar
        {
            public UseMasterGrammar()
            {
                ParseAction<IList<char>> whiteSpace0 = Rep(Ch(char.IsWhiteSpace));
                ParseAction<IList<char>> whiteSpace1 = Rep1(Ch(char.IsWhiteSpace));
                ParseAction<string> startOfElement = Ch("<use");
                ParseAction<Chain<Chain<Chain<string, IList<char>>, char>, IList<char>>> startOfAttribute = Ch("master").And(whiteSpace0).And(Ch('=')).And(whiteSpace0);
                ParseAction<Chain<Chain<char, IList<char>>, char>> attrValue = Ch('\'').And(Rep(ChNot('\''))).And(Ch('\''))
                    .Or(Ch('\"').And(Rep(ChNot('\"'))).And(Ch('\"')));

                ParseAction<string> endOfElement = Ch("/>");

                ParseAction<string> useMaster = startOfElement
                    .And(whiteSpace1)
                    .And(startOfAttribute)
                    .And(attrValue)
                    .And(whiteSpace0)
                    .And(endOfElement)
                    .Build(hit => new string(hit.Left.Left.Down.Left.Down.ToArray()));

                ParseUseMaster =
                    pos =>
                        {
                            for (Position scan = pos; scan.PotentialLength() != 0; scan = scan.Advance(1))
                            {
                                ParseResult<string> result = useMaster(scan);
                                if (result != null)
                                    return result;
                            }
                            return null;
                        };
            }

            public ParseAction<string> ParseUseMaster { get; set; }
        }

        #endregion
    }
}