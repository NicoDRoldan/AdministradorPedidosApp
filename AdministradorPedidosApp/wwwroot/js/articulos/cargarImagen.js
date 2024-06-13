document.addEventListener('DOMContentLoaded', function () {
    var dropzone = document.getElementById('dropzone');
    var fileInput = document.getElementById('fileInput');
    var previewContainer = document.getElementById('previewContainer');

    dropzone.addEventListener('click', function () {
        fileInput.click();
    });

    dropzone.addEventListener('dragover', function (e) {
        e.preventDefault();
        dropzone.classList.add('dragover');
    });

    dropzone.addEventListener('dragleave', function () {
        dropzone.classList.remove('dragover');
    });

    dropzone.addEventListener('drop', function (e) {
        e.preventDefault();
        dropzone.classList.remove('dragover');
        var files = e.dataTransfer.files;
        if (files.length > 0) {
            handleFiles(files);
        }
    });

    fileInput.addEventListener('change', function () {
        if (fileInput.files.length > 0) {
            handleFiles(fileInput.files);
        }
    });

    document.addEventListener('paste', function (e) {
        var items = e.clipboardData.items;
        for (var i = 0; i < items.length; i++) {
            if (items[i].kind === 'file' && items[i].type.indexOf('image/') !== -1) {
                var blob = items[i].getAsFile();
                var container = new DataTransfer();
                container.items.add(blob);
                fileInput.files = container.files;
                handleFiles(container.files);
            }
        }
    });

    function handleFiles(files) {
        var file = files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            var img = document.createElement('img');
            img.src = e.target.result;
            previewContainer.innerHTML = '';
            previewContainer.appendChild(img);
            previewContainer.style.display = 'block';
            dropzone.classList.add('has-image');
        };
        reader.readAsDataURL(file);
    }
});