function deleteSelectedUsers() {

    if ($("input[type=checkbox]:checked").length == 0) {
        event.preventDefault();
        swal.fire({
            customClass: {
                confirmButton: "btn btn-primary warning",
            },
            title: "حدد مستخدم واحد على الأقل!",
            confirmButtonText: "موافق"
        });
    }
    else {

        event.preventDefault();

        swal.fire({
            customClass: {
                confirmButton: "btn btn-success",
            },
            title: "هل أنت متأكد أنك تريد حذف المستخدمين المحددين؟",
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
            .then((willDelete) => {
                if (willDelete.isConfirmed) {

                    selected = $("tbody input:checkbox:checked:not(:disabled)");
                    var deleteArr = [];
                    selected.each(function (index, value) {
                        deleteArr.push($(value).val());
                    });
                    console.log(deleteArr.join(","));
                    var idsString = deleteArr.join(",");
                    var serviceURL = "/Users/MultipleDelete";

                    $.ajax({
                        type: "GET",
                        url: serviceURL,
                        data: { userList: idsString },
                        success: function () {
                            swal.fire({
                                title: "تم الحذف",
                                text: "تم حذف المستخدمين المحددين",
                                icon: "success"
                            }).then(function () {
                                window.location.reload();
                            }
                            )

                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            swal.fire("خطأ في الحذف!", "الرجاء المحاولة مرة أخرى", "error");
                        }
                    });
                } else {
                    swal.fire({
                        title: "تم إلغاء محاولة الحذف",
                        confirmButtonText: "موافق"
                    });
                }
            });
    }
}