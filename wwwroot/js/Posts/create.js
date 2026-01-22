
    // 1. Create a global DataTransfer object to hold files
    const dt = new DataTransfer();
    var fileTypes = ['jpg', 'jpeg', 'png', 'webp', 'mp4'];

    $("#mediaInput").on("change", function(e) {
    // If no files selected, return
    if (!this.files || this.files.length === 0) return;

    // Loop through all selected files (in case user selects multiple)
    for (let i = 0; i < this.files.length; i++) {
    let file = this.files[i];
    let extension = file.name.split('.').pop().toLowerCase();
    let isAccepted = fileTypes.indexOf(extension) > -1;

    if (!isAccepted) {
    // Warning Alert
    swal.fire({
    customClass: { confirmButton: "btn btn-primary warning" },
    title: `${wrongType}`,
    text: `${requiredMediaType}`,
    confirmButtonText: "موافق",
    position: "top"
});
    continue; // Skip this invalid file
}

    // Add valid file to our DataTransfer object
    dt.items.add(file);

    // Preview Logic
    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function (event) {

    // We use the file name as a data attribute to find it later for deletion
    let mediaHtml = '';

    if (extension !== "mp4") {
    // IMAGE
    mediaHtml = `<img src="${event.target.result}" alt="preview" />`;
} else {
    // VIDEO
    mediaHtml = `<video controls><source src="${event.target.result}" type="video/mp4"/></video>`;
}

        var deleteIconHtml = $("#trash-icon-template").html();
    
    let html = `
                     <div class="media-preview-item" data-filename="${file.name}">
                         ${mediaHtml}
                         <div onclick="deleteMedia(this, '${file.name}')" class="delete-btn-overlay">
                           ${deleteIconHtml}
                         </div>
                     </div>
                 `;
    $("#imgPreview").append(html);
}
}

    // Update the actual input with the accumulated files
    this.files = dt.files;
});

    // Updated Delete Function
    function deleteMedia(element, fileName) {
    // 1. Remove visual element
    $(element).closest('.media-preview-item').remove();

    // 2. Remove actual file from DataTransfer object
    for (let i = 0; i < dt.items.length; i++) {
    if (fileName === dt.items[i].getAsFile().name) {
    dt.items.remove(i);
    break;
}
}

    // 3. Update the input element so the backend receives the correct list
    document.getElementById('mediaInput').files = dt.files;
}
