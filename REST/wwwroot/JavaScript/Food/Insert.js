const inputFile = document.getElementById('inputFile');
const previewContainer = document.getElementById('image-preview');
const previewImage = previewContainer.querySelector('.image-preview__image');
const infoArea = document.getElementById('upload-label');

inputFile.addEventListener('change', function () {
    const file = this.files[0];

    if (file) {

        const reader = new FileReader();

        reader.addEventListener('load', function () {
            previewImage.setAttribute('src', this.result);
        });

        infoArea.innerHTML = 'File Name: ' + inputFile.value.match(/[\/\\]([\w\d\s\.\-\(\)]+)$/)[1];

        reader.readAsDataURL(file);
    } else {
        previewImage.setAttribute('src', '/Images/System/NOIMAGE.jpg')
        infoArea.innerHTML = 'เลือกรูปภาพ';
    }
});
