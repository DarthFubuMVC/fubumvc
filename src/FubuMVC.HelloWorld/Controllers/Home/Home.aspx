<%@ Page Language="C#" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.Home.Home" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.Home" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.UrlTemplate" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="login-status">
        <%= this.Partial<LoggedInStatusRequest>() %>
    </div>

    <%:Model.Text%>
    <br />
    The current URL is: <%: Model.CurrentUrl %>
    <br />
     View url template example: <%=this.LinkTo(new UrlTemplateRequest()).Text("url template") %>
     <br />
    Number of files attached: <%: Model.NumberOfFiles %>
    <br />
    <form method="post" enctype="multipart/form-data" action="<%= Urls.UrlFor<HomeFilesModel>() %>">
    <br />
    File 1:  <input type="file" name="homefiles" />
    <br />
    File 2:  <input type="file" name="homefiles" />
    <br />
    File 3:  <input type="file" name="homefiles" />
    <br />
    File 4:  <input type="file" name="homefiles" />
    <input type="submit" />
    </form>

</asp:Content>