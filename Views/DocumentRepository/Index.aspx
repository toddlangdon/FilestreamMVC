<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>
    <form action="<%=Url.Action("Search","DocumentRepository") %>" method="get">
        
        Search for document(s) containing phrase:&nbsp;
        <input type="text" id="phrase" name="phrase" />&nbsp;
        <input type="submit" value="Search" />
    
    </form>
</asp:Content>
