using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace ServiceNode
{
    public static class TextFileWriter
    {
        private static readonly string _file =
            ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory().AppendPath("test-state.txt");

        public static void Clear()
        {
            new FileSystem().DeleteFile(_file);
        }

        public static void Write(string text)
        {
            new FileSystem().AlterFlatFile(_file, list => list.Add(text));
        }

        public static IEnumerable<string> Read()
        {
            return new FileSystem().ReadStringFromFile(_file).Split(Environment.NewLine.ToCharArray()).Where(x => x.IsNotEmpty());
        }
    }
}