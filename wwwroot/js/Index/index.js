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