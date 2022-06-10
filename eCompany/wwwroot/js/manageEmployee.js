﻿var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var role = "";
    if (url.includes("employees")) {
        role = "Employee"
    } else if (url.includes("admins")) {
        role = "Company Admin"
    } else {
        role = "";
    }


    loadDataTable(role);
    
});


function loadDataTable(role) {
    var id = $('#tblData').attr('data-id');
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Manage/GetEmployeeList",
            "type": "POST",
            "data": {id : id, status : role}
        },
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            { "data": "name", "name": "Name", "width": "15%" },
            { "data": "email", "name": "Email", "width": "20%" },
            { "data": "phoneNumber", "name": "PhoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data, data2, row) {
                    if (row.role == "Employee") {
                        return `
                        
                        <div class="w-100 btn-group" role="group">
                        
                        <a href="/Admin/Task/CreateTask?companyId=${id}&id=${row.id}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Assign New Task</a>
                        
                        </div>
                            `
                    } else {
                        return ``
                    }
                },
                "width": "20%"
            },
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