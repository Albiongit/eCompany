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
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            { "data": "taskId", "name": "TaskId", "width": "10%" },
            { "data": "title", "name": "Title", "width": "25%" },
            { "data": "employeeName", "name": "EmployeeName", "width": "25%" },
            { "data": "status", "name": "Status", "width": "15%" },
            {
                "data": "taskId",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Task/Details?taskId=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Details</a>
                        <a onClick=Delete('/Admin/Task/Delete?taskId=${data}')
                        class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
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
                success: function (data) {
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
            location.reload(true);
        }
    })
}