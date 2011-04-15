using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using System.Linq;

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

        public void RegisterBottle(string recipeName, BottleReference reference)
        {
            _recipes[recipeName].AddReference(reference);
        }

        
        public class RecipeDefinition
        {
            private readonly IList<IDirective> _directives = new List<IDirective>();
            private readonly IList<BottleReference> _references = new List<BottleReference>();

            public void AddReference(BottleReference reference)
            {
                _references.Add(reference);
            }

            public void AddDirectives(IEnumerable<IDirective> directives)
            {
                _directives.AddRange(directives);
            }

            public void AddDirective(IDirective directive)
            {
                _directives.Add(directive);
            }
        }

    }


    public class RecipeWriter
    {
        
        private readonly ITypeDescriptorCache _types;
        private readonly TextWriter _writer = new StringWriter();

        public RecipeWriter(ITypeDescriptorCache types)
        {
            _types = types;
        }

        public void WriteReference(BottleReference reference)
        {
            var text = ProfileFiles.BottlePrefix + reference.Name;
            if (reference.Relationship.IsNotEmpty())
            {
                text += " " + reference.Relationship;
            }

            _writer.WriteLine(text);
        }

        public void WriteDirective(IDirective directive)
        {
            var directiveWriter = new DirectiveWriter(_writer, _types);
            directiveWriter.Write(directive);
        }

        public string ToText()
        {
            return _writer.ToString();
        }

        public IEnumerable<string> AllLines()
        {
            var lines = new List<string>();

            using (var reader = new StringReader(ToText()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }
    }

    public class DirectiveWriter
    {
        private readonly TextWriter _writer;
        private readonly ITypeDescriptorCache _types;
        private readonly Stack<string> _names = new Stack<string>();
        private string _prefix;

        public DirectiveWriter(TextWriter writer, ITypeDescriptorCache types)
        {
            _writer = writer;
            _types = types;
        }

        private void setPrefix(Action<Stack<string>> configure)
        {
            configure(_names);
            _prefix = _names.Reverse().Join(".") + ".";
        }

        public void Write(object directive)
        {
            var type = directive.GetType();
            setPrefix(x => x.Push(type.Name));

            write(directive, type);

            setPrefix(x => x.Pop());
        }

        private void write(object directive, Type type)
        {
            _types.ForEachProperty(type, prop =>
            {
                var child = prop.GetValue(directive, null);

                if (prop.PropertyType.IsSimple())
                {
                    var stringValue = child == null ? string.Empty : child.ToString();
                    _writer.WriteLine("{0}{1}={2}", _prefix, prop.Name, stringValue);
                }
                else
                {
                    if (child != null)
                    {
                        setPrefix(x => x.Push(prop.Name));
                        write(child, child.GetType());
                        setPrefix(x => x.Pop());
                    }
                }
            });
        }
    }
}