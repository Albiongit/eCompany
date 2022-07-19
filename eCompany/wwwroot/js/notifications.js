

$(document).ready(function () {
    $.ajax({
        url: "/Customer/Task/GetNewTasks",
        type: 'GET',
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.length != 0) {
                $.each(result, function (index, emp) {
                    $('#notification').append("<a class=\"dropdown-item text-light\" href=\"/Customer/Task/Details?taskId="+emp.taskId+"\">" + emp.title + "</a>");
                });
            }
            else {
                $('#notification').append("<a class=\"dropdown-item text-light\" href=\"#\" onclick = \"javascript: return false;\">You do not have any new task</a>");
            }
        }
    })
})