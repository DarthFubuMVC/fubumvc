using System;
using System.IO;
using FubuCore;
using System.Linq;

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
            new FileSystem().ReadTextFile(_file, ReadText);
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