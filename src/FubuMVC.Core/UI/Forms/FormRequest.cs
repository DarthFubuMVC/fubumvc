using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Forms
{
    public class FormRequest : TagRequest, IServiceLocatorAware 
    {
        private readonly ChainSearch _search;
        private readonly object _input;
        private readonly bool _closeTag;
        private IServiceLocator _services;

        public FormRequest(ChainSearch search, object input) : this(search, input, false) { }

        public FormRequest(ChainSearch search, object input, bool closeTag)
        {
            _search = search;
            _input = input;
            _closeTag = closeTag;
        }

        public ChainSearch Search
        {
            get { return _search; }
        }

        public object Input
        {
            get { return _input; }
        }

        public bool CloseTag
        {
            get { return _closeTag; }
        }

        public string Url { get; set; }
        public BehaviorChain Chain { get; set; }  

        public override object ToToken()
        {
            return new FormRequest(_search, _input);
        }

        public void Attach(IServiceLocator locator)
        {
            _services = locator;
            var resolver = locator.GetInstance<IChainResolver>();
            var urlResolver = locator.GetInstance<IChainUrlResolver>();
            Chain = resolver.Find(Search);

            if (Chain == null)
            {
                throw new FubuException(333, "No chain matches this search:  " + Search);
            }

            if (!(Chain is RoutedChain))
            {
                throw new FubuException(334, "Cannot post to this endpoint because there is no route");
            }

            Url = urlResolver.UrlFor(_input, Chain);
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        protected bool Equals(FormRequest other)
        {
            return Equals(_search, other._search) && Equals(_input, other._input);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FormRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_search != null ? _search.GetHashCode() : 0)*397) ^ (_input != null ? _input.GetHashCode() : 0);
            }
        }
    }
}