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
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Employee/GetEmployeeList",
            "type": "POST",
            "data": {status: role }
        },
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            {"data": "name", "name": "Name", "width": "10%"},
            {"data": "sex", "name": "Sex", "width": "10%"},
            {"data": "email", "name": "Email", "width": "20%"},
            {"data": "phoneNumber", "name": "PhoneNumber", "width": "10%" },
            {"data": "state", "name": "State","width": "10%" },
            {"data": "city", "name": "City","width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Employee/Update?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a onClick=Delete('/Admin/Employee/Delete?userId=${data}')
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
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}