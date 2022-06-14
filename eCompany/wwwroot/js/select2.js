
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
                            text: item.name
                        };
                    }),
                };
            }
        }
    });
