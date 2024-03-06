function deleteSelected(event) {
    selected = $("tbody input:checkbox:checked");
    if (selected.length == 0) {

        swal.fire({
            customClass: {
                confirmButton: "btn btn-primary  warning",
            },
            title: "إختر منشورا على الأقل !",
            confirmButtonText: "موافق",
        });
    }
    else {
        var deleteArr = [];

        var idsString = [];

        selected.each(function (index, value) {
            deleteArr.push($(value).val());
        });

        if (deleteArr.length > 1) {
            idsString = deleteArr.join(",");
        }
        else {

            selected.each(function (index, value) {
                idsString.push($(value).val());
            })

        }

        // console.log(deleteArr.join(","));

        var serviceURL = "/Posts/MultipleDelete";
        $.ajax({
            type: "GET",
            url: serviceURL,
            data: { idsStr: idsString.toString() },
            success: function (data) {
                swal.fire({
                    customClass: {
                        confirmButton: "btn btn-primary",
                    },
                    title: "تم إلغاء المنشورات المحددة",
                    confirmButtonText: "موافق"
                }).then(function () {
                    window.location.reload();
                }
                );
            },
            error: function (data) {
                errorFunc();
                swal.fire({
                    customClass: {
                        confirmButton: "error",
                    },
                    title: "حدث خطأ يرجى المحاولة مرة أخرى!",
                    confirmButtonText: "موافق"
                });
            },
        });
    }

}

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