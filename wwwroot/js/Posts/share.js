document.getElementById('form-share').addEventListener('submit',
    function (event) {

        if ($("input[type=radio]:checked").length == 0) {
            event.preventDefault();
            swal.fire({
                customClass: {
                    confirmButton: "btn btn-primary",
                },
                title: "الرجاء تحديد قناة",
                confirmButtonText: "موافق"
            });
        }

        else {
            var serviceURL = "/Social/Publish/Send" + event;
            var shareForm = $(':input').serializeArray();

            if ($('.shareImage')) {
                var imgName = $('.shareImage').attr('name');
                var imgId = $('.shareImage').attr('value');
                shareForm.push({ name: imgName, value: imgId })
            }

            var channelId = $("input[name='Channels[0]']:checked").val();


            $.ajax({
                type: "POST",
                url: serviceURL,
                data: shareForm,
                success: function () {
                    // location.href = "/Posts/List"
                    //customized alert for success
                    window.location.reload();
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                },
            })
        }
    });

