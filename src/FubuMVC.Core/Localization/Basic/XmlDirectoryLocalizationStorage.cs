using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;

namespace FubuMVC.Core.Localization.Basic
{
    public class XmlDirectoryLocalizationStorage : ILocalizationStorage
    {
        public const string MissingLocaleConfigFile = "missing.locale.config";
        private const string Key = "key";
        private const string RootElement = "fubu-localization";
        private const string Missing = "missing";
        private static readonly string LeafElement;
        private readonly IEnumerable<string> _directories;
        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly string _missingLocaleFile;
        private readonly object _missingLocker = new object();

        static XmlDirectoryLocalizationStorage()
        {
            LeafElement = "string";
        }

        public XmlDirectoryLocalizationStorage(IEnumerable<string> directories)
        {
            _directories = directories;

            _missingLocaleFile = directories.First().AppendPath(MissingLocaleConfigFile);
        }

        public void WriteMissing(string key, string text, CultureInfo culture)
        {
            lock (_missingLocker)
            {
                var document = getMissingKeysDocument();

                var xpath = "{0}[@{1}='{2}']".ToFormat(Missing, Key, key);
                if (document.DocumentElement.SelectSingleNode(xpath) != null)
                {
                    return;
                }
                    
                document.DocumentElement.AddElement(Missing)
                    .WithAtt(Key, key)
                    .WithAtt("culture", culture.Name)
                    .InnerText = text;

                document.Save(_missingLocaleFile);
            }
        }

        private XmlDocument getMissingKeysDocument()
        {
            return _missingLocaleFile.XmlFromFileWithRoot("missing-localization");
        }

        public void LoadAll(Action<string> tracer, Action<CultureInfo, IEnumerable<LocalString>> callback)
        {
            var fileSet = new FileSet{
                DeepSearch = false,
                Include = "*.locale.config",
                Exclude = "missing.locale.config"
            };

            var files = _directories
                .SelectMany(dir => _fileSystem.FindFiles(dir, fileSet))
                .Where(file => !Path.GetFileName(file).StartsWith("missing."));

            files
                .GroupBy(CultureFor)
                .Each(group =>
                {
                    var strings = group.SelectMany(f =>
                    {
                        tracer("Reading localization data from " + f);
                        return LoadFrom(f);
                    });
                    callback(group.Key, strings);
                });
        }

        public IEnumerable<LocalString> Load(CultureInfo culture)
        {
            // Just not going to worry about duplicates here
            var filename = GetFileName(culture);
            return _directories
                .Select(x => x.AppendPath(filename))
                .Where(file => _fileSystem.FileExists(file))
                .SelectMany(LoadFrom);
        }

        public bool HasMissingLocalizationKeys()
        {
            var document = getMissingKeysDocument();
            return document.DocumentElement.SelectNodes(Missing).Count > 0;
        }


        public static void Write(string directory, CultureInfo culture, IEnumerable<LocalString> strings)
        {
            var filename = GetFileName(culture);
            var file = directory.AppendPath(filename);

            Write(file, strings);
        }

        public static void Write(string file, IEnumerable<LocalString> strings)
        {
            var document = new XmlDocument();
            var root = document.WithRoot(RootElement);

            strings.OrderBy(x => x.value).Each(
                x => { root.AddElement(LeafElement).WithAtt(Key, x.value).InnerText = x.display; });

            document.Save(file);
        }

        public string MissingLocaleFile
        {
            get { return _missingLocaleFile; }
        }

        public void MergeAllMissing()
        {

            var cache = new Cache<string, IList<LocalString>> (key => new List<LocalString>());
            var document = getMissingKeysDocument();
            foreach (XmlElement element in document.DocumentElement.SelectNodes(Missing))
            {
                var culture = element.GetAttribute("culture");
                cache[culture].Add(toLocalString(element));
            }

            cache.Each(writeMissingKeys);

            var emptyMissing = new XmlDocument();
            emptyMissing.WithRoot(Missing);
            emptyMissing.Save(_missingLocaleFile);
        }

        private void writeMissingKeys(string culture, IEnumerable<LocalString> missingStrings)
        {
            var filename = GetFileName(culture);
            var file = _directories.First().AppendPath(filename);

            Console.WriteLine("Writing new to " + file);

            var existingStrings = LoadFrom(file);
            var newStrings = missingStrings.ToList();
            newStrings.RemoveAll(x => existingStrings.Any(s => s.value == x.value));

            // TODO -- come from behind and move this out into some kind of observer
            var length = newStrings.Max(x => x.value.Length) + 5;
            newStrings.Each(x =>
            {
                Console.WriteLine("{0} = {1}", x.value.PadLeft(length), x.display);
            });

            Console.WriteLine();
            Console.WriteLine();

            Write(file, existingStrings.Union(newStrings));
        }

        
        private static LocalString toLocalString(XmlElement element)
        {
            return new LocalString(element.GetAttribute(Key), element.InnerText);
        }

        public static IEnumerable<LocalString> LoadFrom(string file)
        {
            var document = file.XmlFromFileWithRoot(RootElement);

            foreach (XmlElement element in document.DocumentElement.SelectNodes(LeafElement))
            {
                yield return toLocalString(element);
            }
        }

        public static string GetFileName(CultureInfo culture)
        {
            var name = culture.Name;
            return GetFileName(name);
        }

        public static string GetFileName(string cultureName)
        {
            return "{0}.locale.config".ToFormat(cultureName);
        }

        public static CultureInfo CultureFor(string filename)
        {
            var cultureName = Path.GetFileName(filename).Split('.').First();
            return new CultureInfo(cultureName);
            
        }

        public IEnumerable<string> Directories
        {
            get { return _directories; }
        }
    }
}