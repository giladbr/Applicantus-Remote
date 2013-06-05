$(document).ready(function () {
    $.validator.setDefaults({
        errorElement: "div",
        errorPlacement: function (error, element) {
            debugger;
            errorClass: "error",
            error.prepend('<em/>'),
            error.insertAfter(element)
        },
    });
  
})

    //    var validator = $("#aspnetForm").validate({
    $(document).on('mouseenter','.CellInput', function (event) {
        this.title = "לחץ כדי לערוך";  //Show tooltip
        this.style.border = "1px solid gray";
    })

    $(document).on('mouseleave','.CellInput',function (event) {
        this.style.border = "none";
    })
 
    $(document).on('click','.CellInput',function () {
        this.title = ""; //Clear tooltip
        this.style.backgroundColor = "yellow";
    })

    $(document).on('blur', '.CellInput', function () {
        this.style.backgroundColor = "transparent";
        this.style.fontSize = "10pt";
        this.style.border = "none";
    })

    $(document).on('change', '.CellInput', function () {
        debugger;
        $("#aspnetForm").validate({
            rules: {
                EmailInput: { email: true },
                //CellPhoneInput: {email:true}
            },
            messages: { EmailInput: { email: "נא להכניס כתובת Email תקינה" }, },
            success: function () {
                $(this).parent().append('<TextArea rows="2" class="ChangesSaved" readOnly="true">השינוי נשמר</TextArea>');
                //Post all necessary data to the server, and it will update the SQL on Page_Load
                //$.post("#", { RequestType: "UpdateSQL", ApplicantID: ApplicantID, FieldName: FieldName, FieldData: this.innerText });
                //Show "השינוי נשמר" message for 2 seconds:
                var TheCellContent = this;
                setTimeout(function () {
                    if (TheCellContent.nextElementSibling.innerHTML == "השינוי נשמר") {
                        TheCellContent.nextElementSibling.remove();
                    }
                }, 2000);
                //שורת ניסיון
            }
        })
    })
        //debugger;
        //if ($(".CellInput").valid()) {
        //        $(this).parent().append('<TextArea rows="2" class="ChangesSaved" readOnly="true">השינוי נשמר</TextArea>');
        //        //Post all necessary data to the server, and it will update the SQL on Page_Load
        //        //$.post("#", { RequestType: "UpdateSQL", ApplicantID: ApplicantID, FieldName: FieldName, FieldData: this.innerText });
        //        //Show "השינוי נשמר" message for 2 seconds:
        //        var TheCellContent = this;
        //        setTimeout(function () {
        //            if (TheCellContent.nextElementSibling.innerHTML == "השינוי נשמר") {
        //                TheCellContent.nextElementSibling.remove();
        //            }
        //        }, 2000);
        //} //of "validator" if