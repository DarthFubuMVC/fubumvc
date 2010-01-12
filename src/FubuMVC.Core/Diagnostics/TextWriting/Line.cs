using System.IO;

namespace FubuMVC.Core.Diagnostics.TextWriting
{
    internal interface Line
    {
        void OverwriteCounts(CharacterWidth[] widths);
        void Write(TextWriter writer, CharacterWidth[] widths);
    }
}