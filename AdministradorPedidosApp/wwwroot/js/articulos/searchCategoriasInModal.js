// Script de busqueda en Modal de categorias.
$(document).ready(function () {
    $("#categoriaSearch").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#categoriasTableBody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});

// Script de busqueda en Modal de categorias.
$(document).ready(function () {
    $("#articulosSearchIndex").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#articulosIndexTableBody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});

$(document).ready(function () {
    var rowCount = $('#categoriasTableBody tr').length;
    if (rowCount > 5) {
        $('#categoriasTableContainer').css('max-height', '400px');
        $('#categoriasTableContainer').css('overflow-y', 'auto');
    } else {
        $('#categoriasTableContainer').css('max-height', '');
        $('#categoriasTableContainer').css('overflow-y', '');
    }
});
