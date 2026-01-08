var fileTypes = ['jpg', 'jpeg', 'png', 'webp', 'mp4'];

// Handle Media Change
$("#mediaInput").on("change", e => {

    let reader = new FileReader();

    // Safety check if user cancels the explorer window
    if (!e.target.files || e.target.files.length === 0) return;

    var extension = e.target.files[0].name.split('.').pop().toLowerCase();
    var isAccepted = fileTypes.indexOf(extension) > -1;

    // Check validity
    if (!isAccepted) {
        swal.fire({
            customClass: {
                confirmButton: "btn btn-primary warning",
            },
            title: `${wrongType}`, // Ensure these variables are defined in your view
            text: `${requiredMediaType}`,
            confirmButtonText: "موافق",
            position: "top"
        });
        // Clear the input so they can try again
        $("#mediaInput").val('');
    } else {
        // Image Logic
        if (extension !== "mp4") {
            reader.readAsDataURL(e.target.files[0]);
            reader.onload = function (e) {
                let html = `
                     <div>
                         <img src="${e.target.result}" alt="placeholder" class="mb-3" />
                         <div onclick="deleteMedia(this)" class="deleteIcon">
                            <i class="bi bi-trash3-fill" style="cursor: pointer"></i>
                         </div>
                     </div>
                 `;
                $("#imgPreview").append(html);
            }
        }

        // Video Logic
        if (extension === "mp4") {
            reader.readAsDataURL(e.target.files[0]);
            reader.onload = function (e) {
                let html = `
                   <div class="video-holder mt-3">
                        <video controls class="uploaded-img">
                            <source src="${e.target.result}" alt="placeholder" class="mb-3" type="video/mp4"/>
                        </video>
                         <div onclick="deleteMedia(this)" class="deleteIcon">
                            <i class="bi bi-trash3-fill" style="cursor: pointer"></i>
                         </div>
                    </div>
                   `;
                $("#imgPreview").append(html);
            }
        }
    }
});

// Validation and Submit Logic
$(function (e) {
    $(".submit-button").on("click", function (event) {

        $("input.form-control.req").each(function (index, element) {
            if ($(element).val() == 0 || $(element).val() == '') {
                event.preventDefault();
                $(this).prev().addClass("star");

                swal.fire({
                    customClass: {
                        confirmButton: "btn btn-primary warning",
                    },
                    title: `${fields}`,
                    text: `${subFields}`,
                    confirmButtonText: "موافق",
                    position: "top",
                    width: "400px"
                });
            }
        });
    });
});

function deleteMedia(event) {
    $(event).parent().remove();
}