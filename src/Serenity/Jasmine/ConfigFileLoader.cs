using System;
using System.IO;
using FubuCore;
using System.Linq;
using FubuCore.CommandLine;

namespace Serenity.Jasmine
{
    

    public class ConfigFileLoader
    {
        private readonly string _file;
        private readonly ISerenityJasmineApplication _application;

        public ConfigFileLoader(string file, ISerenityJasmineApplication application)
        {
            _file = file.ToFullPath();
            _application = application;
        }

        public void ReadFile()
        {
            Console.WriteLine("Reading directives from " + _file);


            var fileSystem = new FileSystem();
            if (!fileSystem.FileExists(_file))
            {
                throw new CommandFailureException("Designated serenity/jasmine file at {0} does not exist".ToFormat(_file));
            }

            fileSystem.ReadTextFile(_file, ReadText);
        }

        public void ReadText(string text)
        {
            if (text.IsEmpty()) return;

            if (text.StartsWith("include:"))
            {
                var folder = text.Split(':').Last();
                if (!Path.IsPathRooted(folder))
                {
                    folder = _file.ParentDirectory().AppendPath(folder);
                }

                Console.WriteLine("Adding content from folder " + folder);
                _application.AddContentFolder(folder);
            }
        }
    }
}