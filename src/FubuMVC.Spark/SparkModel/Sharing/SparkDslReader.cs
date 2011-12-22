using System.Collections.Generic;
using FubuCore;
using FubuCore.CommandLine;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public class SparkDslReader
    {
        private readonly ISharingRegistration _registration;
        public SparkDslReader(ISharingRegistration registration)
        {
            _registration = registration;
        }

        public void ReadLine(string text, string provenance)
        {
            var input = text.Trim();
            if(input.IsEmpty() || input.StartsWith("#"))
            {
                return;
            }

            var tokens = StringTokenizer.Tokenize(text.Replace(',', ' '));
            var queue = new Queue<string>(tokens);

            if(queue.Count < 3)
            {
                throw new InvalidSyntaxException("Not enough tokens");
            }

            var key = queue.Dequeue();
            var verb = queue.Dequeue();

            if (key == "import")
            {
                readImport(queue, verb, provenance);
                return;
            }

            if(key == "export")
            {
                readExport(queue, verb, provenance);
                return;
            }
        }

        private void readImport(Queue<string> queue, string verb, string provenance)
        {
            switch (verb)
            {
                case "from":
                    queue.Each(export => _registration.Dependency(provenance, export));
                    break;

                default:
                    throw new InvalidSyntaxException(invalidVerb(verb));
            }
        }

        private void readExport(Queue<string> queue, string verb, string provenance)
        {
            switch (verb)
            {
                case "to":

                    if(queue.Contains("all"))
                    {
                        _registration.Global(provenance);
                        break;
                    }

                    queue.Each(importer => _registration.Dependency(importer, provenance));
                    break;

                default:
                    throw new InvalidSyntaxException(invalidVerb(verb));
            }
        }

        private static string invalidVerb(string verb)
        {
            return "Invalid verb '{0}'".ToFormat(verb);
        }
    }
}