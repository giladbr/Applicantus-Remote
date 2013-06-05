<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Site.master"  CodeBehind="Applicant.aspx.cs" Inherits="IdeaSparx.CoolControls.Website.Applicant" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="IdeaSparx.CoolControls.Web" Namespace="IdeaSparx.CoolControls.Web"    TagPrefix="cc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"> </asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
           <asp:ScriptManager ID="ScriptManager2" runat="server" EnablePageMethods="true">  </asp:ScriptManager>
           <asp:UpdatePanel ID="ApplicantAjax" runat="server">
<ContentTemplate>
        <asp:SqlDataSource ID="ApplicantData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>" SelectCommand=""></asp:SqlDataSource>
        <asp:SqlDataSource ID="ApplicantPrevJobs" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>" SelectCommand="Select * from PrevJobs"></asp:SqlDataSource>
    <div class="ApplicantTable">
        <div>
            <table>
                <tr>
                    <td>                        <asp:TextBox ID="ApplicantID" runat="server"></asp:TextBox></td>
                    <td>                        <asp:Label ID="Label1" Text="מספר מועמד" runat="server"></asp:Label></td>

                </tr>
                <tr>
                    <td>                        <asp:TextBox ID="FirstName" runat="server"></asp:TextBox></td>
                    <td>                        <asp:Label ID="Label2" Text="שם פרטי" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>                        <asp:TextBox ID="LastName" runat="server"></asp:TextBox></td>
                    <td>                        <asp:Label ID="Label3" Text="שם משפחה" runat="server"></asp:Label></td>

                </tr>
                <tr>
                    <td>                        <asp:TextBox ID="CellPhone" runat="server"></asp:TextBox></td>
                    <td>                        <asp:Label ID="Label4" Text="טלפון נייד" runat="server"></asp:Label></td>

                </tr>
                <tr>
                    <td>                        <asp:TextBox ID="Email" runat="server"></asp:TextBox></td>
                    <td>                        <asp:Label ID="Label5" Text="Email כתובת" runat="server"></asp:Label></td>
                </tr>
        
            </table>
                    <br /><br /><br />
                   <table>

        <tr>
                 <td><asp:TextBox ID="LivesIn" runat="server" ></asp:TextBox></td>
            <td>            <asp:Label ID="Label11" Text="מקום מגורים" runat="server" ></asp:Label></td>
   
        </tr>
        <tr>
                 <td><asp:TextBox ID="CanDrive" runat="server"></asp:TextBox></td>
       <td><asp:Label ID="Label12" Text="רשיון נהיגה" runat="server" ></asp:Label></td>
        </tr>
        <tr>
               <td><asp:TextBox ID="HasCar" runat="server"></asp:TextBox></td>
        <td><asp:Label ID="Label13" Text="בעל רכב" runat="server" ></asp:Label></td>
     
        </tr>
       <tr>
                   <td><asp:TextBox ID="FamilyStatus" runat="server"></asp:TextBox></td>
           <td><asp:Label ID="Label14" Text="סטטוס משפחתי" runat="server" ></asp:Label></td>

        </tr>
       <tr>
           <td><asp:TextBox ID="Age" runat="server"></asp:TextBox></td>
           <td><asp:Label ID="Label15" Text="גיל" runat="server"  ></asp:Label></td>
        
        </tr>
     </table>
        <br /><br />
<table>
          <tr>
           <td><asp:TextBox ID="Notes" runat="server"></asp:TextBox></td>
           <td><asp:Label ID="Label6" Text="הערות" runat="server"  ></asp:Label></td>
        
        </tr>

</table>

        </div>
        <div id="GridDiv" style="width:530px;">
            <h3>עבודות קודמות</h3>
            <br />
             <cc1:CoolGridView ID="PrevJobs" runat="server" AllowSorting="True"  AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" CssClass="GridStyle"  
                         DataSourceID="ApplicantPrevJobs" ShowFooter="false" BorderColor="Silver">
                <Columns>
                    <asp:CommandField />
                    <asp:BoundField DataField="StartYear" HeaderText="שנת התחלה" SortExpression="StartYear" />
                    <asp:BoundField DataField="EndYear" HeaderText="שנת סיום" SortExpression="EndYear" />
                    <asp:BoundField DataField="Employer" HeaderText="שם המעסיק" SortExpression="Employer" />
                    <asp:BoundField DataField="JobTitle" HeaderText="תפקיד" SortExpression="JobTitle" />
                    <asp:BoundField DataField="JobDescription" HeaderText="תיאור התפקיד" SortExpression="JobDescription" />
                </Columns>
            </cc1:CoolGridView>
        </div>

        </div>
             </ContentTemplate>
                 </asp:UpdatePanel>
    </asp:Content>
