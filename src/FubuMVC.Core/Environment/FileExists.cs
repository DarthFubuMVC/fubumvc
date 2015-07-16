using System;
using System.IO;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Environment
{
    public class FileExists : IEnvironmentRequirement
    {
        private readonly string _file;

        public FileExists(string file)
        {
            _file = file;
        }

        public string DescriptionMessage = "File '{0}' must exist";
        public string FailureMessage = "File '{0}' does not exist!";
        public string SuccessMessage = "File '{0}' exists";


        public string Describe()
        {
            return "File '{0}' must exist".ToFormat(_file);
        }

        public void Check(IActivationLog log)
        {
            if (File.Exists(_file))
            {
                log.Trace(SuccessMessage, _file);
            }
            else
            {
                log.MarkFailure(FailureMessage, _file);
            }
        }
    }

    public class FileExists<TSettings> : FileExists
    {
        public FileExists(Expression<Func<TSettings, object>> property, TSettings settings)
            : base((string) ReflectionHelper.GetAccessor(property).GetValue(settings))
        {
            var accessor = ReflectionHelper.GetAccessor(property);

            string propertyName = "{0}.{1}".ToFormat(typeof (TSettings).Name, accessor.Name);


            DescriptionMessage = "File '{0}' defined by {1} must exist".Replace("{1}", propertyName);
            SuccessMessage = "File '{0}' defined by {1} exists'".Replace("{1}", propertyName);
            FailureMessage = "File '{0}' defined by {1} does not exist!".Replace("{1}", propertyName);
        }
    }
}