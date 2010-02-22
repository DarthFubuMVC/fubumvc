<%@ Control Language="C#" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductPartial" %>
        <li>
            <h3><u>Product</u></h3>
            <p>
                Code: <br />
                <%= this.InputFor(m => m.Code) %>
            </p>
            <p> 
                Name: <br />
                <%= this.InputFor(m => m.Name)%>
            </p>
        </li>