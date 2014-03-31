using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuCsProjFile.Templating;
using System.Linq;

namespace Fubu.Generation
{
    public static class SolutionFinder
    {
         public static string FindSolutionFile()
         {
             var currentDirectory = Environment.CurrentDirectory.ToFullPath();
             var files = new FileSystem().FindFiles(currentDirectory, FileSet.Deep("*.sln"));
             if (files.Count() == 1)
             {
                 return Path.GetFileName(files.Single());
             }
             
             if (files.Any())
             {
                 ConsoleWriter.Write(ConsoleColor.Yellow, "Found more than one *.sln file:");
                 files.Each(x => ConsoleWriter.Write(ConsoleColor.Yellow, x));
                 ConsoleWriter.Write(ConsoleColor.Yellow, "You need to specify the solution with the --solution flag");
             }
             else
             {
                 ConsoleWriter.Write(ConsoleColor.Yellow, "Could not find any *.sln files");
             }

             return null;
         }

        public static IEnumerable<string> FindSolutions()
        {
            var currentDirectory = Environment.CurrentDirectory.ToFullPath();
            var files = new FileSystem().FindFiles(currentDirectory, FileSet.Deep("*.sln"));

            return files.Select(x => x.ToFullPath().PathRelativeTo(currentDirectory));
        }
    }
}