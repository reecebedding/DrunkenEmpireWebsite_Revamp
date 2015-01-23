function AddQuestion() {
    //Make the add request
    $.ajax({
        url: "AddQuestion/",
        type: "POST",
        //Data contains the form in a serialized state
        data: $("#AddQuestionCont form").serialize()
    }).done(function (result) {
        //Deserialize the whole JSON result
        var resultObj = JSON.parse(result);
        //If there are any validation error fields visible (From previous process), make them invisible (Valid)
        $("#AddQuestionCont form span[class='field-validation-error']").attr("class", "field-validation-valid");
        //If the insert was successful
        if (resultObj == true) {
            //Change the content of the validation success summary div
            $("#AddQuestionCont div[class='validation-summary-valid-message']").html("<ul><li>Question was successfully added.</li></ul>");
            //Refresh the questions list
            $.get("GetQuestionListView", function (data) {
                $("#QuestionlistCont").empty().html(data);
            });
        }

        if (resultObj.ModelErrors) {
            //Deserialize the error object
            var errors = JSON.parse(resultObj.ModelErrors);
            //If any errors are present, show them using the respective validation spans
            if (errors.length > 0) {
                for (var i = 0; i < errors.length; i++) {
                    //Change the spans text to the error message
                    $("#AddQuestionCont form span[data-valmsg-for='" + errors[i].Key + "']").text("'" + JSON.parse(errors[i].Value).Errors[0].ErrorMessage + "'");
                    //Change the class from validation valid to validation error.
                    $("#AddQuestionCont form span[data-valmsg-for='" + errors[i].Key + "']").attr("class", "field-validation-error")
                }
            }
        }
        else
        {
            $("#AddQuestionCont").hide(600);
            $("#AddQuestionButton").show();
        }

        
    });
}

function EditQuestion() {
    debugger;
    //Make the edit request
    $.ajax({
        url: "EditQuestion/",
        type: "POST",
        //Data contains the form in a serialized state
        data: $("#EditQuestionCont form").serialize()
    }).done(function (result) {
        debugger;
        //Deserialize the whole JSon result
        var resultObj = JSON.parse(result);
        //If there are any validaton error fields visible (From previous process), make them invisible (Valid)
        $("#EditQuestionCont form span[class='field-validation-error']").attr("class", "field-validation-valid");
        //If the update was successfull, show this
        if (resultObj == true) {
            //Change the content of the Validation success summary div
            $("#EditQuestionCont div[class='validation-summary-valid-message']").html("<ul><li>Question was successfully updated.</li></ul>");
            //Refresh the questions list
            $.get("GetQuestionListView", function (data) {
                $("#QuestionlistCont").empty().html(data);
            });
        }

        if (resultObj.ModelErrors) {
            //Deserialize the error object
            var errors = JSON.parse(resultObj.ModelErrors);
            //If any errors are present, show them using the respective validation spans
            if (errors.length > 0) {
                for (var i = 0; i < errors.length; i++) {
                    //Change the spans text to the error message
                    $("#EditQuestionCont form span[data-valmsg-for='" + errors[i].Key + "']").text("'" + JSON.parse(errors[i].Value).Errors[0].ErrorMessage + "'");
                    //Change the class from validation valid to validation error.
                    $("#EditQuestionCont form span[data-valmsg-for='" + errors[i].Key + "']").attr("class", "field-validation-error")
                }
            }
        }        
        else
        {
            $("#EditQuestionCont").hide(600);
        }
    });
}

//Takes an id of question and retrieves a Json version of it, and loads it into the respective Edit fields.
function LoadEditQuestionData(id) {
    $.ajax({
        url: "GetQuestion/",
        data: "id=" + id,
        dataType: "json"
    }).done(function (result) {
        //Populate fields.
        $("#EditQuestionCont").show(600);

        $("#EditQuestionCont #ID").val(result.ID);
        $("#EditQuestionCont #Description").val(result.Description);
        $("#EditQuestionCont #DataType").val(result.DataType);
        $("#EditQuestionCont #Active").prop('checked', result.Active);
    });
}

function AddAPISectionToApplication() {
    debugger;
    var apiCount = $('#ApplicationAPIKeys .APIKey').length;
    $("#ApplicationAPIKeys").append(
    "<div class='APIKey' id='APIKey_" + apiCount + "'><p>KeyID : <input id='apiKeys_0__KeyID' name='apiKeys[" + apiCount + "].KeyID' type='text'><br /> vCode : <input id='apiKeys_0__VCode' name='apiKeys[" + apiCount + "].VCode' type='text'></p></div>"
    );
}

function EditKeyWords() {
    debugger;
    //Make the edit request
    $.ajax({
        url: "EditKeywords/",
        type: "POST",
        //Data contains the form in a serialized state
        data: $("#EditKeywordsCont form").serialize()
    }).done(function (result) {
        debugger;
        //Deserialize the whole JSon result
        var resultObj = JSON.parse(result);
        //If there are any validaton error fields visible (From previous process), make them invisible (Valid)
        $("#EditKeywordsCont form span[class='field-validation-error']").attr("class", "field-validation-valid");
        //If the update was successfull, show this
        if (resultObj == true) {
            //Change the content of the Validation success summary div
            $("#EditKeywordsCont div[class='validation-summary-valid-message']").html("<ul><li>Keywords were successfully updated.</li></ul>");
            //Refresh the questions list
            $.get("GetKeyWordsListView", function (data) {
                $("#BackgroundCheckEveMailKeyWords").empty().html(data);
            });
        }
        else {
            $("#EditKeywordsCont div[class='validation-summary-valid-message']").html("<ul><li>Keywords were unable to be updated. Try again or contact an admin.</li></ul>");
        }
    });
}

function ShowAddQuestionCont() {
    $("#AddQuestionButton").hide();
    $("#AddQuestionCont").show(600);
}
