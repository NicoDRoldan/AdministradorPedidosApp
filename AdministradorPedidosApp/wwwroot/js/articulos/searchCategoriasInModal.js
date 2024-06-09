// Script de busqueda en Modal de categorias.
$(document).ready(function () {
    $("#categoriaSearch").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#categoriasTableBody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});