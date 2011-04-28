using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace Bottles.Deployment.Writing
{
    public class DeploymentWriter
    {
        private readonly string _destination;
        private readonly IFileSystem _system;
        private readonly Cache<string, RecipeDefinition> _recipes = new Cache<string, RecipeDefinition>(name => new RecipeDefinition(name));
        private readonly IList<PropertyValue> _profileValues = new List<PropertyValue>();
        private readonly TypeDescriptorCache _types = new TypeDescriptorCache();


        public DeploymentWriter(string destination) : this(destination, new FileSystem())
        {
        }

        public DeploymentWriter(string destination, IFileSystem system)
        {
            _destination = destination;
            _system = system;
        }

        public RecipeDefinition RecipeFor(string name)
        {
            return _recipes[name];
        }

        public void Flush(FlushOptions options)
        {
            if(options == FlushOptions.Wipeout)
            {
                _system.DeleteDirectory(_destination);
                Thread.Sleep(10); //file system is async
                _system.CreateDirectory(_destination);    
            }
            
            writeBottleManifest();

            writeDirectories();

            _system.WriteStringToFile(FileSystem.Combine(_destination, ProfileFiles.BottlesManifestFile), "");

            writeEnvironmentSettings();

            _recipes.Each(writeRecipe);
        }

        private void writeDirectories()
        {
            createDirectory(_destination, ProfileFiles.BottlesDirectory);
            createDirectory(_destination, ProfileFiles.RecipesDirectory);
            createDirectory(_destination, ProfileFiles.EnvironmentsFolder);
            createDirectory(_destination, ProfileFiles.ProfilesFolder);
        }

        private void writeBottleManifest()
        {
            _system.WriteStringToFile(FileSystem.Combine(_destination, ProfileFiles.BottlesManifestFile), "");
        }

        private void writeEnvironmentSettings()
        {
            var writer = new StringWriter();
            _profileValues.Each(v => writer.WriteLine(v.ToString()));

            var environmentFile = FileSystem.Combine(_destination, ProfileFiles.EnvironmentSettingsFileName);
            _system.WriteStringToFile(environmentFile, writer.ToString());
        }

        private void writeRecipe(RecipeDefinition recipe)
        {
            new RecipeWriter(_types).WriteTo(recipe, _destination);
        }


        public void AddEnvironmentSetting<T>(Expression<Func<T, object>> property, string host, object value)
        {
            _profileValues.Add(new PropertyValue(){
                Accessor = property.ToAccessor(),
                HostName = host,
                Value = value
            });
        }

        public void AddEnvironmentSetting(string name, object value)
        {
            _profileValues.Add(new PropertyValue(){
                Name = name,
                Value = value
            });
        }

        private void createDirectory(params string[] pathParts)
        {
            string directory = FileSystem.Combine(pathParts);

            _system.CreateDirectory(directory);
        }
    }

    public enum FlushOptions
    {
        Preserve,
        Wipeout
    }
}