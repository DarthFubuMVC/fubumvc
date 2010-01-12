using System.IO;

namespace FubuMVC.Core.Diagnostics.TextWriting
{
    internal class PlainLine : Line
    {
        public PlainLine(string contents)
        {
            Contents = contents;
        }

        public string Contents { get; set; }

        #region Line Members

        public void OverwriteCounts(CharacterWidth[] widths)
        {
            // no-op
        }

        public void Write(TextWriter writer, CharacterWidth[] widths)
        {
            writer.WriteLine(Contents);
        }

        #endregion
    }
}