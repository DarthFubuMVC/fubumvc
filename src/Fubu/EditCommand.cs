using System;
using System.ComponentModel;
using System.Diagnostics;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    public class EditorInput
    {
        [Description("Path to the replacement editor")]
        public string Path { get; set; }
    }

    [CommandDescription("Choose or display the path to the editor that you wish fubu.exe to use when opening files")]
    public class EditorCommand : FubuCommand<EditorInput>
    {
        public override bool Execute(EditorInput input)
        {
            if (input.Path.IsNotEmpty())
            {
                Console.WriteLine("Setting the editor to " + input.Path);
                EditorLauncher.SetEditor(input.Path);
            }
            else
            {
                Console.WriteLine("The editor is:");
                Console.WriteLine(EditorLauncher.GetEditor());
            }

            return true;
        }
    }

    public static class EditorLauncher
    {
        // Just leave this be
        public static string Editor = "FubuDocsEditor";

        public static void LaunchFile(string file)
        {
            var editor = GetEditor();

            if (editor.IsNotEmpty())
            {
                Process.Start(editor, file);
            }
            else
            {
                new FileSystem().LaunchEditor(file);
            }
        }

        public static string GetEditor()
        {
            return Environment.GetEnvironmentVariable(Editor, EnvironmentVariableTarget.Machine) ?? "";
        }

        public static void SetEditor(string editor)
        {
            Environment.SetEnvironmentVariable(Editor, editor, EnvironmentVariableTarget.Machine);
        }
    }
}