document.addEventListener('DOMContentLoaded', function () {
    var tipoCuponSelect = document.getElementById('tipoCuponSelect'); // Obtener el nombre del tipo de cupón
    var agregarArticulosBtn = document.getElementById('agregarArticulosBtn'); // Obtener el botón

    function checkTipoBoton() {
        if (tipoCuponSelect.value === 'DESCUENTO') {
            agregarArticulosBtn.style.display = 'none'; // Si el tipo de cupón es DESCUENTO, el botón se oculta
        } else {
            agregarArticulosBtn.style.display = 'block'; // Si el tipo de cupón es DESCUENTO, el botón se muestra
        }
    }

    checkTipoBoton(); // La función se llama al iniciar la página

    tipoCuponSelect.addEventListener('change', checkTipoBoton); // La función se llama siempre que cambie el estado de checkTipoBoton
});