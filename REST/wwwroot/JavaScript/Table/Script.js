var Table = $('#DT_Table').DataTable({
    ajax: {
        url: '/Table/GetTable',
        type: 'POST',
        dataType: 'json',
    },
    columns: [
        { data: "tableId" },
        { data: "tableName" },
        { data: "description" },
        { data: "status" },
        {
            data: null,
            "render": function (data, type, row) {
                return '<button class="btn btn-primary btn-sm"><i class="fas fa-arrow-alt-circle-right"></i></button>';
            }
        },
    ],
    info : false,
    ordering: false,
});