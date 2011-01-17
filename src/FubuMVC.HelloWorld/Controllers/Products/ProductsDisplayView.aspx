<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductsDisplayView" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Home" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Products" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%= this.LinkTo(new HomeInputModel()).Text("Home") %>
    <h2>Products</h2>
    <ul>
        <%= this.PartialForEach(m => m.Products).Using<ProductDisplayPartial>() %>
    </ul>
</asp:Content>