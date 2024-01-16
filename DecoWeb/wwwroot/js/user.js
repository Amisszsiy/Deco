var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'firstName', "width": "15%" },
            { data: 'lastName', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'hotel.name', "width": "15%" },
            { data: 'role', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/admin/user/RoleManager?userId=${data}" class="btn btn-dark mx-2">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                            </div>`
                },
                "width": "10%"
            }
        ]
    });
}