<%@ Page Language="C#" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Home.Home" %>
<%@ Import Namespace="FubuMVC.Core.UI" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="login-status">
        <% this.Partial<LoggedInStatusRequest>(); %>
    </div>

    <%:Model.Text%>
    <br />
    The current URL is: <%: Model.CurrentUrl %>
</asp:Content>