<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductPartial" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Products"%>
        <li>
            <h3><u>Product</u></h3>
            <p>
                <b>Code:</b> <%= this.DisplayFor(m => m.Code) %>
            </p>
            <p> 
                <b>Name:</b> <%= this.DisplayFor(m => m.Name) %>
            </p>
            <p>
                <b>Parts:</b> <%= this.PartialForEach(m => m.Parts).Using<PartPartial>() %>
            </p>
        </li>