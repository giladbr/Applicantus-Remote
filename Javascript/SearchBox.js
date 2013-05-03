
function OnKeyUp(value)
{
    ClearForbidden(value);
    $(".GridStyle").unhighlight();
    if (value != "") {
        $(".GridStyle").highlight(value);
        //Special highlighting for TextAreas:
        $(".TextArea").unhighlight();
        $("TextArea").highlightTextarea({ words: [value] });

        $(".GridFooter").unhighlight();
        $(".GridHeader").unhighlight();
    }
};

function ClearForbidden (value)
{
     var ForbiddenRegEx = /[\!\#\$\%\^\&\*\(\)\;]/g;
    var field = document.getElementById('SearchBox');
    if (ForbiddenRegEx.test(value)) {
        field.value = value.replace(ForbiddenRegEx, "");
        document.getElementById('SearchBoxError').innerHTML = "אין להכניס את התו ''" + value.charAt(value.length - 1) + "'' ולכן הוא נמחק";
        document.getElementById('SearchBoxError').focus;
    }
}

function OnKeyDown(value)
{
    ClearForbidden(value);
    document.getElementById('SearchBoxError').innerHTML = document.getElementById('ctl00_MainContent_SearchBoxErrorAsp').innerHTML = "";
}