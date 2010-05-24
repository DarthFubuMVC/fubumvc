<%@ Page Language="C#" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.IntegrationTests.RunView" %>
<%@ Import Namespace="FubuCore" %>
<%@ Import Namespace="HtmlTags" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<link media="screen" href="<%= VirtualPathUtility.ToAbsolute("~/Shared/testsuite.css") %>" type="text/css" rel="stylesheet"/>    

	    <h1>UrlContext Integration Tests</h1>
	    <h2 id="banner"></h2>
	    <ol id="tests"></ol>
	    <div id="results"></div>
	    <div id="main"></div>	

</asp:Content>

<script runat="server">

    protected const string SERVER_BASE = "http://localhost:52010/helloworld";

    protected string ScriptVar(string name, object item)
    {
        return "var {0} = {1};".ToFormat(name, JsonUtil.ToJson(item));
    }
</script>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
<%--		Intentionally using VirtualPathUtility directly because UrlContext is being tested--%>
		<script src="<%= VirtualPathUtility.ToAbsolute("~/Shared/jquery-1.4.2.js") %>" type="text/javascript"></script>
		<script src="<%= VirtualPathUtility.ToAbsolute("~/Shared/testrunner.js") %>" type="text/javascript"></script>


<script language="javascript" type="text/javascript">
    // from prototype.js
    String.prototype.endsWith = function(pattern) {
        var d = this.length - pattern.length;
        return d >= 0 && this.lastIndexOf(pattern) === d;    
    };
    
    <%= ScriptVar("serverBase", SERVER_BASE) %>

    qUnitTesting(function(config) {
        var receivedMessages = {};

        module("ToAbsolute");

        config.beforeEach = function() {
        }

        test("with an app relative path (~/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToAbsoluteUrl("~/shared/testrunner.js")) %>
            equals(url, "/helloworld/shared/testrunner.js", "the generated url");
        });

        test("with an absolute path (/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToAbsoluteUrl("/shared/testrunner.js")) %>
            equals(url, "/shared/testrunner.js", "the generated url should not change");
        });

        test("with an unqualified path (shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToAbsoluteUrl("shared/testrunner.js")) %>
            equals(url, "/helloworld/shared/testrunner.js", "the generated url should be assumed to be app relative");
        });

        test("with an full server qualified path (http://somewhere.com/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToAbsoluteUrl("http://somewhere.com/shared/testrunner.js")) %>
            equals(url, "http://somewhere.com/shared/testrunner.js", "the url should not be modified");
        });


        module("ToServerQualifiedUrl");

        config.beforeEach = function() {
        }

        test("with an app relative path (~/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToServerQualifiedUrl("~/shared/testrunner.js", SERVER_BASE)) %>
            equals(url, serverBase + "/shared/testrunner.js", "the generated url");
        });

        test("with an absolute path (/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToServerQualifiedUrl("/shared/testrunner.js", SERVER_BASE)) %>
            equals(url, "http://localhost:52010/shared/testrunner.js", "should be treated as outside the app");
        });

        test("with an unqualified path (shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToServerQualifiedUrl("shared/testrunner.js", SERVER_BASE)) %>
            equals(url, serverBase + "/shared/testrunner.js", "the generated url should be assumed to be app relative");
        });
        
        test("with an full server qualified path (http://somewhere.com/shared/testrunner.js)", function() {
            <%= ScriptVar("url", UrlContext.ToServerQualifiedUrl("http://somewhere.com/shared/testrunner.js", SERVER_BASE)) %>
            equals(url, "http://somewhere.com/shared/testrunner.js", "the url should not be modified");
        });



        module("ToPhysicalPath");

        config.beforeEach = function() {
        }

        test("with an app relative path (~/shared/testrunner.js)", function() {
            <%= ScriptVar("path", UrlContext.ToPhysicalPath("~/shared/testrunner.js")) %>
            ok(path.endsWith("src\\FubuMVC.HelloWorld\\shared\\testrunner.js"), "the path was " + path);
        });

        test("with an absolute path (/helloworld/shared/testrunner.js)", function() {
            <%= ScriptVar("path", UrlContext.ToPhysicalPath("/helloworld/shared/testrunner.js")) %>
            ok(path.endsWith("src\\FubuMVC.HelloWorld\\shared\\testrunner.js"), "the path was " + path);
        });

        test("with an unqualified path (shared/testrunner.js)", function() {
            <%= ScriptVar("path", UrlContext.ToPhysicalPath("shared/testrunner.js")) %>
            ok(path.endsWith("src\\FubuMVC.HelloWorld\\shared\\testrunner.js"), "the path was " + path);
        });



    });

</script>



</asp:Content>