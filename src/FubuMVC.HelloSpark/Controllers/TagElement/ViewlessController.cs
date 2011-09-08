using HtmlTags;

namespace FubuMVC.HelloSpark.Controllers.TagElement
{
	public class ViewlessController
	{
		public HtmlTag Tag(HtmlTagRequest request)
		{
			return new HtmlTag(request.TagName).Text(request.Text);
		}

		public HtmlDocument Document(HtmlDocumentRequest request)
		{
			var document = new HtmlDocument { Title = request.Title };
			document.Add(new HtmlTag("h1").Text(request.Title));
			document.Add(new HtmlTag("p").Text(request.Text));

			return document;
		}
	}

	public class HtmlDocumentRequest
	{
		public string Title { get; set; }
		public string Text { get; set; }
	}

	public class HtmlTagRequest
	{
		public HtmlTagRequest(string tagName, string text)
		{
			TagName = tagName;
			Text = text;
		}

		public string TagName { get; set; }
		public string Text { get; set; }
	}
}