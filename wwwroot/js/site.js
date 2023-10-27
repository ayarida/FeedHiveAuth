// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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


