$(document)
    .ready(function () {
        $('#historyTable')
            .DataTable({
                "lengthMenu": [[10, 25, 50, "ALL"], [10, 25, 50, -1]],
                "ordering": false
            });
    });