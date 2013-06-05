using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Applicantus.DataSetTableAdapters;
using System.Text.RegularExpressions;
using IdeaSparx.CoolControls.Web;
 
namespace Applicantus
{
    public partial class _Default : System.Web.UI.Page
    {
        //Global class variables
        int ApplicantIDColumnIndex = 0; //index of the ApplicantID Column. Set on grid init
        string ColumnTitle = ""; //to use in PassThroughColumns
        bool ShowOrHide = true; //dito

        protected void ColumnSort(object sender, CommandEventArgs e)    //do the actual sorting
        {
            string[] SortParams = e.CommandArgument.ToString().Split(',');
            SortDirection direction;
            if (SortParams[1] == "asc") { direction = SortDirection.Ascending; }
                else { direction = SortDirection.Descending; };
            ApplicantsGridView.Sort(SortParams[0], direction);
            ApplicantsGridView.DataBind();
        }

        protected void ClearFilter_Click(object sender, EventArgs e)
        {
            Session["FilterExp"] = MainData.FilterExpression = "";
            Session["SelectCommand"] = MainData.SelectCommand = "select * from Applicants;";
            ApplicantsGridView.DataBind();
        }
        protected void FilterGrid_Click(object sender, EventArgs e)    //Create SQL filtering statements
        {
            string TheColumn = ((Control)sender).ID.Replace("ComboBox", "");
            AjaxControlToolkit.ComboBox ComboBox1 = (AjaxControlToolkit.ComboBox)sender;
            string str = "";
            for (int i = 0; i < ComboBox1.Items.Count; i++)
            {
                if (ComboBox1.Items[i].Selected)
                {
                    if (ComboBox1.Items[i].Text != string.Empty)
                    {
                        if (str != string.Empty) { str += ","; }
                        str += "'" + ComboBox1.Items[i].Text + "'";
                    }
                }
            }
            string FilterExp = TheColumn + " IN (" + str + ")";
            Session["FilterExp"] = MainData.FilterExpression = FilterExp;
        }

        void DoPaging(Object sender, EventArgs e)
        {
        }

        protected void GridView_Sorting(Object sender, GridViewSortEventArgs e)                  //Save sorting expressions to session
        {
            Session["SortExp"] = e.SortExpression;
            Session["SortDir"] = e.SortDirection;
        }

        protected void UpdateTotalRows(object sender, EventArgs e)               
        {
            if (ApplicantsGridView.Rows.Count == 0)
                { TotalRowCounter.Text = "!לא נמצאו תוצאות לחיפוש"; }
            else
            {
                if (ApplicantsGridView.FooterRow != null)
                { TotalRowCounter.Text = "סה''כ: " + ApplicantsGridView.FooterRow.Cells[0].Text + " שורות"; }
            }
        }

        //הפונקציה שמבצעת את החיפוש
        protected void SearchTheGrid(object sender, EventArgs e)
        {
             //Server-side validation: the regular expression is designed to catch -- (SQL Comment) and a bunch of special characters
            string TheRegEx = "--|[!#$%^&();^*]";
            if (Regex.IsMatch(TheRegEx, Request.Form["SearchBox"]))
            {
                SearchBoxErrorAsp.Text = "הביטוי שניסית לחפש מכיל תווים אסורים";
            }
            else
            {
                Session["SelectCommand"] = "select * from applicants where ";
                if (SearchDropDown.SelectedIndex > 1)
                {
                    Session["SelectCommand"] += SearchDropDown.SelectedValue + " like '%" + Request.Form["SearchBox"] + "%' or ";
                }
                else { PassThroughColumns("SearchStatement"); }
                //to close the last "or":
                Session["SelectCommand"] += "1=2";
                MainData.SelectCommand = (string)Session["SelectCommand"];
                ApplicantsGridView.DataBind();
              
            }
            //To check the Regex LIVE - uncomment the following line, and remove comments from relevant controls in aspx: Response.Write(Regex.IsMatch(TestBox.Text,RegexBox.Text)));
        }
        
        string DataSourceSelectSQL(string Field) //Just a simple routine to set the DataSource's SQL Strings
        {
            return "SELECT DISTINCT [" + Field + "] from [Applicants]";
        }

        //פונקציה שעוברת על שורות הטבלה ועושה דברים
        protected void PassThroughRows(string WhatToDo)
        {
            for (int i = 0; i < ApplicantsGridView.Rows.Count; i++)
            {
                switch (WhatToDo)
                {
                    case "":
                        {
                            break;
                        }
                }
            }
        }

        //פונקציה שעוברת על טורי הטבלה ועושה דברים
        protected void PassThroughColumns(string WhatToDo)
        {
            string ColumnHebName = "";
            string ColumnEngName = "";
            //Start with i=2 to avoid columns "מספר שורה" and "פתיחת חלון מועמד"
            for (int i = 2; i < ApplicantsGridView.Columns.Count; i++)
            {
               ColumnHebName = ApplicantsGridView.Columns[i].AccessibleHeaderText;
                if (ColumnHebName == "") {           ColumnHebName = ApplicantsGridView.Columns[i].HeaderText;};
                ColumnEngName = ApplicantsGridView.Columns[i].SortExpression;
                if (ColumnEngName == "") {ColumnEngName = ((BoundField)ApplicantsGridView.Columns[i]).DataField; };
                switch (WhatToDo)
                    {
                        case "PopulateDropDown":
                            if ((ColumnHebName != "") && (ColumnHebName != "פרטים נוספים") && (ColumnHebName != "עבודות קודמות"))
                            {
                                ListItem li = new ListItem();
                                li.Text = ColumnHebName;
                                li.Value = ColumnEngName;
                                SearchDropDown.Items.Add(li);
                            }                           break;
                        case "SearchStatement": if ((ColumnEngName != "") && (ColumnEngName != "DummyExpression"))
                                                                    { Session["SelectCommand"] += ColumnEngName + " like '%" + Request.Form["SearchBox"] + "%' or "; }; break;
                        case "ShowHideColumn":
                            if (ColumnHebName == ColumnTitle) {   ApplicantsGridView.Columns[i].Visible = ShowOrHide;     };        break;
                    }
            } //End of loop in the columns
        }

        protected void ApplicantsGridView_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            
            //This function is run for every row, but the first row it is run for is not a real data row, and should be ignored. So check that the row has same amount of cells like the Grid:
            if (e.Row.Cells.Count == ((CoolGridView)sender).Columns.Count)
            {
                Control TheControl = e.Row.FindControl("CellPhoneTextBox");
                if (TheControl != null) { string s = ClientScript.GetPostBackEventReference(TheControl, ""); }
                SqlDataSource ThePrevJobsData = new SqlDataSource();
                ThePrevJobsData.ConnectionString = MainData.ConnectionString;
                ThePrevJobsData.SelectCommand = "select * from PrevJobs";
                //The heart of this function - adjust the SQL according to the ApplicantID:
                ThePrevJobsData.FilterExpression = "ApplicantID = " +  e.Row.Cells[ApplicantIDColumnIndex].Text;
                CoolGridView TheGridView = (CoolGridView)e.Row.FindControl("PrevJobs");
                if (TheGridView != null)
                {
                    TheGridView.DataSource = ThePrevJobsData;
                    TheGridView.DataBind();
                }
            }
        }
        protected void ApplicantsGridView_Init(object sender, EventArgs e)
        {
            ApplicantIDColumnIndex = FindColumn("ApplicantID");
        }
         
        protected void PopulateSearchDropDown ()
        {
            SearchDropDown.Items.Clear();
            SearchDropDown.Items.Add("הכל");
            SearchDropDown.Items.Add("---------");
            PassThroughColumns("PopulateDropDown");
        }
             
        protected void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Find column by name (using its SortExpression and AccessibleHeaderText) and return its index or -1 if not found.
        protected int FindColumn(string ColumnName)
        {
            int i = 0;
            while (i < ApplicantsGridView.Columns.Count)
            {
                if (ApplicantsGridView.Columns[i].SortExpression == ColumnName ||
                    ApplicantsGridView.Columns[i].AccessibleHeaderText == ColumnName)
                        { return i; }
                i++;
            }
            return -1;
        }
            
        //טיפול בלחיצה על הכפתור שמציג ומסתיר את הטבלה הפנימית של העבודות הקודמות
        protected void ApplicantsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowHidePrevJobs")
            {
                Button TheButton = ((Button)e.CommandSource);
                CoolGridView TheInnerGrid = (CoolGridView)TheButton.Parent.FindControl("PrevJobs");
                int TheWidth = 502;
                //הצג/הסתר את הטבלה הפנימית
                TheInnerGrid.Visible = !TheInnerGrid.Visible;
                if (TheButton.Text == "הצג") { TheButton.Text = "הסתר"; }
                else
                {
                    TheButton.Text = "הצג";
                    TheWidth = 110;
                }
                //שנה את רוחב העמודה בהתאם
                ApplicantsGridView.Columns[FindColumn("עבודות קודמות")].ItemStyle.Width = TheWidth;
                ApplicantsGridView.Columns[FindColumn("עבודות קודמות")].HeaderStyle.Width = TheWidth;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Check if we are here because of an asynchronious request, by checking the parameters in Request.Form
            if (Request.Form.Count > 0)
            {
                if (Request.Form.Get(0) == "UpdateSQL")
                {
                    //בצע עדכון של הנתונים
                    SqlConnection conn = new SqlConnection(MainData.ConnectionString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand
                        ("update applicants set " + Request.Form.Get(2) + "='" + Request.Form.Get(3) + "' where applicantID = " + Request.Form.Get(1), conn);
                    cmd.ExecuteNonQuery();
            }}
            LastSearch.Value = Request.Form["SearchBox"]; //Get last search value from the request, and put it in a hidden field. A script will pass it from there to the html input SearchBox.
            SearchBoxErrorAsp.Text = "";
            if (SearchDropDown.Items.Count == 0) { PopulateSearchDropDown(); };
            MainData.FilterExpression = (string)Session["FilterExp"];
            if ((string)Session["SelectCommand"] != null)
                    { MainData.SelectCommand = (string)Session["SelectCommand"]; };
            string SortExp = (String)Session["SortExp"];
            if (Session["SortExp"] != null) { ApplicantsGridView.Sort(SortExp, (SortDirection)(Session["SortDir"])); };
            //Set SelectCommands of data sources to populate them
            BirthCountryData.SelectCommand = DataSourceSelectSQL("BirthCountry");
            CanDriveData.SelectCommand = DataSourceSelectSQL("CanDrive");
            HasCarData.SelectCommand = DataSourceSelectSQL("HasCar");
            LivesInData.SelectCommand = DataSourceSelectSQL("LivesIn");
            FamilyStatusData.SelectCommand = DataSourceSelectSQL("FamilyStatus");
            AgeData.SelectCommand = DataSourceSelectSQL("Age");
        }


        protected void ShowHideDetails(object sender, EventArgs e)
        {
            ColumnTitle = "פרטים נוספים";
            if ((sender as Button).Text == "הצג פרטים נוספים")
            {
                ShowOrHide = true;
                (sender as Button).Text = "הסתר פרטים נוספים";
            }
            else
            {
                ShowOrHide = false;
                (sender as Button).Text = "הצג פרטים נוספים";
            }
            PassThroughColumns("ShowHideColumn");
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
        }

         protected void OpenApplicantDetails_Click (object sender, EventArgs e)
        {
            Response.Redirect("/applicant.aspx?id=" + (sender as Button).CommandArgument);
        }
    } //End of _Default class
} //End of namespace
