var networkUrl = "/network/config";

//Repository
$('a[href="#nwboot"]').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/repo/all",
        type: "GET",
        success: function (data) {
            $.each(data, function (index, i) {
                $("#NetworkCommandTable").append(AssembleDataRow(i));
            });
            EnableCommand();
            DisableCommand();
            DeleteCommand();
        }
    });
});

function AssembleDataRow(data) {
    var row = '<tr data-guid="' + data.Guid + '">';
    row += "<td><p>" + data.CommandLine + "</p></td>";
    if (data.IsEnabled === true) {
        row += '<td><input type="button" value="Disable" data-role="disable-command" data-guid="' + data.Guid + '" /></td>';
    }
    else {
        row += '<td><input type="button" value="Enable" data-role="enable-command" data-guid="' + data.Guid + '" /></td>';
    }
    row += '<td><input type="button" value="Delete" data-role="delete-command" data-guid="' + data.Guid + '" /></td>';
    row += "</tr>";
    return row;
}

function EnableCommand() {
    $('input[data-role="enable-command"]').click(function () {
        var guid = $(this).attr("data-guid");
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + "/repo/enable",
            type: "POST",
            data: {
                Guid: guid
            },
            success: function () {
                location.reload(true);
            }
        });
    });
}

function DisableCommand() {
    $('input[data-role="disable-command"]').click(function () {
        var guid = $(this).attr("data-guid");
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + "/repo/disable",
            type: "POST",
            data: {
                Guid: guid
            },
            success: function () {
                location.reload(true);
            }
        });
    });
}

function DeleteCommand() {
    $('input[data-role="delete-command"]').click(function () {
        var guid = $(this).attr("data-guid");
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + "/repo/delete",
            type: "POST",
            data: {
                Guid: guid
            },
            success: function () {
                location.reload(true);
            }
        });
    });
}

$("#ExportNetworkCommandTable").click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/repo/export",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
});

//IPV4
$("input#AddNewAddressIPV4").click(function () {
    var funcReference = "AddNewAddressIPV4";
    var range = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Range").val();
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var broadcast = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Broadcast").val();
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (range.length > 0 && address.length > 0 && iface.length > 0) {
        AddNewAddressIPV4(address, range, iface, broadcast);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddNewAddressIPV4(address, range, interfaceName, broadcast) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/add/address",
        type: "POST",
        data: {
            Range: range,
            Address: address,
            Broadcast: broadcast,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddNewAddressIPV4").click(function () {
    var funcReference = "AddNewAddressIPV4";
    var range = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Range").val();
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var broadcast = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Broadcast").val();
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (range.length > 0 && address.length > 0 && iface.length > 0) {
        DeleteAddressIPV4(address, range, iface, broadcast);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteAddressIPV4(address, range, interfaceName, broadcast) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/del/address",
        type: "POST",
        data: {
            Range: range,
            Address: address,
            Broadcast: broadcast,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#FlushConfigurationIPV4").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        FlushConfigurationIPV4(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function FlushConfigurationIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/flush",
        type: "POST",
        data: {
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowInterfaceAddr").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowInterfaceAddr(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowInterfaceAddr(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/address/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowInterfaceAddr").show().text(data);
        }
    });
}

$("input#ShowInterfaceLink").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowInterfaceLink(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowInterfaceLink(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/link/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowInterfaceLink").show().text(data);
        }
    });
}

$("input#ShowInterfaceStats").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowInterfaceStats(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowInterfaceStats(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/stats/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowInterfaceStats").show().text(data);
        }
    });
}

$("input#AddRouteIPV4").click(function () {
    var funcReference = "AddRouteIPV4";
    var gateway = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Gateway").val();
    var destination = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Destination").val();
    if (destination.length > 0 && gateway.length > 0) {
        AddRouteIPV4(gateway, destination);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddRouteIPV4(gateway, destination) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/add/route",
        type: "POST",
        data: {
            Gateway: gateway,
            destination: destination
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteRouteIPV4").click(function () {
    var funcReference = "DeleteRouteIPV4";
    var gateway = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Gateway").val();
    var destination = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Destination").val();
    if (destination.length > 0 && gateway.length > 0) {
        DeleteRouteIPV4(gateway, destination);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteRouteIPV4(gateway, destination) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/del/route",
        type: "POST",
        data: {
            Gateway: gateway,
            Destination: destination
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddMultipathRoute").click(function () {
    var funcReference = "AddMultipathRoute";
    var network1 = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Network1").val();
    var network2 = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Network2").val();
    if (network1.length > 0 && network2.length > 0) {
        AddMultipathRoute(network1, network2);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddMultipathRoute(network1, network2) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/add/route/multipath",
        type: "POST",
        data: {
            Network1: network1,
            Network2: network2
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddNat").click(function () {
    var funcReference = "AddNat";
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var via = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Via").val();
    if (address.length > 0 && via.length > 0) {
        AddNat(address, via);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddNat(address, via) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/add/route/multipath",
        type: "POST",
        data: {
            Address: address,
            Via: via
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowRoutes").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowRoutes(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowRoutes(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/routes/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowRoutes").show().text(data);
        }
    });
}

$("input#EnableInterface").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        EnableInterface(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function EnableInterface(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/enable/if",
        type: "POST",
        data: {
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DisableInterface").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        DisableInterface(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DisableInterface(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/disable/if",
        type: "POST",
        data: {
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddTunnelPointToPointIPV4").click(function () {
    var funcReference = "AddTunnelPointToPointIPV4";
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    var ttl = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Ttl").val();
    var tunnel = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Tunnel").val();
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    if (iface.length > 0 && ttl.length > 0 && tunnel.length > 0 && address.length > 0) {
        AddTunnelPointToPointIPV4(iface, ttl, tunnel, address);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddTunnelPointToPointIPV4(interfaceName, ttl, foreignTunnel, address) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/add/tunnel",
        type: "POST",
        data: {
            Interface: interfaceName,
            Ttl: ttl,
            Tunnel: foreignTunnel,
            Address: address
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteTunnelPointToPointIPV4").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        DeleteTunnelPointToPointIPV4(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteTunnelPointToPointIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/del/tunnel",
        type: "POST",
        data: {
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowTunnelsIPV4").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowTunnelsIPV4(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowTunnelsIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv4/tunnels/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowTunnelsIPV4").show().text(data);
        }
    });
}

//IPV6
$("input#AddNewAddressIPV6").click(function () {
    var funcReference = "AddNewAddressIPV6";
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    if (iface.length > 0 && address.length > 0) {
        AddNewAddressIPV6(address, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddNewAddressIPV6(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/add/address",
        type: "POST",
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteAddressIPV6").click(function () {
    var funcReference = "DeleteAddressIPV6";
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    if (iface.length > 0 && address.length > 0) {
        DeleteAddressIPV6(address, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteAddressIPV6(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/del/address",
        type: "POST",
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#FlushConfigurationIPV6").click(function () {
    FlushConfigurationIPV6();
});

function FlushConfigurationIPV6() {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/del/address",
        type: "POST",
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowNeighborsIPV6").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowNeighborsIPV6(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowNeighborsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/neigh/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowNeighborsIPV6").show().text(data);
        }
    });
}

$("input#AddNeighborsIPV6").click(function () {
    var funcReference = "AddNeighborsIPV6";
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var layer = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Layer").val();
    if (iface.length > 0 && layer.length > 0 && address.length > 0) {
        AddNeighborsIPV6(address, layer, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddNeighborsIPV6(address, layer, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/add/neigh",
        type: "POST",
        data: {
            Address: address,
            Layer: layer,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteNeighborsIPV6").click(function () {
    var funcReference = "DeleteNeighborsIPV6";
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var layer = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Layer").val();
    if (iface.length > 0 && layer.length > 0 && address.length > 0) {
        DeleteNeighborsIPV6(address, layer, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteNeighborsIPV6(address, layer, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/del/neigh",
        type: "POST",
        data: {
            Address: address,
            Layer: layer,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddRouteIPV6Gateway").click(function () {
    var funcReference = "AddRouteIPV6Gateway";
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var gateway = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Gateway").val();
    if (address.length > 0 && gateway.length > 0) {
        AddRouteIPV6Gateway(address, gateway);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddRouteIPV6Gateway(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/add/route/gw",
        type: "POST",
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteRouteIPV6Gateway").click(function () {
    var funcReference = "DeleteRouteIPV6Gateway";
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var gateway = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Gateway").val();
    if (address.length > 0 && gateway.length > 0) {
        DeleteRouteIPV6Gateway(address, gateway);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteRouteIPV6Gateway(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/del/route/gw",
        type: "POST",
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddRouteIPV6Interface").click(function () {
    var funcReference = "AddRouteIPV6Interface";
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var iface = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Interface").val();
    if (address.length > 0 && iface.length > 0) {
        AddRouteIPV6Interface(address, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddRouteIPV6Interface(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/add/route/if",
        type: "POST",
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteRouteIPV6Interface").click(function () {
    var funcReference = "DeleteRouteIPV6Interface";
    var address = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Address").val();
    var iface = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Interface").val();
    if (address.length > 0 && iface.length > 0) {
        DeleteRouteIPV6Interface(address, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteRouteIPV6Interface(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/del/route/if",
        type: "POST",
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowTunnelsIPV6").click(function () {
    var iface = $(this).parents(".nif-content").attr("data-nif-name");
    if (iface.length > 0) {
        ShowTunnelsIPV6(iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowTunnelsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/ipv6/tunnels/" + interfaceName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowTunnelsIPV6").show().text(data);
        }
    });
}

//BRIDGE
$("input#AddBridgeName").click(function () {
    var funcReference = "AddBridgeName";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (bridge.length > 0) {
        AddBridgeName(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddBridgeName(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/add",
        type: "POST",
        data: {
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteBridgeName").click(function () {
    var funcReference = "DeleteBridgeName";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (bridge.length > 0) {
        DeleteBridgeName(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteBridgeName(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/del",
        type: "POST",
        data: {
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#AddNetworkInterfaceToBridge").click(function () {
    var funcReference = "AddNetworkInterfaceToBridge";
    var iface = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Interface").val();
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (iface.length > 0 && bridge.length > 0) {
        AddNetworkInterfaceToBridge(bridge, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function AddNetworkInterfaceToBridge(bridgeName, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/add/if",
        type: "POST",
        data: {
            Interface: interfaceName,
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DeleteNetworkInterfaceToBridge").click(function () {
    var funcReference = "DeleteNetworkInterfaceToBridge";
    var iface = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Interface").val();
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (iface.length > 0 && bridge.length > 0) {
        DeleteNetworkInterfaceToBridge(bridge, iface);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DeleteNetworkInterfaceToBridge(bridgeName, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/del/if",
        type: "POST",
        data: {
            Interface: interfaceName,
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#EnableStpOnBridge").click(function () {
    var funcReference = "EnableStpOnBridge";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (bridge.length > 0) {
        EnableStpOnBridge(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});

function EnableStpOnBridge(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/stp/on/bridge",
        type: "POST",
        data: {
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#DisableStpOnBridge").click(function () {
    var funcReference = "DisableStpOnBridge";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (bridge.length > 0) {
        DisableStpOnBridge(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});

function DisableStpOnBridge(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/stp/off/bridge",
        type: "POST",
        data: {
            Bridge: bridgeName
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#ShowBridgeMACS").click(function () {
    var funcReference = "ShowBridgeMACS";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (Interface.length > 0) {
        ShowBridgeMACS(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});

function ShowBridgeMACS(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/macs/" + bridgeName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowBridgeMACS").show().text(data);
        }
    });
}

$("input#ShowBridgeSTP").click(function () {
    var funcReference = "ShowBridgeSTP";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    if (Interface.length > 0) {
        ShowBridgeSTP(bridge);
    }
    else {
        alert("Value cannot be null!");
    }
});


function ShowBridgeSTP(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/stp/" + bridgeName,
        type: "GET",
        success: function (data) {
            $("textarea#ShowBridgeSTP").show().text(data);
        }
    });
}

$("input#SetBridgePathCost").click(function () {
    var funcReference = "SetBridgePathCost";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    var path = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Path").val();
    var cost = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Cost").val();
    if (bridge.length > 0 && path.length > 0 && cost.length > 0) {
        SetBridgePortPriority(bridge, path, cost);
    }
    else {
        alert("Value cannot be null!");
    }
});

function SetBridgePathCost(bridgeName, path, cost) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/path/cost",
        type: "POST",
        data: {
            Bridge: bridgeName,
            Path: path,
            Cost: cost
        },
        success: function () {
            location.reload(true);
        }
    });
}

$("input#SetBridgePortPriority").click(function () {
    var funcReference = "SetBridgePortPriority";
    var bridge = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Bridge").val();
    var port = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Port").val();
    var priority = $('[data-net-attr-list="' + funcReference + '"]').find("#Value_Priority").val();
    if (bridge.length > 0 && port.length > 0 && priority.length > 0) {
        SetBridgePortPriority(bridge, port, priority);
    }
    else {
        alert("Value cannot be null!");
    }
});

function SetBridgePortPriority(bridgeName, port, prio) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + "/br/port/prio",
        type: "POST",
        data: {
            Bridge: bridgeName,
            Port: port,
            Priority: prio
        },
        success: function () {
            location.reload(true);
        }
    });
}