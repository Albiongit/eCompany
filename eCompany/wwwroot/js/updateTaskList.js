"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/taskHub").build();
connection.start();




document.getElementById("registerSubmit").addEventListener("click", function (event) {
    var employeeId = document.getElementById("EmployeeId").textContent;
    if (employeeId == "") {
        var e = document.getElementById("ajaxSelect2");
        employeeId = e.options[e.selectedIndex].value;
        connection.invoke("UpdateTaskTable", employeeId);
    }
    else {
        connection.invoke("UpdateTaskTable", employeeId);
    }
    
});
