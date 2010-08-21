<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryGreeting.aspx.cs" Inherits="FubuMVC.HelloWorld.Controllers.Home.Home" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Query for Greeting</title>
</head>
<body>
    <div>
        This should only be accessible via GET
    </div>

    <div>
        <form action="querygreeting" method="post">
        Attempt to POST back to this page.
        <input type="submit" value="POST" name="PostButton" />
        </form>
    </div>

    <div>
        <form action="greeting" method="post">
        Attempt to POST to the greeting command page.
        <input type="submit" value="POST" name="OtherPostButton" />
        </form>
    </div>

</body>
</html>
