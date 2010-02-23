<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductPartial" %>
        <li>
            <h3><u>Product</u></h3>
            <p>
                <b>Code:</b> <%= this.DisplayFor(m => m.Code) %>
            </p>
            <p> 
                <b>Name:</b> <%= this.DisplayFor(m => m.Name) %>
            </p>
        </li>