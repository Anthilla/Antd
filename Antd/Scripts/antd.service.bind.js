$("#StopBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigBind").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/bind/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="RemoveBindZone"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
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