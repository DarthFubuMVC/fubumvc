<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleView.aspx.cs" Inherits="AspNetApplication.WebForms.SimpleView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Simple View</title>
</head>
<body>
    <h1><%=Model.Text%></h1>
</body>
</html>
