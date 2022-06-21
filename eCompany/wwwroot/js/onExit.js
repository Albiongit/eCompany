
window.onbeforeunload = function () {
    var table = $("#tblData").DataTable();
    table.state.clear();
    table.destroy();
}