<%@ Page Language="C#" MasterPageFile="~/Shared/Site.Master" Inherits="FubuMVC.HelloWorld.Controllers.OutputModels.Index" %>
<%@ Import Namespace="FubuMVC.HelloWorld.Controllers.OutputModels" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<p><b>SomeOtherProperty:</b> [<%: Model.SomeOtherProperty %>] (should be empty)</p>
<p><b>FavoriteAnimalNameSetting:</b> [<%: Model.Settings.FavoriteAnimalName%>] </p>
<p><b>BestSimpsonsCharacter:</b> [<%: Model.Settings.BestSimpsonsCharacter%>] </p>
</asp:Content>