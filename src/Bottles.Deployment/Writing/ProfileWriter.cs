using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace Bottles.Deployment.Writing
{
    // RecipeDefinition really needs to be HostDefinition

    public class ProfileWriter
    {
        private readonly string _destination;
        private readonly IFileSystem _system;
        private readonly Cache<string, RecipeDefinition> _recipes = new Cache<string, RecipeDefinition>(name => new RecipeDefinition(name));
        private readonly IList<PropertyValue> _profileValues = new List<PropertyValue>();

        public ProfileWriter(string destination) : this(destination, new FileSystem())
        {
        }

        public ProfileWriter(string destination, IFileSystem system)
        {
            _destination = destination;
            _system = system;
        }

        public RecipeDefinition RecipeFor(string name)
        {
            return _recipes[name];
        }

        public void Flush()
        {
            _system.DeleteDirectory(_destination);

            _system.CreateDirectory(_destination);
            _system.CreateDirectory(FileSystem.Combine(_destination, ProfileFiles.RecipesFolder));

            var types = new TypeDescriptorCache();

            _recipes.Each(recipe =>
            {
                var recipeDirectory = FileSystem.Combine(_destination, ProfileFiles.RecipesFolder, recipe.Name);
                _system.CreateDirectory(recipeDirectory);


                // TODO -- need to write recipe control file
                recipe.Hosts().Each(host =>
                {
                    new HostWriter(types).WriteTo(host, recipeDirectory);
                });

            });
        }


        public void AddProfileManifestProperty<T>(Expression<Func<T, object>> property, string host, object value)
        {
            _profileValues.Add(new PropertyValue(){
                Accessor = property.ToAccessor(),
                HostName = host,
                Value = value
            });
        }

        public void AddProfileManifestProperty<T>(Expression<Func<T, object>> property, object value)
        {
            _profileValues.Add(new PropertyValue(){
                Accessor = property.ToAccessor(),
                Value = value
            });
        }



    }
}