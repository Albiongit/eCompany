
$("#simpleSelect2").select2({
    placeholder: "Select a Statis Value",
    theme: "bootstrap4",
    allowClear: true
});


    var companyId = $("#CompanyId").text();
    $("#ajaxSelect2").select2({
        placeholder: "Select an Employee",
        theme: "bootstrap4",
        allowClear: true,
        ajax: {
            url: "/Admin/Task/Search",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                var query =
                {
                    companyId: companyId,
                    term: params.term
                };
                return query;
            },
            processResults: function (result) {
                return {
                    results: $.map(result, function (item) {

                        return {
                            id: item.id,
                            text: item.name,
                            html: '<div style="display:flex;"><div class="d-flex justify-content-center"><img src="' + item.imageUrl + '" alt="" style="height:40px;width:40px;object-fit:cover;" class="img-rounded img-responsive m-auto" /></div><div style="padding:10px" ><div style="font-size: 1.2em">' + item.name  + '</div></div></div >',

                        };
                    }),
                };
            }
        },
        templateResult: template,
        escapeMarkup: function (m) {
            return m;
        }
    });

function template(data) {
    return data.html;
}
