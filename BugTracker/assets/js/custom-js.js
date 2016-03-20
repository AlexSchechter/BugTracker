$(function () {
    $(".edit-profile-button").click(function () {
        $(".editProfileSection").show(200);
        $('html, body').animate({
            scrollTop: $("#edit-profile-section").offset().top - 60
        }, 200);
    });

    $(".btn-demo").click(function (event) {
        var user = $(this).data('assigned-id');
        if (user == "robb@stark.com" || user == "petyr@littlefinger.com" || user == "arya@stark.com" || user == "cersei@lannister.com") {
            event.preventDefault();
            $(".demo-text").show();
        }
    });

    $("#upload-button").click(function () {
        $("#fileUpload").click();
    });
});