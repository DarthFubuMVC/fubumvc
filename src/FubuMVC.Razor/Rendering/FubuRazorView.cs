using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;
using HtmlTags;

namespace FubuMVC.Razor.Rendering
{
    public abstract class FubuRazorView : IFubuRazorView
    {
        private readonly Cache<Type, object> _services = new Cache<Type, object>();
        private readonly IDictionary<string, Action> _sections = new Dictionary<string, Action>();
        private StringBuilder _output = new StringBuilder();
        private IFubuRazorView _child;
        private Action _renderAction;
        private Func<string> _result;
        private IServiceLocator _serviceLocator;

        protected FubuRazorView()
        {
            _services.OnMissing = type => ServiceLocator.GetInstance(type);
            _renderAction = RenderNoLayout;
            _result = () => _output.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceLocator ServiceLocator
        {
            get { return _child == null ? _serviceLocator : _child.ServiceLocator; }
            set { _serviceLocator = value; }
        }

        public T Get<T>()
        {
            return (T)_services[typeof(T)];
        }

        public HttpContextBase Context
        {
            get
            {
                return this.Get<HttpContextBase>();
            }
        }


        public T GetNew<T>()
        {
            return (T)ServiceLocator.GetInstance(typeof(T));
        }

        public IUrlRegistry Urls
        {
            get { return Get<IUrlRegistry>(); }
        }

        void IFubuRazorView.UseLayout(IFubuRazorView layout)
        {
            LayoutView = layout;
            _renderAction = RenderWithLayout;
            _result = () => layout.Result.ToString();
        }

        void IFubuRazorView.NoLayout()
        {
            _renderAction = RenderNoLayout;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void Execute();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void IFubuRazorView.ExecuteLayout(IFubuRazorView child)
        {
            _child = child;
            _renderAction();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Layout { get; set; }

        RazorTemplate IFubuRazorView.OriginTemplate { get; set; }

        string IFubuPage.ElementPrefix { get; set; }

        private void RenderNoLayout()
        {
            Execute();
        }

        private void RenderWithLayout()
        {
            Execute();
            LayoutView.ExecuteLayout(this);
        }

        void IRenderableView.Render(IFubuRequestContext context)
        {
            _renderAction();
            Get<IOutputWriter>().WriteHtml(this.As<IFubuRazorView>().Result);
        }

        public HtmlString RenderBody()
        {
            if(_child == null)
                throw new InvalidUsageException("RenderBody must be called from a layout template");

            return new HtmlString(_child.CurrentOutput);
        }

        public HtmlString RenderSection(string name, bool required = true)
        {
            if (!_sections.ContainsKey(name))
            {
                if (_child == null)
                {
                    if(required) 
                        throw new InvalidUsageException("No section has been defined for required '{0}'".ToFormat(name));
                    return new HtmlString(string.Empty);
                }
                return _child.RenderSection(name, required);
            }
            Action section = _sections[name];

            var current = _output;
            _output = new StringBuilder();
            var output = _output;
            section();
            _output = current;
            return new HtmlString(output.ToString());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DefineSection(string name, Action section)
        {
            _sections[name] = section;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IFubuPage Page { get { return this; } }

        public HtmlTag Tag(string tagName)
        {
            return new HtmlTag(tagName);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Write(object value)
        {
            if (value == null)
                return;
            if (value is IHtmlString)
            {
                WriteLiteral(value.ToString());
            }
            else
            {
                WriteLiteral(WebUtility.HtmlEncode(value.ToString()));
            }
        }

        public virtual void Write(TemplateHelper helper)
        {
            if (helper == null)
                return;

            helper.WriteTo(_output);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (literal == null) 
                return;
            writer.Write(literal);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteTo(TextWriter writer, object value)
        {
            if (value == null)
                return;
            if (value is IHtmlString)
            {
                writer.Write(value.ToString());
            }
            else
            {
                writer.Write(new HtmlString(value.ToString()));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void WriteAttribute(string name,  PositionTagged<string> start, PositionTagged<string> end, params AttributeValue[] args)
        {
            var totalArgWritten = 0;
            foreach (var attributeValue in args)
            {
                if (attributeValue.Value.Value == null)
                    continue;

                totalArgWritten++;
                if (totalArgWritten == 1)
                    WriteLiteral(start.Value);
                
                Write(attributeValue.Prefix.Value);
                Write(attributeValue.Value.Value);
            }

            if (totalArgWritten > 0)
            {
                WriteLiteral(end.Value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteAttributeTo(TextWriter writer, string name, PositionTagged<string> start, PositionTagged<string> end, params AttributeValue[] args)
        {
            var totalArgWritten = 0;
            foreach (var attributeValue in args)
            {
                if (attributeValue.Value.Value == null)
                    continue;

                totalArgWritten++;
                if (totalArgWritten == 1)
                    WriteLiteralTo(writer, start.Value);
                WriteTo(writer, attributeValue.Prefix.Value);
                WriteTo(writer,attributeValue.Value.Value);
            }

            if (totalArgWritten > 0)
            {
                WriteLiteralTo(writer,end.Value);
            }
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteLiteral(string value)
        {
            _output.Append(value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IFubuRazorView LayoutView { get; set; }

        public HtmlString Raw(string rawContent)
        {
            return new HtmlString(rawContent);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        HtmlString IFubuRazorView.Result
        {
            get { return new HtmlString(_result()); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string IFubuRazorView.CurrentOutput { get { return _output.ToString(); } }
    }

    public abstract class FubuRazorView<TViewModel> : FubuRazorView, IFubuPage<TViewModel> where TViewModel : class
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetModel(IFubuRequest request)
        {
            var model = request.Has<TViewModel>() ? request.Get<TViewModel>() : request.Find<TViewModel>().FirstOrDefault();
            SetModel(model);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetModel(object model)
        {
            SetModel(model as TViewModel);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetModel(TViewModel model)
        {
            Model = model;
        }

        public TViewModel Model { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetModel()
        {
            return Model;
        }
    }

    public interface IFubuRazorView : IRenderableView, IFubuPage
    {
        HtmlString Result { get; }
        IFubuRazorView LayoutView { get; }
        void UseLayout(IFubuRazorView layout);
        void NoLayout();
        RazorTemplate OriginTemplate { get; set; }
        string CurrentOutput { get; }
        void Execute();
        void ExecuteLayout(IFubuRazorView child);
        HtmlString RenderBody();
        HtmlString RenderSection(string name, bool required = true);
        void DefineSection(string name, Action sectionAction);
    }
}