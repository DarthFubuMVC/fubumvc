using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;

namespace Bottles.Deployment.Writing
{
    public class ProfileWriter
    {
        private readonly string _destination;
        private readonly IFileSystem _system;
        private readonly Cache<string, RecipeDefinition> _recipes = new Cache<string, RecipeDefinition>(name => new RecipeDefinition());
        

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
            throw new NotImplementedException();
        }

        public void AddProfileManifestProperty<T>(Expression<Func<T, object>> property, object value)
        {
            throw new NotImplementedException();
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