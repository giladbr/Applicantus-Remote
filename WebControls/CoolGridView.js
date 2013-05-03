//------------------------------------------------------------------------------
// IdeaSparx.CoolControls.Web
// Author: John Eric Sobrepena (2009)
// You can use these codes in whole or in parts without warranty.
// By using any part of this code, you agree 
// to keep this information about the author name intact.
// http://johnsobrepena.blogspot.com
//------------------------------------------------------------------------------


//CoolProperties = {GridID : "", GridContainerID : "", HeaderContainerID : "", TableContainerID : "", FooterContainerID : "", PagerContainerID : "", HiddenFieldDataID : "", FormID : "", AllowResizeColumn : ""}
function CoolGridView(CoolProperties) {
    this.HeaderContainer = null;
    this.TableContainer = null;
    this.GridContainer = null
    this.FooterContainer = null;
    this.Grid = null;
    this.PagerContainerTop = null;
    this.PagerContainerBottom = null;
    this.HiddenFieldData = null;
    this.AllowResizeColumn = CoolProperties.AllowResizeColumn;
    this.HeaderCells = new Array();
    this._CoolProperties = CoolProperties;
    this._ControlState = {ColumnWidths : [], ScrollPosition : { x:0, y:0 }};

    //always false for now. Initialize() has to run inline as well as on page onload 
    //required for working with UpdatePanel. Need to optimize this.
    this._isPageLoad = false;

    this.OnPageLoadHandler = function () {
        this._isPageLoad = true;
        this.Initialize();
        this._isPageLoad = false;
        //The following triggers an automatic resize of the GridView, like to the one in the end of OnDocMouseUpHandler.
        //I wrote it to straighten the columns, but now it seems: a) it doens't do that b) it's vital for the resizing...
        for (var i = 0; i < this.HeaderCells.length; i++) {
            var cell = this.HeaderCells[i];
            if (cell.TableColumn) this.SetColWidth(cell.TableColumn, cell.jesWidth);
            if (cell.FooterColumn) this.SetColWidth(cell.FooterColumn, cell.jesWidth);
            this.SetColWidth(cell, cell.jesWidth);
        }
    }

    //פונקציה שאני הוספתי שמשנה רוחב ספציפי של עמודה מסוימת לפי בקשה
    this.SetColumnWidth = function (ColumnName, TheWidth) {
        for (var i = 0; i < this.HeaderCells.length; i++) {
            var cell = this.HeaderCells[i];
            //חפש את שם הטור בתוך הכותרת של התא
            if (cell.innerText.indexOf(ColumnName) >= 0) {
                if (cell.TableColumn) this.SetColWidth(cell.TableColumn, TheWidth);
                if (cell.FooterColumn) this.SetColWidth(cell.FooterColumn, TheWidth);
                this.SetColWidth(cell, TheWidth);
            }
        }
    }

    //Add 100px to the DIV that contains the header and footer as preparation for scrolling
    //    הפונקציה הזאת עשתה לי בעיות עם הגריד הפנימי - כשמגדילים אותו, היא הייתה מוסיפה 100 פיקסלים בהתחלה. לא ברור לי למה צריך אותה בכלל אז פשוט ביטלתי אותה
    this.CorrectHeaderFooterEndSpacing = function() {
        //var objs = [this.HeaderContainer, this.FooterContainer];
        //for (var i = 0; i < objs.length; i++) {
        //    if (objs[i] == null) continue;
        //    var d = objs[i].getElementsByTagName("DIV");
        //    if (d != null && d.length > 0) {
        //        //var t = d[0].getElementsByTagName("TABLE");
        //        //if (t != null && t.length > 0) {
        //        //d[0].style.width = (t[0].offsetWidth + 100) + 'px';

        //            d[0].style.width = (this.TableContainer.scrollWidth + 100) + 'px';
        //        //}
        //    }
        //}
    }

    this.Initialize = function() {
        //If not yet initialized or is being excuted after ajax call then continue, else return.
        if (!this._isPageLoad && !$$$.Event.isAjax)
            return;
        //Initialize References
        this.Grid = document.getElementById(this._CoolProperties.GridID);
        this.HeaderContainer = document.getElementById(this._CoolProperties.HeaderContainerID);
        //TableContainerID is defined in CoolGridView.cs, I think
        this.TableContainer = document.getElementById(this._CoolProperties.TableContainerID);
        this.GridContainer = document.getElementById(this._CoolProperties.GridContainerID);
        this.FooterContainer = document.getElementById(this._CoolProperties.FooterContainerID);
        this.PagerContainerTop = document.getElementById(this._CoolProperties.PagerContainerID + "Top");
        this.PagerContainerBottom = document.getElementById(this._CoolProperties.PagerContainerID + "Bottom");
        this.HiddenFieldData = document.getElementById(this._CoolProperties.HiddenFieldDataID);
        this.Form = document.getElementById(this._CoolProperties.FormID);
        $$$.Event.resizeState.verticalLine = document.getElementById("lLKAopspo28lOANcaju9182ia92u");
        //The next line pushed the inner grid's header by 100px, so I commented it.
        //this.CorrectHeaderFooterEndSpacing();
        this.LoadControlStateData();
        //Register scroll handler
        if (this.TableContainer != null)
            $$$(this.TableContainer).AttachEvent('scroll', this.TableContainerScrollHandler, this);
        //$$$.AttachEvent(this.TableContainer, 'scroll', this.TableContainerScrollHandler, this);
        //Register TableContainer resize handler
        if (this.TableContainer != null)
            $$$(this.TableContainer).AttachEvent('resize', this.TableContainerResizeHandler, this);
        //Register GridContainer resize handler
        if (this.GridContainer != null)
            $$$(this.GridContainer).AttachEvent('resize', this.GridContainerResizeHandler, this);
        //Register the OnFormSubmit event handler
        $$$(this.Form).AttachEvent("submit", this.OnFormSubmit, this);
        //Check if user-resizing column is enabled

        //Initialize column resizing only if there is a header present
        if (this.AllowResizeColumn) {
            var tableWidth = 0;
            var cells = this.GetCellsOfFirstRow(this.HeaderContainer, "TH");
            var tCells = this.GetCellsOfFirstRow(this.TableContainer, "TD");
            var fCells = this.GetCellsOfFirstRow(this.FooterContainer, "TD");
            if (cells != null && cells.length > 0) {
                this.HeaderCells = new Array();
                for (var i = 0; i < cells.length; i++) {
                    this.HeaderCells[i] = cells[i];
                    this.HeaderCells[i].TableColumn = tCells && tCells[i] || null;
                    this.HeaderCells[i].FooterColumn = fCells && fCells[i] || null;
                    this.HeaderCells[i].CoolGrid = this;
                    tableWidth += (this.HeaderCells[i].jesWidth = this.GetColWidth(cells[i]) || $$$(this.HeaderCells[i]).GetWidth());
                    $$$(cells[i]).AttachEvent("mousemove", this.OnCellMouseMoveHandler);
                    $$$(cells[i]).AttachEvent("mousedown", this.OnCellMouseDownHandler);
                }
            }
        }

        $$$.Event.resizeState.originalStyle.cursor = document.body.style.cursor;
        
        //Register these events one time only
        if (!$$$.IsEventNameRegistered("98jkiopIOPULKDkjas9123")) {
            $$$(document.body).AttachEvent("mousemove", this.OnDocMouseMoveHandler);
            $$$(document.body).AttachEvent("mouseup", this.OnDocMouseUpHandler);
            $$$(document.body).AttachEvent("selectstart", this.OnDocSelectStartHandler);
            $$$.RegisterEventName("98jkiopIOPULKDkjas9123")
        }

        //TODO:For AjaxControlToolkit compatibility
        try {
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest($$$.ContextOf(this, this.AjaxToolkitBeginRequestHandler));
        } catch (ex1) { }


        this.GridContainerResizeHandler();
        this.TableContainer.scrollLeft = this._ControlState.ScrollPosition.x;
        this.TableContainer.scrollTop = this._ControlState.ScrollPosition.y;
        this.TableContainerScrollHandler();
    }

    //Get an array [] of cells of the first row in a table inside a DIV or SPAN
    //הפונקציה הזו שונתה על-ידי כדי להתאים למקרה של גריד בתוך גריד
    this.GetCellsOfFirstRow = function(container, FindWhat) {
        if (typeof container =='undefined' || container == null) return null;
        var _tables = container.getElementsByTagName("TABLE");
        if (_tables != null && _tables.length > 0) {
            var _control = _tables[0];
            var rows = _control.getElementsByTagName("TR");           //No problem with rows! אמנם הוא מחזיר המון,אבל הוא משתמש רק בראשונה, וזו החשובה
            if (rows != null && rows.length > 0) {
                if (FindWhat == "TD") {
                    //The function GetCellsOfFirstRow was run to get the TABLE cells. There might be trouble, so let's initliaze some header variables to compare with table cells:
                    if (typeof this.HeaderContainer == 'undefined' || this.HeaderContainer != null) {
                        var TheHeaderTables = this.HeaderContainer.getElementsByTagName("TABLE");
                        if (TheHeaderTables != null && TheHeaderTables.length > 0) {
                            var TheHeaderControl = TheHeaderTables[0];
                            var TheHeaderColumns = TheHeaderControl.getElementsByTagName("COL");
                            //השינוי המהותי שלי: Find the columns according to navigation in the DOM, not by tag name. Use TheHeaderColumns to know how many there should be.
                            var columns = new Array();
                            for (var k = 0; k < TheHeaderColumns.length ; k++) {
                                columns[k] = _control.children[0].children[k];
                            }
                            //כנ"ל לגבי התאים
                            var cells = new Array();
                            for (var n = 0; n < TheHeaderColumns.length ; n++) {
                                cells[n] = _control.children[1].children[0].children[n];
                            }
                        }
                    }
                }
                else // We're looking for the headers - no trouble expected. Use the original TagName way to get the columns & cells. 
                {
                    var columns = _control.getElementsByTagName("COL");
                    var cells = rows[0].getElementsByTagName("TH");
                }
            }
            }
            if (cells != null) {
                for (var i = 0; i < cells.length; i++)
                {
                    //This is the purpose of the GetCellsOfFirstRow function: return the "cells" object referenced to the right place, with the "ColObject" property set to the columns.
                    cells[i].ColObject = columns[i] || null;
                }
            }
            return cells;
    }

    //Resizing Columns Handlers
    this.OnCellMouseMoveHandler = function (e) {
        if ($$$.Event.resizeState.isResize) return false;

        var cursor = $$$.GetMousePosition(e);
        var elem = $$$(this);
        var position = elem.GetAbsolutePosition();
        position.width = elem.offsetWidth;
        position.height = elem.offsetHeight;
        //position.width=רוחב התא, position.left=מיקום הקו השמאלי של התא.
        if (cursor.x >= position.left + position.width - 15 || cursor.x <= position.left + 15) {
            this.style.cursor = 'w-resize';
        }
        else
            this.style.cursor = '';
    }

    //Resizing Columns Handlers
    this.OnCellMouseDownHandler = function(e) {
        var cursor = $$$.GetMousePosition(e);
        var elem = $$$(this);
        var position = elem.GetAbsolutePosition();
        if (cursor.x <= position.left + 15) {
            //This is a resize from the right - change the element to the th on the left
            elem = elem.previousSibling;
            position = elem.GetAbsolutePosition();
        }
        position.width = elem.offsetWidth;
        position.height = elem.offsetHeight;
        $$$.Event.resizeState.originalStyle.cursor = document.body.style.cursor;
        //Check if resizing should start
        //position.width=רוחב התא, position.left=מיקום הקו השמאלי של התא.
        if (cursor.x >= position.left + position.width - 15) {
        //Will be true also if resize came from the right: |           -15  |  cursor       Cursor.x is surely bigger than the 15px area - it's on the RIGHT side of elem's end (elem's been changed to previousSibling)
            document.body.style.cursor = 'w-resize';
            $$$.Event.resizeState.eventTarget = elem;
            $$$.Event.resizeState.originX = cursor.x;
            $$$.Event.resizeState.minX = position.left;
            $$$(document.body).DisableSelection();
            $$$.Event.resizeState.isResize = true;
            var vLine = $$$.Event.resizeState.verticalLine;
            if (vLine != null) {
                vLine.style.width = '3px';
                vLine.style.height = (elem.CoolGrid.GridContainer.offsetHeight - 2) + 'px';
                vLine.style.left = (cursor.x - 1) + 'px';
                vLine.style.top = $$$(elem.CoolGrid.GridContainer).GetAbsolutePosition().top + 'px';
                vLine.style.display = '';
            }
        }

        return false;
    }

    //Resizing Columns Handlers
    this.OnDocMouseMoveHandler = function(e) {
        //if not resizing mode then do  nothing
        if (!$$$.Event.resizeState.isResize || $$$.Event.resizeState.eventTarget == null) return;

        var cursor = $$$.GetMousePosition(e);

        var vLine = $$$.Event.resizeState.verticalLine;
        if (vLine != null) {
            vLine.style.left = (cursor.x - 1) + 'px';
        }

        return false;
    }

    //Resizing Columns Handlers
    this.OnDocMouseUpHandler = function(e) {
        //if not resizing mode then do  nothing
        if (!$$$.Event.resizeState.isResize || $$$.Event.resizeState.eventTarget == null) return;
        $$$(document.body).EnableSelection();
        document.body.style.cursor = ''; //$$$.Event.resizeState.originalStyle.cursor;
        var cursor = $$$.GetMousePosition(e);
        var totalWidth = 0;
        var width = cursor.x - $$$.Event.resizeState.minX;
        if (width <= 10)            width = 10;
        $$$.Event.resizeState.eventTarget.jesWidth = width;
        var vLine = $$$.Event.resizeState.verticalLine;
        if (vLine != null)
            vLine.style.display = 'none';
        var coolgrid = $$$.Event.resizeState.eventTarget.CoolGrid;
        //For every column, two "coolgrid.SetColWidth" are being performed:
        for (var i = 0; i < $$$.Event.resizeState.eventTarget.CoolGrid.HeaderCells.length; i++) {
            var cell = $$$.Event.resizeState.eventTarget.CoolGrid.HeaderCells[i];
            //The first will be done from the following statement:
            if (cell.TableColumn) coolgrid.SetColWidth(cell.TableColumn , cell.jesWidth);
            if (cell.FooterColumn) coolgrid.SetColWidth(cell.FooterColumn, cell.jesWidth);
            // And the second from the following statement:
            coolgrid.SetColWidth(cell , cell.jesWidth);
        }

        $$$.Event.resizeState.eventTarget.CoolGrid.CorrectHeaderFooterEndSpacing();
        $$$.Event.resizeState.eventTarget.CoolGrid.GridContainerResizeHandler();
        $$$.Event.resizeState.eventTarget.CoolGrid.TableContainerScrollHandler();
        $$$.Event.resizeState.isResize = false;
    }

    //Resizing Columns Handlers
    this.OnDocMouseDownHandler = function(e) {
        return false;
    }

    //Resizing Columns Handlers
    this.OnDocSelectStartHandler = function(e) {
        return !$$$.Event.resizeState.isResize;
    }

    this.SetColWidth = function(THorTDObject, value) {
        if (typeof THorTDObject != 'undefined' && THorTDObject != null) {
            
            if (typeof THorTDObject.ColObject != 'undefined' && THorTDObject.ColObject != null) {
                THorTDObject.ColObject.width = value;
            } else {
                THorTDObject.style.width = value + 'px';
            }
        }
    }

    this.GetColWidth = function(THorTDObject) {
        if (typeof THorTDObject != 'undefined' && THorTDObject != null) {
            if (typeof THorTDObject.ColObject != 'undefined' && THorTDObject.ColObject != null) {
                return THorTDObject.ColObject.width;
            }

            return $$$(THorTDObject).GetWidth();
        }
        return 0;
    }

    this.GetAjaxToolkitEndRequestHandler = function() {
        // return the function to run this.AjaxToolkitEndRequestHandler to run in "this" context
        return $$$.ContextOf(this, this.AjaxToolkitEndRequestHandler);
    }

    //Compatibility with AjaxControlToolkit
    this.AjaxToolkitBeginRequestHandler = function(sender, args) {
        try {
            //TODO: This is a work-around with Ajax Control Toolkit that modify the request body to inject the new value of the hidden field.
            //TODO: Blog about how AjaxControlToolkit ignore assigned values in hidden field if assignment is done in initializeRequest and beginRequest event. This hack works.
            var $body = args.get_request().get_body();
            this.OnFormSubmit();
            var $p = '&' + this.HiddenFieldData.id + '=' + escape(this.HiddenFieldData.value);
            var $pat = '&' + this.HiddenFieldData.id + '=[^&]*';
            $body = $body.replace(new RegExp($pat), $p);
            args.get_request().set_body($body);
        } catch (ex1) { }
    }

    this.AjaxToolkitEndRequestHandler = function(sender, args) {
        this.Initialize();
    }

    //ScrollHandler to synchronize grid content's and header content's scrolling
    this.TableContainerScrollHandler = function() {   
        if (this.HeaderContainer != null) {
            if ($$$.IsBrowserIE6)
                this.HeaderContainer.children[0].style.marginLeft = '-' + this.TableContainer.scrollLeft + 'px';
            else
                this.HeaderContainer.scrollLeft = this.TableContainer.scrollLeft;
        }
        if (this.FooterContainer != null) {
            if ($$$.IsBrowserIE6)
                this.FooterContainer.children[0].style.marginLeft = '-' + this.TableContainer.scrollLeft + 'px';
            else
                this.FooterContainer.scrollLeft = this.TableContainer.scrollLeft;
        }
    }

    //Handle the TableContainer resize event
    this.TableContainerResizeHandler = function() {
        this.TableContainerScrollHandler();
    }

    this.GridContainerResizeHandler = function() {
        if (this.GridContainer == null || this.GridContainer.style.height == '')
            return;

        var _height = this.GridContainer.clientHeight;

        //compatibility with IE6 and IE7
        if (_height + this.GridContainer.offsetTop <= this.GridContainer.clientHeight)
            _height += this.GridContainer.offsetTop;

        if (this.HeaderContainer != null)
            _height -= this.HeaderContainer.offsetHeight;
        if (this.FooterContainer != null)
            _height -= this.FooterContainer.offsetHeight;
        if (this.PagerContainerTop != null)
            _height -= this.PagerContainerTop.offsetHeight;
        if (this.PagerContainerBottom != null)
            _height -= this.PagerContainerBottom.offsetHeight;
        if (_height > 0)
            this.TableContainer.style.height = _height.toString() + 'px';
    }

    this.UpdateControlStateData = function() {
        var json = '{ColumnWidths : ';
        var s1 = '';
        for (var i = 0; i < this.HeaderCells.length; i++) {
            var h = this.HeaderCells[i];
            s1 += (s1.length > 0 ? ',' : '') + ((h.ColObject && h.ColObject.width) || h.jesWidth || h.offsetWidth);
        }
        json += '[' + s1 + ']';
        json += ', ScrollPosition : { x: ' + this.TableContainer.scrollLeft + ', y: ' + this.TableContainer.scrollTop + '}';
        json += '}';

        this.HiddenFieldData.value = json;
    }
    
    this.LoadControlStateData = function()
    {
        try{
            var state = eval("[" + this.HiddenFieldData.value + "]");
            if (typeof state != 'undefined' && state != null && state.length > 0)
                this._ControlState = state[0];
        }catch(e){ }
    }

    //Event handler for form submission
    this.OnFormSubmit = function() {
        this._ControlState.ScrollPosition.x = this.TableContainer.scrollLeft;
        this._ControlState.ScrollPosition.y = this.TableContainer.scrollTop;
        //update control state data of the column changes
        this.UpdateControlStateData();
    }

    //Attach an eventhandler for on load
    $$$.AttachEvent(window, 'load', this.OnPageLoadHandler, this);
}