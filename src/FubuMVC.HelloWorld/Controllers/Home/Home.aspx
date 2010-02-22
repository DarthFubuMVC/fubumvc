<%@ Page Language="C#" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Home.Home" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="login-status">
        <% this.Partial<LoggedInStatusRequest>(); %>
    </div>

    <%=Model.Text%>
</asp:Content>