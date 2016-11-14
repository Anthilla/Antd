$('[data-role="load-page"]').on("click", function () {
    AbortAllAjaxRequests();
    var page = $(this).attr("data-page");
    var ico = $(this).find("i");
    $('[data-icon="load"]').each(function () {
        $(this).hide();
    });
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/" + page,
        type: "GET",
        beforeSend: function () {
            ico.show();
        },
        success: function (data) {
            $('[data-role="page-container"]').html(data);
            ico.hide();
            ReloadJS();
        }
    });
    _requests.push(aj);
});