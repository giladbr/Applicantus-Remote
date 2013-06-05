using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IdeaSparx.CoolControls.Website
{
    public partial class Applicant : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Get the Applicant ID from URL and get all his data to the datasource
            string TheApplicantID = Request.QueryString["id"];
            if (TheApplicantID == null) {TheApplicantID="1";}
            ApplicantData.SelectCommand = "Select * from Applicants where ApplicantID=" + TheApplicantID;
            ApplicantPrevJobs.SelectCommand += " where ApplicantID=" + TheApplicantID;
            ApplicantData.DataSourceMode = SqlDataSourceMode.DataReader;
            //Set page title
            SqlDataReader Reader = (SqlDataReader)ApplicantData.Select(DataSourceSelectArguments.Empty);
            if (Reader.Read())
            {
                Page.Title = Reader["FirstName"].ToString() + " " + Reader["LastName"].ToString() + " - פרטי מועמד";
            }
            //Unfortuantely and for no obvious reason, Page.Controls and FindControls just didn't work, so... איכלוס ידני של הטקסטבוקסים
            ApplicantID.Text = Reader[ApplicantID.ID].ToString();
            FirstName.Text =  Reader[FirstName.ID].ToString();
            LastName.Text =  Reader[LastName.ID].ToString();
            CellPhone.Text =  Reader[CellPhone.ID].ToString();
            Email.Text =  Reader[Email.ID].ToString();
            LivesIn.Text =  Reader[LivesIn.ID].ToString();
            CanDrive.Text =  Reader[CanDrive.ID].ToString();
            HasCar.Text =  Reader[HasCar.ID].ToString();
            FamilyStatus.Text =  Reader[FamilyStatus.ID].ToString();
            Age.Text =  Reader[Age.ID].ToString();
        }
    }
}