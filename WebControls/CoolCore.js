//------------------------------------------------------------------------------
// IdeaSparx.CoolControls.Web
// Author: John Eric Sobrepena (2009)
// You can use these codes in whole or in parts without warranty.
// By using any part of this code, you agree 
// to keep this information about the author name intact.
// http://johnsobrepena.blogspot.com
//------------------------------------------------------------------------------


var $$$ = function(element) {

    //Check if the parameter is a DOM element or a string ID of the DOM element
    if (typeof element == 'String')
        element = document.getElementById(elementId);
    else
        element = element;

    if (typeof element != 'undefined'
        && element != null
        && typeof element.$elementExtenderID != 'undefined'
        && element.$elementExtenderID != null
        && element.$elementExtenderID == "jesCoolXoiopq98012iPOqw1908")
            return element;


    return $$$.Extend(element, {
        //Marker that element is extended
        $elementExtenderID: "jesCoolXoiopq98012iPOqw1908",

        //Attaches an event handler to the "element"
        AttachEvent: function(EventString, Handler, Context) {
            $$$.AttachEvent(element, EventString, Handler, Context);
        },

        GetWidth: function() {
            if (element.style.width != "")
                return element.style.width.replace('px', '');
            return element.offsetWidth;
        },

        GetHeight: function() {
            if (element.style.width != "")
                return element.style.height.replace('px', '');
            return element.offsetHeight;
        },        

        GetAbsolutePosition: function() {
            var e = element;
            //--- Determine real "element" position relative to the main page.
            var ElementLeft = e.offsetLeft;
            var ElementTop = e.offsetTop;
            var TmpElem = e.offsetParent;
            while (TmpElem != null) {
                ElementLeft += TmpElem.offsetLeft;
                ElementTop += TmpElem.offsetTop;
                TmpElem = TmpElem.offsetParent;
            }

            var TmpElem = e.parentElement || e.parentNode;
            while (TmpElem != null && TmpElem != document.body) {
                ElementLeft -= TmpElem.scrollLeft;
                ElementTop -= TmpElem.scrollTop;
                TmpElem = TmpElem.parentElement || TmpElem.parentNode;
            }

            return { top: ElementTop, left: ElementLeft };
        },
        //Get children
        GetChildren: function() {
            if (element.children)
                return element.children;
            else
                return element.childNodes;
        },
        //Disable text selection in the page
        DisableSelection: function() {
            if (typeof element.style.MozUserSelect != "undefined") //Firefox route
                this.style.MozUserSelect = "none"
            else if (typeof element.style.webkitUserSelect != "undefied") //WebKit Chrome Safari branch
                this.style.webkitUserSelect = "none";
        },
        //Enable text selection in the page
        EnableSelection: function() {
            if (typeof element.style.MozUserSelect != "undefined") //Firefox route
                this.style.MozUserSelect = ""
            else if (typeof element.style.webkitUserSelect != "undefied") //WebKit Chrome Safari branch
                this.style.webkitUserSelect = "";
        },
        //Get the first child element with tagname specified
        FirstChildWithTagName: function(tagName) {
            var children = element.getElementsByTagName(tagName);
            if (children != null && children.length > 0)
                return children[1];
            return null;
        }
    });
}

$$$.Extend = function(target, varExtensions) {
    for (var name in varExtensions) {
        if (target[name] == 'undefined' || target[name] == null || target[name] != varExtensions[name])
            target[name] = varExtensions[name];
    }
    
    return target;
}

//Set the globally common functions $$$.fn
$$$.fn =
    {
        Event: {
            resizeState: {
                eventTarget: null,
                isResize: false,
                originX: 0,
                minX: 0,
                originalStyle: { cursor: '' },
                verticalLine: null,
                count: 0
            },
            isAjax: false,
            eventHandlers: []
        },
        //cross-browser attach event.
        //Element - is the object where eventhandler will be attached to
        //EventString    - is the event name to handle of Element
        //Handler is a function pointing to the event handler
        //Context is what context the Handler will be executed
        AttachEvent: function(Element, EventString, Handler, Context) {
            var isAttached = true;
            if (typeof Handler == 'undefined' || Handler == null) return;
            if (typeof (Context) != 'undefined' && Context != null) {
                if (Element.addEventListener) // W3C DOM
                    Element.addEventListener(EventString, Handler = $$$.ContextOf(Context, Handler), false);
                else if (Element.attachEvent) // IE DOM
                    Element.attachEvent('on' + EventString, Handler = $$$.ContextOf(Context, Handler));
                else {
                    isAttached = false;
                    alert('Unable to attach to event ' + EventString + ' of ' + Element.toString());
                }
            } else {
                if (Element.addEventListener) // W3C DOM
                    Element.addEventListener(EventString, Handler, false);
                else if (Element.attachEvent) // IE DOM
                    Element.attachEvent('on' + EventString, Handler = $$$.ContextOf(Element, Handler));
                else {
                    isAttached = false;
                    alert('Unable to attach to event ' + EventString + ' of ' + Element.toString());
                }
            }

            return Handler;
        },

        RegisterEventName: function(EventName) {
            if (!$$$.IsEventNameRegistered(EventName))
                $$$.Event.eventHandlers[$$$.Event.eventHandlers.length] = EventName;
        },

        IsEventNameRegistered: function(EventName) {
            for (var i = 0; i < $$$.Event.eventHandlers.length; i++) {
                if (EventName == $$$.Event.eventHandlers[i])
                    return true;
            }
            return false;
        },

        RemoveEvent: function(Element, EventString, Handler) {
            if (Element.removeEventListener) // W3C DOM
                Element.removeEventListener(EventString, Handler, false);
            else if (Element.detachEvent) // IE DOM
                Element.detachEvent('on' + EventString, Handler);
            else
                alert('Unable to remove event ' + EventString + ' of ' + Element.toString());
        },

        //Returns a function that executes [Method] in the context of [ThisObj]
        ContextOf: function(ThisObj, Method) {
            return function() { return Method.apply(ThisObj, arguments); };
        },

        GetMousePosition: function(evt) {
            var e = evt || window.event;
            var cursor = { x: 0, y: 0 };
            if (e.pageX || e.pageY) {
                cursor.x = e.pageX;
                cursor.y = e.pageY;
            }
            else {
                cursor.x = e.clientX +
                (document.documentElement.scrollLeft ||
                document.body.scrollLeft) -
                document.documentElement.clientLeft;
                cursor.y = e.clientY +
                (document.documentElement.scrollTop ||
                document.body.scrollTop) -
                document.documentElement.clientTop;
            }
            return cursor;
        },

        AjaxToolkitEndRequestHandler: function(sender, args) {
            var p = sender._updatePanelClientIDs;
            if (p != null) {
                for (var j = 0; j < p.length; j++) {
                    var scripts = $get(p[j]).getElementsByTagName("script");
                    $$$.Event.isAjax = true;
                    // .text is necessary for IE.
                    for (var i = 0; i < scripts.length; i++) {
                        try {
                            eval(scripts[i].innerHTML || scripts[i].text);
                        } catch (e2) { }
                    }
                    $$$.Event.isAjax = false;
                }
            }
        },

        _OnLoad: function(evt) {
            try {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest($$$.AjaxToolkitEndRequestHandler);
            }
            catch (e) { }
        },

        IsDefined: function(obj) {
            return (typeof obj != 'undefined' && obj != null);
        },

        /*@cc_on
        @if (@_jscript_version <= 5.6)
        IsBrowserIE6: true
        @else @*/
            IsBrowserIE6 : false
        /*@end
        @*/
    }

//Extend the $$$ from $$$.fn
$$$ = $$$.Extend($$$, $$$.fn);
$$$(window).AttachEvent('load', $$$._OnLoad);
