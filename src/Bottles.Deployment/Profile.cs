using System.Collections.Generic;
using System.Linq;
using Bottles.Configuration;
using FubuCore;
using FubuCore.Configuration;

namespace Bottles.Deployment
{
    public class Profile
    {
        public static readonly string RecipePrefix = "recipe:";
        private readonly IList<string> _recipes = new List<string>();
        private readonly EnvironmentSettings _settings;
        private readonly SettingsData _profileSettings;

        public Profile() : this(new EnvironmentSettings())
        {
        }

        public Profile(EnvironmentSettings settings)
        {
            _settings = settings;
            _profileSettings = new SettingsData(SettingCategory.profile);
            _profileSettings.Provenance = "profile";
        }

        public void ReadText(string text)
        {
            if (text.Trim().IsEmpty()) return;
            if (text.StartsWith(RecipePrefix))
            {
                var recipeName = text.Substring(RecipePrefix.Length).Trim();
                AddRecipe(recipeName);
            }
            else
            {
                _profileSettings.With("key", "value");

                if (!text.Contains("="))
                {
                    throw new EnvironmentSettingsException(text);
                }

                var parts = text.Split('=').Select(x => x.Trim()).ToArray();
                if (parts.Count() != 2)
                {
                    throw new EnvironmentSettingsException(text);
                }

                _profileSettings.With(parts[0], parts[1]);

                //TODO: -- tuck prov in here
                //_settings.ReadText(text);
                
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