<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<FilestreamMVC.Models.Document>>" %>
<%@ Import Namespace="FilestreamMVC.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SearchResults
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>SearchResults</h2>
    <table>
        <thead>
            <tr><td>ID</td><td>Name</td></tr>
        </thead>
    <%if (ViewData.Model.Count() == 0)
      {%>
        <tr><td align="center" colspan="2">No matches found!</td></tr>
    <%}
      else
      {%>
      <%
          foreach (Document d in ViewData.Model)
          {%>
    <tr>
        <td><%=d.ID%></td>
        <td><%=d.Name%></td>
    </tr>        
        <%
          }
      } %>
    </table> 
</asp:Content>
