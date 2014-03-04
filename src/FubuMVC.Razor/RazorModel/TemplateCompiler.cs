using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Razor;
using FubuCore;

namespace FubuMVC.Razor.RazorModel
{
    public interface ITemplateCompiler
    {
        Type Compile(string className, CodeCompileUnit codeCompileUnit, RazorEngineHost host);
    }

    public class TemplateCompiler : ITemplateCompiler
    {
        public Type Compile(string className, CodeCompileUnit codeCompileUnit, RazorEngineHost host)
        {
            var compilerParameters = new CompilerParameters {GenerateInMemory = true, CompilerOptions = "/optimize"};
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Each(x => compilerParameters.ReferencedAssemblies.Add(x.Location));

            CompilerResults compilerResults;
            using (var codeDomProvider = Activator.CreateInstance(host.CodeLanguage.CodeDomProviderType).As<CodeDomProvider>())
            {
                compilerResults = codeDomProvider.CompileAssemblyFromDom(compilerParameters, codeCompileUnit);
                if (compilerResults.Errors.HasErrors)
                {
                    using (var sw = new StringWriter())
                    using (var tw = new IndentedTextWriter(sw, "    "))
                    {
                        codeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, tw, new CodeGeneratorOptions());
                        var source = sw.ToString();
                        throw CreateExceptionFromCompileError(compilerResults, source);
                    }
                }
            }

            var templateTypeName = "{0}.{1}".ToFormat(host.DefaultNamespace, className);
            var templateType = compilerResults.CompiledAssembly.GetType(templateTypeName);
            return templateType;
        }

        private static HttpCompileException CreateExceptionFromCompileError(CompilerResults compilerResults, string source)
        {
            var message = string.Join("{0}{0}".ToFormat(Environment.NewLine), compilerResults
                                                                                  .Errors
                                                                                  .OfType<CompilerError>()
                                                                                  .Where(x => !x.IsWarning)
                                                                                  .Select(error =>
                                                                                          "Compile error at {0}{1}line {2}: {1}compile error: {3}: {4}"
                                                                                          .ToFormat(error.FileName,
                                                                                                    Environment.NewLine,
                                                                                                    error.Line,
                                                                                                    error.ErrorNumber,
                                                                                                    error.ErrorText))
                                                                                  .ToArray());
            return new HttpCompileException("{0}{1}{2}".ToFormat(message, Environment.NewLine, source));
        }
    }
}