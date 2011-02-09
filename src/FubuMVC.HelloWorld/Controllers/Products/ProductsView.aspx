<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductsView" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Home" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Products" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%= this.LinkTo(new HomeInputModel()).Text("Home") %>
    <h2>Products</h2>
    <%= this.FormFor<ProductsForm>() %>
    <ul>
        <%= this.PartialForEach(m => m.Products) %>
    </ul>
    <input type="submit" value="Submit" />
    <%= this.EndForm() %>
</asp:Content>