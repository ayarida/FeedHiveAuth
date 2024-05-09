// check media type and preview it

var fileTypes = ['jpg', 'jpeg', 'png', 'webp', 'mp4'];
$(".custom-file-upload").on("click", function () {
    $("#mediaInput").trigger("click");
})
$("#mediaInput").on("change", e => {

    let reader = new FileReader();
    var extension = e.target.files[0].name.split('.').pop().toLowerCase();

    var isAccepted = fileTypes.indexOf(extension) > -1;

    // Check if file type is valid and show error message if not
    if (!isAccepted) {
        swal.fire({
            customClass: {
                confirmButton: "btn btn-primary  warning",
            },
            title: "الرجاء التحميل ميديا من الأنواع التالية",
            text: "mp4 jpeg jpg أو png",
            confirmButtonText: "موافق",
        });
    } else {

        // If file type is image
        if (extension !== "mp4") {
            reader.readAsDataURL(e.target.files[0]);
            reader.onload = function (e) {
                let html = `
                                 <div class = "uploaded-img mt-3">
                                     <img src = "${e.target.result}" alt="placeholder" class="mb-3" />
                                     <div onclick="deleteMedia(this)" class="deleteIcon"><i class="bi bi-trash3-fill" style="cursor: pointer"></i>
                                     </div>
                                 </div>
                             `;
                $("#imgPreview").append(html);
            }
        }
    }

    // If file type is video
    if (extension === "mp4") {
        reader.readAsDataURL(e.target.files[0]);
        reader.onload = function (e) {
            let html = `
                               <div class="video-holder">
                                <video controls class = "uploaded-img">
                                    <source src = "${e.target.result}" alt="placeholder" class="mb-3" type="video/mp4"/>
                                   
                                </video>
                                 <div onclick="deleteMedia(this)" class="deleteIcon"><i class="bi bi-trash3-fill" style="cursor: pointer"></i>
                                 </div>
                            </div>
                               `;
            $("#imgPreview").append(html);
        }
    }

})


//check for missing fields
$(function (e) {

    $(".submit-button").on("click", function () {

        $("input.form-control.req").each(function (index, element) {
            if ($(element).val() === 0 || $(element).val() === '') {
                event.preventDefault();
                $(this).prev().addClass("star");
                swal.fire({
                    customClass: {
                        confirmButton: "btn btn-primary  warning",
                    },
                    title: "Fill all required fields",
                    text: "fields indicated by red stars",
                    confirmButtonText: "موافق",
                });

            }

        })

    })
});


// Delete media              
function deleteMedia(event) {
    // console.log(event)
    $(event).parent().remove();
}
