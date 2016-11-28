var tid = setInterval(reloadPage, 10000);
function reloadPage() {
    var container = $("#ResourcesMonitor");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/monitor/resources/html",
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
$('[data-role="load-page"]').on("click", function () {
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
            //var container = $('[data-script="container"]');
            //$('[data-script="reload"]').each(function () {
            //    var src = $(this).attr("src");
            //    var type = $(this).attr("type");
            //    var dataScript = $(this).attr("data-script");
            //    $(this).remove();
            //    $("<script>").attr("src", src).attr("type", type).attr("data-script", dataScript).appendTo(container);
            //});
        },
        error: function () {
            ico.hide();
        }
    });
    _requests.push(aj);
});
