// Function to delete media with updated SweetAlert syntax
function deletePostMedia(mediaid, postid) {

    swal.fire({
        title: "هل أنت متأكد؟", // Or use your resource string
        text: "لا يمكن التراجع عن هذا الإجراء",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "نعم، احذف",
        cancelButtonText: "إلغاء",
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        position: "top", // <--- Requested Position
        customClass: {
            confirmButton: "btn btn-danger",
            cancelButton: "btn btn-secondary ms-2"
        }
    }).then((result) => {
        if (result.isConfirmed) {

            var serviceURL = "/MediaItem/DeletePostMedia";
            var data = {
                media: mediaid,
                post: postid
            };

            $.ajax({
                type: "DELETE",
                url: serviceURL,
                data: $.param(data),
                success: function (data) {
                    swal.fire({
                        title: "تم الحذف بنجاح",
                        icon: "success",
                        position: "top",
                        timer: 1500,
                        showConfirmButton: false
                    }).then(function () {
                        window.location.reload();
                    });
                },
                error: function (data) {
                    swal.fire({
                        title: "خطأ",
                        text: "حدث خطأ أثناء الحذف، يرجى المحاولة مرة أخرى",
                        icon: "error",
                        position: "top"
                    });
                },
            });

        }
    });
}