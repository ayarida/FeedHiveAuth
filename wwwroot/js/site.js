// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Swiper
try {
    const swiper = new Swiper('.swiper', {
        loop: true,
        centeredSlides: true,
        slidesPerView: '1',
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
    });
}

catch (error) {
    console.log("no swiper found");
}


//Table List
function toggle(source) {
    checkboxes = $("tbody tr input");
    console.log(checkboxes)
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        checkboxes[i].checked = source.checked;
    }
}

function deleteSelected() {
    selected = $("tbody input:checkbox:checked");
    var deleteArr = [];
    selected.each(function (index, value) {
        deleteArr.push($(value).val());
    })
    console.log(deleteArr);
}

function publishSelected() {
    selected = $("tbody input:checkbox:checked");
    var publishArr = [];
    selected.each(function (index, value) {
        publishArr.push($(value).val());
    })
    console.log(publishArr);
}

