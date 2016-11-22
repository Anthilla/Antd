var $bind = jQuery.noConflict();

$bind("#StopBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$bind("#ReloadBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$bind("#EnableBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$bind("#DisableBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$bind("#ApplyConfigBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$bind('[data-role="RemoveBindZone"]').click(function () {
    var guid = $bind(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $bind.ajax({
        url: "/services/bind/zone/del",
        type: "POST",
        data: {
            Guid: guid
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});