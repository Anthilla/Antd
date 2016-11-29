$("#StopDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ReloadDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#EnableDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#DisableDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$("#ApplyConfigDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$('[data-role="DeleteDhcpdPool"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/pool/del",
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

$('[data-role="DeleteDhcpdClass"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/class/del",
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

$('[data-role="DeleteDhcpdReservation"]').click(function () {
    var guid = $(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $.ajax({
        url: "/services/dhcpd/reservation/del",
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
