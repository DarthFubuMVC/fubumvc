using System;
using System.Collections.Generic;
using System.Text;

namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPlanVisitor
    {
        void Push(IContentSource node);
        void Pop();
    }

    public class ContentPlanPreviewer : IContentPlanVisitor
    {
        private readonly IList<string> _descriptions = new List<string>();
        private int _level = 0;


        public void Push(IContentSource node)
        {

            _descriptions.Add("".PadRight(_level * 2, ' ') + node.ToString());
            _level++;
        }

        public void Pop()
        {
            _level--;
        }

        public void WriteToDebug()
        {
            _descriptions.Each(x => System.Diagnostics.Debug.WriteLine(x));
        }

        public IEnumerable<string> Descriptions
        {
            get
            {
                return _descriptions;
            }
        }

        public string ToFullDescription()
        {
            var builder = new StringBuilder();
            _descriptions.Each(x => builder.AppendLine(x));

            return builder.ToString().Trim();
        }
    }

    
}