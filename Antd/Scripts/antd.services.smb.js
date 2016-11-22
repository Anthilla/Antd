var $mb = jQuery.noConflict();

$mb("#StopSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$mb("#ReloadSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$mb("#EnableSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$mb("#DisableSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$mb("#ApplyConfigSamba").on("click", function () {
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$mb('[data-role="RemoveSambaResource"]').click(function () {
    var guid = $mb(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $mb.ajax({
        url: "/services/samba/resource/del",
        type: "POST",
        data: {
            Guid : guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});
