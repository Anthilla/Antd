var networkUrl = '/network/config';

//Repository
$('a[href="#nwboot"]').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/repo/all',
        type: 'GET',
        success: function (data) {
            $.each(data, function (index, i) {
                $('#NetworkCommandTable').append(AssembleDataRow(i));
            });
            EnableCommand();
            DisableCommand();
            DeleteCommand();
        }
    });
});

function AssembleDataRow(data) {
    var row = '<tr data-guid="' + data.Guid + '">';
    row += '<td><p>' + data.CommandLine + '</p></td>';
    if (data.IsEnabled == true) {
        row += '<td><input type="button" value="Disable" data-role="disable-command" data-guid="' + data.Guid + '" /></td>';
    }
    else {
        row += '<td><input type="button" value="Enable" data-role="enable-command" data-guid="' + data.Guid + '" /></td>';
    }
    row += '<td><input type="button" value="Delete" data-role="delete-command" data-guid="' + data.Guid + '" /></td>';
    row += '</tr>'
    return row;
}

function EnableCommand() {
    $('input[data-role="enable-command"]').click(function () {
        var guid = $(this).attr('data-guid');
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + '/repo/enable',
            type: 'POST',
            data: {
                Guid: guid
            },
            success: function (data) {
                location.reload(true);
            }
        });
    });
}

function DisableCommand() {
    $('input[data-role="disable-command"]').click(function () {
        var guid = $(this).attr('data-guid');
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + '/repo/disable',
            type: 'POST',
            data: {
                Guid: guid
            },
            success: function (data) {
                location.reload(true);
            }
        });
    });
}

function DeleteCommand() {
    $('input[data-role="delete-command"]').click(function () {
        var guid = $(this).attr('data-guid');
        jQuery.support.cors = true;
        $.ajax({
            url: networkUrl + '/repo/delete',
            type: 'POST',
            data: {
                Guid: guid
            },
            success: function (data) {
                location.reload(true);
            }
        });
    });
}

$('#ExportNetworkCommandTable').click(function () {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/repo/export',
        type: 'POST',
        success: function (data) {
            location.reload(true);
        }
    });
});

//IPV4
$('input#AddNewAddressIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Range = $('#Value_' + funcReference + 'Range').val();
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Range.length > 0 && Address.length > 0 && Interface.length > 0) {
        AddNewAddressIPV4(Range, Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddNewAddressIPV4(range, address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/add/address',
        type: 'POST',
        data: {
            Range: range,
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteAddressIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Range = $('#Value_' + funcReference + 'Range').val();
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Range.length > 0 && Address.length > 0 && Interface.length > 0) {
        DeleteAddressIPV4(Range, Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteAddressIPV4(range, address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/del/address',
        type: 'POST',
        data: {
            Range: range,
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#FlushConfigurationIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        FlushConfigurationIPV4(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function FlushConfigurationIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/flush',
        type: 'POST',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowInterfaceAddr').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowInterfaceAddr(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowInterfaceAddr(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/address/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowInterfaceAddr').show().text(data);
        }
    });
}

$('input#ShowInterfaceLink').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowInterfaceLink(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowInterfaceLink(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/link/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowInterfaceLink').show().text(data);
        }
    });
}

$('input#ShowInterfaceStats').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowInterfaceStats(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowInterfaceStats(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/stats/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowInterfaceStats').show().text(data);
        }
    });
}

$('input#AddRouteIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Gateway = $('#Value_' + funcReference + 'Gateway').val();
    if (Address.length > 0 && Gateway.length > 0) {
        AddRouteIPV4(Address, Gateway);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddRouteIPV4(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/add/route',
        type: 'POST',
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddMultipathRoute').click(function () {
    var funcReference = $(this).attr('id');
    var Network1 = $('#Value_' + funcReference + 'Network1').val();
    var Network2 = $('#Value_' + funcReference + 'Network2').val();
    if (Network1.length > 0 && Network2.length > 0) {
        AddMultipathRoute(Network1, Network2);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddMultipathRoute(network1, network2) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/add/route/multipath',
        type: 'POST',
        data: {
            Network1: network1,
            Network2: network2
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddNat').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Via = $('#Value_' + funcReference + 'Via').val();
    if (Address.length > 0 && Via.length > 0) {
        AddNat(Address, Via);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddNat(address, via) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/add/route/multipath',
        type: 'POST',
        data: {
            Address: address,
            Via: via
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteRouteIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Gateway = $('#Value_' + funcReference + 'Gateway').val();
    if (Address.length > 0 && Gateway.length > 0) {
        DeleteRouteIPV4(Address, Gateway);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteRouteIPV4(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/del/route',
        type: 'POST',
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowRoutes').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowRoutes(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowRoutes(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/routes/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowRoutes').show().text(data);
        }
    });
}

$('input#EnableInterface').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        EnableInterface(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function EnableInterface(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/enable/if',
        type: 'POST',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DisableInterface').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        DisableInterface(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DisableInterface(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/disable/if',
        type: 'POST',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddTunnelPointToPointIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    var Ttl = $('#Value_' + funcReference + 'Ttl').val();
    var Tunnel = $('#Value_' + funcReference + 'Tunnel').val();
    var Address = $('#Value_' + funcReference + 'Address').val();
    if (Interface.length > 0 && Ttl.length > 0 && Tunnel.length > 0 && Address.length > 0) {
        AddTunnelPointToPointIPV4(Interface, Ttl, Tunnel, Address);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddTunnelPointToPointIPV4(interfaceName, ttl, foreignTunnel, address) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/add/tunnel',
        type: 'POST',
        data: {
            Interface: interfaceName,
            Ttl: ttl,
            Tunnel: foreignTunnel,
            Address: address
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteTunnelPointToPointIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        DeleteTunnelPointToPointIPV4(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteTunnelPointToPointIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/del/tunnel',
        type: 'POST',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowTunnelsIPV4').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowTunnelsIPV4(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowTunnelsIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/tunnels/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowTunnelsIPV4').show().text(data);
        }
    });
}

//IPV6
$('input#AddNewAddressIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    var Address = $('#Value_' + funcReference + 'Address').val();
    if (Interface.length > 0 && Address.length > 0) {
        AddNewAddressIPV6(Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddNewAddressIPV6(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/add/address',
        type: 'POST',
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteAddressIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    var Address = $('#Value_' + funcReference + 'Address').val();
    if (Interface.length > 0 && Address.length > 0) {
        DeleteAddressIPV6(Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteAddressIPV6(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/del/address',
        type: 'POST',
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#FlushConfigurationIPV6').click(function () {
    FlushConfigurationIPV6();
});

function FlushConfigurationIPV6() {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/del/address',
        type: 'POST',
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowNeighborsIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowNeighborsIPV6(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowNeighborsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/neigh/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowNeighborsIPV6').show().text(data);
        }
    });
}

$('input#AddNeighborsIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Layer = $('#Value_' + funcReference + 'Layer').val();
    if (Interface.length > 0 && Layer.length > 0 && Address.length > 0) {
        AddNeighborsIPV6(Address, Layer, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddNeighborsIPV6(address, layer, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/add/neigh',
        type: 'POST',
        data: {
            Address: address,
            Layer: layer,
            Interface: interfaceName,
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteNeighborsIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Layer = $('#Value_' + funcReference + 'Layer').val();
    if (Interface.length > 0 && Layer.length > 0 && Address.length > 0) {
        DeleteNeighborsIPV6(Address, Layer, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteNeighborsIPV6(address, layer, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/del/neigh',
        type: 'POST',
        data: {
            Address: address,
            Layer: layer,
            Interface: interfaceName,
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddRouteIPV6Gateway').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Gateway = $('#Value_' + funcReference + 'Gateway').val();
    if (Address.length > 0 && Gateway.length > 0) {
        AddRouteIPV6Gateway(Address, Gateway);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddRouteIPV6Gateway(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/add/route/gw',
        type: 'POST',
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteRouteIPV6Gateway').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Gateway = $('#Value_' + funcReference + 'Gateway').val();
    if (Address.length > 0 && Gateway.length > 0) {
        DeleteRouteIPV6Gateway(Address, Gateway);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteRouteIPV6Gateway(address, gateway) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/del/route/gw',
        type: 'POST',
        data: {
            Address: address,
            Gateway: gateway
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddRouteIPV6Interface').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Interface = $('#Value_' + funcReference + 'Interface').val();
    if (Address.length > 0 && Interface.length > 0) {
        AddRouteIPV6Interface(Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddRouteIPV6Interface(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/add/route/if',
        type: 'POST',
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteRouteIPV6Interface').click(function () {
    var funcReference = $(this).attr('id');
    var Address = $('#Value_' + funcReference + 'Address').val();
    var Interface = $('#Value_' + funcReference + 'Interface').val();
    if (Address.length > 0 && Interface.length > 0) {
        DeleteRouteIPV6Interface(Address, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteRouteIPV6Interface(address, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/del/route/if',
        type: 'POST',
        data: {
            Address: address,
            Interface: interfaceName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowTunnelsIPV6').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $(this).parents('.nif-content').attr('data-nif-name');
    if (Interface.length > 0) {
        ShowTunnelsIPV6(Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowTunnelsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/tunnels/' + interfaceName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowTunnelsIPV6').show().text(data);
        }
    });
}

//BRIDGE
$('input#AddBridgeName').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Bridge.length > 0) {
        AddBridgeName(Bridge);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddBridgeName(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/add',
        type: 'POST',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteBridgeName').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Bridge.length > 0) {
        DeleteBridgeName(Bridge);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteBridgeName(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/del',
        type: 'POST',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#AddNetworkInterfaceToBridge').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $('#Value_' + funcReference + 'Interface').val();
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Interface.length > 0 && Bridge.length > 0) {
        AddNetworkInterfaceToBridge(Bridge, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function AddNetworkInterfaceToBridge(bridgeName, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/add/if',
        type: 'POST',
        data: {
            Interface: interfaceName,
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DeleteNetworkInterfaceToBridge').click(function () {
    var funcReference = $(this).attr('id');
    var Interface = $('#Value_' + funcReference + 'Interface').val();
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Interface.length > 0 && Bridge.length > 0) {
        DeleteNetworkInterfaceToBridge(Bridge, Interface);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DeleteNetworkInterfaceToBridge(bridgeName, interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/del/if',
        type: 'POST',
        data: {
            Interface: interfaceName,
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#EnableStpOnBridge').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Bridge.length > 0) {
        EnableStpOnBridge(Bridge);
    }
    else {
        alert('Value cannot be null!');
    }
});

function EnableStpOnBridge(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/stp/on/bridge',
        type: 'POST',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#DisableStpOnBridge').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Bridge.length > 0) {
        DisableStpOnBridge(Bridge);
    }
    else {
        alert('Value cannot be null!');
    }
});

function DisableStpOnBridge(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/stp/off/bridge',
        type: 'POST',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#ShowBridgeMACS').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Interface.length > 0) {
        ShowBridgeMACS(Bridge =);
    }
    else {
        alert('Value cannot be null!');
    }
});

function ShowBridgeMACS(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/macs/' + bridgeName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowBridgeMACS').show().text(data);
        }
    });
}

$('input#ShowBridgeSTP').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    if (Interface.length > 0) {
        ShowBridgeSTP(Bridge =);
    }
    else {
        alert('Value cannot be null!');
    }
});


function ShowBridgeSTP(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/stp/' + bridgeName,
        type: 'GET',
        success: function (data) {
            $('textarea#ShowBridgeSTP').show().text(data);
        }
    });
}

$('input#SetBridgePathCost').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    var Path = $('#Value_' + funcReference + 'Path').val();
    var Cost = $('#Value_' + funcReference + 'Cost').val();
    if (Bridge.length > 0 && Path.length > 0 && Cost.length > 0) {
        SetBridgePortPriority(Bridge, Path, Cost);
    }
    else {
        alert('Value cannot be null!');
    }
});

function SetBridgePathCost(bridgeName, path, cost) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/path/cost',
        type: 'POST',
        data: {
            Bridge: bridgeName,
            Path: path,
            Cost: cost
        },
        success: function (data) {
            location.reload(true);
        }
    });
}

$('input#SetBridgePortPriority').click(function () {
    var funcReference = $(this).attr('id');
    var Bridge = $('#Value_' + funcReference + 'Bridge').val();
    var Port = $('#Value_' + funcReference + 'Port').val();
    var Priority = $('#Value_' + funcReference + 'Priority').val();
    if (Bridge.length > 0 && Port.length > 0 && Priority.length > 0) {
        SetBridgePortPriority(Bridge, Port, Priority);
    }
    else {
        alert('Value cannot be null!');
    }
});

function SetBridgePortPriority(bridgeName, port, prio) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/port/prio',
        type: 'POST',
        data: {
            Bridge: bridgeName,
            Port: port,
            Priority: prio
        },
        success: function (data) {
            location.reload(true);
        }
    });
}