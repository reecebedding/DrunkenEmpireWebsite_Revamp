function ShowMail(mailid) {
    debugger;
    var buttonText = $("#mailBodyButton_" + mailid).html();
    if (buttonText == "Hide Mail") {
        $("#mailBodyButton_" + mailid).html("Show Mail");
    }
    else {
        $(".BackgroundMailBody").hide(200);
        $("#mailBodyButton_" + mailid).html("Hide Mail");
    }
    var element = $("#mail_" + mailid);
    element.toggle(200);

}

function SendEveMailAndFinalize(value) {
    //1 = Accepted, 2 = Declined, 3 = Escalated
    debugger;
    var urlString = "";
    var send = false;
    var appID = $("#applicationID").attr('value');
    switch (value)
    {
        case 1 :
            urlString = "GetConfirmationEVEMail/";
            send = true;
        break;
        case 2 :
            urlString = "GetRejectionEVEMail/";
            send = true;
        break;
    }
    if (send) {
        $.ajax({
            url: urlString,
            data: "appID=" + appID,
            dataType: "json",
            async: false
        }).done(function (result) {
            try {
                CCPEVE.sendMail(result.ToPilotIdList[0], result.Subject, result.Body);
            } catch (e) {
                prompt("Remember To Copy, Paste And Send This Message To The Recruit!", result.Body);
            }
        });
    }
}