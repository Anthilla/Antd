var networkUrl = '/network/config';

//IPV4
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

$('#TEST').click(function () {
    ShowInterfaceAddr('eth0');
});

function ShowInterfaceAddr(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/address',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

function ShowInterfaceLink(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/link',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

function ShowInterfaceStats(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/stats',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

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

function ShowRoutes(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/routes',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

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

function ShowTunnelsIPV4(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv4/tunnels',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

//IPV6
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

function ShowNeighborsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/neigh',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

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

function ShowTunnelsIPV6(interfaceName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/ipv6/tunnels',
        type: 'GET',
        data: {
            Interface: interfaceName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

//BRIDGE
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

function ShowBridgeMACS(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/macs',
        type: 'GET',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

function ShowBridgeSTP(bridgeName) {
    jQuery.support.cors = true;
    $.ajax({
        url: networkUrl + '/br/stp',
        type: 'GET',
        data: {
            Bridge: bridgeName
        },
        success: function (data) {
            console.log(data);
        }
    });
}

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