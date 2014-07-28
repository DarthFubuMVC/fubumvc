using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Services.Messaging;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using HtmlTags;
using StructureMap;

namespace FubuMVC.StructureMap.Diagnostics
{
    public class SettingsFubuDiagnostics
    {
        private readonly IContainer _container;

        public SettingsFubuDiagnostics(IServiceFactory facility)
        {
            if (facility is StructureMapContainerFacility)
            {
                _container = facility.As<StructureMapContainerFacility>().Container;
            }
            else
            {
                _container = new Container();
            }
        }

        public Dictionary<string, object> get_settings()
        {
            var types = settingTypes();

            var settings = types.Select(x => _container.GetInstance(x)).Select(x => new SettingType(x)).ToArray();

            return new Dictionary<string, object> {{"settings", settings}};
        }

        private IEnumerable<Type> settingTypes()
        {
            return _container.Model.PluginTypes
                .Where(x => x.PluginType.IsConcrete() && x.PluginType.Name.EndsWith("Settings"))
                .Select(x => x.PluginType)
                .OrderBy(x => x.Name);
        }

        public SettingVisualization get_setting_Name(SettingSearch search)
        {
            var type = settingTypes().FirstOrDefault(x => x.FullName == search.Name);
            var setting = _container.GetInstance(type);

            return new SettingVisualization(setting);
        }
    }

    public class SettingSearch
    {
        public string Name { get; set; }
    }

    public class SettingVisualization
    {
        public string title;
        public string type;
        public string body;


        public SettingVisualization()
        {
        }

        public SettingVisualization(object settings)
        {
            var description = Description.For(settings);
            title = description.Title;


            if (settings.GetType().HasAttribute<SerializableAttribute>())
            {
                type = "json";
                body = JsonUtil.ToJson(settings);
            }
            else
            {
                type = "html";
                body = new DescriptionBodyTag(description).ToString();
            }
        }
    }

    public class SettingType
    {
        public SettingType()
        {
        }

        public SettingType(object settings)
        {
            type = settings.GetType().FullName;
            name = settings.GetType().Name;
            description = Description.For(settings).Title;
        }

        public string name;
        public string description;
        public string type;
    }
}