using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Assets
{
    public class AssetContentFileHandler : IRouteHandler
    {
        public static readonly string AssetsUrlFolder = "_content";

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            throw new NotImplementedException();
        }

        public static string DetermineAssetUrl(IAssetTagSubject subject)
        {
            var folder = subject.Folder;
            var name = subject.Name;

            return DetermineAssetUrl(folder, name);
        }

        public static string DetermineAssetUrl(AssetFolder? folder, string name)
        {
            var url = "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, folder, name);
            return url.ToAbsoluteUrl();
        }
    }

    public class AssetHttpHandler : IHttpHandler
    {
        public AssetHttpHandler(IContentWriter writer, IEnumerable<string> routeValues)
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public interface IImageWriter
    {
        void WriteImageToOutput(string name);
    }

    public class ImageWriter : IImageWriter
    {
        private readonly IOutputWriter _writer;
        private readonly IAssetPipeline _pipeline;

        public ImageWriter(IOutputWriter writer, IAssetPipeline pipeline)
        {
            _writer = writer;
            _pipeline = pipeline;
        }

        public void WriteImageToOutput(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class ContentPipeline : IContentPipeline
    {
        private readonly IServiceLocator _services;
        private readonly IFileSystem _fileSystem;

        public ContentPipeline(IServiceLocator services, IFileSystem fileSystem)
        {
            _services = services;
            _fileSystem = fileSystem;
        }

        public string ReadContentsFrom(string file)
        {
            throw new NotImplementedException();
        }

        public ITransformer GetTransformer<T>() where T : ITransformer
        {
            throw new NotImplementedException();
        }
    }

    public interface IContentPlanExecutor
    {
        void Execute(string name, Action<string, IEnumerable<AssetFile>> continuation);
    }

    public class ContentPlanExecutor : IContentPlanExecutor
    {
        private readonly IContentPlanCache _cache;
        private readonly IContentPipeline _pipeline;

        public ContentPlanExecutor(IContentPlanCache cache, IContentPipeline pipeline)
        {
            _cache = cache;
            _pipeline = pipeline;
        }

        public void Execute(string name, Action<string, IEnumerable<AssetFile>> continuation)
        {
            var plan = _cache.PlanFor(name);
            //var contents = plan.
        }
    }

    public class ContentWriter
    {
        
    }

    public interface IContentWriter
    {
        
    }

    public class ContentHttpPlanHandler : IHttpHandler
    {
        private readonly ContentPlan _plan;

        public ContentHttpPlanHandler(ContentPlan plan)
        {
            _plan = plan;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();

            //var response = context.Response;
            
            //response.AddFileDependencies(_plan.AllFileNames());

            //setCachePolicies(response.Cache);

            // need to write with MimeType, encoding.
        }

        private void setCachePolicies(HttpCachePolicy cache)
        {
            cache.VaryByParams["files"] = true;
            cache.SetLastModifiedFromFileDependencies();
            cache.SetETagFromFileDependencies();
            cache.SetCacheability(HttpCacheability.Public);
        }

        // These handlers CANNOT be reused
        public bool IsReusable
        {
            get { return false; }
        }
    }
}