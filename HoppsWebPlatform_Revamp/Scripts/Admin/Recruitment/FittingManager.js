function SetFittingStatus(id) {    
    var status = $("#Fitting-" + id + " input#active").is(':checked');
    $.ajax({
        url: "SetFittingStatus/",
        type: "POST",
        data: "id=" + id + "&status=" + status
    });
}