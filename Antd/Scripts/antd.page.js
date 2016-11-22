var $page = jQuery.noConflict();

var tid = setInterval(reloadPage, 10000);
function reloadPage() {
    var container = $page("#ResourcesMonitor");
    jQuery.support.cors = true;
    var aj = $page.ajax({
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

$page('[data-role="load-page"]').not("i").on("click", function () {
    AbortAllAjaxRequests();
    var page = $page(this).attr("data-page");
    var ico = $page(this).find('[data-icon="load"]');
    $page('[data-icon="load"]').each(function () {
        $page(this).hide();
    });
    jQuery.support.cors = true;
    var aj = $page.ajax({
        url: "/part/" + page,
        type: "GET",
        beforeSend: function () {
            ico.show();
        },
        success: function (data) {
            $page('[data-role="page-container"]').html(data);
            ico.hide();
            ReloadJS();
        },
        error: function () {
            ico.hide();
        }
    });
    _requests.push(aj);
});