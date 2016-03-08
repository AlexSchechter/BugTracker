$(function () {
    $(".edit-profile-button").click(function (event) {
        
        $(".editProfileSection").show(200);
        $('html, body').animate({
            scrollTop: $("#edit-profile-section").offset().top - 60
        }, 200);
        //event.preventDefault();
    })
});