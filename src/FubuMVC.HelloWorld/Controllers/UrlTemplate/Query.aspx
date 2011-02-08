<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Site.Master" AutoEventWireup="true" Inherits="FubuMVC.HelloWorld.Controllers.UrlTemplate.Query" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.UrlTemplate" %>
<%@ Import Namespace="HtmlTags" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/html" id="myTemplate">
        <li>
            <a href='<%=Urls.TemplateFor<ViewProductRequest>() %>'> ${ Code } </a>
        </li>
    </script>
    <ul id="templatedList">
        
    </ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
	<script src="<%= VirtualPathUtility.ToAbsolute("~/Shared/jquery-1.4.2.js") %>" type="text/javascript"></script>
	<script src="<%= VirtualPathUtility.ToAbsolute("~/Shared/jquery.tmpl.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        jQuery(function ($) {
            var products = <%=JsonUtil.ToJson(Model.Products) %>;

            $.each(products, function (i){
                var temp = $("#myTemplate").tmpl(products[i]);

                $('#templatedList').append(temp);
            });
        });
    </script>

</asp:Content>
