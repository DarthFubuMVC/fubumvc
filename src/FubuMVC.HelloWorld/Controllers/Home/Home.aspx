<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/Site.Master" CodeBehind="Home.aspx.cs" Inherits="FubuMVC.HelloWorld.Controllers.Home.Home" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="login-status">
        <% this.Partial<LoggedInStatusRequest>(); %>
    </div>

    <%=Model.Text%>
</asp:Content>