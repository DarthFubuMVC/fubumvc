<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.PartPartial" %>
<br />
<i><%= this.LabelFor(m => m.PartNum) %></i>
<i><%= this.InputFor(m => m.PartNum) %></i>