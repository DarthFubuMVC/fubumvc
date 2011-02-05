<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoggedInStatus.aspx.cs" Inherits="FubuMVC.HelloWorld.Controllers.Login.LoggedInStatus" %>
<%
    if (Model.IsLoggedIn) {
%>
        Welcome <b><%= Model.UserName %></b>!
        [ <%= this.LinkTo<LogoffRequestModel>().Text("Log Off") %> ]
<%
    }
    else {
%> 

    <form method="post" action="<%= Urls.UrlFor(new LoginRequestModel()) %>">
		<%= this.AntiForgeryToken("Login") %>
		<input type="submit" value="Log On!" />
	</form>
<%
    }
%>