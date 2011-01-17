<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true"  Inherits="FubuMVC.HelloWorld.Controllers.UrlTemplate.Find" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1> Hi from <%: Model.Product.Code %></h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
