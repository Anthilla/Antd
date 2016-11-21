var tid = setInterval(reloadPage, 10000);
function reloadPage() {
    var container = $("#ResourcesMonitor");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/part/info/resources",
        type: "GET",
        success: function (data) {
            container.html(data);
        }
    });
    _requests.push(aj);
}
function abortTimer() {
    clearInterval(tid);
}

$("#ToggleResourceMonitor").on("click", function () {
    $("#ResourcesMonitor").toggle();
});

$('[data-role="load-page"]').not("i").on("click", function () {
    AbortAllAjaxRequests();
    var page = $(this).attr("data-page");
    var ico = $(this).find('[data-icon="load"]');
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
        },
        error: function () {
            ico.hide();
        }
    });
    _requests.push(aj);
});