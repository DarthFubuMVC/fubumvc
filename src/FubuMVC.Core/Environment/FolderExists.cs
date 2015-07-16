using System;
using System.IO;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Environment
{
    public class FolderExists : IEnvironmentRequirement
    {
        private readonly string _folder;

        public FolderExists(string folder)
        {
            _folder = folder;
        }

        public string DescriptionMessage = "Folder '{0}' should exist";
        public string FailureMessage = "Folder '{0}' does not exist!";
        public string SuccessMessage = "Folder '{0}' exists";


        public string Describe()
        {
            return DescriptionMessage;
        }

        public void Check(IActivationLog log)
        {
            if (Directory.Exists(_folder))
            {
                log.Trace(SuccessMessage, _folder);
            }
            else
            {
                log.MarkFailure(FailureMessage, _folder);
            }
        }
    }

    public class FolderExists<TSettings> : FolderExists
    {
        public FolderExists(Expression<Func<TSettings, object>> property, TSettings settings)
            : base((string)ReflectionHelper.GetAccessor(property).GetValue(settings))
        {
            var accessor = ReflectionHelper.GetAccessor(property);

            string propertyName = "{0}.{1}".ToFormat(typeof(TSettings).Name, accessor.Name);

            DescriptionMessage = "Folder '{0}' defined by {1} must exist".Replace("{1}", propertyName);
            SuccessMessage = "Folder '{0}' defined by {1} exists".Replace("{1}", propertyName);
            FailureMessage = "Folder '{0}' defined by {1} does not exist!".Replace("{1}", propertyName);
        }
    }
}