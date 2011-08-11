using HtmlTags;

namespace FubuMVC.HelloSpark.Controllers.TagElement
{
	public class HtmlController
	{
		public HtmlTag Tag(TagRequest request)
		{
			return new HtmlTag(request.TagName).Text(request.Text);
		}

		public HtmlDocument Document(DocumentRequest request)
		{
			var document = new HtmlDocument { Title = request.Title };
			document.Add(new HtmlTag("h1").Text(request.Title));
			document.Add(new HtmlTag("p").Text(request.Text));

			return document;
		}
	}

	public class DocumentRequest
	{
		public string Title { get; set; }
		public string Text { get; set; }
	}

	public class TagRequest
	{
		public TagRequest(string tagName, string text)
		{
			TagName = tagName;
			Text = text;
		}

		public string TagName { get; set; }
		public string Text { get; set; }
	}
}