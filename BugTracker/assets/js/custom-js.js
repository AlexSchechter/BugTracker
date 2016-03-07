$(function () {
    //$(".edit-profile-button").click(function () {
    //    $(".editProfileSection").show(200);
    //})

    $(".edit-profile-button").click(function (event) {
        $('html, body').animate({
            scrollTop: $("#edit-profile-section").offset().top - 60
        }, 200);
    })
});