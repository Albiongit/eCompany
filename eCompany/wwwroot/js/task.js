"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/taskHub").build();
connection.start();

var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var status = "";
    if (url.includes("New")) {
        status = "New"
    } else if (url.includes("Active")) {
        status = "Active"
    } else if (url.includes("Problem")) {
        status = "Problem";
    } else if (url.includes("Rejected")) {
        status = "Rejected";
    } else if (url.includes("Done")) {
        status = "Done";
    } else {
        status = "";
    }


    loadDataTable(status);
    
    

});


function loadDataTable(status) {
    var id = $('#tblData').attr('data-id');
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Task/GetTaskList",
            "type": "POST",
            "data": { id: id, status: status }
        },
        "stateSave": "true",
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            { "data": "taskId", "name": "TaskId", "width": "10%" },
            { "data": "title", "name": "Title", "width": "25%" },
            { "data": "employeeName", "name": "EmployeeName", "width": "25%" },
            { "data": "statusInfo", "name": "Status", "width": "15%"},
            {
                "data": "taskId",
                "render": function (data, data2, row) {
                    return `
                        <div class="w-100 btn-group" role="group">
                        <a href="/Admin/Task/Details?taskId=${data}"
                        class="btn btn-primary mx-2" style="border-radius:5px;"> <i class="bi bi-pencil-square"></i> Details</a>
                        <a onClick=Delete('/Admin/Task/Delete?taskId=${data}&id=${row.employeeId}')
                        class="btn btn-danger mx-2" style="border-radius:5px;"> <i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                            `
                },
                "width": "30%"
            }
        ]
    })
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: "Yes, delete it!"
    }).then((willDelete) => {
        if (willDelete.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function(data) {
                    if (data.success) {
                        
                                                
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                        loadDataTable(status);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
            var id = url.substring(url.lastIndexOf('=') + 1);
            onDeleteTask(id);
            redrawAfterDelete($('#tblData').DataTable());
            
        }
    })
};

function onDeleteTask(id)
{
    
    connection.invoke("DeleteTask", id);
};

function redrawAfterDelete(tableToRedraw) {
    var info = tableToRedraw.page.info();

    if (info.page > 0) {
        // when we are in the second page or above
        if (info.recordsTotal - 1 > info.page * info.length) {
            // after removing 1 from the total, there are still more elements
            // than the previous page capacity 
            location.reload(null, false);
        } else {
            // there are less elements, so we navigate to the previous page
            tableToRedraw.page('previous').draw('page');
            /*location.reload(null, false);*/
        }
    }
}