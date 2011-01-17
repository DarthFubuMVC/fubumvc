<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductDisplayPartial" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Products" %>
<h3><u>Product</u></h3>
<p>
    <%= this.LabelFor(m => m.Code) %>
    <%= this.DisplayFor(m => m.Code) %>
</p>
<p>
    <%= this.LabelFor(m => m.Name) %>
    <%= this.DisplayFor(m => m.Name) %>
</p>
<p>
    <%= this.LabelFor(x => x.Parts) %>
    <%= this.PartialForEach(m => m.Parts).Using<PartDisplayPartial>().WithoutItemWrapper() %>
</p>
