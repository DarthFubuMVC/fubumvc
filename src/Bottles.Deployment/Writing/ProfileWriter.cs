using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;

namespace Bottles.Deployment.Writing
{
    public class ProfileWriter
    {
        private readonly string _destination;
        private readonly IFileSystem _system;
        private readonly Cache<string, RecipeDefinition> _recipes = new Cache<string, RecipeDefinition>(name => new RecipeDefinition());
        private readonly IList<PropertyValue> _profileValues = new List<PropertyValue>();

        public ProfileWriter(string destination) : this(destination, new FileSystem())
        {
        }

        public ProfileWriter(string destination, IFileSystem system)
        {
            _destination = destination;
            _system = system;
        }

        public void Flush()
        {
            _system.CleanDirectory(_destination);
            _system.DeleteDirectory(_destination);

            _system.CreateDirectory(_destination);
            _system.CreateDirectory(FileSystem.Combine(_destination, ProfileFiles.RecipesFolder));
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

        public void AddToRecipe(string recipeName, params IDirective[] directives)
        {
            _recipes[recipeName].AddDirectives(directives);
        }

        public void AddValueToRecipe<T>(string recipeName, Expression<Func<T, object>> expression, object value)
        {
            _recipes[recipeName].AddProperty(expression, value);
        }

        public void RegisterBottle(string recipeName, BottleReference reference)
        {
            _recipes[recipeName].AddReference(reference);
        }
    }
}