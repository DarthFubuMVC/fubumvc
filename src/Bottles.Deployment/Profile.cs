using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Configuration;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;

namespace Bottles.Deployment
{
    public class Profile
    {
        public static readonly string RecipePrefix = "recipe:";
        private readonly IList<string> _recipes = new List<string>();
        private readonly EnvironmentSettings _settings;
        private readonly Cache<string,string> _overrides;
        private readonly Cache<string, SettingsData> _settingsByHost = new Cache<string, SettingsData>(name => new SettingsData(SettingCategory.profile) { Provenance = "PROFILE NAME" });

        public Profile() : this(new EnvironmentSettings())
        {
        }

        public Profile(EnvironmentSettings settings)
        {
            _settings = settings;
            _overrides = new Cache<string, string>();

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
                if (!text.Contains("="))
                {
                    throw new EnvironmentSettingsException(text);
                }

                var parts = text.Split('=').Select(x => x.Trim()).ToArray();
                if (parts.Count() != 2)
                {
                    throw new EnvironmentSettingsException(text);
                }

                var value = parts.Last();
                var directiveParts = parts.First().Split('.');


                if (directiveParts.Length == 1) //override 'property=value'
                {
                    _overrides[parts.First()] = value;
                }
                else if (directiveParts.Length >= 3) // host.directive.property=value
                {
                    var hostName = directiveParts.First();
                    var propertyName = directiveParts.Skip(1).Join(".");

                    _settingsByHost[hostName][propertyName] = value;
                }
                else
                {
                    throw new EnvironmentSettingsException(text);
                }
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

        public SettingsData DataForHost(string hostName)
        {
            return _settingsByHost[hostName];
        }

        public Cache<string, string> Overrides
        {
            get { return _overrides; }
        }
    }
}