﻿var dataTable;

$(document).ready(function () {

    loadDataTable();
    
});


function loadDataTable() {
    var id = $('#tblData').attr('data-id');
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Manage/GetAll",
            "data": {id : id}
        },
        "columns": [
            {"data": "name", "width": "10%"},
            {"data": "sex", "width": "10%"},
            {"data": "email", "width": "20%"},
            {"data": "phoneNumber", "width": "10%" },
            {"data": "state", "width": "10%" },
            {"data": "city", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Manage/UpdateEmployee?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/Manage/Delete?userId=${data}&id=${id}')
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
        confirmButtonText:  "Yes, delete it!"
    }).then((willDelete) => {
        if (willDelete.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                        location.reload(true);
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