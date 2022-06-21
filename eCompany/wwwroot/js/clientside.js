"use strict";


var currentEmployeeId = document.getElementById("CurrentEmployeeId").textContent;
var connection = new signalR.HubConnectionBuilder().withUrl("/taskHub").build();
connection.start();
this.connection.serverTimeoutInMilliseconds = 1000 * 1000;

connection.on("ReceiveId", function (employeeId) {
    if (employeeId == currentEmployeeId) {
        document.location.reload(true);
    }
});


connection.on("ReceiveDelete", function (id) {

    if (currentEmployeeId == id) {
        document.location.reload(true);
    }
    
});