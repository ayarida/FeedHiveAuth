$(function () {

    $("#submit-user").on("click", function (event) {
        var allFilled = true;
        $("form input.form-control").each(function (index, element) {
            if ($(element).val() === 0 || $(element).val() === '')
                allFilled = false;
        })

        if (allFilled == false) {
            event.preventDefault();
            swal.fire({
                customClass: {
                    confirmButton: "btn btn-primary warning",
                },
                title: "يرجى ملء جميع الخانات!",
                confirmButtonText: "موافق"
            });
        }

    })

})
