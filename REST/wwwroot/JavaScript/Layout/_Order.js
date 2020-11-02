$(document).ready(function () {
    screenScroll()
});

function screenScroll() {
    var height_P = $(window).height() - $('.navbar').height()
    $('[scroll]').slimScroll({
        height: height_P,
        railBorderRadius: 0,
        railVisible: true
    });
}