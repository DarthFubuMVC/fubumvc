<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Products.ProductsView" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Products"%>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Products</h2>
    <ul>
        <%= this.PartialForEach(m => m.Products) %>
    </ul>
</asp:Content>