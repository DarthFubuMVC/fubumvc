using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using System.Text;

namespace FubuMVC.Tests.Assets.Content
{
    public class ContentExpectationWriter
    {
        private readonly IList<string> _expectations;
        private readonly IList<string> _actuals;
        private readonly int _leftLength;
        private readonly int _startOfRightColumn;
        private readonly int _rightLength;

        public ContentExpectationWriter(IList<string> expectations, IList<string> actuals)
        {
            _expectations = expectations;
            _actuals = actuals;

            _leftLength = actuals.Max(x => x.Length);
            _startOfRightColumn = _leftLength + 10;

            _rightLength = _expectations.Max(x => x.Length);
        }

        public void Write()
        {
            writeHeaders();

            writeBodyRows();

            Debug.WriteLine("".PadRight(_startOfRightColumn + _rightLength, '='));
        }

        private void writeBodyRows()
        {
            var numberOfRows = new int[] { _expectations.Count, _actuals.Count }.Max();

            for (int i = 0; i < numberOfRows; i++)
            {
                writeBodyRow(i);
            }
        }

        private void writeBodyRow(int i)
        {
            var text = "".PadRight(_startOfRightColumn);

            if (i < _expectations.Count)
            {
                text = _expectations[i].PadRight(_startOfRightColumn);
            }

            if (i < _actuals.Count)
            {
                text += _actuals[i];
            }

            Debug.WriteLine(text);
        }

        private void writeHeaders()
        {
            var header1 = "Expected".PadRight(_startOfRightColumn) + "Actual";
            Debug.WriteLine(header1);

            var header2 = "".PadRight(_leftLength, '=') + "".PadRight(10, ' ') + "".PadRight(_rightLength, '=');
            Debug.WriteLine(header2);
        }
    }


    [TestFixture]
    public class ContentPlanPreviewerTester
    {
        [Test]
        public void try_out_the_writer()
        {
            var expected = new List<string>(){
                "Jasper",
                "Carthage"

            };

            var actual = new List<string>(){
                "Jasper",
                "Carthage",
                "Joplin",
                "Bentonville",
                "Fayetteville"
            };

            new ContentExpectationWriter(expected, actual).Write();
        }



        [Test]
        public void try_it_against_a_3_deep_hierarchy()
        {
            var theFiles = new AssetFile[]{
                new AssetFile("a.js"){FullPath = "a.js"}, 
                new AssetFile("b.js"){FullPath = "b.js"}, 
                new AssetFile("c.js"){FullPath = "c.js"}, 
                new AssetFile("d.js"){FullPath = "d.js"}, 
            };

            var plan = new ContentPlan("something", theFiles);
            var read0 = plan.GetAllSources().ElementAt(0);
            var read1 = plan.GetAllSources().ElementAt(1);
            var read2 = plan.GetAllSources().ElementAt(2);
            var read3 = plan.GetAllSources().ElementAt(3);

            var combo1 = plan.Combine(new IContentSource[] { read1, read2 });
            var combo2 = plan.Combine(new IContentSource[] { read0, combo1, read3 });


            var previewer = new ContentPlanPreviewer();
            plan.AcceptVisitor(previewer);

            previewer.WriteToDebug();
			
			var expected = new StringBuilder()
				.AppendLine("Combination")
				.AppendLine("  FileRead:a.js")
				.AppendLine("  Combination")
				.AppendLine("    FileRead:b.js")
				.AppendLine("    FileRead:c.js")					
				.Append("  FileRead:d.js");					
					
            previewer.ToFullDescription().ShouldEqual(expected);
        }
    }
}