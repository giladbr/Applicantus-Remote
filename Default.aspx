<%@ Page Language="C#"  MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Applicantus._Default" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolKit" %>
<%@ Register Assembly="IdeaSparx.CoolControls.Web" Namespace="IdeaSparx.CoolControls.Web"    TagPrefix="cc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"> 
    <style type="text/css">
        .auto-style1 { width: 140px; height: 25px; }
        .auto-style2 { width: 60px; height: 25px; }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager2" runat="server" EnablePageMethods="true">  </asp:ScriptManager>
<%-- To check the Regex matching, uncomment the following lines (and do stuff on the cs as well):
      <asp:TextBox runat="server" ID="RegexBox" ReadOnly="false" />
      <asp:TextBox runat="server" ID="TestBox" ReadOnly="false" />
     <asp:Button runat="server" id="Button2" OnClick="SearchBoxServerValidation" />--%>
             <asp:UpdatePanel ID="MainAjax" runat="server">

<ContentTemplate>

    <%--Standard Jquery js files--%>
    <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/jquery-validation-1.9.0/jquery.validate.js"></script>
    <script src="Scripts/jquery.validate.wrapper.js"></script>
    <script src="Scripts/jquery-ui-1.10.1.custom.js"></script>
    <script src="Scripts/jquery-ui-1.10.1.custom.min.js"></script>
    <script src="Scripts/jquery.highlighttextarea.js"></script>
    <script src="Scripts/jquery.highlight-3.js"></script>
    <%--My js files --%>
    <script src="Javascript/SearchBox.js" type="text/javascript"></script>
    <script src="Javascript/Misc.js"></script>
    <script src="Javascript/DataEdit.js"></script>

   <div ID="TitleDiv" class="FullWidth" style="position:absolute; top: 0px; height:0px; text-align:center;">
        <asp:Label runat="server" cssclass="Title" Text="מועמדים" />    
    </div>
     
    <%--Data Sources--%>
    <asp:SqlDataSource ID="BirthCountryData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="CanDriveData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource> 
     <asp:SqlDataSource ID="HasCarData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="LivesInData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="FamilyStatusData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="AgeData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"></asp:SqlDataSource>
    <asp:SqlDataSource ID="MainData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"
                SelectCommand="select * from applicants"></asp:SqlDataSource>
    <asp:SqlDataSource ID="PrevJobsData" runat="server" ConnectionString="<%$ ConnectionStrings:ApplicantusConnectionString %>"  
                SelectCommand="select * from PrevJobs"></asp:SqlDataSource>
    
     <%-- Search Controls--%>
            <script type="text/javascript">
                if (document.readyState == "complete") {
                    //Get last search value from ctl00_MainContent_LastSearch - the DOM element for the asp:HiddenField LastSearch. Then run "OnKeyUp" to do hightlighting.
                    var LastSearch = document.getElementById("SearchBox").value = document.getElementById("ctl00_MainContent_LastSearch").value;
                    OnKeyUp(LastSearch);
                }
        </script>
   <table class="SearchTable">
       <tr>
            <td class="auto-style1"> ...חפש </input>
            <td class="auto-style1"> ...ב </input>
            <td class="auto-style2">      <asp:ImageButton runat="server" CssClass="ClickableText" ImageUrl="~/SearchGo.png" OnClick="SearchTheGrid"/>   </input>
        </tr>
        <asp:HiddenField ID="LastSearch" runat="server"></asp:HiddenField>
    <tr style="position:absolute; top:17px;">
        <td class="BigWidth">            <input id="SearchBox" name="SearchBox" class="SearchBox" onkeydown="OnKeyDown(this.value);" onkeyup="OnKeyUp(this.value);"/>           </input>
        <td class="BigWidth">            <asp:DropDownList ID="SearchDropDown" runat="server" CssClass="NormalBorder"/>        </input>
        <td class="MiddleWidth">            <asp:ImageButton ID="SearchButton" runat="server" CssClass="SearchIcon" ImageUrl="~/Search.png" OnClick="SearchTheGrid"/>    </input>
    </tr>
    <tr style="position:absolute; top:67px;">
        <td class="ErrorMessage" id="SearchBoxError"> </input>
        <asp:Label ID="SearchBoxErrorAsp" runat="server" CssClass="ErrorMessage"></asp:Label>
    </tr>
     </table>

<%-- "כפתור "נקה את כל הסינונים--%>
    <div ID="ButtonDiv" class="FullWidth" style="position:absolute; top:150px;">
        <div ID="Div2" class="BigButtonDiv">
            <asp:Button ID="ShowHideDetalis" runat="server" CssClass="BigButton" Text="הצג פרטים נוספים" onclick="ShowHideDetails" />
             &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
            <asp:Button ID="ClearAllFilters" runat="server" CssClass="BigButton" OnClick="ClearFilter_Click" Text="נקה את כל הסינונים" />
        </div>
    </div>
    <asp:Label ID="TotalRowCounter" runat="server" style="position:absolute; top:170px;"></asp:Label>
    <%-- Main GridView --%>
    <div style="margin-top:190px;">
        <%-- אפשר להוסיף כאן טקסט לכפתורים של הפייג'ר! --%> 
    <cc1:CoolGridView ID="ApplicantsGridView" runat="server" AllowPaging="True" AllowSorting="True" OnRowDataBound="ApplicantsGridView_RowDataBound" OnInit="ApplicantsGridView_Init"
        AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" CssClass="GridStyle" 
        DataKeyNames="ApplicantID" DataSourceID="MainData" OnSorting="GridView_Sorting" OnPreRender="UpdateTotalRows" ShowFooter="false" OnPageIndexChange="DoPaging"
        PagerSettings-Position="TopAndBottom" PagerStyle-BackColor="#B3DCEF" PagerStyle-Font-Bold="true" PageSize="10"
        PagerStyle-Font-Size="Larger" PagerStyle-HorizontalAlign="Center" PagerStyle-Width="100%" BorderColor="Silver"
        OnRowCommand="ApplicantsGridView_RowCommand">
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:TemplateField AccessibleHeaderText="פתיחת חלון מועמד">
                 <HeaderTemplate>
                     <br /><br />
                    פתיחת חלון מועמד<br></br>
                    <br></br>
                     </HeaderTemplate>
                <ItemTemplate>
               <Button ID="OpenApplicantDetails" onclick="OpenApplicantDetails_Click(<%# Eval("ApplicantID") %>)">פתח חלון מועמד</Button>
                <script type="text/javascript">
                    function OpenApplicantDetails_Click (TheApplicantID)
                    {
                        //Chrome ignores the width/height properties... But writing something there makes it open in a new window instead of a new tab
                        window.open("/applicant.aspx?id=" + TheApplicantID, '', 'height=1000');
                    }
                </script>
                    </ItemTemplate>

            </asp:TemplateField>
  
            <asp:BoundField DataField="ApplicantID" HeaderText="מספר מועמד" ReadOnly="True" SortExpression="ApplicantID" />
           <asp:TemplateField AccessibleHeaderText="שם פרטי" SortExpression="FirstName">
                <HeaderTemplate>                    שם פרטי                </HeaderTemplate>
                <ItemTemplate>
                          <input name="FirstNameInput" Class="CellInput" value="<%# Eval("FirstName") %>"/>
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField AccessibleHeaderText="שם משפחה" SortExpression="LastName">
                <HeaderTemplate>                    שם משפחה                </HeaderTemplate>
                <ItemTemplate>
                         <Input name="LastNameInput" Class="CellInput" value="<%# Eval("LastName") %>" />
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField AccessibleHeaderText="טלפון נייד" SortExpression="CellPhone">
                <HeaderTemplate>                    טלפון נייד                </HeaderTemplate>
                <ItemTemplate>
                <Input name="CellPhoneInput" Class="CellInput"  style="direction:ltr" value="<%# Eval("CellPhone") %>" />
                     </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField AccessibleHeaderText="כתובת Email" SortExpression="Email">
                <HeaderTemplate>                    כתובת Email                </HeaderTemplate>
                <ItemTemplate>
                           <Input name="EmailInput" Class="CellInput"  style="direction:ltr" value="<%# Eval("Email") %>" />
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField AccessibleHeaderText="פרטים נוספים" Visible="false" SortExpression="DummyExpression">
                <HeaderTemplate>  <br /><br /><br /><br /><br /> <h2>                    פרטים נוספים         </h2>
                 <br /><br /><br />
                <table class="TableInsideGrid">
                    <tr>
                        <td>טלפון נייד נוסף</input>
                        <td> טלפון בבית</input>
                        <td>מספר ת"ז</input>
                        <td > כתובת</input>
                        <td> שנת לידה</input>
                        <td> תאריך לידה</input>
                    </tr>
                </table>
                </HeaderTemplate>
                <ItemTemplate>
                      <table class="TableInsideGrid">
                    <tr>
                        <td contenteditable id="CellPhone2TD" Class="CellInput" style="direction: ltr" ><%# Eval("CellPhone2") %></input>
                        <td contenteditable id="HomePhoneTD" Class="CellInput" style="direction: ltr" ><%# Eval("HomePhone") %>             </input>
                        <td contenteditable id="IsraeliIDTD" Class="CellInput" style="direction: ltr"> <%# Eval("IsraeliID") %>             </input>
                        <td><textarea name="AddressID" id="AddressTD" Class="CellInput"       style="resize:none"                     > <%# Eval("Address") %></textarea></input>
                        <td contenteditable id="BirthYearTD" Class="CellInput" style="direction: ltr" ><%# Eval("BirthYear") %>              </input>
                        <td contenteditable id="BirthDateTD" Class="CellInput" style="direction: ltr" ><%# Eval("BirthDate") %>             </input>
                    </tr>
                </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="מקום מגורים" SortExpression="LivesIn">
                <HeaderTemplate>
                    מקום מגורים<br></br>
                    <br></br>
                    <asp:ImageButton ID="LivesInAsc" runat="server" CommandArgument="LivesIn,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="LivesInDesc" runat="server" CommandArgument="LivesIn,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="LivesInComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend"
                    DataSourceID="LivesInData" DataValueField="LivesIn" SelectionMode="Multiple"  AutoPostBack="true" OnSelectedIndexChanged="FilterGrid_Click"></AjaxControlToolKit:ComboBox>
                    <br />
                    <br />
                    <asp:Button ID="LivesInClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
                <ItemTemplate>
                           <table class="CellText"><td contenteditable id="LivesInTD" Class="CellInput"> <%# Eval("LivesIn") %></input> </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="רשיון נהיגה" SortExpression="CanDrive">
                <HeaderTemplate>
                    רשיון נהיגה<br></br>
                    <br></br>
                    <asp:ImageButton ID="CanDriveAsc" runat="server" CommandArgument="CanDrive,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="CanDriveDesc" runat="server" CommandArgument="CanDrive,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="CanDriveComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend"
                        DataSourceID="CanDriveData" DataValueField="CanDrive" AutoPostBack="true" SelectionMode="Multiple"  OnSelectedIndexChanged="FilterGrid_Click"></AjaxControlToolKit:ComboBox>
                    <br />
                    <br />
                    <asp:Button ID="CanDriveClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
                <ItemTemplate>
                                           <asp:TextBox ID="TextBox1" runat="server" CssClass="CellText" Text=' <%# Eval("CanDrive") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="בעל רכב" SortExpression="HasCar">
                <HeaderTemplate>
                    בעל רכב<br></br>
                    <br></br>
                    <asp:ImageButton ID="HasCarAsc" runat="server" CommandArgument="HasCar,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="HasCarDesc" runat="server" CommandArgument="HasCar,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="HasCarComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend"   DataSourceID="HasCarData"
                    DatavalueField="HasCar" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterGrid_Click"></AjaxControlToolKit:ComboBox>
                    <br />
                    <br />
                    <asp:Button ID="HasCarClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
                <ItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" CssClass="CellText" Text=' <%# Eval("HasCar") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
           
            <%-- גריד פנימי --%>
            <%-- יש כאן הגדרה מפורשת של רוחב כדי שיהיה ניתן לשנות אותה בלחיצה על הכפתור --%>
             <asp:TemplateField AccessibleHeaderText="עבודות קודמות" ItemStyle-Width="110px" HeaderStyle-Width="110px" SortExpression="DummyExpression">
                  <HeaderTemplate>                  עבודות קודמות                 </HeaderTemplate>
               <ItemTemplate>
                <asp:Button runat="server" ID="ServerButton" Text="הצג" CommandName="ShowHidePrevJobs"/>
                <cc1:CoolGridView ID="PrevJobs" runat="server" AllowSorting="True"  AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" CssClass="NestedGrid" 
                         DataKeyNames="ApplicantID" OnSorting="GridView_Sorting" ShowFooter="false" BorderColor="Silver" Visible="false">
                <Columns>
                    <asp:CommandField />
                    <asp:BoundField DataField="StartYear" HeaderText="שנת התחלה" SortExpression="StartYear" />
                    <asp:BoundField DataField="EndYear" HeaderText="שנת סיום" SortExpression="EndYear" />
                    <asp:BoundField DataField="Employer" HeaderText="שם המעסיק" SortExpression="Employer" />
                    <asp:BoundField DataField="JobTitle" HeaderText="תפקיד" SortExpression="JobTitle" />
                    <asp:BoundField DataField="JobDescription" HeaderText="תיאור התפקיד" SortExpression="JobDescription" />
             
                </Columns>
            </cc1:CoolGridView>
               </ItemTemplate>
            </asp:TemplateField>
                   <asp:BoundField DataField="Notes" HeaderText="הערות" SortExpression="Notes" />
            <asp:TemplateField AccessibleHeaderText="ארץ לידה" SortExpression="BirthCountry">
                <HeaderTemplate>
                    ארץ לידה<br></br>
                    <br></br>
                    <asp:ImageButton ID="BirthCountryAsc" runat="server" CommandArgument="BirthCountry,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="BirthCountryDesc" runat="server" CommandArgument="BirthCountry,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="BirthCountryComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend"
                    DataSourceID="BirthCountryData" DataValueField="BirthCountry" SelectionMode="Multiple"  AutoPostBack="true" OnSelectedIndexChanged="FilterGrid_Click"> </AjaxControlToolKit:ComboBox>
                    <br />     <br />
                    <asp:Button ID="BirthCountryClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
               
                <ItemTemplate>
                    <%# Eval("BirthCountry") %>
                </ItemTemplate>
            </asp:TemplateField>
                         <asp:BoundField DataField="AliyahYear" HeaderText="שנת עלייה" SortExpression="AliyahYear" />
             <asp:TemplateField AccessibleHeaderText="גיל" SortExpression="Age">
                <HeaderTemplate>
                    גיל<br></br>
                    <br></br>
                    <asp:ImageButton ID="AgeAsc" runat="server" CommandArgument="Age,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="AgeDesc" runat="server" CommandArgument="Age,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="AgeComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend"
                    DataSourceID="AgeData" DataValueField="Age" SelectionMode="Multiple"  AutoPostBack="true" OnSelectedIndexChanged="FilterGrid_Click"></AjaxControlToolKit:ComboBox>
                    <br />
                    <br />
                    <asp:Button ID="AgeClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
                <ItemTemplate>
                    <%# Eval("Age") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="סטטוס משפחתי" SortExpression="FamilyStatus">
                <HeaderTemplate>
                    סטטוס משפחתי<br></br>
                    <br></br>
                    <asp:ImageButton ID="FamilyStatusAsc" runat="server" CommandArgument="FamilyStatus,asc" ImageUrl="SortAsc.gif" OnCommand="ColumnSort" />
                    <asp:ImageButton ID="FamilyStatusDesc" runat="server" CommandArgument="FamilyStatus,desc" ImageUrl="~/SortDesc.gif" OnCommand="ColumnSort" />
                    <AjaxControlToolKit:ComboBox ID="FamilyStatusComboBox" runat="server" CssClass="ComboBox" AutoCompleteMode="SuggestAppend" 
                    DataSourceID="FamilyStatusData" DataValueField="FamilyStatus" SelectionMode="Multiple"  AutoPostBack="true" OnSelectedIndexChanged="FilterGrid_Click"></AjaxControlToolKit:ComboBox>
                    <br />
                    <br />
                    <asp:Button ID="FamilyStatusClearFilter" runat="server" onclick="ClearFilter_Click" Text="נקה סינון" />
                </HeaderTemplate>
                <ItemTemplate>
                    <%# Eval("FamilyStatus") %>
                </ItemTemplate>
            </asp:TemplateField>
            
        </Columns>
        <HeaderStyle CssClass="GridHeader" />
         <FooterStyle CssClass="GridFooter"/>
    </cc1:CoolGridView>
          </ContentTemplate>
                 </asp:UpdatePanel>
        
                   </div>

 
</asp:Content>