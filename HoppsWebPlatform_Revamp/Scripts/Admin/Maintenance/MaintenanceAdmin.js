function RefreshData(button) {
    $(button).text('Refreshing');
    debugger;
    $.ajax({
        url: $(button).val(),
        type: 'POST'
    }).done(function (result) {
        $(button).text('Refresh');
        alert('Refresh Complete');
    });
}

setInterval(function () {
    //Make the add request
    $.ajax({
        url: "GetEventLogs",
        type: "GET"
    }).done(function (result) {
        debugger;
        $("#logContainer").html(result);
    })
}, 5000);