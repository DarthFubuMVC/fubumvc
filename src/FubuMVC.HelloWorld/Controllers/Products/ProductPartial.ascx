<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductPartial" %>
<h3><u>Product</u></h3>
<p>
    <%= this.LabelFor(m => m.Code) %>
    <%= this.InputFor(m => m.Code) %>
</p>
<p>
    <%= this.LabelFor(m => m.Name) %>
    <%= this.InputFor(m => m.Name) %>
</p>
<p>
    <%= this.LabelFor(x => x.Parts) %>
    <%= this.PartialForEach(m => m.Parts).WithoutItemWrapper() %>
</p>