using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.View.Model
{


    public interface IParsingRegistrations<T> where T : ITemplateFile
    {
        Parsing ParsingFor(T template);
    }
}