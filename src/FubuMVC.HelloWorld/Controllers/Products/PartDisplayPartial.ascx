<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.PartDisplayPartial" %>
<br />
<i><%= this.LabelFor(m => m.PartNum) %></i>
<i><%= this.DisplayFor(m => m.PartNum) %></i>