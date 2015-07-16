using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Environment
{
    public class CanWriteToFolder : IEnvironmentRequirement
    {
        private readonly string _folder;

        public CanWriteToFolder(string folder)
        {
            _folder = folder;
        }

        public string DescriptionMessage = "Can write to folder '{0}'";
        public string FailureMessage = "Could not write to folder '{0}'";
        public string SuccessMessage = "Was able to write to folder '{0}'";

        public string TracerFile = Guid.NewGuid() + ".txt";

        public string Describe()
        {
            return DescriptionMessage.ToFormat(_folder);
        }

        public void Check(IActivationLog log)
        {
            var file = _folder.AppendPath(TracerFile);
            try
            {
                var system = new FileSystem();
                system.WriteStringToFile(file, "just a test of whether or not a process can write to a folder");
                system.DeleteFile(file);

                log.Trace(SuccessMessage.ToFormat(_folder));
            }
            catch (Exception)
            {
                log.MarkFailure(FailureMessage.ToFormat(_folder));
            }
        }
    }

    public class CanWriteToFolder<TSettings> : CanWriteToFolder
    {
        public CanWriteToFolder(TSettings settings, Expression<Func<TSettings, object>> property)
            : base((string)ReflectionHelper.GetAccessor(property).GetValue(settings))
        {
            var propertyName = "{0}.{1}".ToFormat(typeof(TSettings).Name, ReflectionHelper.GetAccessor(property).Name);

            DescriptionMessage += " defined by " + propertyName;
            SuccessMessage += " defined by " + propertyName;
            FailureMessage += " defined by " + propertyName;
        }
    }

}