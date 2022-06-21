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
            "url": "/Customer/Task/GetTaskList",
            "type": "POST",
            "data": { id: id, status: status }
        },
        "stateSave": "true",
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            { "data": "taskId", "name": "TaskId", "width": "10%" },
            { "data": "title", "name": "Title", "width": "20%" },
            { "data": "employeeName", "name": "EmployeeName", "width": "20%" },
            { "data": "dueDateTask", "name": "DueDateTask", "width": "15%" },
            { "data": "status", "name": "Status", "width": "15%" },
            {
                "data": "taskId",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Customer/Task/Details?taskId=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Open Task</a>
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
            redrawAfterDelete($('#tblData').DataTable());
        }
    })
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