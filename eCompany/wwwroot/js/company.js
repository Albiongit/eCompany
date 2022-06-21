var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetCompanyList",
            "type": "POST"
        },
        "stateSave": "true",
        "proccesing": "true",
        "serverSide": "true",
        "filter": "true",
        "columns": [
            {"data": "companyName", "name": "CompanyName", "width": "20%"},
            {"data": "companyPhone", "name": "CompanyPhone", "width": "20%"},
            {"data": "companyState", "name": "CompanyState", "width": "20%"},
            {"data": "companyWeb", "name": "CompanyWeb", "width": "20%" },
            {
                "data": "companyId",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Manage/Index?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Manage</a>
                        <a onClick=Delete('/Admin/Company/Delete/${data}')
                        class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                            `
                },
                "width": "20%"
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