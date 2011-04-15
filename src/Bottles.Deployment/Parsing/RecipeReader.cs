using System;
using System.Collections.Generic;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class RecipeReader
    {
        private readonly string _directory;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public RecipeReader(string directory)
        {
            _directory = directory;
        }

        public Recipe Read()
        {
            var recipe = new Recipe();

            // need to read the recipe control file
            // need to read each host file


            _fileSystem.FindFiles(_directory, new FileSet(){
                Include = "*.host"
            }).Each(file =>
            {

            });

            throw new NotImplementedException();
        }
    }
}