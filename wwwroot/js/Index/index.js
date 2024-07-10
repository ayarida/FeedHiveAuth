function deletePost(event) {
    swal.fire({
        customClass: {
            confirmButton: "btn btn-success",
        },
        title: "هل أنت متأكد من رغبتك في حذف الوسائط؟ ",
        text: "لا يمكن التراجع عن هذا الإجراء",
        icon: "warning",
        buttons: true,
        dangerMode: true,
        // buttons: ["كلّا", "نعم"],
        confirmButtonText: "نعم",
        cancelButtonText: "لا",
        showCancelButton: true,
        showConfirmButton: true,
        className: {
            text: 'pleaseWork',
        },
    })
        .then((willDeletePost) => {
            if (willDeletePost.isConfirmed) {

                var serviceURL = "/Posts/Delete/" + event;
                var postId = event;
                $.ajax({
                    type: "DELETE",
                    url: serviceURL,
                    data: postId,
                    success: function (data) {
                        swal.fire({
                            title: "تم الحذف",
                            text: "تم حذف الميديا",
                            icon: "success"
                        }).then(function () {
                            window.location.reload();
                        }
                        )
                    },
                    error: function (data) {
                        swal.fire("خطأ في الحذف!", "الرجاء المحاولة مرة أخرى", "error");
                    },
                })

            } else {
                swal.fire({
                    title: "تم إلغاء محاولة الحذف",
                    confirmButtonText: "موافق"
                });
            }
        });
}

$(function (e) {

    function getCurrentDateTime() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');

        return `${year}-${month}-${day}T${hours}:${minutes}`;
    }

    // Set the value of the hidden input field with the current date and time
    document.getElementsByClassName('hiddenDate').value = getCurrentDateTime();

    $(".quick_submit").on("click", function () {
        if ($(".quick_title").val() == 0) {
            event.preventDefault();
            swal.fire({
                customClass: {
                    confirmButton: "btn btn-primary  warning",
                },
                title: "الرجاء املأ خانة العنوان!",
                confirmButtonText: "موافق",
            })
        }
    })
})