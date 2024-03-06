// Delete user
function deleteRole(eventId) {

    swal.fire({
        customClass: {
            confirmButton: "btn btn-success",
        },
        title: "هل أنت متأكد من رغبتك في حذف الدور؟ ",
        text: "لا يمكن التراجع عن هذا الإجراء",
        icon: "warning",
        buttons: true,
        dangerMode: true,
        confirmButtonText: "نعم",
        cancelButtonText: "لا",
        showCancelButton: true,
        showConfirmButton: true,
    }).then((willDeletePost) => {
        if (willDeletePost.isConfirmed) {
            axios.post(`Delete/${eventId}`)
                .then(function () {
                    swal.fire({
                        title: "تم حذف الدور",
                        icon: "success"
                    }).then(function () {
                        window.location.reload();
                    }
                    )
                }).catch(function (error) {
                    swal.fire("خطأ في الحذف!", "الرجاء المحاولة مرة أخرى", "error");
                });
        }
    })
}