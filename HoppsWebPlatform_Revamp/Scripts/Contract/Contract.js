var contractContentArray;
var itemCounter = 0;

$(document).ready(function () {
    $.get("GetContractCatalogueItems", function (data) {
        contractContentArray = JSON.parse(JSON.parse(data).Values);
        contractContentArray.unshift({ ItemID: -1, ItemName: "" });
    });
});

function AddNewItem() {
    
    debugger;
    itemCounter = $("#ItemList li").length;

    var newSelectName = 'ContractItem[' + itemCounter + '].ItemID';
    $("#ItemList").append("<li><select name='" + newSelectName + "' id='" + newSelectName + "'></select>" + "<input type='text' name='ContractItem[" + itemCounter + "].Quantity'></input></li>");
   
    $.each(contractContentArray, function (key, value) {
        debugger;
        $(document.getElementById(newSelectName)).append("<option value='" + value.ItemID + "'>" + value.ItemName + "</option");
    });
    

    itemCounter = itemCounter + 1;
}
