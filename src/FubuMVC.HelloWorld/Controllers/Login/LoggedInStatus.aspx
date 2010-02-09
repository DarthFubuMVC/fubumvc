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
        [ <%= this.LinkTo<LoginRequestModel>().Text("Log On") %> ]
<%
    }
%>