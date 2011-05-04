using System;
using System.Collections.Generic;
using Bottles.Configuration;
using FubuCore;

namespace Bottles.Deployment
{
    public class Profile
    {
        public static readonly string RecipePrefix = "recipe:";
        private readonly IList<string> _recipes = new List<string>();
        private readonly EnvironmentSettings _settings = new EnvironmentSettings();
        
        public void ReadText(string text)
        {
            if (text.IsEmpty()) return;
            if (text.StartsWith(RecipePrefix))
            {
                var recipeName = text.Substring(RecipePrefix.Length).Trim();
                AddRecipe(recipeName);
            }
            else
            {
                _settings.ReadText(text);
            }
        }

        public void AddRecipe(string recipe)
        {
            _recipes.Fill(recipe);
        }

        public IEnumerable<string> Recipes
        {
            get
            {
                return _recipes;
            }
        }

        public EnvironmentSettings Settings
        {
            get { return _settings; }
        }
    }
}