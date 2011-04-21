using System;
using System.Linq.Expressions;
using System.Xml;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Parsing
{
    public class PackageManifestXmlReader
    {
        public PackageManifest ReadFrom(string fileName)
        {
            var document = new XmlDocument();
            document.Load(fileName);

            var manifest = new PackageManifest{
                Name = document.DocumentElement.SelectSingleNode("Name").InnerText,
                ContentFileSet = buildFileSet(document, x => x.ContentFileSet),
                DataFileSet = buildFileSet(document, x => x.DataFileSet)
            };

            foreach (XmlNode node in document.DocumentElement.SelectNodes("assembly"))
            {
                manifest.AddAssembly(node.InnerText);
            }

            return manifest;
        }

        private FileSet buildFileSet(XmlDocument document, Expression<Func<PackageManifest, object>> expression)
        {
            var node = document.DocumentElement.SelectSingleNode(expression.ToAccessor().Name) as XmlElement;

            var fileSet = new FileSet();

            if (node == null) return fileSet;


            if (node.HasAttribute("Include"))
            {
                fileSet.Include = node.GetAttribute("Include");
            }

            if (node.HasAttribute("Exclude"))
            {
                fileSet.Exclude = node.GetAttribute("Exclude");
            }


            var deepSearchNode = node.SelectSingleNode("DeepSearch");
            if (deepSearchNode != null) fileSet.DeepSearch = bool.Parse(deepSearchNode.InnerText);

            return fileSet;
        }
    }
}