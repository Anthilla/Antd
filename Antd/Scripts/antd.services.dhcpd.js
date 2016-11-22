var $dhcp = jQuery.noConflict();

$dhcp("#StopDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
        url: "/services/dhcpd/stop",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$dhcp("#ReloadDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
        url: "/services/dhcpd/restart",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$dhcp("#EnableDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
        url: "/services/dhcpd/enable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$dhcp("#DisableDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
        url: "/services/dhcpd/disable",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$dhcp("#ApplyConfigDhcpd").on("click", function () {
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
        url: "/services/dhcpd/set",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$dhcp('[data-role="DeleteDhcpdPool"]').click(function () {
    var guid = $dhcp(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
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

$dhcp('[data-role="DeleteDhcpdClass"]').click(function () {
    var guid = $dhcp(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
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

$dhcp('[data-role="DeleteDhcpdReservation"]').click(function () {
    var guid = $dhcp(this).attr("data-id");
    jQuery.support.cors = true;
    var aj = $dhcp.ajax({
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
