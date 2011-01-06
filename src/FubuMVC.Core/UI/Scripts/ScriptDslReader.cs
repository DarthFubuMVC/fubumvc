using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{
    /* --------
     * Patterns
     * --------
     * jquery is jquery.1.4.2.js
     * foo requires bar1, bar2
     * foo extends bar
     * foo includes blah,blah,blah
     * 
     * 
     * 
     * 
     * 
     * 
     */
    public class ScriptDslReader
    {
        private readonly IScriptRegistration _registration;

        public ScriptDslReader(IScriptRegistration registration)
        {
            _registration = registration;
        }

        public void ReadLine(string text)
        {
            var tokens = new Queue<string>(StringTokenizer.Tokenize(text));

            // TODO -- more specific exception
            if (tokens.Count() < 3)
            {
                throw new ApplicationException("Invalid syntax");
            }

            var key = tokens.Peek();
            var verb = tokens.Peek();

            throw new NotImplementedException();
        }
    }
}