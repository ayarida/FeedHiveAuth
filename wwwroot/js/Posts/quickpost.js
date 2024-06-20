$(function (e) {
    $("input.submit-button.btn-primary.mt-3.quick_submit").on("click", function () {
        if ($("input#Title").val() === 0 || $("input#Title").val() === '') {
            event.preventDefault();
            $(this).prev().addClass("star");
            swal.fire({
                customClass: {
                    confirmButton: "btn btn-primary  warning",
                },
                title: `${quickPostTitle}`,
                text: `${subFields}`,
                confirmButtonText: "موافق",
            });

        }

    })
})