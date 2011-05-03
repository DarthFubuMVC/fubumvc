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
        private readonly IFileSystem _system;
        private readonly Cache<string, RecipeDefinition> _recipes = new Cache<string, RecipeDefinition>(name => new RecipeDefinition(name));
        private readonly IList<PropertyValue> _profileValues = new List<PropertyValue>();
        private readonly TypeDescriptorCache _types = new TypeDescriptorCache();
        private DeploymentSettings _settings;


        public DeploymentWriter(string destination) : this(destination, new FileSystem())
        {
        }

        public DeploymentWriter(string destination, IFileSystem system)
        {
            _settings = new DeploymentSettings(destination);
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
                _system.DeleteDirectory(_settings.DeploymentDirectory);
                Thread.Sleep(10); //file system is async
                _system.CreateDirectory(_settings.DeploymentDirectory);    
            }
            
            writeBottleManifest();

            writeDirectories();

            _system.WriteStringToFile(_settings.BottleManifestFile, "");

            writeEnvironmentSettings();

            _recipes.Each(writeRecipe);
        }

        private void writeDirectories()
        {
            createDirectory(_settings.BottlesDirectory);
            createDirectory(_settings.RecipesDirectory);
            createDirectory(_settings.EnvironmentsDirectory);
            createDirectory(_settings.ProfilesDirectory);
        }

        private void writeBottleManifest()
        {
            _system.WriteStringToFile(_settings.BottleManifestFile, "");
        }

        private void writeEnvironmentSettings()
        {
            var writer = new StringWriter();
            _profileValues.Each(v => writer.WriteLine(v.ToString()));

            _system.WriteStringToFile(_settings.EnvironmentFile, writer.ToString());
        }

        private void writeRecipe(RecipeDefinition recipe)
        {
            new RecipeWriter(_types).WriteTo(recipe, _settings);
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