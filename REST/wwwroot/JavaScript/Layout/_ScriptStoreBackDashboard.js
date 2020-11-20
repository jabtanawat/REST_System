$(".sidemenuClick").on("click", function () {
    $(".wrapper").toggleClass("active");
});

//$(".item_click").on("click", function () {
//    $(this).addClass("active").siblings().removeClass("active");
//});

$(function () {
    $('.sidbar-menu').slimScroll({
        height: $(window).height() - $('.nav_header').height(),
        distance: '-10px'
    });

    $(window).resize(function () {
        $('.sidbar-menu').slimScroll({
            height: $(window).height() - $('.nav_header').height() - $('.navbar').outerHeight()
        });
    });
});