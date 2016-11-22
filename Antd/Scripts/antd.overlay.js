var $ol = jQuery.noConflict();

$ol('[data-role="SetOverlayDirectory"]').on("click", function () {
    var dir = $ol(this).attr("data-dir");
    jQuery.support.cors = true;
    var aj = $ol.ajax({
        url: "/overlay/directory",
        type: "POST",
        data: {
            Directory: dir
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});