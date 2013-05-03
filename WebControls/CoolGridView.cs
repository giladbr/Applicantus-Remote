//------------------------------------------------------------------------------
// IdeaSparx.CoolControls.Web
// Author: John Eric Sobrepena (2009)
// You can use these codes in whole or in parts without warranty.
// By using any part of this code, you agree 
// to keep this information about the author intact.
// http://johnsobrepena.blogspot.com
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;

[assembly: WebResource("IdeaSparx.CoolControls.Web.CoolGridView.js", "application/x-javascript")]
[assembly: WebResource("IdeaSparx.CoolControls.Web.CoolCore.js", "application/x-javascript")]

namespace IdeaSparx.CoolControls.Web
{
    //From SearchGridView
    #region TemplateColumn
    public class NumberColumn : ITemplate
    {
        public void InstantiateIn(Control container)
        {

        }
    }
    #endregion
    [ParseChildren(true, "SearchFilters")]
    //Ended from SearchGridView
    [ToolboxData("<{0}:CoolGridView Width=\"400px\" Height=\"300px\"></{0}:CoolGridView>")]
            //("<{0}:SearchGridView runat=server></{0}:SearchGridView>")]
    public class CoolGridView : GridView
    {
        //From SearchGridView
        #region Search event and delegate
            public delegate void SearchGridEventHandler(string _strSearch);
            public event SearchGridEventHandler SearchGrid;
        #endregion
         #region Controls and constants
        //Constants to hold value in view state
        private const string SHOW_EMPTY_FOOTER = "ShowEmptyFooter";
        private const string SHOW_EMPTY_HEADER = "ShowEmptyHeader";
        private const string SHOW_TOTAL_ROWS = "ShowTotalRows";
        private const string NO_OF_ROWS = "NoOfRows";
        private const string SHOW_ROWNUM = "ShowRowNum";
        ListItemCollection _lstFilter;
        #endregion
        #region Constructor
        // Constructor
        public CoolGridView(): base()
        {
            //By default turn on the footer shown property
            ShowFooter = true;
            _BoundaryStyle.BorderColor = Color.Gray;
            _BoundaryStyle.BorderWidth = new Unit(1, UnitType.Pixel);
            _BoundaryStyle.BorderStyle = BorderStyle.Solid;
        }
        #endregion

        #region properties
        [Category("Appearance")]
        [DefaultValue(true)]
        [Bindable(BindableSupport.No)]
        public bool ShowEmptyFooter
        {
            get
            {
                if (this.ViewState[SHOW_EMPTY_FOOTER] == null)
                {
                    this.ViewState[SHOW_EMPTY_FOOTER] = true;
                }

                return (bool)this.ViewState[SHOW_EMPTY_FOOTER];
            }
            set
            {
                this.ViewState[SHOW_EMPTY_FOOTER] = value;
            }
        }

        [Category("Appearance")]
        [Bindable(BindableSupport.No)]
        [DefaultValue(true)]
        public bool ShowEmptyHeader
        {
            get
            {
                if (this.ViewState[SHOW_EMPTY_HEADER] == null)
                {
                    this.ViewState[SHOW_EMPTY_HEADER] = true;
                }

                return (bool)this.ViewState[SHOW_EMPTY_HEADER];
            }
            set
            {
                this.ViewState[SHOW_EMPTY_HEADER] = value;
            }
        }

        [Category("Behavior")]
        [Bindable(BindableSupport.No)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [MergableProperty(false)]
        [Editor(typeof(ListItemsCollectionEditor), typeof(UITypeEditor))]
        public virtual ListItemCollection SearchFilters
        {
            get
            {
                if (_lstFilter == null)
                {
                    _lstFilter = new ListItemCollection();
                    ((IStateManager)_lstFilter).TrackViewState();
                }
                return _lstFilter;
            }
        }

        [Category("Appearance")]
        [Bindable(BindableSupport.No)]
        [DefaultValue(true)]
        public bool ShowTotalRows
        {
            get
            {
                if (this.ViewState[SHOW_TOTAL_ROWS] == null)
                {
                    this.ViewState[SHOW_TOTAL_ROWS] = true;
                }

                return (bool)this.ViewState[SHOW_TOTAL_ROWS];
            }
            set
            {
                this.ViewState[SHOW_TOTAL_ROWS] = value;
            }
        }

        [Category("Appearance")]
        [Bindable(BindableSupport.No)]
        [DefaultValue(false)]
        public bool ShowRowNumber
        {
            get
            {
                if (this.ViewState[SHOW_ROWNUM] == null)
                {
                    this.ViewState[SHOW_ROWNUM] = true;
                }

                return (bool)this.ViewState[SHOW_ROWNUM];
            }
            set
            {
                this.ViewState[SHOW_ROWNUM] = value;
            }
        }
        #endregion

        #region overridden functions     

        protected override ICollection CreateColumns(PagedDataSource dataSource, bool useDataSource)
        {
            if (dataSource != null)
                ViewState[NO_OF_ROWS] = dataSource.DataSourceCount;
            return base.CreateColumns(dataSource, useDataSource);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ////If showrownumber option is turned on then add 
            ////the template column as the first column.
            //if (!IsDesign() && ShowRowNumber) 
            //{
            //    TemplateField tmpCol = new TemplateField();
            //    NumberColumn numCol = new NumberColumn();
            //    tmpCol.ItemTemplate = numCol;
            //    // Insert this as the first column
            //    this.Columns.Insert(0, tmpCol);
            //}
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            base.OnRowCreated(e);
            if (!IsDesign()) //During Runtime
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                        if (ShowTotalRows)
                        {
                            //רשום את מס' השורות בפוטר, לא רואים את זה, אבל שם זה יישמר כדי שיהיה אפשר לקחת את זה משם
                            e.Row.Cells[0].Text = "" + ViewState[NO_OF_ROWS] + "";
                        }
                }
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    // If ShowHeader is set to true and row number has to be shown
                    if (ShowRowNumber && ShowHeader)                    {     e.Row.Cells[0].Text = "מספר שורה";      }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (ShowRowNumber)
                    {
                        //Set the row number in every row
                        e.Row.Cells[0].Text = (e.Row.RowIndex + (this.PageSize * this.PageIndex) + 1).ToString();
                    }
                }
            }
        }

        //This is run for every gridview including the inner ones.
        //protected override void OnPreRender(EventArgs e)
        //{
        //    base.OnPreRender(e);
        //}

        private bool IsDesign()
        {
            if (this.Site != null)
                return this.Site.DesignMode;
            return false;
        }
        #endregion

        #region Search Functions
        public void SetFilter(GridViewRowEventArgs e)
        {
        }

        //protected string ConstructSearchString()
        //{
        //    string _strText = _tbSearch.Text.Trim();

        //    if (_strText == string.Empty)
        //        return string.Empty;

        //    return _ddlFinder.SelectedValue + " like '" + _strText + "%'";
        //}

        void _btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            //string sSearchText = ConstructSearchString();
            //OnSearchGrid(sSearchText);
        }

        protected void OnSearchGrid(string _strSearch)
        {
            if (SearchGrid != null)
            {
                SearchGrid(_strSearch);
            }
        }

        #endregion
         //Ended from SearchGridView

        private const string Suffix = "jEsCoOl";
        private CoolGridViewRow PagerRow = null;
        private string _HiddenFieldDataValue = String.Empty;

        private bool _FixHeaders = false;
        [Browsable(true), Category("Behavior"), DefaultValue(false)]
        public bool FixHeaders
        {
            get { return _FixHeaders; }
            set { _FixHeaders = value; }
        }

        private bool _AllowResizeColumn = true;
        /// <summary>
        /// Get or set if user can resize the column.
        /// </summary>
        [Browsable(true), Category("Behavior"), DefaultValue(true)]
        public bool AllowResizeColumn
        {
            get { return _AllowResizeColumn; }
            set { _AllowResizeColumn = value; }
        }

        //Default column width is set here with the number:

        private Unit _DefaultColumnWidth = new Unit(100,UnitType.Pixel);
        [Browsable(true), Category("Appearance"), DefaultValue(typeof(Unit),"100px")]
        public Unit DefaultColumnWidth
        {
            get { return _DefaultColumnWidth; }
            set {
                if (value == Unit.Empty)
                    throw new ArgumentException("DefaultColumnWidth cannot be empty.");
                if (value.Type != UnitType.Pixel)
                    throw new ArgumentException("DefaultColumnWidth can only be of type Pixel");
                _DefaultColumnWidth = value; 
            }
        }

        #region Parsers
        public List<Unit> ParseColumnWidthsFromJson(string JsonString)
        {
            List<Unit> columnWidths = new List<Unit>();
            if (!String.IsNullOrEmpty(JsonString))
            {

                RegexOptions options = (RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                Regex rx = new Regex("ColumnWidths\\s*:\\s*[[](?:([0-9.]+) || [,\\s]*)*[]]", options);

                MatchCollection mc = rx.Matches(JsonString);
                if (mc.Count > 0 && mc[0].Groups.Count > 1)
                    foreach (Capture c in mc[0].Groups[1].Captures)
                    {
                        columnWidths.Add(Unit.Parse(c.Value + "px"));
                    }
            }

            return columnWidths;
        }
        #endregion

        protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            if (rowType == DataControlRowType.Header
                || rowType == DataControlRowType.Footer
                || rowType == DataControlRowType.Pager
                //|| rowType == DataControlRowType.EmptyDataRow
                )
            {
                CoolGridViewRow c = new CoolGridViewRow(rowIndex, dataSourceIndex, rowType, rowState);
                c.ParentCoolGridView = this;
                if (rowType == DataControlRowType.Pager) PagerRow = c;
                return c;
            }
            else if (rowIndex == 0)
            {
                CoolGridViewFirstRow c = new CoolGridViewFirstRow(rowIndex, dataSourceIndex, rowType, rowState);
                c.ParentCoolGridView = this;
                return c;
            }
            else
            {
                return base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);
            }
        }

        private Unit _Width = Unit.Empty;
        [DefaultValue(typeof(Unit), ""), Category("Layout")]
        public override Unit Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private Unit _Height = Unit.Empty;
        [DefaultValue(typeof(Unit), ""), Category("Layout")]
        public override Unit Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        private new bool EnableSortingAndPagingCallbacks
        {
            get { return false; }
            set { }
        }

        private Style _BoundaryStyle = new Style();
        [PersistenceMode(PersistenceMode.InnerProperty), Description("Boundary Style"), NotifyParentProperty(true),
        Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public Style BoundaryStyle
        {
            get { return _BoundaryStyle; }
        }

        private string GridContainerID { get { return ClientID + Suffix + "_mainDiv"; } }
        private string HeaderContainerID { get { return ClientID + Suffix + "_headerDiv"; } }
        private string FooterContainerID { get { return ClientID + Suffix + "_footerDiv"; } }
        private string TableContainerID { get { return ClientID + Suffix + "_tableDiv"; } }
        private string PagerContainerID { get { return ClientID + Suffix + "_pagerDiv"; } }
        private string HiddenFieldDataID { get { return ClientID + Suffix + "_data"; } }

        //This is called after Page Load of default.aspx and Site.Master
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "CoolCore.js"))
            {
                string _url = Page.ClientScript.GetWebResourceUrl(this.GetType(), "IdeaSparx.CoolControls.Web.CoolCore.js");
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "CoolCore.js", _url);
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "CoolGridView.js"))
            {
                string _url = Page.ClientScript.GetWebResourceUrl(this.GetType(), "IdeaSparx.CoolControls.Web.CoolGridView.js");
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "CoolGridView.js", _url);
            }

            //Register the DIV resize column vertical line
            if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "lLKAopspo28lOANcaju9182ia92u"))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "lLKAopspo28lOANcaju9182ia92u", "<div id=\"lLKAopspo28lOANcaju9182ia92u\" style=\"display:none;border:solid 1px gray; background-color:#E5E5E5; width:100px; height:100px; top:0px; left:0px; position:absolute;\" ></div>", false);
            }

            //if (!Page.ClientScript.IsOnSubmitStatementRegistered(this.GetType(), "CoolGridView.js.OnFormSubmit"))
            //{
            //    Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "CoolGridView.js.OnFormSubmit", String.Format(
            //        @"if (typeof var{0} != 'undefined' || var{0} != null) var{0}.OnFormSubmit();", ClientID
            //        ));
            //}

            //Register the DIV resize column vertical line
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "CoolGridView.Style"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CoolGridView.Style",
@"<style>
//    .CoolGridViewTable TH
//    {
//        height: 250px;
//    }
//    .CoolGridViewTable TR TD, .CoolGridViewTable TR TH, .CoolGridViewTable TR TH SPAN, .CoolGridViewTable TR TD SPAN
//    {
//    }
//    .CoolGridViewTable TR TH SPAN, .CoolGridViewTable TR TD SPAN
//    {
//        width: 150px;
//        margin : 0 0 0 0;
//        padding : 0 0 0 0;
//    }
</style>", false);
            }

            //Check control state from hidden field
            _HiddenFieldDataValue = Page.Request.Form[HiddenFieldDataID];
            //Load column widths from current UI state
            //Initializes the column widths base on current UI state.
            List<Unit> columnWidths = ParseColumnWidthsFromJson(_HiddenFieldDataValue);
            int cI = 0;
            foreach (DataControlField fld in Columns)
            {
                if (fld.Visible && cI < columnWidths.Count)
                    fld.HeaderStyle.Width = columnWidths[cI++];
            }
        }

        private void AddPropertiesToRenderForGridContainer(HtmlTextWriter writer)
        {
            if (Width != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            if (Height != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            //TODO:Integrate this line with main trunk
            writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "hidden");
            BoundaryStyle.AddAttributesToRender(writer);
        }


        private string ToTableRulesValue(GridLines GridLines)
        {
            switch (GridLines)
            {
                case GridLines.Both: return "all";
                case GridLines.Horizontal: return "rows";
                case GridLines.Vertical: return "cols";
                case GridLines.None:
                default: return "none";
            }
        }

        private void RenderAttributesForTable(HtmlTextWriter writer)
        {
            //writer.AddAttribute(HtmlTextWriterAttribute.Style, Style.Value);
            //writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, CellSpacing.ToString());
            if (GridLines != GridLines.None)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Border, (BorderWidth == Unit.Empty ? "1" : BorderWidth.Value.ToString()));
                //writer.AddAttribute(HtmlTextWriterAttribute.Rules, ToTableRulesValue(GridLines));
            }
            //if (BorderWidth != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, BorderWidth.ToString());
            ControlStyle.AddAttributesToRender(writer);
            writer.AddStyleAttribute("table-layout", "fixed");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-table");
        }

        private void RenderHeader(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                if (Height != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "30px");
                if (Width != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, HeaderContainerID);
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "hidden");
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "visible");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (GetCorrectedWidth().Value + 100).ToString() + "px");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderAttributesForTable(writer);
            //TODO: Have to emit header specific Style
            //HeaderStyle.AddAttributesToRender(writer);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "0");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            if (HeaderRow != null && HeaderRow is CoolGridViewRow)
            {
                ((CoolGridViewRow)HeaderRow).RenderColGroup(writer);
                ((CoolGridViewRow)HeaderRow).RenderRow(writer);
            }

            writer.RenderEndTag();//Table
            writer.RenderEndTag();//Div
            writer.RenderEndTag();//Div
        }

        private void RenderFooter(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                if (Height != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "30px");
                if (Width != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, FooterContainerID);
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "hidden");
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "visible");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (GetCorrectedWidth().Value + 100).ToString() + "px");
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderAttributesForTable(writer);
            //TODO: Have to emit header specific Style
            //FooterStyle.AddAttributesToRender(writer);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "0");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            if (FooterRow != null && FooterRow is CoolGridViewRow)
            {
                ((CoolGridViewRow)FooterRow).RenderColGroup(writer);
                ((CoolGridViewRow)FooterRow).RenderRow(writer);
            }

            writer.RenderEndTag();//Table
            writer.RenderEndTag();//Div
            writer.RenderEndTag();//Div
        }

        private void RenderPager(HtmlTextWriter writer, string IDSufix)
        {
            if (DesignMode)
            {
                if (Height != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "30px");
                if (Width != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, PagerContainerID + IDSufix);
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, "hidden");
            writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, "visible");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            PagerStyle.AddAttributesToRender(writer);
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, GetCorrectedWidth().ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            if (AllowPaging && PagerRow != null)
            {
                ((CoolGridViewRow)PagerRow).RenderRow(writer);
            }

            writer.RenderEndTag();//Table
            writer.RenderEndTag();//Div
        }

        protected override void Render(HtmlTextWriter writer)
        {
            String orig_CssClass = CssClass;
            try
            {
                CssClass = (String.IsNullOrEmpty(this.CssClass) ? "CoolGridViewTable" : "CoolGridViewTable " + CssClass);
                Style["table-layout"] = "fixed";
                Style[HtmlTextWriterStyle.Display] = "table";
                Style[HtmlTextWriterStyle.BorderCollapse] = "collapse";
                //base.Width = GetCorrectedWidth();
                base.Width = new Unit(0, UnitType.Pixel);
                base.Height = Unit.Empty;

                //from base Render
                if (this.Page != null)
                    this.Page.VerifyRenderingInServerForm(this);
                //from base Render
                this.PrepareControlHierarchy();

                writer.AddAttribute(HtmlTextWriterAttribute.Id, GridContainerID);
                AddPropertiesToRenderForGridContainer(writer);
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //Div 1

                //Render the Pager on Top
                if (AllowPaging && (this.PagerSettings.Position == PagerPosition.Top || this.PagerSettings.Position == PagerPosition.TopAndBottom) && this.PagerRow != null)
                    RenderPager(writer, "Top");

                //Render the fixed header
                if (ShowHeader && Rows.Count > 0)
                    RenderHeader(writer);

                if (DesignMode)
                {
                    if (Height != Unit.Empty)
                    {
                        double _height = Height.Value;
                        if (ShowHeader) _height -= 30;
                        if (ShowFooter) _height -= 30;
                        if (AllowPaging) _height -= 30;
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, _height.ToString() + "px");
                    }
                    if (Width != Unit.Empty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Id, TableContainerID);
                writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");
                //Compatibility for IE 6.0
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
                writer.RenderBeginTag(HtmlTextWriterTag.Div); //Div 2
                base.RenderContents(writer);
                writer.RenderEndTag();//Div 2

                if (ShowFooter)
                    RenderFooter(writer);
                //Render the pager at bottom
                if (AllowPaging && (this.PagerSettings.Position == PagerPosition.Bottom || this.PagerSettings.Position == PagerPosition.TopAndBottom) && this.PagerRow != null)
                    RenderPager(writer, "Bottom");

                writer.RenderEndTag();//Div 1

                string jsInitialize = @"<input type=""hidden"" id=""{6}"" name=""{6}"" value=""{7}"" /><script type=""text/javascript"" language=""javascript"">
//<![CDATA[
if (typeof var{3} =='undefined' || var{3} == null)
    var var{3} = new CoolGridView({{GridContainerID: ""{0}"",HeaderContainerID: ""{1}"",TableContainerID: ""{2}"",GridID: ""{3}"",FooterContainerID: ""{4}"",PagerContainerID: ""{5}"",HiddenFieldDataID: ""{6}"",FormID:""{8}"", AllowResizeColumn : {9}}});
var{3}.Initialize();
//]]>
</script>";

                //Register JavaScript Initialization
                jsInitialize = String.Format(
                    jsInitialize,
                    GridContainerID,
                    HeaderContainerID,
                    TableContainerID,
                    ClientID,
                    FooterContainerID,
                    PagerContainerID,
                    HiddenFieldDataID,
                    _HiddenFieldDataValue,
                    this.Page.Form.ClientID,
                    AllowResizeColumn.ToString().ToLower()
                    );
                writer.Write(jsInitialize);
            }
            finally
            {
                CssClass = orig_CssClass;
            }
        }
    }

    /// <summary>
    /// The base CoolGridViewRow
    /// </summary>
    public class BaseCoolGridViewRow : GridViewRow
    {
        //Regular expression for inserting an inner span in <TD></TD>
        private Regex _reg;
        private CoolGridView _ParentCoolGridView = null;
        /// <summary>
        /// Gets the parent CoolGridView
        /// </summary>
        internal CoolGridView ParentCoolGridView
        {
            get {
                if (_ParentCoolGridView == null)
                {
                    Control c = this;
                    while((c = c.Parent) != null){
                        if (c is CoolGridView)
                        {
                            _ParentCoolGridView = (CoolGridView)c;
                            break;
                        }
                    }
                }
                return _ParentCoolGridView; 
            }
            set
            {
                _ParentCoolGridView = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the System.Web.UI.WebControls.GridViewRow class.
        /// </summary>
        /// <param name="rowIndex">The index of the System.Web.UI.WebControls.GridViewRow object in the System.Web.UI.WebControls.GridView.Rows
        ///     collection of a System.Web.UI.WebControls.GridView control.</param>
        /// <param name="dataItemIndex">The index of the System.Web.UI.WebControls.GridViewRow object in the System.Web.UI.WebControls.GridView.Rows
        ///     collection of a System.Web.UI.WebControls.GridView control.</param>
        /// <param name="rowType">One of the System.Web.UI.WebControls.DataControlRowType enumeration values.</param>
        /// <param name="rowState">A bitwise combination of the System.Web.UI.WebControls.DataControlRowState
        ///     enumeration values.</param>
        public BaseCoolGridViewRow(int rowIndex, int dataItemIndex, DataControlRowType rowType, DataControlRowState rowState)
            : base(rowIndex, dataItemIndex, rowType, rowState)
        {
            string regex = "(^\\s*<(?:td|th)[^>]*?>)(.*)(</(?:td|th)>\\s*$)";
            RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase;
            _reg = new Regex(regex, options);
        }

        /// <summary>
        /// Render a control and return the resulting HTML
        /// </summary>
        /// <param name="control">Control to render</param>
        /// <returns>String that contains the rendered HTML</returns>
        protected string GetRendering(Control control)
        {
            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter htmlw = new HtmlTextWriter(sw))
            {
                control.RenderControl(htmlw);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Place a SPAN just inside TD
        /// </summary>
        /// <param name="Html">The html of the table cell</param>
        /// <returns>returns the resulting string HTML with span inserted inside, enclosing the TD's content</returns>
        protected string PutInsideSpan(string Html)
        {
            if (String.IsNullOrEmpty(Html))
                return Html;

            Match m = _reg.Match(Html);
            if (m != null && m.Groups.Count >= 3)
                return m.Groups[1].Captures[0].Value + "<span>" + m.Groups[2].Captures[0].Value + "</span>" + m.Groups[3].Captures[0].Value;

            return Html;
        }

        internal virtual void RenderColGroup(HtmlTextWriter writer)
        {
            //foreach (DataControlFieldCell cell in Cells)
            //    cell.Width = GetCorrectedCellWidth(cell);
            
            writer.RenderBeginTag(HtmlTextWriterTag.Colgroup);
            foreach (DataControlFieldCell cell in Cells)
            {
                if (cell.ContainingField.Visible)
                {
                    writer.Write("<col width=\"{0}\" {1} />",
                        GetCorrectedCellWidth(cell).Value.ToString(),
                        (String.IsNullOrEmpty(cell.ContainingField.HeaderStyle.CssClass) ? String.Empty : "class=\"" + cell.ContainingField.HeaderStyle.CssClass + "\"")
                        );
                }
            }
            writer.RenderEndTag();
        }

        //כאן יוגדרו תכל'ס רוחבי התאים, המתודה תועבר גם לקלאסים אחרים
        public static Unit SetDefaultColumnWidth(DataControlFieldCell cell)
        {
            Unit ColumnWidth = new Unit(0, UnitType.Pixel);
            string str = cell.ContainingField.HeaderText;
            if (str == "") { str = cell.ContainingField.AccessibleHeaderText; }
            //If no header text was found, this is probably the auto-generated line number column:
            if (str == "") { ColumnWidth = 50; }
            else
                 {
                        switch (str)
                        {
                            case "פתיחת חלון מועמד": ColumnWidth = 140; break;
                            case "מספר מועמד": ColumnWidth = 60; break;
                            case "שם פרטי": ColumnWidth = 70; break;
                            case "שם משפחה": ColumnWidth = 90; break;
                            case "טלפון נייד": ColumnWidth = 100; break;
                            case "טלפון נייד נוסף": ColumnWidth = 100; break;
                            case "טלפון בבית": ColumnWidth = 100; break;
                            case "מספר ת''ז": ColumnWidth = 100; break;
                            case "כתובת Email": ColumnWidth = 200; break;
                            case "פרטים נוספים": ColumnWidth = 700; break;
                            case "הערות": ColumnWidth = 200; break;
                            //לטבלה הפנימית, "עבודות קודמות", אנחנו לא כותבים כאן רוחב כדי שהוא יהיה דינמי
                            case "שנת התחלה": ColumnWidth = 55; break;
                            case "שנת סיום": ColumnWidth = 50; break;
                            case "שם המעסיק": ColumnWidth = 60; break;
                            case "תפקיד": ColumnWidth = 80; break;
                            case "תיאור התפקיד": ColumnWidth = 200; break;
                            //חזרה לטבלה הראשית
                            case "ארץ לידה":
                                {
                                    string ComboName = cell.ContainingField.SortExpression + "ComboBox";
                                   AjaxControlToolkit.ComboBox TheCombo = (AjaxControlToolkit.ComboBox)cell.FindControl(ComboName);
                                    int TheWidth = (int)ColumnWidth.Value - 20;
                                    //TheCombo.Attributes.Add("style", "width: 20");
                                    ColumnWidth = 110; break;
                                }
                            case "סטטוס משפחתי": ColumnWidth = 110; break;
                            case "שנת לידה": ColumnWidth = 50; break;
                            case "תאריך לידה": ColumnWidth = 110; break;
                            case "שנת עלייה": ColumnWidth = 50; break;
                            case "גיל": ColumnWidth = 110; break;
                            case "מקום מגורים": ColumnWidth = 110; break;
                            case "כתובת": ColumnWidth = 125; break;
                            case "רשיון נהיגה": ColumnWidth = 110; break;
                            case "בעל רכב": ColumnWidth = 110; break;
                        }
                 }
            return ColumnWidth;
        }

        //The original function wrote by IdeaSparx, customized by me
        protected virtual Unit GetCorrectedCellWidth(DataControlFieldCell cell)
        {
            switch (RowType)
            {
                case DataControlRowType.Header:
                case DataControlRowType.Footer:
                    if (cell.ContainingField.HeaderStyle.Width == Unit.Empty || cell.ContainingField.HeaderStyle.Width.Type == UnitType.Percentage)     //return ParentCoolGridView.DefaultColumnWidth;
                    {
                        return SetDefaultColumnWidth(cell);
                    }
                    else  {return cell.ContainingField.HeaderStyle.Width;}
                default:
                    return Width;
            }
        }

        internal virtual void RenderRow(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }

    /// <summary>     /// CoolGridViewRow represents Header, Footer or Pager of CoolGridView    /// </summary>
    public class CoolGridViewRow : BaseCoolGridViewRow
    {
        public CoolGridViewRow(int rowIndex, int dataItemIndex, DataControlRowType rowType, DataControlRowState rowState)
            : base(rowIndex, dataItemIndex, rowType, rowState)
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            switch (RowType)
            {
                case DataControlRowType.Header:
                case DataControlRowType.Footer:
                    break;
            }
        }
    }

    /// <summary>
    /// CoolGridViewFirstRow is the first row of CoolGridView. It is the header if header is visible, or the first datarow if header is not visible.
    /// </summary>
    public class CoolGridViewFirstRow : BaseCoolGridViewRow
    {
        public CoolGridViewFirstRow(int rowIndex, int dataItemIndex, DataControlRowType rowType, DataControlRowState rowState)
            : base(rowIndex, dataItemIndex, rowType, rowState)
        {           
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderColGroup(writer);
            base.Render(writer);
        }

        protected override Unit GetCorrectedCellWidth(DataControlFieldCell cell)
        {
            if (cell.ContainingField.HeaderStyle.Width == Unit.Empty || cell.ContainingField.HeaderStyle.Width.Type == UnitType.Percentage) //   //return ParentCoolGridView.DefaultColumnWidth;
            {
                //Call public method from the other class:
                return BaseCoolGridViewRow.SetDefaultColumnWidth(cell);
            }
            else     {return cell.ContainingField.HeaderStyle.Width;}
        }
    } 
}
