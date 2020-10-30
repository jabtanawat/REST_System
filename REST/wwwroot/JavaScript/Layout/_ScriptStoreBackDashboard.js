$(".sidemenuClick").on("click", function () {
    $(".wrapper").toggleClass("active");
});

//$(".item_click").on("click", function () {
//    $(this).addClass("active").siblings().removeClass("active");
//});

$(function () {
    $('.sidebar').slimScroll({
        height: document.documentElement.clientHeight
    });

    $(window).resize(function () {
        $('.sidebar').slimScroll({
            height: document.documentElement.clientHeight - $('.navbar').outerHeight()
        });
    });
});