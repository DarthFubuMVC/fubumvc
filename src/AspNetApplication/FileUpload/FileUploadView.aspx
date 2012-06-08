<%@ Page Language="C#" AutoEventWireup="true" Inherits="AspNetApplication.FileUpload.FileUploadView" %>
<%@ Import Namespace="AspNetApplication.FileUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Upload View</title>
</head>
<body>
    <h1><%=Model.Text%></h1>
    
    <form method="post" enctype="multipart/form-data" action="<%= Urls.UrlFor<FileUploadInput>(null) %>">
    <br />
    File 1:  <input type="file" name="File1" />
        <br/>
    <input type="submit" />
    </form>
</body>
</html>
