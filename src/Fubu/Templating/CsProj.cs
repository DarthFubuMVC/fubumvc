namespace Fubu.Templating
{
    public class CsProj
    {
        public CsProj()
        {
            ProjectType = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"; // default C#
        }

        public string Name { get; set; }
        public string ProjectType { get; set; }
        public string RelativePath { get; set; }
        public string ProjectGuid { get; set; }
    }
}