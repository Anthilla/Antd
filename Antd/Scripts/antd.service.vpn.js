$("#StopVpn").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/vpn/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadVpn").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/vpn/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableVpn").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/vpn/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableVpn").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/vpn/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigVpn").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/vpn/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});